using Azure.Bicep.Types.Concrete;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BicepAzToDotNet
{
    public abstract class TypeGenerator
    {
        protected TypeGenerator(string typeName, TypeBase typeBase)
        {
            TypeName = typeName.ToPascalCase();
            TypeBase = typeBase;
        }
        protected string TypeName { get; private set; } = null!;

        /// <summary>
        /// Gets or sets the type declaration being generated.
        /// </summary>
        protected BaseTypeDeclarationSyntax TypeDeclaration { get; set; } = null!;

        protected HashSet<string> Usings { get; private set; } = null!;
        protected TypeBase TypeBase { get; set; } = null!;

        public abstract BaseTypeDeclarationSyntax GenerateTypeDeclaration();

        /// <summary>
        /// Adds members to the type as directed by the schema.
        /// </summary>
        public abstract void AddMembers();

        /// <summary>
        /// Generate the text for a type from a JSON schema.
        /// </summary>
        /// <param name="namespaceName">
        /// The name of the namespace in which to generate the type.
        /// </param>
        /// <param name="typeName">
        /// The unqualified name of the type to generate.
        /// </param>
        /// <param name="copyrightNotice">
        /// The text of the copyright notice to place at the top of the generated file.
        /// </param>
        /// <param name="description">
        /// The text of the summary comment on the type.
        /// </param>
        public string Generate(string namespaceName, string copyrightNotice, string description)
        {
            TypeDeclaration = GenerateTypeDeclaration();

            AddMembers();

            return TypeDeclaration.Format(copyrightNotice, Usings, namespaceName, description);
        }

        protected void AddUsing(string namespaceName)
        {
            Usings ??= new HashSet<string>();

            Usings.Add(namespaceName);
        }
    }
}
