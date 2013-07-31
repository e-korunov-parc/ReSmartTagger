using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using ReSmartChecker.SmartTagActions;
using SpellChecker.SmartTagActions;

namespace SpellChecker
{
    internal class ReSmartTagger : ITagger<ReSmartTag>, IDisposable
    {
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private ITextBuffer _buffer;
        private ITextView _view;
        private ReSmartTaggerProvider _provider;
        private bool _disposed;

        public ReSmartTagger(ITextBuffer buffer, ITextView view, ReSmartTaggerProvider provider)
        {
            _buffer = buffer;
            _view = view;
            _provider = provider;
            _view.LayoutChanged += OnLayoutChanged;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _view.LayoutChanged -= OnLayoutChanged;
                    _view = null;
                }

                _disposed = true;
            }
        }

        public IEnumerable<ITagSpan<ReSmartTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            ITextSnapshot snapshot = _buffer.CurrentSnapshot;
            if (snapshot.Length == 0)
                yield break; //don't do anything if the buffer is empty

            //set up the navigator
            ITextStructureNavigator navigator = _provider.NavigatorService.GetTextStructureNavigator(_buffer);

            foreach (var span in spans)
            {
                ITextCaret caret = _view.Caret;
                SnapshotPoint point;

                if (caret.Position.BufferPosition > 0)
                    point = caret.Position.BufferPosition - 1;
                else
                    yield break;

                TextExtent extent = navigator.GetExtentOfWord(point);
                var classification = _provider.Classification.GetClassificationType("");
                var cs = new ClassificationSpan(span, classification);

                //don't display the tag if the extent has whitespace
                if (extent.IsSignificant)
                    yield return new TagSpan<ReSmartTag>(extent.Span, new ReSmartTag(GetSmartTagActions(extent.Span)));
                else yield break;
            }
        }

        private ReadOnlyCollection<SmartTagActionSet> GetSmartTagActions(SnapshotSpan span)
        {
            List<SmartTagActionSet> actionSetList = new List<SmartTagActionSet>();
            List<ISmartTagAction> actionList = new List<ISmartTagAction>();

            ITrackingSpan trackingSpan = span.Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeInclusive);

            actionList.Add(new UpperCaseSmartTagAction(trackingSpan));
            actionList.Add(new RenameSmartTagAction(trackingSpan));

            SmartTagActionSet actionSet = new SmartTagActionSet(actionList.AsReadOnly());
            actionSetList.Add(actionSet);
            return actionSetList.AsReadOnly();
        }

        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            ITextSnapshot snapshot = e.NewSnapshot;
            //don't do anything if this is just a change in case
            if (!snapshot.GetText().ToLower().Equals(e.OldSnapshot.GetText().ToLower()))
            {
                SnapshotSpan span = new SnapshotSpan(snapshot, new Span(0, snapshot.Length));
                EventHandler<SnapshotSpanEventArgs> handler = this.TagsChanged;
                if (handler != null)
                {
                    handler(this, new SnapshotSpanEventArgs(span));
                }
            }
        }
    }
}