using System.Windows.Controls;

namespace PulseLogin.Services
{
    public interface INavigationService
    {
        void Navigate(UserControl page);
        void GoBack();
    }
}