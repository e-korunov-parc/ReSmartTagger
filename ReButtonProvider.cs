using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using ReSmartChecker.Providers;

namespace ReSmartChecker
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