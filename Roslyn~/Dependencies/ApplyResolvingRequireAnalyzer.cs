// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

﻿using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TinyUtilities.Roslyn;

namespace TinyMVC.Roslyn.Dependencies {
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ApplyResolvingRequireAnalyzer : InterfaceRequireAnalyser {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);
        protected override string _methodName { get; }
        protected override string _interfaceName { get; }
        protected override DiagnosticDescriptor _rule { get; }
        
        private const string _TITLE = "Missing IApplyResolving interface";
        private const string _MESSAGE_FORMAT = "Class '{0}' has ApplyResolving method but does not implement IApplyResolving";
        
        public ApplyResolvingRequireAnalyzer() {
            _methodName = "ApplyResolving";
            _interfaceName = "IApplyResolving";
            _rule = new DiagnosticDescriptor(Labels.ID_APPLY_RESOLVING, _TITLE, _MESSAGE_FORMAT, Labels.CATEGORY, DiagnosticSeverity.Warning, true);
        }
    }
}