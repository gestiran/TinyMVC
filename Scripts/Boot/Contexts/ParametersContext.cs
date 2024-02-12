using System.Collections.Generic;
using TinyMVC.Boot.Empty;
using TinyMVC.Dependencies;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using TinyMVC.Exceptions;
#endif

namespace TinyMVC.Boot.Contexts {
    /// <summary> Contains parameters initialization </summary>
    public abstract class ParametersContext {
        private readonly List<IDependency> _parameters;

        protected ParametersContext() => _parameters = new List<IDependency>();

        public static ParametersEmptyContext Empty() => new ParametersEmptyContext();

        internal void Create() {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
            try {
            #endif

                Create(_parameters);

            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            } catch (Exception exception) {
                throw new ParametersException(exception);
            }
        #endif
        }

        internal void AddDependencies(List<IDependency> dependencies) => dependencies.AddRange(_parameters);
        
        /// <summary>  Create parameters and connect initialization  </summary>
        /// <param name="parameters"> Scriptable object data, contain readonly start parameters </param>
        protected abstract void Create(List<IDependency> parameters);
    }
}