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
            var basepath = System.AppDomain.CurrentDomain.BaseDirectory;
            var vlmcspath = Path.Combine(basepath, @"data\vlmcs.exe");
            var setuppath = Path.Combine(basepath, @"data\setup.exe");
            string ospppath = Path.Combine(OSHelper.GetProgramFiles(), @"Microsoft Office\Office16\ospp.vbs");
            var tmpfolder = Path.GetTempPath();

            AddLog(LangHelper.GetStr("InstallStart"));

            try
            {
                AddLog(LangHelper.GetStr("ExportConfig"));
                var xml = Config.Default.GetXml();
                filepath = Path.Combine(tmpfolder, "config.xml");
                xml.Save(filepath);
                AddLog($"{LangHelper.GetStr("ExportConfigSucc")}{filepath}");
            }
            catch (Exception ex)
            {
                AddLog($"{LangHelper.GetStr("ExportConfigFail")}\n{ex.Message}");
                return;
            }

            //try
            //{
            //    AddLog(LangHelper.GetStr("ExtractExe"));
            //    var setupres = "pack://application:,,,/OfficeInstaller;component/setup.exe";
            //    var setupuri = new Uri(setupres, UriKind.RelativeOrAbsolute);
            //    var setupstream = Application.GetResourceStream(setupuri).Stream;
            //    setuppath = Path.Combine(tmpfolder, "OfficeSetup.exe");
            //    if (!File.Exists(setuppath))
            //    {
            //        using (var fileStream = File.Create(setuppath))
            //        {
            //            setupstream.Seek(0, SeekOrigin.Begin);
            //            setupstream.CopyTo(fileStream);
            //            fileStream.Close();
            //        }
            //    }

            //    var vlmcsres = "pack://application:,,,/OfficeInstaller;component/vlmcs.exe";
            //    var vlmcsuri = new Uri(vlmcsres, UriKind.RelativeOrAbsolute);
            //    var vlmcsstream = Application.GetResourceStream(vlmcsuri).Stream;
            //    vlmcspath = Path.Combine(tmpfolder, "vlmcs.exe");
            //    if (!File.Exists(vlmcspath))
            //    {
            //        using (var fileStream = File.Create(vlmcspath))
            //        {
            //            vlmcsstream.Seek(0, SeekOrigin.Begin);
            //            vlmcsstream.CopyTo(fileStream);
            //            fileStream.Close();
            //        }
            //    }

            //    //var osppres = "pack://application:,,,/OfficeInstaller;component/ospp.vbs";
            //    //var osppuri = new Uri(osppres, UriKind.RelativeOrAbsolute);
            //    //var osppstream = Application.GetResourceStream(osppuri).Stream;
            //    //ospppath = Path.Combine(tmpfolder, "ospp.vbs");
            //    //if (!File.Exists(ospppath))
            //    //{
            //    //    using (var fileStream = File.Create(ospppath))
            //    //    {
            //    //        osppstream.Seek(0, SeekOrigin.Begin);
            //    //        osppstream.CopyTo(fileStream);
            //    //        fileStream.Close();
            //    //    }
            //    //}
            //    AddLog(LangHelper.GetStr("ExtractExeSucc"));
            //}
            //catch (Exception ex)
            //{
            //    AddLog($"{LangHelper.GetStr("ExtractExeFail")}\n{ex.ToString()}");
            //    return;
            //}
            //return;
            Process p = null;
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.Arguments = "/configure " + filepath;
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
                AddLog(LangHelper.GetStr("Activating"));
                CommandRunner cr = new CommandRunner("cscript");
                if (!File.Exists(ospppath))
                    throw new Exception($"{LangHelper.GetStr("OSPP")}{ospppath}");
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
