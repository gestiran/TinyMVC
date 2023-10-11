using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.LowLevel;

namespace TinyMVC.Utilities.Updating {
    public static class UpdateLoopUtility {
        private static PlayerLoopSystem _currentLoop;
        private static readonly PlayerLoopSystem _defaultLoop;

        static UpdateLoopUtility() {
            _currentLoop = PlayerLoop.GetCurrentPlayerLoop();
            _defaultLoop = PlayerLoop.GetDefaultPlayerLoop();
            
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeChanged;
        #endif
        }
        
        public static bool TryAddSystem<T>(T loop) {
            if (loop is ITick tick) {
                AddSystem(loop.GetType(), tick.Tick);
                return true;
            }

            return false;
        }

        public static bool TryRemoveSystem<T>(T loop) {
            if (loop is ITick) {
                RemoveSystem(loop.GetType());
                return true;
            }

            return false;
        }

        public static void AddSystem<T>(T loop) where T : ITick {
            Type type = loop.GetType();

            if (type.IsInterface) {
                Debug.LogWarning("Can`t add interface to loop!");
                return;
            }
            
            AddSystem(type, loop.Tick);
        }

        public static void RemoveSystem<T>(T loop) where T : ITick {
            Type type = loop.GetType();

            if (type.IsInterface) {
                Debug.LogWarning("Can`t remove interface from loop!");
                return;
            }

            RemoveSystem(type);
        }

        public static void ReturnDefaultLoop() => PlayerLoop.SetPlayerLoop(_defaultLoop);

        private static void AddSystem(Type type, Action loop) {
            PlayerLoopSystem[] currentSystems = _currentLoop.subSystemList;
            
            PlayerLoopSystem system = new PlayerLoopSystem();
            system.type = type;
            system.updateDelegate = loop.Invoke;
            
            Type updateType = typeof(UnityEngine.PlayerLoop.Update);
            Type behaviourUpdateType = typeof(UnityEngine.PlayerLoop.Update.ScriptRunBehaviourUpdate);

            for (int systemId = 0; systemId < currentSystems.Length; systemId++) {
                if (currentSystems[systemId].type != updateType) {
                    continue;
                }
                
                List<PlayerLoopSystem> subSystems = currentSystems[systemId].subSystemList.ToList();
                
                for (int subSystemId = 0; subSystemId < subSystems.Count; subSystemId++) {
                    if (subSystems[subSystemId].type != behaviourUpdateType) {
                        continue;
                    }
                    
                    subSystems.Insert(subSystemId + 1, system);
                    break;
                }

                currentSystems[systemId].subSystemList = subSystems.ToArray();
                break;
            }
            
            _currentLoop.subSystemList = currentSystems;
            PlayerLoop.SetPlayerLoop(_currentLoop);
        }

        private static void RemoveSystem(Type type) {
            Type updateType = typeof(UnityEngine.PlayerLoop.Update);
            
            PlayerLoopSystem[] currentSystems = _currentLoop.subSystemList;
            
            for (int systemId = 0; systemId < currentSystems.Length; systemId++) {
                if (currentSystems[systemId].type != updateType) {
                    continue;
                }

                List<PlayerLoopSystem> subSystems = currentSystems[systemId].subSystemList.ToList();
                
                for (int subSystemId = 0; subSystemId < subSystems.Count; subSystemId++) {
                    if (subSystems[subSystemId].type != type) {
                        continue;
                    }
                    
                    subSystems.RemoveAt(subSystemId);
                    break;
                }

                currentSystems[systemId].subSystemList = subSystems.ToArray();
                break;
            }
            
            _currentLoop.subSystemList = currentSystems;
            PlayerLoop.SetPlayerLoop(_currentLoop);
        }
        
    #if UNITY_EDITOR
        private static void OnPlayModeChanged(UnityEditor.PlayModeStateChange obj) {
            if (obj != UnityEditor.PlayModeStateChange.ExitingPlayMode) {
                return;
            }
            
            ReturnDefaultLoop();
        }
        
        public static void DisplaySystemsEditor() {
            StringBuilder log = new StringBuilder(100);
            PlayerLoopSystem[] currentSystems = _currentLoop.subSystemList;

            for (int systemId = 0; systemId < currentSystems.Length; systemId++) {
                log.Append($"ID:{systemId}\t= {currentSystems[systemId].type}\n");
                
                PlayerLoopSystem[] subSystems = currentSystems[systemId].subSystemList;

                if (subSystems == null) {
                    continue;
                }

                if (subSystems.Length <= 0) {
                    continue;
                }
                
                for (int subSystemId = 0; subSystemId < subSystems.Length; subSystemId++) {
                    log.Append($"ID:{systemId}.{subSystemId}\t= {subSystems[subSystemId].type}\n");
                }
            }
            
            Debug.LogWarning(log.ToString());
        }
    #endif
    }
}