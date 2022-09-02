using OfficeInstaller.Helpers;
using OfficeInstaller.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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

        private bool result;
        

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(Run);
            t.ApartmentState = ApartmentState.STA;
            t.Start();
        }

        public void Run()
        {
            StateService.IsRunning = true;
            Install();
            StateService.IsRunning = false;
            if (result)
                this.Dispatcher.Invoke(() => {
                    Navigation.Navigate(new ResultPage());
                });
            else
            {
                this.Dispatcher.Invoke(() => { 
                    LogService.Logs = Logs.ToList();
                    Navigation.Navigate(new ResultPageFail()); 
                });
            }
        }

        public void Install()
        {
            string filepath = "";
            string setuppath = "";
            string vlmcspath = "";
            var tmpfolder = Path.GetTempPath();

            AddLog("安装开始");

            try
            {
                AddLog($"正在导出配置文件");
                var xml = Config.Default.GetXml();
                filepath = Path.Combine(tmpfolder, "config.xml");
                xml.Save(filepath);
                AddLog($"导出配置文件成功：{filepath}");
            }
            catch (Exception ex)
            {
                AddLog($"生成配置文件失败！\n{ex.Message}");
                return;
            }

            try
            {
                AddLog($"正在解压资源");
                var setupres = "pack://application:,,,/OfficeInstaller;component/setup.exe";
                var setupuri = new Uri(setupres, UriKind.RelativeOrAbsolute);
                var setupstream = Application.GetResourceStream(setupuri).Stream;
                setuppath = Path.Combine(tmpfolder, "OfficeSetup.exe");
                if (!File.Exists(setuppath))
                {
                    using (var fileStream = File.Create(setuppath))
                    {
                        setupstream.Seek(0, SeekOrigin.Begin);
                        setupstream.CopyTo(fileStream);
                        fileStream.Close();
                    }
                }

                var vlmcsres = "pack://application:,,,/OfficeInstaller;component/vlmcs.exe";
                var vlmcsuri = new Uri(vlmcsres, UriKind.RelativeOrAbsolute);
                var vlmcsstream = Application.GetResourceStream(vlmcsuri).Stream;
                vlmcspath = Path.Combine(tmpfolder, "vlmcs.exe");
                if (!File.Exists(vlmcspath))
                {
                    using (var fileStream = File.Create(vlmcspath))
                    {
                        vlmcsstream.Seek(0, SeekOrigin.Begin);
                        vlmcsstream.CopyTo(fileStream);
                        fileStream.Close();
                    }
                }
                AddLog($"解压资源成功！");
            }
            catch (Exception ex)
            {
                AddLog($"解压资源失败！\n{ex.ToString()}");
                return;
            }
            //return;
            Process p = null;
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.Arguments = "/configure " + filepath;
                AddLog($"安装程序路径：{setuppath}");
                psi.FileName = setuppath;
                psi.CreateNoWindow = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                p = Process.Start(psi);
                AddLog($"安装程序启动成功");
            }
            catch (Exception ex)
            {
                AddLog($"安装程序启动失败！\n{ex.Message}");
                return;
            }
            try
            {
                p.WaitForExit();
                if (p.ExitCode != 0)
                    throw new Exception("setup.exe 返回值非0");
                AddLog($"安装完成");
            }
            catch (Exception ex)
            {
                AddLog($"安装失败！\n{ex.Message}");
                return;
            }
            try
            {
                AddLog($"正在检测 KMS 服务器可用性");
                CommandRunner cr = new CommandRunner(vlmcspath);
                var res = cr.Run("kms.sjtu.edu.cn");
                if (res != null && res.ToLower().Contains("success"))
                {
                    AddLog(res);
                    AddLog($"KMS 服务器可用！");
                    StateService.KMSOK = true;
                }
                else
                {
                    AddLog(res);
                    AddLog($"KMS 服务器不可用！");
                    StateService.KMSOK = false;
                }
            }
            catch (Exception ex)
            {
                AddLog($"检测 KMS 服务器可用性失败！\n{ex.Message}");
                return;
            }
            try
            {
                AddLog($"正在激活！");
                CommandRunner cr = new CommandRunner("cscript");
                var ospp = Path.Combine(Environment.GetFolderPath(
                    Environment.SpecialFolder.ProgramFiles
                    ), @"Microsoft Office\Office16\ospp.vbs");
                if (!File.Exists(ospp))
                    throw new Exception($"找不到 ospp.vbs：{ospp}");
                var res = cr.Run($"\"{ospp}\" /sethst:kms.sjtu.edu.cn");
                if (res != null && res.ToLower().Contains("success"))
                {
                    AddLog(res);
                    AddLog($"激活成功！");
                }
                else
                {
                    throw new Exception(res);
                }
            }
            catch (Exception ex)
            {
                AddLog($"激活失败！\n{ex.Message}");
                return;
            }
            result = true;
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
