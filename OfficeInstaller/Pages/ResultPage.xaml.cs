using OfficeInstaller.Helpers;
using OfficeInstaller.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class ResultPage : Page, INotifyPropertyChanged
    {
        public ResultPage()
        {
            InitializeComponent();
            this.DataContext = this;
            Message = LangHelper.GetStr("InstalledAndActed");
            Info = LangHelper.GetStr("InstalledAndActedTip");
        }

        private string message;

        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                this.RaisePropertyChanged("Message");
            }
        }

        private string info;

        public string Info
        {
            get { return info; }
            set
            {
                info = value;
                this.RaisePropertyChanged("Info");
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
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
    }
}
