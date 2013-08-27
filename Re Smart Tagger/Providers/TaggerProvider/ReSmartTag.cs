using System.Collections.ObjectModel;
using Microsoft.VisualStudio.Language.Intellisense;

namespace ReSmartChecker.Providers.TaggerProvider
{
    internal class ReSmartTag : SmartTag
    {
        public ReSmartTag(ReadOnlyCollection<SmartTagActionSet> actionSets)
            : base(SmartTagType.Factoid, actionSets)
        {
        }
    }
}