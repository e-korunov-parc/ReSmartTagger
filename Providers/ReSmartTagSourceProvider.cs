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
            if (args.Key == Key.E && IsAlt)
            {
                if (KeyDownEvent != null)
                {
                    KeyDownEvent(this, args);
                }
            }

            //if (args.Key == Key.Q && (Keyboard.Modifiers & ModifierKeys.Alt) != 0)
            //{
            //    if (KeyDownEvent != null)
            //    {
            //        KeyDownEvent(this, args);
            //    }
            //}
            //base.KeyDown(args);
        }

        public bool IsAlt
        {
            get { return Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt); }
        }
    }
}