using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace OfficeInstaller.Helpers
{
    public static class OSHelper
    {
        public static int GetOSBit()
        {
            try
            {
                string addressWidth = String.Empty;
                ConnectionOptions mConnOption = new ConnectionOptions();
                ManagementScope mMs = new ManagementScope(@"\\localhost", mConnOption);
                ObjectQuery mQuery = new ObjectQuery("select AddressWidth from Win32_Processor");
                ManagementObjectSearcher mSearcher = new ManagementObjectSearcher(mMs, mQuery);
                ManagementObjectCollection mObjectCollection = mSearcher.Get();
                foreach (ManagementObject mObject in mObjectCollection)
                {
                    addressWidth = mObject["AddressWidth"].ToString();
                }
                return Int32.Parse(addressWidth);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 32;
            }
        }

        public static string GetProgramFiles()
        {
            if (Environment.Is64BitOperatingSystem)
                return Environment.GetEnvironmentVariable("ProgramW6432");
            else
                return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        }
    }

}
