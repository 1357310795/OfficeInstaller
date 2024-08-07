﻿using OfficeInstaller.Helpers;
using OfficeInstaller.Services;
using System;
using System.Collections.Generic;
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
    /// WelcomePage.xaml 的交互逻辑
    /// </summary>
    public partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            InitializeComponent();
            this.DataContext = Config.Default;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Navigation.Navigate(new InstallPage());
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            LangHelper.ChangeLang(true);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            LangHelper.ChangeLang(false);
        }
    }
}
