using OfficeInstaller.Helpers;
using OfficeInstaller.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
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

        private bool insresult;
        private bool actresult;
        public bool onlyact;

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
            if (insresult)
            {
                if (actresult)
                {
                    this.Dispatcher.Invoke(() => {
                        Navigation.Navigate(new ResultPage());
                    });
                }
                else
                {
                    this.Dispatcher.Invoke(() => {
                        LogService.Logs = Logs.ToList();
                        Navigation.Navigate(new ResultPageActFail());
                    });
                }
            }
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
            string filepath = Path.GetTempFileName();
            var vlmcspath = Path.Combine(Config.Default.DataPath, @"vlmcs.exe");
            var setuppath = Path.Combine(Config.Default.DataPath, @"setup.exe");

            if (!onlyact)
            {
                AddLog(LangHelper.GetStr("InstallStart"));
                try
                {
                    AddLog(LangHelper.GetStr("ExportConfig"));
                    var xml = Config.Default.GetXml();
                    xml.Save(filepath);
                    AddLog($"{LangHelper.GetStr("ExportConfigSucc")}{filepath}");
                }
                catch (Exception ex)
                {
                    AddLog($"{LangHelper.GetStr("ExportConfigFail")}\n{ex.Message}");
                    return;
                }

                Process p = null;
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.Arguments = $"/configure \"{filepath}\"";
                    AddLog($"{LangHelper.GetStr("SetupPath")}{setuppath}");
                    psi.FileName = setuppath;
                    psi.CreateNoWindow = true;
                    psi.WindowStyle = ProcessWindowStyle.Hidden;
                    p = Process.Start(psi);
                    AddLog(LangHelper.GetStr("LaunchSucc"));
                }
                catch (Exception ex)
                {
                    AddLog($"{LangHelper.GetStr("LaunchFail")}\n{ex.Message}");
                    return;
                }
                try
                {
                    p.WaitForExit();
                    if (p.ExitCode != 0)
                        throw new Exception(LangHelper.GetStr("SetupReturnNonZero"));
                    AddLog(LangHelper.GetStr("InstallSucc"));
                }
                catch (Exception ex)
                {
                    AddLog($"{LangHelper.GetStr("InstallFail")}\n{ex.Message}");
                    return;
                }
            }
            insresult = true;

            try
            {
                AddLog(LangHelper.GetStr("CheckKMS"));
                CommandRunner cr = new CommandRunner(vlmcspath);
                var res = cr.Run("kms.sjtu.edu.cn");
                if (res != null && res.ToLower().Contains("success"))
                {
                    AddLog(res);
                    AddLog(LangHelper.GetStr("KMSAvail"));
                    StateService.KMSOK = true;
                }
                else
                {
                    AddLog(res);
                    AddLog(LangHelper.GetStr("KMSUnavail"));
                    StateService.KMSOK = false;
                }
            }
            catch (Exception ex)
            {
                AddLog($"{LangHelper.GetStr("CheckKMSFail")}\n{ex.Message}");
                return;
            }
            try
            {
                // 创建WMI会话
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM SoftwareLicensingService");
                ManagementObject obj = searcher.Get().Cast<ManagementObject>().First();

                // 获取产品密钥，这里假设你已经有了产品密钥
                string productKey = "XJ2XN-FW8RK-P4HMP-DKDBV-GCVGB"; // 替换为你的产品密钥

                // 准备方法参数
                ManagementBaseObject inParams = obj.GetMethodParameters("InstallProductKey");
                inParams["ProductKey"] = productKey;

                // 调用InstallProductKey方法
                ManagementBaseObject outParams = obj.InvokeMethod("InstallProductKey", inParams, null);

                //Refresh
                obj.InvokeMethod("RefreshLicenseStatus", null, null);
            }
            catch(Exception ex)
            {
                AddLog($"{ex.ToString()}");
                return;
            }
            try
            {
                AddLog(LangHelper.GetStr("Activating"));
                CommandRunner cr = new CommandRunner("cscript");
                string ospppath = Path.Combine(OSHelper.GetProgramFiles(), @"Microsoft Office\Office16\ospp.vbs");
                if (!File.Exists(ospppath))
                {
                    ospppath = Path.Combine(OSHelper.GetProgramFilesX86(), @"Microsoft Office\Office16\ospp.vbs");
                    if (!File.Exists(ospppath))
                        throw new Exception($"{LangHelper.GetStr("OSPP")}{ospppath}");
                }

                var res = cr.Run($"\"{ospppath}\" /sethst:kms.sjtu.edu.cn");
                if (res != null && res.ToLower().Contains("success"))
                {
                    AddLog(res);
                    AddLog(LangHelper.GetStr("ActSucc"));
                }
                else
                {
                    throw new Exception(res);
                }
            }
            catch (Exception ex)
            {
                AddLog($"{LangHelper.GetStr("ActFail")}\n{ex.Message}");
                return;
            }
            actresult = true;
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
