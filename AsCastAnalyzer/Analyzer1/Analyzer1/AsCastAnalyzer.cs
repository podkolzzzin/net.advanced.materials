using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Analyzer1
{
    class Wrong
    {
        public int M(object x)
        {

            string str;
            str = x as string;
            return str.Length;
        }

        public int M3(object x)
        {

            var str = x as string;
            if (str == null)
                return str.Length;
            return 0;
        }
    }


    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AsCastAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AsCastAnalyzer";
        internal static readonly LocalizableString Title = "AsCastAnalyzer Title";
        internal static readonly LocalizableString MessageFormat = "AsCastAnalyzer '{0}'";
        internal const string Category = "AsCastAnalyzer Category";

        private HashSet<ISymbol> asAsignedSymbols = new HashSet<ISymbol>();

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeNode, 
                SyntaxKind.SimpleAssignmentExpression, SyntaxKind.VariableDeclarator);
            context.RegisterSyntaxNodeAction(AnalyzeAccess, SyntaxKind.SimpleMemberAccessExpression);
        }

        private void AnalyzeAccess(SyntaxNodeAnalysisContext obj)
        {
            var symbol = obj.SemanticModel.GetSymbolInfo(obj.Node);
            MemberAccessExpressionSyntax node = obj.Node as MemberAccessExpressionSyntax;
            CheckSymbol(obj, symbol);
            CheckSymbol(obj, obj.SemanticModel.GetSymbolInfo(node.Expression));
        }

        private void CheckSymbol(SyntaxNodeAnalysisContext obj, SymbolInfo symbol)
        {
            if (asAsignedSymbols.Contains(symbol.Symbol))
            {
                var diagnostic = Diagnostic.Create(Rule, obj.Node.GetLocation(), symbol.Symbol.Name);
                obj.ReportDiagnostic(diagnostic);
            }
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext obj)
        {
            if (obj.Node is VariableDeclaratorSyntax vds)
            {
                if (vds.Initializer.Value.IsKind(SyntaxKind.AsExpression))
                {
                    var candidate = obj.SemanticModel.GetDeclaredSymbol(vds);
                    asAsignedSymbols.Add(candidate);
                }
            }
            else if (obj.Node is AssignmentExpressionSyntax aes)
            {
                if (aes.Right.IsKind(SyntaxKind.AsExpression))
                {
                    var candidate = obj.SemanticModel.GetSymbolInfo(aes.Left);
                    asAsignedSymbols.Add(candidate.Symbol);
                }
            }
        }
    }
}