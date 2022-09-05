using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeInstaller.Helpers
{
    public static class LangHelper
    {
        public static void ChangeLang(bool eng)
        {
            App.Current.Resources.MergedDictionaries[0] = new System.Windows.ResourceDictionary()
            {
                Source = new Uri((@"pack://application:,,,/OfficeInstaller;component/Resources/" + (eng ? "en-us" : "zh-cn") + ".xaml"), UriKind.RelativeOrAbsolute)
            };
        }

        public static string GetStr(string key)
        {
            var dic = App.Current.Resources.MergedDictionaries[0];
            return dic[key] as string;
        }
    }
}
