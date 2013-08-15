using System.Windows.Controls;

namespace ReSmartChecker.Controls
{
    /// <summary>
    /// Логика взаимодействия для ReButtonMenu.xaml
    /// </summary>
    public partial class ReButtonMenu : UserControl
    {
        public readonly string TAG_BUTTON = "ReButtonMenuControl";

        public ReButtonMenu()
        {
            InitializeComponent();
        }

        public void CreateMenu(System.Collections.ObjectModel.ReadOnlyCollection<Microsoft.VisualStudio.Language.Intellisense.SmartTagActionSet> _smartTagActionSets)
        {
            RootItem.Items.Clear();
            foreach (var item in _smartTagActionSets)
            {
                foreach (var item2 in item.Actions)
                {
                    var mi = new MenuItem() { Header = item2.DisplayText, Tag = item2 };
                    mi.Click += (s, e) => item2.Invoke();
                    RootItem.Items.Add(mi);
                }
            }
        }
    }
}