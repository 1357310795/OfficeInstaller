using OfficeInstaller.Pages;
using OfficeInstaller.Services;
using System.Windows;

namespace OfficeInstaller
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Navigation.SetFrame(MainFrame);
            Navigation.Navigate(new WelcomePage());
        }
    }
}
