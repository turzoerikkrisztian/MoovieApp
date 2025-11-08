using MoovieApp.ViewModels;


namespace MoovieApp.Pages
{

    public partial class LoginPage : ContentPage
    {
        public LoginPage(AuthViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel; 
        }
    }
}