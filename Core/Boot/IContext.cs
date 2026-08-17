// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TinyMVC.Controllers;
using TinyReactive;

namespace TinyMVC.Boot {
    public interface IContext : IUnloadLink {
        public string key { get; }
        public CancellationToken cancellation { get; }
        
        internal Dictionary<IController, UnloadPool> unloads { get; }
        
        internal void Create();
        
        internal Task InitAsync();
        
        internal Task Remove();
        
        internal void Connect<T1, T2>(T2 system, T1 controller) where T1 : IController where T2 : IController;
        
        internal void Disconnect<T1, T2>(T2 system, T1 controller) where T1 : IController where T2 : IController;
    }
}