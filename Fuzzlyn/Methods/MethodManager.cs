﻿using Fuzzlyn.Statics;
using Fuzzlyn.Types;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Fuzzlyn.Methods
{
    internal class MethodManager
    {
        private readonly List<FuncGenerator> _funcs = new();

        public MethodManager(Randomizer random, TypeManager types, StaticsManager statics)
        {
            Random = random;
            Types = types;
            Statics = statics;
        }

        public Randomizer Random { get; }
        public TypeManager Types { get; }
        public StaticsManager Statics { get; }

        internal void GenerateMethods(Func<string> genChecksumSiteId)
        {
            FuncGenerator gen = new(_funcs, Random, Types, Statics, genChecksumSiteId);
            gen.Generate(null, false);
        }

        internal (List<MethodDeclarationSyntax> StaticFuncs, Dictionary<FuzzType, List<MethodDeclarationSyntax>> TypeMethods) OutputMethods()
        {
            var staticFuncs = new List<MethodDeclarationSyntax>();
            var typeMethods = new Dictionary<FuzzType, List<MethodDeclarationSyntax>>();
            foreach (FuzzType type in ((IEnumerable<FuzzType>)Types.AggregateTypes).Concat(Types.InterfaceTypes))
            {
                typeMethods.Add(type, new List<MethodDeclarationSyntax>());
            }

            foreach (FuncGenerator func in _funcs)
            {
                func.Output(staticFuncs, typeMethods);
            }

            return (staticFuncs, typeMethods);
        }
    }
}
