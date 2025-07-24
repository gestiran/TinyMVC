// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using Cysharp.Threading.Tasks;

namespace TinyMVC.Loop {
    public interface IInitAsync {
        public UniTask Init();
    }
}