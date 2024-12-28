using OfficeInstaller.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OfficeInstaller.Pages
{
    /// <summary>
    /// ResultPage.xaml 的交互逻辑
    /// </summary>
    public partial class ResultPageFail : Page, INotifyPropertyChanged
    {
        public ResultPageFail()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public List<string> Logs
        {
            get { return LogService.Logs; }
        }


        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Navigation.Navigate(new WelcomePage());
        }

        private void ButtonFeedback_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer", "https://shuiyuan.sjtu.edu.cn/c/campus-life/genuine-software/106");
        }
    }
}
