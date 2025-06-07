using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NetworkTypes;
using NetworkTypes.Commands;
using NetworkTypes.Utilities;
using TinyMVC.Modules.Networks.Handlers;
using UnityEngine;

namespace TinyMVC.Modules.Networks {
    public static class NetLogicService {
        public static bool isInitialized { get; private set; }
        
        private static INetLogicHandler _handler;
        private static IPEndPoint _serverPoint;
        private static Socket _socket;
        private static ulong _uid;
        private static ushort _version;
        private static ulong _csrf;
        private static int _sendCount;
        private static int _sendLimit;
        
        private static CancellationTokenSource _cancellation;
        
        private static readonly List<NetActionCommand> _actions;
        
        private const int _BUFFER_SIZE = 8192;
        private const int _COMMAND_BUFFER_SIZE = 256;
        private const int _NETWORK_TICK = 33;
        
        static NetLogicService() {
            _actions = new List<NetActionCommand>(_COMMAND_BUFFER_SIZE);
        }
        
        public static bool Initialize(INetLogicHandler handler, string ip, int port, int sendLimit = 60, int receiveTimeout = 3000) {
            if (isInitialized) {
                Debug.LogError("NetService.Initialize - Already initialized!");
                return isInitialized;
            }
            
            _handler = handler;
            
            if (IPAddress.TryParse(ip, out IPAddress serverIP)) {
                _serverPoint = new IPEndPoint(serverIP, port);
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _socket.ReceiveTimeout = receiveTimeout;
                _sendLimit = sendLimit;
                isInitialized = true;
                
                _cancellation = new CancellationTokenSource();
                SyncProcess(_cancellation.Token);
            } else {
                Debug.LogError("NetService.Initialize - Invalid ip data!");
            }
            
            return isInitialized;
        }
        
        public static void Deinitialize() {
            if (isInitialized == false) {
                Debug.LogError("NetService.Initialize - Already deinitialized!");
                return;
            }
            
            if (_cancellation != null) {
                _cancellation.Cancel();
                _cancellation.Dispose();
            }
            
            _socket.Close();
            _socket.Dispose();
            
            isInitialized = false;
        }
        
        public static void UpdateUID(ulong uid) => _uid = uid;
        
        public static void UpdateVersion(ushort version) => _version = version;
        
        public static void UpdateCSRF(ulong csrf) => _csrf = csrf;
        
        private static async void SyncProcess(CancellationToken cancellation) {
            while (isInitialized) {
                if (_sendCount < _sendLimit) {
                    try {
                        Sync();
                    } catch (Exception exception) {
                        Debug.LogWarning(new Exception("NetService.Sync", exception));
                    }
                }
                
                await UniTask.Delay(_NETWORK_TICK, DelayType.UnscaledDeltaTime, PlayerLoopTiming.LastPostLateUpdate, cancellation);
            }
        }
        
        private static async void Sync() {
            byte[] buffer = new byte[_BUFFER_SIZE];
            
            NetWriteCommand[] writes;
            
            if (_actions.Count > 0) {
                List<NetWriteCommand> writesList = new List<NetWriteCommand>(_COMMAND_BUFFER_SIZE);
                
                foreach (NetActionCommand action in _actions) {
                    if (_handler.TryHandle(action, out NetWriteCommand[] result)) {
                        writesList.AddRange(result);
                    }
                }
                
                if (writesList.Count > 0) {
                    writes = new NetWriteCommand[writesList.Count];
                    writesList.CopyTo(writes);
                } else {
                    writes = null;
                }
            } else {
                writes = null;
            }
            
            int size = new NetMessage(_uid, _version, _csrf, writes).ToBytes(buffer);
            _sendCount++;
            
            _socket.SendTo(buffer, size, SocketFlags.None, _serverPoint);
            
            EndPoint listenPoint = new IPEndPoint(IPAddress.Any, 0);
            Task<SocketReceiveFromResult> receive = _socket.ReceiveFromAsync(buffer, SocketFlags.None, listenPoint);
            
            await Task.WhenAny(receive, Task.Delay(_socket.ReceiveTimeout));
            
            _sendCount--;
            
            if (receive.IsCompleted == false) {
                return;
            }
            
            if (receive.Result.ReceivedBytes <= 0) {
                return;
            }
            
            try {
                NetMessage message = buffer.ToMessage();
                
                if (message.actions != null) {
                    _actions.AddRange(message.actions);
                }
            } catch (Exception exception) {
                Debug.LogWarning(new Exception("NetService.Sync - Response read", exception));
            }
        }
    }
}