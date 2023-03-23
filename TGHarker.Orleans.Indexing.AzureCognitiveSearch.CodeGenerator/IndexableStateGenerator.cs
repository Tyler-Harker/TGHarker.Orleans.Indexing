using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

[Generator]
public class IndexableStateGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new IndexableStateAttributeSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        // context.AddSource("IndexableAttributes", SourceText.From(AttributeText, Encoding.UTF8));
        if (context.SyntaxReceiver is not IndexableStateAttributeSyntaxReceiver receiver)
        {
            return;
        }

        var compilation = context.Compilation;

        // Find the attribute symbols by name
        var indexableStateAttributeName = "IndexableStateAttribute";
        var indexablePropertyAttributeName = "IndexablePropertyAttribute";
        var indexableStateAttributeSymbol = FindAttributeSymbolByName(compilation, indexableStateAttributeName);
        var indexablePropertyAttributeSymbol = FindAttributeSymbolByName(compilation, indexablePropertyAttributeName);


        if (indexableStateAttributeSymbol == null || indexablePropertyAttributeSymbol == null)
        {
            return;
        }

        foreach (var classDeclaration in receiver.CandidateClasses)
        {
            var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);

            // Check if the class has the IndexableStateAttribute
            if (classSymbol.GetAttributes().Any(a =>
                    a.AttributeClass.Equals(indexableStateAttributeSymbol, SymbolEqualityComparer.Default)))
            {
                // Generate the new class with the required properties
                var generatedClass = GenerateIndexableClass(classDeclaration, classSymbol,
                    indexablePropertyAttributeSymbol, semanticModel);

                // Add the generated class to the compilation
                context.AddSource($"{classSymbol.Name}Indexable.g.cs", generatedClass);
            }
        }
    }

    private SourceText GenerateIndexableClass(ClassDeclarationSyntax originalClass, INamedTypeSymbol classSymbol,
        INamedTypeSymbol indexablePropertyAttributeSymbol, SemanticModel semanticModel)
    {
        var newClassName = $"{classSymbol.Name}Indexable";
        var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

        var sb = new StringBuilder();
        sb.AppendLine("using Azure.Search.Documents.Indexes;");
        sb.AppendLine("using TGHarker.Orleans.Indexing.Core;");
        sb.AppendLine("using System.Text.Json;");
        sb.AppendLine("using System.Text.Json.Serialization;");
        sb.AppendLine($"namespace {namespaceName}");
        sb.AppendLine("{");
        sb.AppendLine($"    [GeneratedIndexClass]");
        sb.AppendLine($"    public class {newClassName} : BaseIndexState<{classSymbol.Name}>");
        sb.AppendLine("    {");
        //sb.AppendLine($"        [FieldBuilderIgnore]");
        //sb.AppendLine($"        [JsonIgnore]");
        //sb.AppendLine($"        public override Type Type => typeof({newClassName});");

        foreach (var member in originalClass.Members)
        {
            if (member is PropertyDeclarationSyntax property)
            {
                var propertySymbol = semanticModel.GetDeclaredSymbol(property);

                // Check if the property has the IndexablePropertyAttribute
                if (propertySymbol.GetAttributes().Any(a =>
                        a.AttributeClass.Equals(indexablePropertyAttributeSymbol, SymbolEqualityComparer.Default)))
                {
                    var propertyName = propertySymbol.Name;

                    // Check if the property type implements IValueObject<T>
                    var typeArgSymbol = GetIValueObjectTypeArgument(propertySymbol.Type, semanticModel.Compilation);
                    var propertyType = typeArgSymbol != null
                        ? typeArgSymbol.ToDisplayString()
                        : propertySymbol.Type.ToDisplayString();

                    propertyType = propertyType == "System.Guid"
                        ? "string"
                        : propertyType;

                    // Generate the property decorated with 'SearchableAttribute'
                    sb.AppendLine(
                        $"        [SimpleField(IsFilterable = {GetBooleanValue("IsFilterable")}, IsSortable = {GetBooleanValue("IsSortable")}, IsKey = {GetBooleanValue("IsId")}, IsFacetable = {GetBooleanValue("IsFacetable")}, IsHidden = {GetBooleanValue("IsHidden")})]");
                    sb.AppendLine($"        public {propertyType} {propertyName} {{ get; set; }}");

                    string GetBooleanValue(string propertyName)
                    {
                        return propertySymbol.GetAttributes().FirstOrDefault(a =>
                                a.AttributeClass.Equals(indexablePropertyAttributeSymbol,
                                    SymbolEqualityComparer.Default))
                            .NamedArguments.FirstOrDefault(kv => kv.Key == propertyName).Value.Value?.ToString()
                            ?.ToLowerInvariant() ?? "false";
                    }
                }
            }
        }

        sb.AppendLine($@"
        public override void Map({classSymbol.Name} obj)
        {{");

        foreach (var member in originalClass.Members)
        {
            if (member is PropertyDeclarationSyntax property)
            {
                var propertySymbol = semanticModel.GetDeclaredSymbol(property);

                // Check if the property has the IndexablePropertyAttribute
                if (propertySymbol.GetAttributes().Any(a =>
                        a.AttributeClass.Equals(indexablePropertyAttributeSymbol, SymbolEqualityComparer.Default)))
                {
                    var propertyName = propertySymbol.Name;

                    // Check if the property type implements IValueObject<T>
                    var typeArgSymbol = GetIValueObjectTypeArgument(propertySymbol.Type, semanticModel.Compilation);
                    var propertyType = typeArgSymbol != null
                        ? typeArgSymbol.ToDisplayString()
                        : propertySymbol.Type.ToDisplayString();

                    // Generate the property decorated with 'SearchableAttribute'
                    if (typeArgSymbol != null)
                    {
                        if (propertyType == "System.Guid")
                        {
                            sb.AppendLine($@"           {propertyName} = obj.{propertyName}.Value?.ToString();");
                        }
                        else
                        {
                            sb.AppendLine($@"           {propertyName} = obj.{propertyName}.Value;");
                        }
                    }
                    else
                    {
                        if (propertyType == "System.Guid")
                        {
                            sb.AppendLine($@"           {propertyName} = obj.{propertyName}.ToString();");
                        }
                        else
                        {
                            sb.AppendLine($@"           {propertyName} = obj.{propertyName};");
                        }
                        
                    }
                }
            }
        }


        sb.AppendLine($@"        }}");

        sb.AppendLine("    }");
        sb.AppendLine("}");

        return SourceText.From(sb.ToString(), Encoding.UTF8);
    }
    
    private INamedTypeSymbol FindAttributeSymbolByName(Compilation compilation, string attributeName)
    {
        foreach (var referencedAssembly in compilation.References)
        {
            var assemblySymbol = compilation.GetAssemblyOrModuleSymbol(referencedAssembly) as IAssemblySymbol;
            if (assemblySymbol != null)
            {
                var attributeSymbol = FindAttributeInNamespace(assemblySymbol.GlobalNamespace, attributeName);
                if (attributeSymbol != null)
                {
                    return attributeSymbol;
                }
            }
        }

        return null;
    }

    private INamedTypeSymbol FindAttributeInNamespace(INamespaceSymbol namespaceSymbol, string attributeName)
    {
        foreach (var member in namespaceSymbol.GetMembers())
        {
            if (member is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.Name == attributeName)
            {
                return namedTypeSymbol;
            }
        }

        foreach (var childNamespace in namespaceSymbol.GetNamespaceMembers())
        {
            var attributeSymbol = FindAttributeInNamespace(childNamespace, attributeName);
            if (attributeSymbol != null)
            {
                return attributeSymbol;
            }
        }

        return null;
    }

    private INamedTypeSymbol GetIValueObjectTypeArgument(ITypeSymbol typeSymbol, Compilation compilation)
    {
        // Provide the fully qualified name of the IValueObject<T> interface
        var fullyQualifiedIValueObjectName = "TGHarker.Orleans.Indexing.Core.IValueObject`1";

        // Use GetTypeByMetadataName to find the IValueObject<T> symbol
        var iValueObjectSymbol = compilation.GetTypeByMetadataName(fullyQualifiedIValueObjectName);
        if (iValueObjectSymbol == null)
        {
            return null;
        }

        var implementedInterface = typeSymbol.AllInterfaces.FirstOrDefault(
            i => SymbolEqualityComparer.Default.Equals(i.ConstructedFrom, iValueObjectSymbol));

        if (implementedInterface != null)
        {
            return (INamedTypeSymbol)implementedInterface.TypeArguments[0];
        }

        return null;
    }

    public class IndexableStateAttributeSyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax &&
                classDeclarationSyntax.AttributeLists.Count > 0)
            {
                CandidateClasses.Add(classDeclarationSyntax);
            }
        }
    }
}
