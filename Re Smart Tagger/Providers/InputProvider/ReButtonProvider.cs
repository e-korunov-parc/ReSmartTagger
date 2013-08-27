using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace ReSmartChecker.Providers.InputProvider
{
    [Export(typeof(IKeyProcessorProvider))]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    [ContentType("any")]
    [Name("ReButtonProvider")]
    [Order(Before = "default")]
    internal class ReButtonProvider : IKeyProcessorProvider
    {
        [ImportingConstructor]
        public ReButtonProvider()
        {
        }

        public KeyProcessor GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            return new ReButtonKeyProc(wpfTextView);
        }
    }
}