namespace MoovieApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(Pages.MovieDetailsPage), typeof(Pages.MovieDetailsPage));
        }
    }
}
