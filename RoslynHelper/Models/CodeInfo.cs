using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynHelper.Models
{
    public class CodeInfo
    {
        public int UsingsCount { get; set; }
        public int NamespaceCount { get; set; }
        public int ClassCount { get; set; }
        public int PropertiesCount { get; set; }
        public int MethodsCount { get; set; }

        public List<string> Usings { get; set; }
        public List<string> Namespaces { get; set; }
        public List<string> Classes { get; set; }
        public List<string> Methods { get; set; }
        public List<string> Properties { get; set; }

        public CodeInfo()
        {
            Usings = new List<string>();
            Namespaces = new List<string>();
            Classes = new List<string>();
            Methods = new List<string>();
            Properties = new List<string>();
        }
    }
}