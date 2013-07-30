using System.Collections.ObjectModel;
using Microsoft.VisualStudio.Language.Intellisense;

namespace SpellChecker
{
    internal class ReSmartTag : SmartTag
    {
        public ReSmartTag(ReadOnlyCollection<SmartTagActionSet> actionSets)
            : base(SmartTagType.Factoid, actionSets)
        {
        }
    }
}