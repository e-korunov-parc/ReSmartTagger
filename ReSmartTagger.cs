using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
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
        private IClassifier _classifier;
        private ISmartTagBroker _broke;
        private BitmapImage _img = null;

        public System.Windows.Media.ImageSource Icon
        {
            get
            {
                if (_img == null)
                    _img = new BitmapImage(new System.Uri("texticon.png", System.UriKind.Relative));
                return _img;
            }
        }

        public ReSmartTagger(ITextBuffer buffer, ITextView view, ReSmartTaggerProvider provider, IClassifier classifier, ISmartTagBroker broke)
        {
            _buffer = buffer;
            _view = view;
            _provider = provider;
            _view.LayoutChanged += OnLayoutChanged;
            _classifier = classifier;
            _view.Caret.PositionChanged += Caret_PositionChanged;
            _broke = broke;
            this.TagsChanged += ReSmartTagger_TagsChanged;
        }

        private void ReSmartTagger_TagsChanged(object sender, SnapshotSpanEventArgs e)
        {
            if (true) { }
        }

        private void Caret_PositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            var tempEvent = TagsChanged;
            if (tempEvent != null)
                tempEvent(this, new SnapshotSpanEventArgs(new SnapshotSpan(_buffer.CurrentSnapshot, 0, _buffer.CurrentSnapshot.Length)));
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
                if (_classifier != null)
                {
                    var listClassifier = _classifier.GetClassificationSpans(span);

                    if (listClassifier != null)
                    {
                        ITextCaret caret = _view.Caret;
                        SnapshotPoint point;

                        if (caret.Position.BufferPosition > 0)
                            point = caret.Position.BufferPosition - 1;
                        else
                            yield break;

                        var groupClassifications = listClassifier.GroupBy(item => item.ClassificationType.Classification,
                                                                                  item => item.Span.GetText(),
                                                                                  (key, type) => new KeyValuePair<string, IEnumerable<string>>(key, type));

                        var targetSpan = listClassifier.FirstOrDefault(item => item.Span.Start <= point.Position
                                                                               && item.Span.End >= point.Position
                                                                               && (item.ClassificationType.Classification == "identifier"
                                                                               && !groupClassifications.Any(i => i.Key == "keyword" && i.Value.Any(a => a == item.Span.GetText()))));
                        if (targetSpan != null)
                            yield return new TagSpan<ReSmartTag>(targetSpan.Span, new ReSmartTag(GetSmartTagActions(targetSpan.Span)));
                        else yield break;
                    }
                }
                else
                    yield break;
            }

            var sessions = _broke.GetSessions(_view);
            if (_broke.IsSmartTagActive(_view))
            {
                var s = sessions.FirstOrDefault();
                if (s != null)
                {
                    IsIntersectedWithSmartTag(_view);
                    s.State = SmartTagState.Intermediate;
                    //s.IconSource = Icon;
                    //ITextSnapshot snapshot2 = _buffer.CurrentSnapshot;
                    //SnapshotPoint? caretPoint = _view.Caret.Position.Point.GetPoint(snapshot, PositionAffinity.Successor);
                    //ITrackingPoint triggerPoint = snapshot.CreateTrackingPoint(caretPoint.Value, PointTrackingMode.Positive);
                    //var sessions2 = _broke.CreateSmartTagSession(_view, SmartTagType.Factoid, triggerPoint, SmartTagState.Intermediate);
                    //sessions2.ActionSets.Union(sessions2.ActionSets);
                    //sessions2.IconSource = Icon;
                }
                if (true) { }
            }
        }

        private bool IsIntersectedWithSmartTag(ITextView view)
        {
            foreach (ISmartTagSession s in _broke.GetSessions(view))
            {
                var wpfTextView = (IWpfTextView)view;

                var spaceReservationManager = wpfTextView.GetSpaceReservationManager("smarttag");
                var adornmentLayer = wpfTextView.GetAdornmentLayer("SmartTag");

                //adornmentLayer.RemoveAllAdornments();
                //Canvas.SetLeft(, _view.ViewportRight - 255);
                //Canvas.SetTop(, _view.ViewportTop + 10);
                //wpfTextView.VisualElement.Margin

                foreach (var alement in adornmentLayer.Elements)
                {
                    var but = new Button();
                    but.Content = "hello";
                    adornmentLayer.RemoveAllAdornments();
                    adornmentLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, alement.VisualSpan, null, but, null);

                    //alement.Adornment alement.Adornment.PointToScreen(new Point(0, 0));
                    //if (rect.Contains(alement.Adornment.PointToScreen(new Point(0, 0))))
                    //    return true;
                }
            }

            return false;
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
                if (this.TagsChanged != null)
                {
                    this.TagsChanged(this, new SnapshotSpanEventArgs(span));
                }
            }
        }
    }
}