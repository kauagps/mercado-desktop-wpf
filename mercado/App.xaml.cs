using System.Windows;
using mercado.View;

namespace mercado
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Mostrar tela de login primeiro
            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}
