using SilkySouls2.ViewModels;

namespace SilkySouls2.Views.Tabs
{
    public partial class EventTab
    {
        public EventTab(EventViewModel eventViewModel)
        {
            InitializeComponent();
            DataContext = eventViewModel;
        }
    }
}