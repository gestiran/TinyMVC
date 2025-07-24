// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using TinyMVC.Boot.Helpers;
using UnityEngine.LowLevel;

namespace TinyMVC.Loop {
    internal static class LoopUtility {
    #if UNITY_EDITOR
        
        static LoopUtility() => UnityEditor.EditorApplication.playModeStateChanged += PlayModeStateChanged;
        
        private static void PlayModeStateChanged(UnityEditor.PlayModeStateChange state) {
            if (state != UnityEditor.PlayModeStateChange.ExitingPlayMode) {
                return;
            }
            
            PlayerLoop.SetPlayerLoop(PlayerLoop.GetDefaultPlayerLoop());
        }
        
    #endif
        
        private const string _FIXED_TICK = "FixedUpdate";
        private const string _TICK = "Update";
        private const string _LATE_TICK = "PostLateUpdate";
        
        public static void ApplyLoop(Action tick, Action fixedTick, Action lateTick) {
            PlayerLoopSystem current = PlayerLoop.GetCurrentPlayerLoop();
            
            PlayerLoopSystem[] systems = current.subSystemList;
            
            // for (int i = 0; i < systems.Length; i++) {
            //     string name = systems[i].type.Name;
            //     
            //     if (name == _FIXED_TICK) {
            //         systems[i].subSystemList = AddSystem(systems[i].subSystemList, CreateSystem(fixedTick, typeof(LoopContext.FixedTickContext)));
            //     } else if (name == _TICK) {
            //         systems[i].subSystemList = AddSystem(systems[i].subSystemList, CreateSystem(tick, typeof(LoopContext.TickContext)));
            //     } else if (name == _LATE_TICK) {
            //         systems[i].subSystemList = AddSystem(systems[i].subSystemList, CreateSystem(lateTick, typeof(LoopContext.LateTickContext)));
            //     }
            // }
            
            PlayerLoop.SetPlayerLoop(current);
        }
        
        private static PlayerLoopSystem[] AddSystem(PlayerLoopSystem[] systems, PlayerLoopSystem system) {
            PlayerLoopSystem[] newSystems = new PlayerLoopSystem[systems.Length + 1];
            
            Array.Copy(systems, newSystems, systems.Length);
            newSystems[systems.Length] = system;
            
            return newSystems;
        }
        
        private static PlayerLoopSystem CreateSystem(Action loop, Type type) {
            PlayerLoopSystem system = new PlayerLoopSystem();
            
            system.updateDelegate = loop.Invoke;
            system.type = type;
            
            return system;
        }
    }
}