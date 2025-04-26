using System.Windows.Controls;
using SilkySouls2.ViewModels;

namespace SilkySouls2.Views
{
    public partial class UtilityTab
    {
        private readonly UtilityViewModel _utilityViewModel;

        public UtilityTab(UtilityViewModel utilityViewModel)
        {
            InitializeComponent();
            _utilityViewModel = utilityViewModel;
            DataContext = utilityViewModel;
        }
    }
}