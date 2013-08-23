using Microsoft.VisualStudio.Text;

namespace ReSmartChecker.SmartTagActions
{
    internal class RenameSmartTagAction : BaseSmartTagAction
    {
        private string _replace;

        public RenameSmartTagAction(ITrackingSpan span)
            : base(span)
        {
            if (_spanText.StartsWith("_"))
            {
                _display = "Переименовать в глобальную";
                string temp = _spanText.Remove(0, 1);
                if (!string.IsNullOrEmpty(temp))
                {
                    char a = temp[0];
                    _replace = string.Concat(a.ToString().ToUpper(), temp.Remove(0, 1));
                }
            }
            else
            {
                _display = "Переименовать в локальную";
                if (!string.IsNullOrEmpty(_spanText))
                {
                    char a = _spanText[0];
                    _replace = string.Concat("_", a.ToString().ToLower(), _spanText.Remove(0, 1));
                }
            }
        }

        public override void Invoke()
        {
            if (!string.IsNullOrEmpty(_replace))
                _span.TextBuffer.Replace(_span.GetSpan(_snapshot), _replace);
        }
    }
}