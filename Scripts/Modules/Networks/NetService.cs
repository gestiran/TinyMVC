using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using NetworkTypes;
using NetworkTypes.Commands;
using NetworkTypes.Utilities;
using TinyMVC.Modules.Networks.Commands;
using TinyMVC.ReactiveFields;
using TinyMVC.ReactiveFields.Extensions;
using TinySerializer.Core.Misc;
using UnityEngine;

namespace TinyMVC.Modules.Networks {
    public static class NetService {
        public static event Action<int> ping;
        
        private static IPAddress _serverIp;
        private static int _serverPort;
        private static uint _uid;
        private static uint _csrf;
        private static long _lastReceiveTime;
        private static sbyte _sendCount;
        
        private static bool _isInitialized;
        
        private static readonly UdpClient _udp;
        private static readonly List<NetWriter> _bufferWrite;
        private static readonly List<NetReader> _bufferRead;
        
        private const int _BUFFER_SIZE = 256;
        private const int _RECEIVE_TIMEOUT = 4000;
        private const sbyte _SEND_LIMIT = 16;
        
        static NetService() {
            _udp = new UdpClient();
            _bufferWrite = new List<NetWriter>(_BUFFER_SIZE);
            _bufferRead = new List<NetReader>(_BUFFER_SIZE);
            
            SyncProcess();
        }
        
        public static bool Initialize(string ip, int port) {
            if (_isInitialized) {
                return _isInitialized;
            }
            
            if (IPAddress.TryParse(ip, out _serverIp)) {
                _serverPort = port;
                _isInitialized = true;
            }
            
            return _isInitialized;
        }
        
        public static void UpdateUID(uint uid) => _uid = uid;
        
        public static void UpdateCSRF(uint csrf) => _csrf = csrf;
        
        internal static void Write<T>(ushort group, ushort part, byte key, T value) {
            for (int writeId = 0; writeId < _bufferWrite.Count; writeId++) {
                if (_bufferWrite[writeId].IsCurrent(group, part, key) == false) {
                    continue;
                }
                
                _bufferWrite[writeId].value = value;
                return;
            }
            
            _bufferWrite.Add(new NetWriter(group, part, key, value));
        }
        
        internal static void AddRead(ushort group, ushort part, byte key, ActionListener<object> listener) {
            for (int bufferId = 0; bufferId < _bufferRead.Count; bufferId++) {
                if (_bufferRead[bufferId].IsCurrent(group, part, key) == false) {
                    continue;
                }
                
                _bufferRead[bufferId].listeners.Add(listener);
                return;
            }
            
            _bufferRead.Add(new NetReader(group, part, key, listener));
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
        
        private static async void SyncProcess() {
            while (Application.isPlaying) {
                if (_isInitialized && _sendCount < _SEND_LIMIT) {
                    try {
                        Sync().Forget();
                    } catch (Exception exception) {
                        Debug.LogWarning(new Exception("NetService.Sync", exception));
                    }
                }
                
                await UniTask.Yield();
            }
        }
        
        private static async UniTask Sync() {
            NetReadCommand[] readCommands = GetReadCommands();
            NetWriteCommand[] writeCommands = GetWriteCommands();
            
            if (readCommands == null && writeCommands == null) {
                return;
            }
            
            byte[] data = new NetMessage(_uid, _csrf, readCommands, writeCommands).ToBytes();
            
            DateTime now = DateTime.Now;
            _sendCount++;
            
            await _udp.SendAsync(data, data.Length, new IPEndPoint(_serverIp, _serverPort));
            
            if (readCommands == null) {
                _sendCount--;
                return;
            }
           
            UniTask.WhenAny(Receive(now), WaitTimeout()).Forget();
        }
        
        private static async UniTask Receive(DateTime now) {
            UdpReceiveResult receive = await _udp.ReceiveAsync();
            _sendCount--;
            
            if (receive.Buffer.Length <= 0) {
                return;
            }
            
            try {
                ping?.Invoke((int)DateTime.Now.Subtract(now).TotalMilliseconds);
            } catch (Exception exception) {
                Debug.LogWarning(new Exception("NetService.Sync - Ping invoke", exception));
            }
            
            try {
                NetMessage message = receive.Buffer.ToMessage();
                
                if (message.time > _lastReceiveTime) {
                    _lastReceiveTime = message.time;
                    
                    if (message.write != null && _bufferRead.Count > 0) {
                        NetReader[] bufferRead = new NetReader[_bufferRead.Count];
                        _bufferRead.CopyTo(bufferRead);
                        
                        foreach (NetWriteCommand command in message.write) {
                            object value;
                            
                            if (command.data != null) {
                                value = SerializationUtility.DeserializeValueWeak(command.data, DataFormat.Binary);
                            } else {
                                value = null;
                            }
                            
                            foreach (NetReader buffer in bufferRead) {
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
        
        private static async UniTask WaitTimeout() {
            await UniTask.Delay(_RECEIVE_TIMEOUT, true);
            _sendCount--;
        }
        
        private static NetReadCommand[] GetReadCommands() {
            if (_bufferRead.Count == 0) {
                return null;
            }
            
            NetReadCommand[] commands = new NetReadCommand[_bufferRead.Count];
            
            for (int bufferId = 0; bufferId < _bufferRead.Count; bufferId++) {
                NetReader buffer = _bufferRead[bufferId];
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
                NetWriter buffer = _bufferWrite[bufferId];
                commands[bufferId] = new NetWriteCommand(buffer.group, buffer.part, buffer.key, SerializationUtility.SerializeValueWeak(buffer.value, DataFormat.Binary));
            }
            
            _bufferWrite.Clear();
            return commands;
        }
    }
}