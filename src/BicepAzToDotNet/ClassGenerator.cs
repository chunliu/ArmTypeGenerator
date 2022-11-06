using Azure.Bicep.Types.Concrete;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BicepAzToDotNet
{
    internal class ClassGenerator : TypeGenerator
    {
        public ClassGenerator(string className, TypeBase typeBase)
            : base(className, typeBase)
        {
        }

        public override void AddMembers()
        {
            throw new NotImplementedException();
        }

        public override BaseTypeDeclarationSyntax GenerateTypeDeclaration()
        {
            var classDeclaration = SyntaxFactory.ClassDeclaration(TypeName)
                .AddAttributeLists()
                .AddModifiers(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.PartialKeyword)
                );

            AddUsing("System");

            return classDeclaration;
        }
    }
}
