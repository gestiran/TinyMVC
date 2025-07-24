// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using NetworkTypes.Commands;

namespace TinyMVC.Modules.Networks.Handlers {
    public interface INetLogicHandler {
        public bool TryHandle(NetActionCommand command, out NetWriteCommand[] result);
    }
}