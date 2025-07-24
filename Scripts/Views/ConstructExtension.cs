// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyMVC.Views {
    public static class ConstructExtension {
        public static void Construct<TConstruct, T>(this TConstruct[] constructs, T arg) where TConstruct : IConstruct<T> {
            for (int i = 0; i < constructs.Length; i++) {
                constructs[i].Construct(arg);
            }
        }
        
        public static void Construct<TConstruct, T1, T2>(this TConstruct[] constructs, T1 arg1, T2 arg2) where TConstruct : IConstruct<T1, T2> {
            for (int i = 0; i < constructs.Length; i++) {
                constructs[i].Construct(arg1, arg2);
            }
        }
        
        public static void Construct<TConstruct, T1, T2, T3>(this TConstruct[] constructs, T1 arg1, T2 arg2, T3 arg3) where TConstruct : IConstruct<T1, T2, T3> {
            for (int i = 0; i < constructs.Length; i++) {
                constructs[i].Construct(arg1, arg2, arg3);
            }
        }
        
        public static void Construct<TConstruct, T1, T2, T3, T4>(this TConstruct[] constructs, T1 arg1, T2 arg2, T3 arg3, T4 arg4) where TConstruct : IConstruct<T1, T2, T3, T4> {
            for (int i = 0; i < constructs.Length; i++) {
                constructs[i].Construct(arg1, arg2, arg3, arg4);
            }
        }
    }
}