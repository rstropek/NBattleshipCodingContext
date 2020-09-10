namespace NBattleshipCodingContest.PlayersGenerator
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Generates a static list of battleship players.
    /// </summary>
    /// <remarks>
    /// This generator makes runtime reflection for identifying battleship player no
    /// longer necessary. Leads to faster startup.
    /// </remarks>
    [Generator]
    public class PlayersGenerator : ISourceGenerator
    {
        private class SyntaxReceiver : ISyntaxReceiver
        {
            public List<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                // Players must derive from PlayerBase -> candidate classes must
                // have at least one base type.
                if (syntaxNode is ClassDeclarationSyntax cds && cds.BaseList != null && cds.BaseList.Types.Any())
                {
                    CandidateClasses.Add(cds);
                }
            }
        }

        public void Initialize(InitializationContext context)
        {
            // Register a syntax receiver that will be created for each generation pass
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(SourceGeneratorContext context)
        {
            // retreive the populated receiver 
            if (!(context.SyntaxReceiver is SyntaxReceiver receiver))
                return;

            // begin creating the source we'll inject into the users compilation
            StringBuilder sourceBuilder = new(@"
namespace NBattleshipCodingContest.Players
{
    using System;

    public static class PlayerList
    {
        public static readonly PlayerBase[] Players = new PlayerBase[] 
        {
");

            // Mandator base class for players
            var playerBaseSymbol = context.Compilation.GetTypeByMetadataName("NBattleshipCodingContest.Players.PlayerBase");
            if (playerBaseSymbol == null)
            {
                return;
            }

            // Attribute used to ignore specific players (e.g. players that crash or are not finished)
            var playerIgnoreSymbol = context.Compilation.GetTypeByMetadataName("NBattleshipCodingContest.Players.IgnoreAttribute");
            if (playerIgnoreSymbol == null)
            {
                return;
            }

            var playerClasses = new List<INamedTypeSymbol>();
            foreach (var f in receiver.CandidateClasses)
            {
                var model = context.Compilation.GetSemanticModel(f.SyntaxTree);
                if (model.GetDeclaredSymbol(f) is INamedTypeSymbol ti && ti != null)
                {
                    // Check whether the class has the correct base class
                    if (ti.BaseType != null && ti.BaseType.Equals(playerBaseSymbol, SymbolEqualityComparer.Default))
                    {
                        // Check whether the class has no ignore attribute
                        if (!ti.GetAttributes().Any(a => a.AttributeClass?.Equals(playerIgnoreSymbol, SymbolEqualityComparer.Default) ?? false))
                        {
                            // Found a player
                            playerClasses.Add(ti);
                        }
                    }
                }
            }

            // Add all players to generated code
            sourceBuilder.Append(string.Join(",", playerClasses.Select(c => $"new {c.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}()")));

            // finish creating the source to inject
            sourceBuilder.Append(@"
        };
    }
}");

            // inject the created source into the users compilation
            context.AddSource("PlayersGenerated", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }
    }
}
