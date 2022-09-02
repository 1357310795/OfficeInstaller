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
            if (StateService.KMSOK)
            {
                Message = "已安装并激活 Office";
                Info = "若显示未激活，请连接到校园网，重启 Office 应用程序即可。";
            }
            else
            {
                Message = "已安装 Office";
                Info = "当前交大 KMS 服务器不可用，请连接到校园网，Office 应用程序将自动激活。";
            } 
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
