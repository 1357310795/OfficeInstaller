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

        public void Install()
        {
            string filepath = "";
            var vlmcspath = Path.Combine(Config.Default.DataPath, @"vlmcs.exe");
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
                    return;
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
                AddLog(LangHelper.GetStr("SettingKMS"));
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
                    
                var res = cr.Run($"\"{ospppath}\" /act");
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
