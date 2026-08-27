// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TinyUtilities.Roslyn;

namespace TinyMVC.Roslyn.Loop {
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class InitRequireAnalyzer : InterfaceRequireAnalyser {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);
        protected override string _methodName { get; }
        protected override string _interfaceName { get; }
        protected override DiagnosticDescriptor _rule { get; }
        
        private const string _TITLE = "Missing IInit interface";
        private const string _MESSAGE_FORMAT = "Class '{0}' has Init method but does not implement IInit";
        
        public InitRequireAnalyzer() {
            _methodName = "Init";
            _interfaceName = "IInit";
            _rule = new DiagnosticDescriptor(Labels.ID_INIT, _TITLE, _MESSAGE_FORMAT, Labels.CATEGORY, DiagnosticSeverity.Warning, true);
        }
    }
}