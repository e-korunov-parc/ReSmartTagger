using Roslyn.Compilers.CSharp;
using RoslynHelper.Models;
using System.Linq;
using System.Collections.Generic;

namespace RoslynHelper
{
    public class Analyze
    {
        public static CodeInfo AnalyzeCode(string code, int position = -1)
        {
            CodeInfo info = null;
            SyntaxTree tree = SyntaxTree.ParseText(code);
            CompilationUnitSyntax root = null;

            if (!tree.TryGetRoot(out root))
            {
                return null;
            }

            info = new CodeInfo();
            info.UsingsCount = root.Usings.Count;
            info.Usings = root.Usings.Select(item => (item.Name as IdentifierNameSyntax).Identifier.Value.ToString()).ToList();

            if (root.Members == null || root.Members.Count == 0) return null;

            IEnumerable<MemberDeclarationSyntax> namespaceNodes = root.Members.Where(item => item.Kind == SyntaxKind.NamespaceDeclaration);
            if (namespaceNodes != null && namespaceNodes.Count() > 0)
            {
                // смотрим неймспейсы
                info.NamespaceCount = namespaceNodes.Count();
                info.Namespaces.AddRange(namespaceNodes
                    .Select(item => ((item as NamespaceDeclarationSyntax).Name as IdentifierNameSyntax).Identifier.Value.ToString())
                    .ToList());

                foreach (NamespaceDeclarationSyntax nn in namespaceNodes)
                {
                    NamespaceParse(nn, info);
                }
            }

            // Проходим по отальным чилдам и смотрим что там лежит
            foreach (var node in root.Members)
            {
                switch (node.Kind)
                {
                    case SyntaxKind.FieldDeclaration:
                        break;

                    case SyntaxKind.PropertyDeclaration:

                        if (position > 0)
                        {
                            var selectNode = node.DescendantTokens().FirstOrDefault(item => item.Span.IntersectsWith(position));
                        }

                        break;

                    case SyntaxKind.DelegateDeclaration:
                        break;
                }
            }

            return info;
        }

        private static void NamespaceBlock(MemberDeclarationSyntax root, CodeInfo info)
        {
            //IEnumerable<MemberDeclarationSyntax> namespaceNodes = root.Members.Where(item => item.Kind == SyntaxKind.NamespaceDeclaration);
            //if (namespaceNodes != null && namespaceNodes.Count() > 0)
            //{
            //    // смотрим неймспейсы
            //    info.NamespaceCount = namespaceNodes.Count();
            //    info.Namespaces.AddRange(namespaceNodes
            //        .Select(item => ((item as NamespaceDeclarationSyntax).Name as IdentifierNameSyntax).Identifier.Value.ToString())
            //        .ToList());

            //    foreach (NamespaceDeclarationSyntax nn in namespaceNodes)
            //    {
            //        NamespaceParse(nn, info);
            //    }
            //}
        }

        private static void NamespaceParse(NamespaceDeclarationSyntax nn, CodeInfo info)
        {
            if (nn.Members != null && nn.Members.Count > 0)
            {
                IEnumerable<MemberDeclarationSyntax> classNodes = nn.Members.Where(item => item.Kind == SyntaxKind.ClassDeclaration);
                if (classNodes != null && classNodes.Count() > 0)
                {
                    // смотрим классы
                    info.ClassCount = classNodes.Count();
                    info.Classes.AddRange(classNodes
                        .Select(item => (item as ClassDeclarationSyntax).Identifier.Value.ToString())
                        .ToList());

                    foreach (ClassDeclarationSyntax cn in classNodes)
                    {
                        ClassParse(cn, info);
                    }
                }
            }
        }

        private static void ClassParse(ClassDeclarationSyntax cn, CodeInfo info)
        {
            if (cn.Members != null && cn.Members.Count > 0)
            {
                IEnumerable<MemberDeclarationSyntax> methodNodes = cn.Members.Where(item => item.Kind == SyntaxKind.MethodDeclaration);
                if (methodNodes != null && methodNodes.Count() > 0)
                {
                    info.MethodsCount = methodNodes.Count();
                    info.Methods.AddRange(methodNodes
                        .Select(item => (item as MethodDeclarationSyntax).Identifier.Value.ToString()));

                    foreach (MethodDeclarationSyntax mn in methodNodes)
                    {
                        MethodParse(mn, info);
                    }
                }
            }
        }

        private static void MethodParse(MethodDeclarationSyntax mn, CodeInfo info)
        {
            if (mn.Body != null)
            {
                if (mn.Body.Statements != null)
                {
                }
            }
        }

        public static void ParseLine(string line, int position)
        {
            AnalyzeCode(line, position);
        }
    }
}