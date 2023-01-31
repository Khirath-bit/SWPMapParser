using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace SWPMapParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(DialogCoordinator.Instance);
        }
    }
}
