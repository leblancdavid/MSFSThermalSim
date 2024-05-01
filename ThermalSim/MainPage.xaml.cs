namespace ThermalSim
{
    public partial class MainPage : ContentPage
    {
        private readonly MainPageViewModel viewModel;
        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
            this.viewModel = viewModel;
        }

        private void OnStartClicked(object sender, EventArgs e)
        {
            viewModel.Start();
        }

        private void OnStopClicked(object sender, EventArgs e)
        {
            viewModel.Stop();
        }
    }

}
