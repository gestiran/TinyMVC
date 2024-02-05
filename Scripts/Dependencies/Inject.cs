using System;

namespace TinyMVC.Dependencies {
    /// <summary> Inject field value in initialization </summary>
    /// <remarks> Required <see cref="IResolving"/> or <see cref="IApplyResolving"/> interface </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class Inject : Attribute { }
}