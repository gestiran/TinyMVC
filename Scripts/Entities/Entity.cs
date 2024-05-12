using System.Collections.Generic;

namespace TinyMVC.Entities {
    public abstract class Entity {
        private readonly List<IComponentData> _components;
        
        private const int _CAPACITY = 8;

        internal Entity() => _components = new List<IComponentData>(_CAPACITY);

        public void Add<T>(T component) where T : IComponentData => _components.Add(component);

        public void Remove<T>(T component) where T : IComponentData => _components.Remove(component);

        public void Clear() => _components.Clear();
        
        internal IEnumerable<T> ForEach<T>() where T : IComponentData {
            foreach (IComponentData component in _components) {
                if (component is T target) {
                    yield return target;
                    break;
                }
            }
        }
        
        internal IEnumerable<(T1, T2)> ForEach<T1, T2>() where T1 : IComponentData where T2 : IComponentData {
            bool result1 = false;
            T1 component1 = default;
            
            bool result2 = false;
            T2 component2 = default;
            
            foreach (IComponentData component in _components) {
                if (result1 == false && component is T1 target1) {
                    component1 = target1;
                    result1 = true;
                } 
                
                if (result2 == false && component is T2 target2) {
                    component2 = target2;
                    result2 = true;
                }
            }

            if (result1 && result2) {
                yield return (component1, component2);
            }
        }
        
        internal IEnumerable<(T1, T2, T3)> ForEach<T1, T2, T3>() where T1 : IComponentData where T2 : IComponentData where T3 : IComponentData {
            bool result1 = false;
            T1 component1 = default;
            
            bool result2 = false;
            T2 component2 = default;
            
            bool result3 = false;
            T3 component3 = default;
            
            foreach (IComponentData component in _components) {
                if (result1 == false && component is T1 target1) {
                    component1 = target1;
                    result1 = true;
                } 
                
                if (result2 == false && component is T2 target2) {
                    component2 = target2;
                    result2 = true;
                }
                
                if (result3 == false && component is T3 target3) {
                    component3 = target3;
                    result3 = true;
                }
            }

            if (result1 && result2) {
                yield return (component1, component2, component3);
            }
        }
    }
}