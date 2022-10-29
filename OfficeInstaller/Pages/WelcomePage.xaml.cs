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
    /// WelcomePage.xaml 的交互逻辑
    /// </summary>
    public partial class WelcomePage : Page, INotifyPropertyChanged
    {
        public WelcomePage()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private List<string> products;

        public List<string> Products
        {
            get { return products; }
            set
            {
                products = value;
                this.RaisePropertyChanged("Products");
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Products == null)
            {
                var res = MessageBox.Show(LangHelper.GetStr("NoInstalled"), LangHelper.GetStr("Warning"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var unsupported = "";
            foreach (var pro in Products)
            {
                if (pro != "ProPlus2021Volume" && pro != "VisioPro2021Volume" && pro != "ProjectPro2021Volume")
                    unsupported += pro + "\n";
            }
            if (unsupported != "")
            {
                var res = MessageBox.Show(LangHelper.GetStr("NotSupported") + "\n" + unsupported + "-------\n" + LangHelper.GetStr("Continue"), LangHelper.GetStr("Warning"), MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (res != MessageBoxResult.Yes)
                    return;
            }
                
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Products = OfficeHelper.GetInstalledProducts();
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
