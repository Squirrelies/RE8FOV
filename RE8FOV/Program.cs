using System;
using System.Windows.Forms;

namespace RE8FOV
{
    public static class Program
    {
        public static ApplicationContext context;

        [STAThread]
        public static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (context = new ApplicationContext(new MainUI()))
                Application.Run(context);
        }
    }
}
