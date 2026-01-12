using SilkySouls2.ViewModels;

namespace SilkySouls2.Views.Tabs
{
    public partial class TravelTab
    {
        public TravelTab(TravelViewModel travelViewModel)
        {
            InitializeComponent();
            DataContext = travelViewModel;
        }
        
    }
}