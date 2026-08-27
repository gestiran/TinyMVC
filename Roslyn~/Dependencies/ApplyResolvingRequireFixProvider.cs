// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TinyUtilities.Roslyn;
using TinyUtilities.Roslyn.Extensions;

namespace TinyMVC.Roslyn.Dependencies {
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ApplyResolvingRequireFixProvider)), Shared]
    public sealed class ApplyResolvingRequireFixProvider : InterfaceRequireFixProvider {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Labels.ID_APPLY_RESOLVING);
        
        protected override string _title { get; }
        protected override string _key { get; }
        protected override string _namespace { get; }
        
        private readonly string _interfaceName;
        
        public ApplyResolvingRequireFixProvider() {
            _title = "Add IApplyResolving interface";
            _key = nameof(ApplyResolvingRequireFixProvider);
            _namespace = "TinyMVC.Dependencies";
            _interfaceName = "IApplyResolving";
        }
        
        protected override ClassDeclarationSyntax ApplyFix(ClassDeclarationSyntax declaration, SemanticModel semantic) {
            ClassDeclarationSyntax newClassDeclaration;
            
            if (declaration.BaseList == null) {
                newClassDeclaration = declaration.AddInterface(_interfaceName);
            } else if (declaration.BaseList.Types.TryFindAnyPlace(out int placeId, "IController","IInit")) {
                newClassDeclaration = declaration.InsertInterface(_interfaceName, placeId + 1);
            } else if (declaration.IsHaveParentClass(semantic)) {
                newClassDeclaration = declaration.InsertInterface(_interfaceName, 1);
            } else {
                newClassDeclaration = declaration.InsertInterface(_interfaceName, 0);
            }
            
            return newClassDeclaration;
        }
    }
}