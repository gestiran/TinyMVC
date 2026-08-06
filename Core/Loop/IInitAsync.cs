// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Threading.Tasks;

namespace TinyMVC.Loop {
    public interface IInitAsync {
        public Task Init();
    }
}