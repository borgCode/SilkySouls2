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

        private void RescueKnights_Click(object sender, RoutedEventArgs e) => _eventViewModel.RescueKnights();

        private void UnlockNash_Click(object sender, RoutedEventArgs e) => _eventViewModel.UnlockNash();

        private void UnlockAldia_Click(object sender, RoutedEventArgs e) => _eventViewModel.UnlockAldia();

        private void VisibleAava_Click(object sender, RoutedEventArgs e) => _eventViewModel.VisibleAava();

        private void BreakIce_Click(object sender, RoutedEventArgs e) => _eventViewModel.BreakIce();

        private void KingsRingAcquired_Click(object sender, RoutedEventArgs e) => _eventViewModel.KingsRingAcquired();

        private void SetAlive_Click(object sender, RoutedEventArgs e) => _eventViewModel.SetNpcAlive();
        private void SetDead_Click(object sender, RoutedEventArgs e) => _eventViewModel.SetNpcDead();
        private void SetFriendly_Click(object sender, RoutedEventArgs e) => _eventViewModel.SetNpcFriendly();
        private void SetHostile_Click(object sender, RoutedEventArgs e) => _eventViewModel.SetNpcHostile();
        private void MovetoMajula_Click(object sender, RoutedEventArgs e) => _eventViewModel.MoveNpcToMajula();
    }
}