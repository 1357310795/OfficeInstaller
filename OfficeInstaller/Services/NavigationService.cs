using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OfficeInstaller.Services
{
    public static class Navigation
    {
        static Frame frame;
        public static void SetFrame(Frame _frame)
        {
            frame = _frame;
        }

        public static void Navigate(Page page)
        {
            App.Current.Dispatcher.Invoke(() => {
                frame.Navigate(page);
            });
        }
    }
}
