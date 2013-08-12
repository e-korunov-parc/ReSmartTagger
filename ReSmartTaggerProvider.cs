using System.ComponentModel.Composition;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using ReSmartChecker;

namespace SpellChecker
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("csharp")]
    [TagType(typeof(SmartTag))]
    internal class ReSmartTaggerProvider : IViewTaggerProvider
    {
        [Import(typeof(ITextStructureNavigatorSelectorService))]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

        [Import]
        public IClassificationTypeRegistryService Classification = null;

        [Import]
        internal IClassifierAggregatorService AggregatorService = null;

        [Import]
        internal ISmartTagBroker SmartTagBroker { get; set; }

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (buffer == null || textView == null)
            {
                return null;
            }

            var propKey = typeof(ISmartTagBroker);

            if (!textView.Properties.ContainsProperty(propKey) && SmartTagBroker != null)
                textView.Properties.AddProperty(propKey, SmartTagBroker);

            var classifier = AggregatorService.GetClassifier(buffer);

            if (buffer == textView.TextBuffer)
            {
                return new ReSmartTagger(buffer, textView, this, classifier, SmartTagBroker) as ITagger<T>;
            }
            else return null;
        }

        //private static bool IsIntersectedWithSmartTag(ITextView view)
        //{
        //    var smartTagBroker = view.GetSmartTagBroker();

        //    if (smartTagBroker != null && smartTagBroker.IsSmartTagActive(textView))
        //    {
        //        foreach (ISmartTagSession s in smartTagBroker.GetSessions(textView))
        //        {
        //            var wpfTextView = (IWpfTextView)textView;
        //            var spaceReservationManager = wpfTextView.GetSpaceReservationManager("smarttag");
        //            var adornmentLayer = wpfTextView.GetAdornmentLayer("SmartTag");

        //            foreach (var alement in adornmentLayer.Elements)
        //                if (rect.Contains(alement.Adornment.PointToScreen(new Point(0, 0))))
        //                    return true;
        //        }
        //    }

        //    return false;
        //}
    }
}