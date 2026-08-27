// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TinyUtilities.Roslyn;
using TinyUtilities.Roslyn.Extensions;

namespace TinyMVC.Roslyn.Loop {
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InitRequireFixProvider)), Shared]
    public sealed class InitRequireFixProvider : InterfaceRequireFixProvider {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(Labels.ID_INIT);
        
        protected override string _title { get; }
        protected override string _key { get; }
        protected override string _namespace { get; }
        
        private readonly string _interfaceName;
        
        public InitRequireFixProvider() {
            _title = "Add IInit interface";
            _key = nameof(InitRequireFixProvider);
            _namespace = "TinyMVC.Loop";
            _interfaceName = "IInit";
        }
        
        protected override ClassDeclarationSyntax ApplyFix(ClassDeclarationSyntax declaration, SemanticModel semantic) {
            ClassDeclarationSyntax newClassDeclaration;
            
            if (declaration.BaseList == null) {
                newClassDeclaration = declaration.AddInterface(_interfaceName);
            } else if (declaration.BaseList.Types.TryFindAnyPlace(out int placeId, "IController")) {
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