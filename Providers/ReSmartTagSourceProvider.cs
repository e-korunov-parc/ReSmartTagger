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
            if (args.SystemKey == Key.E && (Keyboard.Modifiers & ModifierKeys.Alt) != 0)
            {
                if (KeyDownEvent != null)
                    KeyDownEvent(this, args);
                args.Handled = true;
            }
        }

        public bool IsAlt
        {
            get { return Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt); }
        }
    }
}