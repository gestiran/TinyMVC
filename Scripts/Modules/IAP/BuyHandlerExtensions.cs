// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;

namespace TinyMVC.Modules.IAP {
    public static class BuyHandlerExtensions {
        public static List<BuyHandler> Add(this List<BuyHandler> list, BuyHandler[] handlers) {
            list.Capacity += handlers.Length;
            
            for (int handlerId = 0; handlerId < handlers.Length; handlerId++) {
                list.Add(handlers[handlerId]);
            }
            
            return list;
        }
        
        public static void Purchase(this BuyHandler[] handlers, bool markAsPurchased = true) {
            for (int handlerId = 0; handlerId < handlers.Length; handlerId++) {
                handlers[handlerId].Purchase(markAsPurchased);
            }
        }
        
        public static void Restore(this BuyHandler[] handlers, bool markAsPurchased = true) {
            for (int handlerId = 0; handlerId < handlers.Length; handlerId++) {
                handlers[handlerId].Restore(markAsPurchased);
            }
        }
        
        public static void Confiscate(this BuyHandler[] handlers) {
            for (int handlerId = 0; handlerId < handlers.Length; handlerId++) {
                handlers[handlerId].Confiscate();
            }
        }
        
        public static bool IsPurchased(this BuyHandler[] handlers) {
            for (int handlerId = 0; handlerId < handlers.Length; handlerId++) {
                if (handlers[handlerId].IsPurchased()) {
                    return true;
                }
            }
            
            return false;
        }
    }
}