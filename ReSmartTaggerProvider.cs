using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace SpellChecker
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("csharp")]
    [Order(Before = "default")]
    [TagType(typeof(SmartTag))]
    internal class ReSmartTaggerProvider : IViewTaggerProvider
    {
        [Import(typeof(ITextStructureNavigatorSelectorService))]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

        [Import]
        public IClassificationTypeRegistryService Classification = null;

        [Import]
        internal IClassifierAggregatorService AggregatorService = null;

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (buffer == null || textView == null)
            {
                return null;
            }

            // TODO : Походу так можно получить класификаторы
            var classifer = AggregatorService.GetClassifier(buffer);
            //classifer.GetClassificationSpans();
            //make sure we are tagging only the top buffer

            {
                //                internal
                // OokClassifier(ITextBuffer
                // buffer, ITagAggregator<OokTokenTag>
                // ookTagAggregator, IClassificationTypeRegistryService
                // typeService)

                //{
                //   _buffer = buffer;

                //   _aggregator = ookTagAggregator;

                //   _ookTypes = newDictionary<OokTokenTypes,
                //IClassificationType>();
                //   _ookTypes[OokTokenTypes.OokExclaimation]
                // = typeService.GetClassificationType(PredefinedClassificationTypeNames.Comment);

                //   _ookTypes[OokTokenTypes.OokPeriod]
                // = typeService.GetClassificationType(PredefinedClassificationTypeNames.Literal);

                //   _ookTypes[OokTokenTypes.OokQuestion]
                // = typeService.GetClassificationType(PredefinedClassificationTypeNames.Keyword);

                //}
            }

            if (buffer == textView.TextBuffer)
            {
                return new ReSmartTagger(buffer, textView, this) as ITagger<T>;
            }
            else return null;
        }
    }
}