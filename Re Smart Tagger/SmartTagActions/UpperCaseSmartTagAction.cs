using System.Collections.ObjectModel;
using System.Windows.Media;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace SpellChecker.SmartTagActions
{
    internal class UpperCaseSmartTagAction : ISmartTagAction
    {
        private ITrackingSpan _span;
        private string _upper;
        private string _display;
        private ITextSnapshot _snapshot;

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

        public UpperCaseSmartTagAction(ITrackingSpan span)
        {
            _span = span;
            _snapshot = span.TextBuffer.CurrentSnapshot;
            _upper = span.GetText(_snapshot).ToUpper();
            _display = "Преобразовать в заглавные буквы";
        }

        public void Invoke()
        {
            _span.TextBuffer.Replace(_span.GetSpan(_snapshot), _upper);
        }
    }
}