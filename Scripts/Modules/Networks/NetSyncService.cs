using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Cysharp.Threading.Tasks;
using NetworkTypes;
using NetworkTypes.Commands;
using NetworkTypes.Utilities;
using TinyMVC.Modules.Networks.Buffers;
using TinyMVC.ReactiveFields;
using TinyMVC.ReactiveFields.Extensions;
using TinySerializer.Core.Misc;
using UnityEngine;

namespace TinyMVC.Modules.Networks {
    public static class NetSyncService {
        public static event Action<int> ping;
        
        private static IPEndPoint _serverPoint;
        private static Socket _socket;
        private static ulong _uid;
        private static ushort _version;
        private static ulong _csrf;
        private static long _lastReceiveTime;
        private static int _sendCount;
        private static int _sendLimit;
        
        private static CancellationTokenSource _cancellation;
        private static bool _isInitialized;
        
        private static readonly List<NetReaderBuffer> _bufferRead;
        private static readonly List<NetWriterBuffer> _bufferWrite;
        private static readonly List<NetActionBuffer> _bufferAction;
        
        private const int _BUFFER_SIZE = 8192;
        private const int _COMMAND_BUFFER_SIZE = 256;
        private const int _NETWORK_TICK = 33;
        
        static NetSyncService() {
            _bufferRead = new List<NetReaderBuffer>(_COMMAND_BUFFER_SIZE);
            _bufferWrite = new List<NetWriterBuffer>(_COMMAND_BUFFER_SIZE);
            _bufferAction = new List<NetActionBuffer>(_COMMAND_BUFFER_SIZE);
        }
        
        public static bool Initialize(string ip, int port, int sendLimit = 120, int receiveTimeout = 4000) {
            if (_isInitialized) {
                Debug.LogError("NetService.Initialize - Already initialized!");
                return _isInitialized;
            }
            
            if (IPAddress.TryParse(ip, out IPAddress serverIP)) {
                _serverPoint = new IPEndPoint(serverIP, port);
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _socket.Bind(_serverPoint);
                _socket.ReceiveTimeout = receiveTimeout;
                _sendLimit = sendLimit;
                _isInitialized = true;
                
                _cancellation = new CancellationTokenSource();
                SyncProcess(_cancellation.Token);
            } else {
                Debug.LogError("NetService.Initialize - Invalid ip data!");
            }
            
            return _isInitialized;
        }
        
        public static void Deinitialize() {
            if (_isInitialized == false) {
                Debug.LogError("NetService.Initialize - Already deinitialized!");
                return;
            }
            
            if (_cancellation != null) {
                _cancellation.Cancel();
                _cancellation.Dispose();
            }
            
            _socket.Close();
            _socket.Dispose();
            
            _isInitialized = false;
        }
        
        public static void UpdateUID(ulong uid) => _uid = uid;
        
        public static void UpdateVersion(ushort version) => _version = version;
        
        public static void UpdateCSRF(ulong csrf) => _csrf = csrf;
        
        internal static void AddRead(ushort group, ushort part, byte key, ActionListener<object> listener) {
            for (int bufferId = 0; bufferId < _bufferRead.Count; bufferId++) {
                if (_bufferRead[bufferId].IsCurrent(group, part, key) == false) {
                    continue;
                }
                
                _bufferRead[bufferId].listeners.Add(listener);
                return;
            }
            
            _bufferRead.Add(new NetReaderBuffer(group, part, key, listener));
        }
        
        internal static void RemoveRead(ushort group, ushort part, byte key, ActionListener<object> listener) {
            for (int bufferId = 0; bufferId < _bufferRead.Count; bufferId++) {
                if (_bufferRead[bufferId].IsCurrent(group, part, key) == false) {
                    continue;
                }
                
                _bufferRead[bufferId].listeners.Remove(listener);
                
                if (_bufferRead[bufferId].listeners.Count == 0) {
                    _bufferRead.RemoveAt(bufferId);
                }
                
                return;
            }
        }
        
        internal static void Write<T>(ushort group, ushort part, byte key, T value) {
            for (int bufferId = 0; bufferId < _bufferWrite.Count; bufferId++) {
                if (_bufferWrite[bufferId].IsCurrent(group, part, key) == false) {
                    continue;
                }
                
                _bufferWrite[bufferId].value = value;
                return;
            }
            
            _bufferWrite.Add(new NetWriterBuffer(group, part, key, value));
        }
        
        internal static void Action(ushort type, ushort location, float x, float z) {
            for (int bufferId = 0; bufferId < _bufferAction.Count; bufferId++) {
                if (_bufferAction[bufferId].IsCurrent(type, location) == false) {
                    continue;
                }
                
                _bufferAction[bufferId].UpdateDirection(x, z);
                return;
            }
            
            _bufferAction.Add(new NetActionBuffer(type, location, x, z));
        }
        
        private static async void SyncProcess(CancellationToken cancellation) {
            while (_isInitialized) {
                if (_sendCount < _sendLimit) {
                    try {
                        Sync(cancellation);
                    } catch (SocketException exception) {
                        if (exception.ErrorCode == 10060) {
                            Debug.Log(new Exception("NetService.Sync - Timeout", exception));
                            _sendCount--;
                        } else {
                            Debug.LogWarning(new Exception("NetService.Sync", exception));
                        }
                    } catch (OperationCanceledException exception) {
                        Debug.Log(new Exception("NetService.Sync - Canceled", exception));
                    } catch (Exception exception) {
                        Debug.LogWarning(new Exception("NetService.Sync", exception));
                    }
                }
                
                await UniTask.Delay(_NETWORK_TICK, DelayType.UnscaledDeltaTime, PlayerLoopTiming.LastPostLateUpdate, cancellation);
            }
        }
        
        private static async void Sync(CancellationToken cancellation) {
            NetReadCommand[] readCommands = GetReadCommands();
            NetWriteCommand[] writeCommands = GetWriteCommands();
            NetActionCommand[] actionCommands = GetActionCommands();
            
            if (readCommands == null && writeCommands == null && actionCommands == null) {
                return;
            }
            
            byte[] data = new NetMessage(_uid, _version, _csrf, readCommands, writeCommands, actionCommands).ToBytes();
            _sendCount++;
            
            await _socket.SendAsync(data, SocketFlags.None, cancellation);
            
            if (readCommands == null) {
                _sendCount--;
                return;
            }
            
            EndPoint listenPoint = new IPEndPoint(IPAddress.Any, 0);
            data = new byte[_BUFFER_SIZE];
            
            SocketReceiveFromResult result = await _socket.ReceiveFromAsync(data, SocketFlags.None, listenPoint);
            _sendCount--;
            
            if (result.ReceivedBytes <= 0) {
                return;
            }
            
            try {
                NetMessage message = data.ToMessage();
                
                if (message.time > _lastReceiveTime) {
                    _lastReceiveTime = message.time;
                    
                    try {
                        ping?.Invoke((int)DateTime.Now.Subtract(new DateTime(message.time)).TotalMilliseconds);
                    } catch (Exception exception) {
                        Debug.LogWarning(new Exception("NetService.Sync - Ping invoke", exception));
                    }
                    
                    if (message.writes != null && _bufferRead.Count > 0) {
                        NetReaderBuffer[] bufferRead = new NetReaderBuffer[_bufferRead.Count];
                        _bufferRead.CopyTo(bufferRead);
                        
                        foreach (NetWriteCommand command in message.writes) {
                            object value;
                            
                            if (command.data != null) {
                                value = SerializationUtility.DeserializeValueWeak(command.data, DataFormat.Binary);
                            } else {
                                value = null;
                            }
                            
                            foreach (NetReaderBuffer buffer in bufferRead) {
                                if (buffer.IsCurrent(command.group, command.part, command.key)) {
                                    try {
                                        buffer.listeners.Invoke(value);
                                    } catch (Exception exception) {
                                        Debug.LogWarning(new Exception("NetService.Sync - Listener.Invoke", exception));
                                    }
                                }
                            }
                        }
                    }
                }
            } catch (Exception exception) {
                Debug.LogWarning(new Exception("NetService.Sync - Response read", exception));
            }
        }
        
        private static NetReadCommand[] GetReadCommands() {
            if (_bufferRead.Count == 0) {
                return null;
            }
            
            NetReadCommand[] commands = new NetReadCommand[_bufferRead.Count];
            
            for (int bufferId = 0; bufferId < _bufferRead.Count; bufferId++) {
                NetReaderBuffer buffer = _bufferRead[bufferId];
                commands[bufferId] = new NetReadCommand(buffer.group, buffer.part, buffer.key);
            }
            
            return commands;
        }
        
        private static NetWriteCommand[] GetWriteCommands() {
            if (_bufferWrite.Count == 0) {
                return null;
            }
            
            NetWriteCommand[] commands = new NetWriteCommand[_bufferWrite.Count];
            
            for (int bufferId = 0; bufferId < _bufferWrite.Count; bufferId++) {
                NetWriterBuffer buffer = _bufferWrite[bufferId];
                commands[bufferId] = new NetWriteCommand(buffer.group, buffer.part, buffer.key, SerializationUtility.SerializeValueWeak(buffer.value, DataFormat.Binary));
            }
            
            _bufferWrite.Clear();
            return commands;
        }
        
        private static NetActionCommand[] GetActionCommands() {
            if (_bufferAction.Count == 0) {
                return null;
            }
            
            NetActionCommand[] commands = new NetActionCommand[_bufferAction.Count];
            
            for (int bufferId = 0; bufferId < _bufferAction.Count; bufferId++) {
                NetActionBuffer buffer = _bufferAction[bufferId];
                commands[bufferId] = new NetActionCommand(buffer.type, buffer.locationId, buffer.directionX, buffer.directionZ);
            }
            
            _bufferAction.Clear();
            return commands;
        }
    }
}