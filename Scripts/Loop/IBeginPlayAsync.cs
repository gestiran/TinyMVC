using Cysharp.Threading.Tasks;

namespace TinyMVC.Loop {
    public interface IBeginPlayAsync {
        public UniTask BeginPlay();
    }
}