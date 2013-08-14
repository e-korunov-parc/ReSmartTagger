using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace ReSmartChecker.Providers
{
    [Export(typeof(ISmartTagSourceProvider))]
    [Order(Before = Priority.Default)]
    [Name("Refactoring Smart Tag Source Provider")]
    internal class ReSmartTagSourceProvider : ISmartTagSourceProvider
    {
        public ISmartTagSource TryCreateSmartTagSource(ITextBuffer textBuffer)
        {
            return new ReSmartTagSource();
        }
    }

    internal class ReSmartTagSource : ISmartTagSource
    {
        public void AugmentSmartTagSession(ISmartTagSession session, System.Collections.Generic.IList<SmartTagActionSet> smartTagActionSets)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}