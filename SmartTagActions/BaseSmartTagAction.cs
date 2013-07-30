using System.Collections.ObjectModel;
using System.Windows.Media;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace ReSmartChecker.SmartTagActions
{
    internal abstract class BaseSmartTagAction : ISmartTagAction
    {
        protected ITrackingSpan _span;
        protected string _display;
        protected ITextSnapshot _snapshot;
        protected string _spanText;

        public string DisplayText
        {
            get { return _display; }
        }
        public ImageSource Icon
        {
            get { return null; }
        }
        public bool IsEnabled
        {
            get { return true; }
        }

        public ISmartTagSource Source
        {
            get;
            private set;
        }

        public ReadOnlyCollection<SmartTagActionSet> ActionSets
        {
            get { return null; }
        }

        public BaseSmartTagAction(ITrackingSpan span, string display = "")
        {
            _span = span;
            _snapshot = span.TextBuffer.CurrentSnapshot;
            _spanText = span.GetText(_snapshot);
            _display = display;
        }

        public abstract void Invoke();
    }
}