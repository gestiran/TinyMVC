using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NetworkTypes;
using NetworkTypes.Commands;
using TinyMVC.Modules.Networks.Commands;
using TinyMVC.ReactiveFields;
using TinyMVC.ReactiveFields.Extensions;
using TinySerializer.Core.Misc;
using UnityEngine;

namespace TinyMVC.Modules.Networks {
    public static class NetService {
        private static IPAddress _serverIp;
        private static int _serverPort;
        private static uint _uid;
        private static uint _csrf;
        private static bool _isInitialized;
        
        private static readonly UdpClient _udp;
        private static readonly List<NetWriter> _bufferWrite;
        private static readonly List<NetReader> _bufferRead;
        
        private const int _BUFFER_SIZE = 256;
        private const int _RECEIVE_TIMEOUT = 1000;
        
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
        
        internal static void Write<T>(T value, ushort key, byte[] group) {
            for (int writeId = 0; writeId < _bufferWrite.Count; writeId++) {
                if (_bufferWrite[writeId].IsCurrent(key, group) == false) {
                    continue;
                }
                
                _bufferWrite[writeId].value = value;
                return;
            }
            
            _bufferWrite.Add(new NetWriter(value, key, group));
        }
        
        internal static void AddRead(ActionListener<object> listener, ushort key, params byte[] group) {
            for (int bufferId = 0; bufferId < _bufferRead.Count; bufferId++) {
                if (_bufferRead[bufferId].IsCurrent(key, group) == false) {
                    continue;
                }
                
                _bufferRead[bufferId].listeners.Add(listener);
                return;
            }
            
            _bufferRead.Add(new NetReader(listener, key, group));
        }
        
        internal static void RemoveRead(ActionListener<object> listener, ushort key, params byte[] group) {
            for (int bufferId = 0; bufferId < _bufferRead.Count; bufferId++) {
                if (_bufferRead[bufferId].IsCurrent(key, group) == false) {
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
                if (_isInitialized) {
                    try {
                        Sync();
                    } catch (Exception exception) {
                        Debug.LogWarning(exception);
                    }
                }
                
                await UniTask.Yield();
            }
        }
        
        private static async void Sync() {
            NetReadCommand[] readCommands = GetReadCommands();
            NetWriteCommand[] writeCommands = GetWriteCommands();
            
            if (readCommands == null && writeCommands == null) {
                return;
            }
            
            NetMessage messageData = new NetMessage(_uid, _csrf, readCommands, writeCommands);
            
            byte[] data = SerializationUtility.SerializeValue(messageData, DataFormat.Binary);
            
            await _udp.SendAsync(data, data.Length, new IPEndPoint(_serverIp, _serverPort));
            
            if (readCommands == null) {
                return;
            }
            
            Task<UdpReceiveResult> receive = _udp.ReceiveAsync();
            await Task.WhenAny(receive, Task.Delay(_RECEIVE_TIMEOUT));
            
            if (receive.IsCompletedSuccessfully == false) {
                return;
            }
            
            if (receive.Result.Buffer.Length <= 0) {
                return;
            }
            
            NetMessage message = SerializationUtility.DeserializeValue<NetMessage>(receive.Result.Buffer, DataFormat.Binary);
            
            if (message.write == null) {
                return;
            }
            
            for (int commandId = 0; commandId < message.write.Length; commandId++) {
                NetWriteCommand command = message.write[commandId];
                
                for (int bufferId = 0; bufferId < _bufferRead.Count; bufferId++) {
                    if (_bufferRead[bufferId].IsCurrent(command.key, command.group)) {
                        _bufferRead[bufferId].listeners.Invoke(command.value);
                    }
                }
            }
        }
        
        private static NetReadCommand[] GetReadCommands() {
            if (_bufferRead.Count == 0) {
                return null;
            }
            
            NetReadCommand[] commands = new NetReadCommand[_bufferRead.Count];
            
            for (int bufferId = 0; bufferId < _bufferRead.Count; bufferId++) {
                commands[bufferId] = new NetReadCommand(_bufferRead[bufferId].key, _bufferRead[bufferId].group);
            }
            
            return commands;
        }
        
        private static NetWriteCommand[] GetWriteCommands() {
            if (_bufferWrite.Count == 0) {
                return null;
            }
            
            NetWriteCommand[] commands = new NetWriteCommand[_bufferWrite.Count];
            
            for (int bufferId = 0; bufferId < _bufferWrite.Count; bufferId++) {
                commands[bufferId] = new NetWriteCommand(_bufferWrite[bufferId].value, _bufferWrite[bufferId].key, _bufferWrite[bufferId].group);
            }
            
            _bufferWrite.Clear();
            return commands;
        }
    }
}