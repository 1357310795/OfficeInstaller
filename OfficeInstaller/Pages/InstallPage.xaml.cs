using OfficeInstaller.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OfficeInstaller.Pages
{
    /// <summary>
    /// InstallPage.xaml 的交互逻辑
    /// </summary>
    public partial class InstallPage : Page, INotifyPropertyChanged
    {
        public InstallPage()
        {
            InitializeComponent();
            this.DataContext = this;
            Logs = new ObservableCollection<string>();
            //Logs.Add("测试测试测试");
            //Logs.Add("TestTest");
            //Logs.Add("你大V今年暑假被敌军课间放可男可女");
        }

        private ObservableCollection<string> logs;

        public ObservableCollection<string> Logs
        {
            get { return logs; }
            set
            {
                logs = value;
                this.RaisePropertyChanged("Logs");
            }
        }


        

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(Go);
            t.Start();
        }

        public void Go()
        {
            AddLog("安装开始");
            string filepath = "";
            try
            {
                var xml = Config.Default.GetXml();
                var tmpfolder = Path.GetTempPath();
                filepath = Path.Combine(tmpfolder, "config.xml");
                xml.Save(filepath);
                AddLog($"成功导出配置文件：{filepath}");
            }
            catch(Exception ex)
            {
                AddLog($"生成配置文件失败！\n{ex.Message}");
                return;
            }
            Process p = null;
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.Arguments = "/configure " + filepath;
                var exepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\setup.exe");
                AddLog($"安装程序路径：{exepath}");
                psi.FileName = exepath;
                psi.CreateNoWindow = true;
                p = Process.Start(psi);
                AddLog($"安装程序启动成功");
            }
            catch(Exception ex)
            {
                AddLog($"安装程序启动失败！\n{ex.Message}");
                return;
            }
            try
            {
                p.WaitForExit();
                AddLog($"安装完成");
            }
           catch(Exception ex)
            {
                AddLog($"安装失败！\n{ex.Message}");
                return;
            }
        }

        public void AddLog(string str)
        {
            this.Dispatcher.Invoke(() => { Logs.Add(str); });
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
