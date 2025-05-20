using System.Windows.Input;
using System.Windows;
using PulseLogin.Helpers;
using PulseLogin.Views; // Asegúrate de añadir esta referencia
using System.Threading.Tasks; // Para soporte de async/await
using PulseLogin.Services;
using System;

namespace PulseLogin.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string? _username;
        public string? Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string? _password;
        public string? Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private readonly INavigationService _navigationService;

        public ICommand LoginCommand { get; }
        public ICommand ForgotPasswordCommand { get; }

        // Constructor con inyección de dependencias para el servicio de navegación
        public LoginViewModel(INavigationService navigationService)
        {
            _username = string.Empty; // Inicializar para evitar advertencia
            _password = string.Empty;
            _navigationService = navigationService;

            LoginCommand = new AsyncRelayCommand(ExecuteLoginAsync);
            ForgotPasswordCommand = new RelayCommand(ExecuteForgotPassword);
        }

        // Constructor alternativo si no estás usando inyección de dependencias
        public LoginViewModel()
        {
            _username = string.Empty;
            _password = string.Empty;
            _navigationService = new NavigationService(); // Utilizaremos la instancia global inicializada

            LoginCommand = new AsyncRelayCommand(ExecuteLoginAsync);
            ForgotPasswordCommand = new RelayCommand(ExecuteForgotPassword);
        }

        private async Task ExecuteLoginAsync()
        {
            try
            {
                // Verifica que los campos no estén vacíos
                if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                {
                    MessageBox.Show("Por favor, ingrese usuario y contraseña", "Error de inicio de sesión",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Aquí iría la lógica de autenticación real
                // Por ejemplo, una llamada a un servicio de autenticación:
                bool isAuthenticated = await AuthenticateUserAsync(Username, Password);

                if (isAuthenticated)
                {
                    // Establecer el usuario actual en el servicio de estado
                    App.StateService.SetCurrentUser(Username);
                    
                    // Si el login es exitoso, navega a la nueva vista
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            // Imprimir mensaje para depuración
                            System.Diagnostics.Debug.WriteLine("Iniciando navegación al dashboard");
                            
                            // Crear la vista directamente
                            var mainDashboardView = new MainDashboardView();
                            
                            // Crear el ViewModel explícitamente
                            var viewModel = new MainDashboardViewModel();
                            
                            // Asignar el ViewModel a la vista
                            mainDashboardView.DataContext = viewModel;
                            System.Diagnostics.Debug.WriteLine($"ViewModel asignado: {mainDashboardView.DataContext != null}");
                            
                            // Obtener la ventana principal
                            var mainWindow = Application.Current.MainWindow as MainWindow;
                            if (mainWindow == null)
                            {
                                throw new InvalidOperationException("No se pudo obtener la ventana principal");
                            }
                            
                            // Navegar directamente usando la ventana principal
                            // Indicación visual para desarrollo - se puede eliminar en producción
                            System.Diagnostics.Debug.WriteLine("Navegando al dashboard...");
                            mainWindow.NavigateToPage(mainDashboardView);
                            System.Diagnostics.Debug.WriteLine("Navegación al dashboard completada");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error al navegar: {ex.Message}");
                            MessageBox.Show($"Error al navegar al dashboard: {ex.Message}", "Error de navegación",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    });
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrectos", "Error de autenticación",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error durante el inicio de sesión: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Método simulado de autenticación
        private async Task<bool> AuthenticateUserAsync(string username, string password)
        {
            // Simula una operación de autenticación que toma tiempo
            await Task.Delay(1000); // Simula una llamada a la API

            // Aquí implementarías tu lógica real de autenticación
            // Por ahora, simplemente aceptamos cualquier usuario/contraseña
            return true;
        }

        private void ExecuteForgotPassword()
        {
            MessageBox.Show("Funcionalidad de recuperación de contraseña");
        }
    }
}