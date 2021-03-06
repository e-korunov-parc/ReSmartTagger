﻿using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using ReSmartChecker.Providers;

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

        [Import]
        internal IWpfKeyboardTrackingService KS { get; set; }

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (buffer == null || textView == null)
            {
                return null;
            }

            var classifier = AggregatorService.GetClassifier(buffer);

            if (buffer == textView.TextBuffer)
            {
                return new ReSmartTagger(buffer, textView, this, classifier, SmartTagBroker) as ITagger<T>;
            }
            else return null;
        }
    }
}