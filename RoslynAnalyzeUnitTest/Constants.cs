using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoslynAnalyzeUnitTest
{
    public static class Constants
    {
        public const string CODE = @"
using System;

namespace test_app
{
	class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine('Hello World!');

			// TODO: Implement Functionality Here

			Console.Write('Press any key to continue . . . ');
			Console.ReadKey(true);
			Console.Read();
		}
	}

	class MyClass
	{
		int _numb;

		public int Numb {
			get {
				return _numb;
			}
			set {
				_numb = value;
			}
		}
	}
}";
    }
}