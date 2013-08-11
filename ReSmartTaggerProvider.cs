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

            var sessions = SmartTagBroker.GetSessions(textView);
            if (true) { }

            ITextSnapshot snapshot = buffer.CurrentSnapshot;
            SnapshotPoint? caretPoint = textView.Caret.Position.Point.GetPoint(snapshot, PositionAffinity.Successor);
            ITrackingPoint triggerPoint = snapshot.CreateTrackingPoint(caretPoint.Value, PointTrackingMode.Positive);
            var sessions2 = SmartTagBroker.CreateSmartTagSession(textView, SmartTagType.Factoid, triggerPoint, SmartTagState.Collapsed);
            sessions2.IconSource = Icon;
            Global.Session = sessions2;
            if (true) { }

            //make sure we are tagging only the top buffer
            //if (buffer == textView.TextBuffer)
            //    return new NemerleImplementsSmartTagger(buffer, textView, this) as ITagger<T>;

            var classifier = AggregatorService.GetClassifier(buffer);

            if (buffer == textView.TextBuffer)
            {
                return new ReSmartTagger(buffer, textView, this, classifier) as ITagger<T>;
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

        public System.Windows.Media.ImageSource Icon 
        {
            get 
            {
                BitmapImage img = new BitmapImage(new System.Uri("texticon.png", System.UriKind.Relative));
                return img;
            }
        }
    }
}