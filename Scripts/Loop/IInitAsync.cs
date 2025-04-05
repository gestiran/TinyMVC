using Cysharp.Threading.Tasks;

namespace TinyMVC.Loop {
    public interface IInitAsync {
        public UniTask Init();
    }
}