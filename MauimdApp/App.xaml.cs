using MauimdApp.Services;

namespace MauimdApp
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;
        public App(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            InitializeComponent();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }
        
        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new MainPage()) { };
            window.Destroying += Window_Destroying; ;
            return window;
        }

        private void Window_Destroying(object? sender, EventArgs e)
        {
            
        }
    }
}
