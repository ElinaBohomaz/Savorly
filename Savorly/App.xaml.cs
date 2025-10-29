using System.Windows;
using Savorly.Services;

namespace Savorly
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DatabaseService.InitializeDatabase();
        }
    }
}