using SilkySouls2.ViewModels;

namespace SilkySouls2.Views.Tabs
{
    public partial class UtilityTab
    {
        public UtilityTab(UtilityViewModel utilityViewModel)
        {
            InitializeComponent();
            DataContext = utilityViewModel;
        }

    }
}