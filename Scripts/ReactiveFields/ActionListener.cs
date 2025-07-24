// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyMVC.ReactiveFields {
    public delegate void ActionListener();
    public delegate void ActionListener<in T>(T value);
    public delegate void ActionListener<in T1, in T2>(T1 value1, T2 value2);
    public delegate void ActionListener<in T1, in T2, in T3>(T1 value1, T2 value2, T3 value3);
}