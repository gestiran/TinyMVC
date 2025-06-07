using NetworkTypes.Commands;

namespace TinyMVC.Modules.Networks.Handlers {
    public interface INetLogicHandler {
        public bool TryHandle(NetActionCommand command, out NetWriteCommand[] result);
    }
}