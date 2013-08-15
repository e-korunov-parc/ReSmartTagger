using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace ReSmartChecker.Providers
{
    //[Export(typeof(ISmartTagSourceProvider))]
    //[Order(Before = Priority.Default)]
    //[Name("Refactoring Smart Tag Source Provider")]
    //internal class ReSmartTagSourceProvider : ISmartTagSourceProvider
    //{
    //    public ISmartTagSource TryCreateSmartTagSource(ITextBuffer textBuffer)
    //    {
    //        return new ReSmartTagSource();
    //    }
    //}

    //internal class ReSmartTagSource : ISmartTagSource
    //{
    //    public void AugmentSmartTagSession(ISmartTagSession session, System.Collections.Generic.IList<SmartTagActionSet> smartTagActionSets)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Dispose()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    [Export(typeof(IKeyProcessorProvider))]
    //[ContentType("code")]
    [Name("ReButtonMenuOpen")]
    //[Order(Before = "VisualStudioKeyProcessor")]
    internal class ReButtonProvider : IKeyProcessorProvider
    {
        public KeyProcessor GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            return new ReButtonKeyProc(wpfTextView);
        }
    }

    internal class ReButtonKeyProc : KeyProcessor
    {
        private ITextView textView;

        internal static event KeyEventHandler KeyDownEvent;

        public ReButtonKeyProc(ITextView textView)
        {
            this.textView = textView;
        }

        public override void KeyDown(KeyEventArgs args)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0 &&
                args.Key == Key.Enter)
            {
                if (KeyDownEvent != null)
                {
                    KeyDownEvent(this, args);
                }
            }
        }
    }
}