using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynHelper;

namespace RoslynAnalyzeUnitTest
{
    [TestClass]
    public class RoslynAnalyze
    {
        [TestMethod]
        public void TestMethod1()
        {
            var info = Analyze.AnalyzeCode(Constants.CODE);

            //var info2 = Analyze.AnalyzeCode("int number = 5;");

            Assert.IsTrue(true);
        }
    }
}