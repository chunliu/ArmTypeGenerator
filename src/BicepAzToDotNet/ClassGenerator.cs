using Azure.Bicep.Types.Concrete;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BicepAzToDotNet
{
    internal class ClassGenerator : TypeGenerator
    {
        private readonly List<string> BasePropsToSkip = new()
        {
            "name",
            "location",
            "tags",
        };
        private readonly List<string> BasePropsToOverride = new()
        {
            "type",
            "apiVersion",
        };
        private readonly List<string> PropsToJsonIgnore = new()
        {
            "id",
            "etag",
        };
        private const string RequiredAttributeNamespaceName = "System.ComponentModel.DataAnnotations";
        private const string RequiredAttributeName = "Required";

        private const string JsonPropertyAttributeNamespaceName = "System.Text.Json.Serialization";
        private const string JsonPropertyAttributeName = "JsonPropertyName";
        private const string JsonIgnoreAttributeName = "JsonIgnore";

        private const string AzureResourceAttributeName = "AzureResource";
        public ClassGenerator(string className, ObjectType objectType, AdditionalTypeRequiredDelegate additionalTypeRequired)
            : base(className, objectType, additionalTypeRequired)
        {
        }

        public override void AddMembers()
        {
            var members = new List<MemberDeclarationSyntax>();

            foreach (var prop in ResourceType.Properties)
            {
                if (IsAzureResource && BasePropsToSkip.Contains(prop.Key))
                {
                    continue;
                }

                members.Add(CreatePropertyDeclaration(prop.Key, prop.Value));
            }

            TypeDeclaration = (TypeDeclaration as ClassDeclarationSyntax)!.AddMembers(members.ToArray());
        }

        private PropertyDeclarationSyntax CreatePropertyDeclaration(string propertyName, ObjectTypeProperty propertyValue)
        {
            var propType = GetPropertyType(propertyValue.Type.Type);

            PropertyDeclarationSyntax propDecl = SyntaxFactory.PropertyDeclaration(propType, propertyName.ToPascalCase())
                .AddModifiers(GeneratePropertyModifiers(propertyName));

            if (propertyValue.Flags.HasFlag(ObjectTypePropertyFlags.ReadOnly)
                && propertyValue.Type.Type is StringLiteralType stringType)
            {
                propDecl = propDecl.WithExpressionBody(GeneratePropertyExpressionBody(stringType.Value))
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            }
            else
            {
                propDecl = propDecl.AddAccessorListAccessors(GeneratePropertyAccessors(propertyName));
            }
            
            AddUsing(JsonPropertyAttributeNamespaceName);

            var attributes = GeneratePropertyAttributes(propertyName, propertyValue);
            if (attributes.Length > 0)
            {
                propDecl = propDecl.AddAttributeLists(attributes
                    .Select(attr => SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(attr)))
                    .ToArray());
            }

            if (!string.IsNullOrEmpty(propertyValue.Description))
            {
                propDecl = propDecl.WithLeadingTrivia(
                        SyntaxHelper.MakeDocComment(propertyValue.Description));
            }

            return propDecl;
        }

        private SyntaxToken[] GeneratePropertyModifiers(string propertyName)
        {
            IList<SyntaxToken> modifierTokens = new List<SyntaxToken>
            {
                SyntaxFactory.Token(SyntaxKind.PublicKeyword)
            };

            if (IsAzureResource && BasePropsToOverride.Contains(propertyName))
            {
                modifierTokens.Add(SyntaxFactory.Token(SyntaxKind.OverrideKeyword));
            }

            return modifierTokens.ToArray();
        }

        private AccessorDeclarationSyntax[] GeneratePropertyAccessors(string propertyName)
        {
            if (IsAzureResource && PropsToJsonIgnore.Contains(propertyName))
            {
                return new AccessorDeclarationSyntax[]
                {
                    SyntaxHelper.MakeGetAccessor(),
                    SyntaxHelper.MakeSetAccessor(privateSet: true),
                };
            }

            return new AccessorDeclarationSyntax[]
            {
                SyntaxHelper.MakeGetAccessor(),
                SyntaxHelper.MakeSetAccessor(),
            };
        }

        private AttributeSyntax[] GeneratePropertyAttributes(string propertyName, ObjectTypeProperty property)
        {
            var attributes = new List<AttributeSyntax>();

            // Required
            if (property.Flags.HasFlag(ObjectTypePropertyFlags.Required))
            {
                AddUsing(RequiredAttributeNamespaceName);
                AttributeSyntax requiredAttribute =
                        SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(RequiredAttributeName));

                attributes.Add(requiredAttribute);
            }

            AttributeSyntax jsonPropertyAttribute;
            if (IsAzureResource && PropsToJsonIgnore.Contains(propertyName))
            {
                // JsonIgnore
                jsonPropertyAttribute =
                    SyntaxFactory.Attribute(
                        SyntaxFactory.IdentifierName(JsonIgnoreAttributeName));
            }
            else
            {
                // JsonPropertyName
                jsonPropertyAttribute =
                    SyntaxFactory.Attribute(
                        SyntaxFactory.IdentifierName(JsonPropertyAttributeName),
                        SyntaxFactory.AttributeArgumentList(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.AttributeArgument(
                                    SyntaxFactory.LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        SyntaxFactory.Literal(propertyName))))));
            }

            if (jsonPropertyAttribute != null)
                attributes.Add(jsonPropertyAttribute);

            return attributes.ToArray();
        }

        private TypeSyntax GetPropertyType(TypeBase typeBase)
        {
            TypeSyntax typeSyntax;
            switch (typeBase)
            {
                case BuiltInType builtInType:
                    {
                        typeSyntax = builtInType.Kind switch
                        {
                            BuiltInTypeKind.Any => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword)),
                            BuiltInTypeKind.Null => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.NullKeyword)),
                            BuiltInTypeKind.Bool => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
                            BuiltInTypeKind.Int => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                            BuiltInTypeKind.String => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                            BuiltInTypeKind.Object => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword)),
                            // BuiltInTypeKind.Array => MakeArrayType(builtInType),
                            // BuiltInTypeKind.ResourceRef => TODO,
                            _ => throw new TypeLoadException($"Type not found. {typeBase}"),
                        };
                        break;
                    }
                case StringLiteralType:
                    {
                        typeSyntax = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword));
                        break;
                    }
                case ObjectType objType:
                    {
                        if (objType.Name.Equals("ResourceTags", StringComparison.InvariantCulture))
                        {
                            typeSyntax = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword));
                        }
                        else
                        {
                            typeSyntax = SyntaxFactory.ParseTypeName(objType.Name);
                            OnAdditionalTypeRequired(objType);
                        }
                        break;
                    }
                case UnionType unionType:
                    {
                        typeSyntax = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword));
                        break;
                    }
                case ArrayType arrayType:
                    {
                        typeSyntax = MakeArrayType(arrayType);
                        break;
                    }
                default:
                    {
                        throw new TypeLoadException($"Type not found. {typeBase}");
                    }
            }

            return typeSyntax;
        }

        private static ArrowExpressionClauseSyntax GeneratePropertyExpressionBody(string defaultValue)
        {
            if (string.IsNullOrEmpty(defaultValue))
            {
                return SyntaxFactory.ArrowExpressionClause(
                    SyntaxFactory.ThrowExpression(
                        SyntaxFactory.ObjectCreationExpression(
                            SyntaxFactory.IdentifierName("NotImplementedException")
                            ).AddArgumentListArguments()));
            }

            return SyntaxFactory.ArrowExpressionClause(
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(defaultValue)));
        }

        private TypeSyntax MakeArrayType(ArrayType arrayType)
        {
            var itemType = GetPropertyType(arrayType.ItemType.Type);

            return SyntaxFactory.GenericName(
                    SyntaxFactory.Identifier("IList"),
                    SyntaxFactory.TypeArgumentList(
                        SyntaxFactory.SingletonSeparatedList(itemType)));
        }

        public override BaseTypeDeclarationSyntax GenerateTypeDeclaration()
        {
            var classDeclaration = SyntaxFactory.ClassDeclaration(TypeName)
                .AddModifiers(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.PartialKeyword)
                );

            var baseTypes = new List<BaseTypeSyntax>();

            if (IsAzureResource)
            {
                baseTypes.Add(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("ResourceBase")));
                AddUsing("AzureDesignStudio.AzureResources.Base");

                classDeclaration = classDeclaration.AddAttributeLists(
                    SyntaxFactory.AttributeList(
                        SyntaxFactory.SingletonSeparatedList(MakeAzureResourceAttribute())));
            }

            if (baseTypes.Count > 0)
            {
                SeparatedSyntaxList<BaseTypeSyntax> separatedBaseList = SyntaxFactory.SeparatedList(baseTypes);
                BaseListSyntax baseList = SyntaxFactory.BaseList(separatedBaseList);
                classDeclaration = classDeclaration.WithBaseList(baseList);
            }

            return classDeclaration;
        }

        private AttributeSyntax MakeAzureResourceAttribute()
        {
            if (!IsAzureResource)
                return null!;

            var resType = (ResourceType.Properties["type"].Type.Type as StringLiteralType)!.Value;
            var apiVersion = (ResourceType.Properties["apiVersion"].Type.Type as StringLiteralType)!.Value;

            return SyntaxFactory.Attribute(
                SyntaxFactory.IdentifierName(AzureResourceAttributeName),
                SyntaxFactory.AttributeArgumentList(
                    SyntaxFactory.SeparatedList(
                        new AttributeArgumentSyntax[]
                        {
                            SyntaxFactory.AttributeArgument(
                                SyntaxFactory.LiteralExpression(
                                    SyntaxKind.StringLiteralExpression,
                                    SyntaxFactory.Literal(resType))),
                            SyntaxFactory.AttributeArgument(
                                SyntaxFactory.LiteralExpression(
                                    SyntaxKind.StringLiteralExpression,
                                    SyntaxFactory.Literal(apiVersion))),
                        })));
        }
    }
}
