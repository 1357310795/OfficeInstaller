using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeInstaller.Helpers
{
    public static class OfficeHelper
    {
        public static List<string> GetInstalledProducts()
        {
            RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\Office\\ClickToRun\\Configuration");
            if (registryKey2 == null)
            {
                registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\Office\\ClickToRun\\Configuration");
                if (registryKey2 == null)
                {
                    return null;
                }
            }
            object obj = registryKey2.GetValue("ProductReleaseIds");
            if (obj != null)
            {
                registryKey?.Close();
                registryKey2?.Close();
                string text3 = obj.ToString();
                return text3.Split(new char[] { ',' }).ToList();
            }
            else
                return null;
        }
    }
}
