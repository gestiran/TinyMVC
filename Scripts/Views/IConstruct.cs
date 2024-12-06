namespace TinyMVC.Views {
    public interface IConstruct { }
    
    public interface IConstruct<in T> : IConstruct {
        public void Construct(T arg);
    }
    
    public interface IConstruct<in T1, in T2> : IConstruct {
        public void Construct(T1 arg1, T2 arg2);
    }
    
    public interface IConstruct<in T1, in T2, in T3> : IConstruct {
        public void Construct(T1 arg1, T2 arg2, T3 arg3);
    }
    
    public interface IConstruct<in T1, in T2, in T3, in T4> : IConstruct {
        public void Construct(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }
}