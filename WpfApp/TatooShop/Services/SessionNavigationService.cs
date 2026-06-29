using System.Linq;
using System.Windows;
using TatooShop.View;

namespace TatooShop.Services
{
    public static class SessionNavigationService
    {
        public static void LogoutToLoginWindow()
        {
            Manager.Logout();

            var loginWindow = new LogWindow();
            var previousWindows = Application.Current.Windows
                .OfType<Window>()
                .Where(window => window != loginWindow)
                .ToList();

            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            Application.Current.MainWindow = loginWindow;
            loginWindow.Show();

            foreach (var window in previousWindows)
                window.Close();

            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }
    }
}
