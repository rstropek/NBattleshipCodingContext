namespace NBattleshipCodingContest.PlayersGenerator.Tests
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;
    using Xunit;

    public class PlayersGeneratorTest
    {
        [Fact]
        public void Initialize()
        {
            var generator = new PlayersGenerator();

            SyntaxReceiverCreator? receiver = null;
            generator.InitializeImpl((r) => receiver = r);

            // Make sure that syntax receiver is registered during initialization
            Assert.NotNull(receiver);
            Assert.IsAssignableFrom<PlayersGenerator.SyntaxReceiver>(receiver!());
        }

        private const string Code = @"
namespace NBattleshipCodingContest.Players
{
    using System;

    public abstract class PlayerBase { }

    public class IgnoreAttribute : Attribute { }

    public abstract class PlayerBase2 { }

    public class MyPlayer : PlayerBase { }

    [IgnoreAttribute]
    public class MyPlayer2 : PlayerBase { }

    public class MyNonPlayer : PlayerBase2 { }
}
";

        private static (SyntaxTree, Compilation) Compile(string code)
        {
            var tree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("TestPlayerGenerator")
                .AddReferences(new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) })
                .AddSyntaxTrees(tree);
            return (tree, compilation);
        }

        private class ClassCollector : CSharpSyntaxWalker
        {
            private readonly PlayersGenerator.SyntaxReceiver receiver;

            public ClassCollector(PlayersGenerator.SyntaxReceiver receiver) => this.receiver = receiver;

            public override void VisitClassDeclaration(ClassDeclarationSyntax classDeclarationSyntax) => 
                receiver.OnVisitSyntaxNode(classDeclarationSyntax);
        }

        private PlayersGenerator.SyntaxReceiver FillReceiver(SyntaxTree tree)
        {
            var receiver = new PlayersGenerator.SyntaxReceiver();
            var collector = new ClassCollector(receiver);
            collector.Visit(tree.GetRoot());
            return receiver;
        }

        [Fact]
        public void SyntaxReceiver()
        {
            var (tree, _) = Compile(Code);
            var receiver = FillReceiver(tree);
            Assert.NotEmpty(receiver.CandidateClasses);
        }

        [Fact]
        public void Generate()
        {
            var (tree, compilation) = Compile(Code);
            var receiver = FillReceiver(tree);

            var generator = new PlayersGenerator();
            SourceText? sourceText = null;
            generator.ExecuteImpl(receiver.CandidateClasses, compilation, st => sourceText = st);

            Assert.NotNull(sourceText);
            Assert.Contains("MyPlayer", sourceText!.ToString());
        }
    }
}
