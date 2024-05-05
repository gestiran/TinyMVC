﻿using System;
using TinyMVC.ApplicationLevel.Saving.Serialization;
using Sirenix.Serialization;

[assembly: RegisterFormatterLocator(typeof (VFileFormatterLocator))]

namespace TinyMVC.ApplicationLevel.Saving.Serialization {
    public sealed class VFileFormatterLocator : IFormatterLocator {
        private const string _TYPE_NAME = "TinyMVC.ApplicationLevel.Saving.VirtualFiles.VFile";
        
        public bool TryGetFormatter(Type type, FormatterLocationStep step, ISerializationPolicy policy, bool allowWeakFallbackFormatters, out IFormatter formatter) {
            if (type.FullName == _TYPE_NAME) {
                formatter = new VFileFormatter();
                return true;
            }

            formatter = null;
            return false;
        }
    }
}