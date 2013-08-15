using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using ReSmartChecker.Controls;
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
        private ReButtonMenu _reButtonMenu = null;

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

            CreateRebutton(_view);
        }

        private void ReSmartTagger_TagsChanged(object sender, SnapshotSpanEventArgs e)
        {
            RemoveReButton();
        }

        private void Caret_PositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            if (TagsChanged != null)
                TagsChanged(this, new SnapshotSpanEventArgs(new SnapshotSpan(_buffer.CurrentSnapshot, 0, _buffer.CurrentSnapshot.Length)));
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
                            //point = caret.Position.BufferPosition - 1;
                            point = caret.Position.BufferPosition;
                        else
                            yield break;

                        var groupClassifications = listClassifier.GroupBy(item => item.ClassificationType.Classification,
                                                                                  item => item.Span.GetText(),
                                                                                  (key, type) => new KeyValuePair<string, IEnumerable<string>>(key, type));

                        var targetSpan = listClassifier.FirstOrDefault(item => item.Span.Start <= point.Position
                                                                               && item.Span.End >= point.Position
                                                                               && (item.ClassificationType.Classification == "identifier"
                                                                               && !groupClassifications.Any(i => i.Key == "keyword" && i.Value.Any(a => a == item.Span.GetText()))));
                        //if (targetSpan != null)
                        //    yield return new TagSpan<ReSmartTag>(targetSpan.Span, new ReSmartTag(GetSmartTagActions(targetSpan.Span)));
                        //else yield break;
                        if (targetSpan != null)
                        {
                            _span = targetSpan.Span;
                            _smartTagActionSets = GetSmartTagActions(_span.Value);
                        }
                        else
                        {
                            _span = null;
                            _smartTagActionSets = null;
                        }
                    }
                }
                else
                    yield break;
            }

            UpdateAdornmentLayer();
            yield break;
        }

        private SnapshotSpan? _span;
        private ReadOnlyCollection<SmartTagActionSet> _smartTagActionSets = null;

        private void UpdateAdornmentLayer()
        {
            if (_span != null)
                InsertReButton(_view);
        }

        private void CreateRebutton(ITextView view)
        {
            _reButtonMenu = new ReButtonMenu();
            _reButtonMenu.Margin = new Thickness(0);
        }

        private void UpdateReButton(ITextView view)
        {
            var wpfTextView = (IWpfTextView)view;
            var line = view.GetTextViewLineContainingBufferPosition(_view.Caret.Position.BufferPosition);
            if (_reButtonMenu == null)
                _reButtonMenu = new ReButtonMenu();
            _reButtonMenu.Margin = new Thickness(0, line.Top, 0, 0);
        }

        private void InsertReButton(ITextView view)
        {
            UpdateReButton(view);
            var adornmentLayer = (view as IWpfTextView).GetAdornmentLayer("SmartTag");
            if (!adornmentLayer.Elements.Any(item => item.Tag == _reButtonMenu.TAG_BUTTON))
            {
                //var visualSpan = adornmentLayer.Elements.FirstOrDefault().VisualSpan;
                //adornmentLayer.RemoveAllAdornments();
                if (_span != null)
                {
                    var visualSpan = _span;
                    HideSmartTags(view);
                    adornmentLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, visualSpan, _reButtonMenu.TAG_BUTTON, _reButtonMenu, null);
                }
            }
            else
            {
                HideSmartTags(view);
                _reButtonMenu.Visibility = Visibility.Visible;
                _reButtonMenu.CreateMenu(_smartTagActionSets);
                //var elems = adornmentLayer.Elements.Where(item => item.Tag != _reButtonMenu.TAG_BUTTON);
                //foreach (var item in elems)
                //{
                //    item.Adornment.Visibility = Visibility.Hidden;
                //}
                //adornmentLayer.RemoveAdornmentsByTag(item.Tag);
            }
        }

        private void HideSmartTags(ITextView view)
        {
            var adornmentLayer = (view as IWpfTextView).GetAdornmentLayer("SmartTag");
            var elems = adornmentLayer.Elements.Where(item => item.Tag != _reButtonMenu.TAG_BUTTON);
            foreach (var item in elems)
            {
                item.Adornment.Visibility = Visibility.Hidden;
            }
        }

        private void RemoveReButton()
        {
            if (_reButtonMenu != null)
            {
                _reButtonMenu.Visibility = Visibility.Hidden;
                //var wpfTextView = (IWpfTextView)_view;
                //var adornmentLayer = wpfTextView.GetAdornmentLayer("SmartTag");
                //adornmentLayer.RemoveAdornmentsByTag(_reButtonMenu.TAG_BUTTON);
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
                if (this.TagsChanged != null)
                {
                    this.TagsChanged(this, new SnapshotSpanEventArgs(span));
                }
            }
        }
    }
}