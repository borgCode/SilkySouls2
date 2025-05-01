using System.Windows;
using System.Windows.Controls;
using SilkySouls2.ViewModels;

namespace SilkySouls2.Views
{
    public partial class EventTab
    {
        private readonly EventViewModel _eventViewModel;
        
        public EventTab(EventViewModel eventViewModel)
        {
            InitializeComponent();
            _eventViewModel = eventViewModel;
            DataContext = eventViewModel;
        }

        private void Unlock_Darklurker(object sender, RoutedEventArgs e) => _eventViewModel.UnlockDarklurker();
    }
}