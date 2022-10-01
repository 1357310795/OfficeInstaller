using OfficeInstaller.Helpers;
using OfficeInstaller.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace OfficeInstaller
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            if (OSHelper.GetOSBit() == 64)
                Config.Default.X64 = true;

            var path = System.AppDomain.CurrentDomain.BaseDirectory;
            var vlmcs = Path.Combine(path, @"data\vlmcs.exe");
            var setup = Path.Combine(path, @"data\setup.exe");
            if (!File.Exists(vlmcs))
            {
                if (!File.Exists(setup))
                {
                    MessageBox.Show("请先解压整个文件夹，再运行程序！");
                    App.Current.Shutdown();
                    return;
                }
                else
                {
                    MessageBox.Show("文件丢失，请重新下载、解压整个文件夹，再运行程序！");
                    App.Current.Shutdown();
                    return;
                }
            }
            else
            {
                if (!File.Exists(setup))
                {
                    MessageBox.Show("文件丢失，请重新下载、解压整个文件夹，再运行程序！");
                    App.Current.Shutdown();
                    return;
                }
            }
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString());
        }
    }
}
