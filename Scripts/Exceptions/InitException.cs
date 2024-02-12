﻿#if UNITY_EDITOR || DEVELOPMENT_BUILD
    using System;
    using TinyMVC.Loop;

    namespace TinyMVC.Exceptions {
        public sealed class InitException : Exception {
            public readonly IInit other;

            public InitException(IInit other, Exception innerException) : base($"Init.Error: {LogUtility.Link(other)}", innerException) => this.other = other;
        }
    }
#endif