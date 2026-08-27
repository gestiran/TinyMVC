// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TinyUtilities.Roslyn;

namespace TinyMVC.Roslyn.Loop {
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class FixedTickRequireAnalyzer : InterfaceRequireAnalyser {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);
        protected override string _methodName { get; }
        protected override string _interfaceName { get; }
        protected override DiagnosticDescriptor _rule { get; }
        
        private const string _TITLE = "Missing IFixedTick interface";
        private const string _MESSAGE_FORMAT = "Class '{0}' has FixedTick method but does not implement IFixedTick";
        
        public FixedTickRequireAnalyzer() {
            _methodName = "FixedTick";
            _interfaceName = "IFixedTick";
            _rule = new DiagnosticDescriptor(Labels.ID_FIXED_TICK, _TITLE, _MESSAGE_FORMAT, Labels.CATEGORY, DiagnosticSeverity.Warning, true);
        }
    }
}