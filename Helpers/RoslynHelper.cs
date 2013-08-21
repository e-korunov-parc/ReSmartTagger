using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roslyn.Compilers.CSharp;

namespace ReSmartChecker.Helpers
{
    internal static class RoslynHelper
    {
        public static void Analiz(string text)
        {
            SyntaxTree tree = SyntaxTree.ParseText(text);
            var root = tree.GetRoot();
        }
    }
}