using System.Windows.Input;
using Microsoft.VisualStudio.Text.Editor;

namespace ReSmartChecker.Providers.InputProvider
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
            if (args.SystemKey == Key.E && IsAlt)
            {
                if (KeyDownEvent != null)
                    KeyDownEvent(this, args);
                args.Handled = true;
            }
        }

        public bool IsAlt
        {
            get { return (Keyboard.Modifiers & ModifierKeys.Alt) != 0; }
        }
    }
}