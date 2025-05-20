using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using PulseLogin.ViewModels;
using PulseLogin.Services;
using PulseLogin.Views;
using System;
using System.Diagnostics;

namespace PulseLogin
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            // Permitir arrastrar la ventana desde cualquier parte
            this.MouseLeftButtonDown += (s, e) =>
            {
                if (e.ButtonState == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            };
            
            try
            {
                // Verificar que MainFrame esté inicializado
                if (MainFrame == null)
                {
                    MessageBox.Show("Error al inicializar el marco de navegación - MainFrame es nulo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al acceder al MainFrame: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Inicializar el servicio de navegación
            try
            {
                NavigationService.Initialize(MainFrame);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar el servicio de navegación: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Crear una instancia del servicio de navegación para el ViewModel
            var navigationService = new NavigationService();
            
            try
            {
                // Asignar el DataContext con el viewmodel y la navegación
                var loginViewModel = new LoginViewModel(navigationService);
                DataContext = loginViewModel;

                // Configurar el binding para el PasswordBox si estamos en la vista de login
                PasswordBox.PasswordChanged += (s, e) =>
                {
                    loginViewModel.Password = PasswordBox.Password;
                };

                // Decidir si mostrar directamente el login o navegar a la vista de login
                // Opción 1: Usar el contenido integrado en la ventana principal
                LoginContent.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error durante la inicialización: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void MinimizeButton_Click(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ForgotPassword_Click(object sender, MouseButtonEventArgs e)
        {
            var viewModel = this.DataContext as LoginViewModel;
            viewModel?.ForgotPasswordCommand.Execute(null);
        }

        // Método para navegar a una página manteniendo la barra superior
        public void NavigateToPage(UserControl userControl)
        {
            try
            {
                if (userControl == null)
                {
                    throw new ArgumentNullException(nameof(userControl), "El control de usuario no puede ser nulo");
                }
                
                // Ocultar el contenido de login
                if (LoginContent != null)
                {
                    LoginContent.Visibility = Visibility.Collapsed;
                }
                
                // Establecer el contenido del Frame
                MainFrame.Content = userControl;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error durante la navegación: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (LoginContent != null)
                {
                    LoginContent.Visibility = Visibility.Visible;
                }
            }
        }
    }
}