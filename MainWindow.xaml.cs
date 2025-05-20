using System;
using System.Windows;
using System.Windows.Controls;
using PulseLogin.Services;
using PulseLogin.Views;
using System.Diagnostics;

namespace PulseLogin
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NavigationService _navigationService;
        
        /// <summary>
        /// Constructor de la ventana principal
        /// </summary>
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                
                // Crear e inicializar el servicio de navegación
                _navigationService = new NavigationService();
                _navigationService.SetContentContainer(MainContent);
                
                Debug.WriteLine("MainWindow: NavigationService inicializado");
                
                // Configurar eventos
                Loaded += MainWindow_Loaded;
                Closing += MainWindow_Closing;
                
                Debug.WriteLine("MainWindow: Inicialización completada");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al inicializar MainWindow: {ex.Message}");
                MessageBox.Show($"Error al inicializar la ventana principal: {ex.Message}", 
                    "Error de Inicialización", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Evento que se dispara cuando la ventana se ha cargado completamente
        /// </summary>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Navegar a la vista de dashboard por defecto
                NavigateToDefaultPage();
                Debug.WriteLine("MainWindow: Navegación a página predeterminada");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en MainWindow_Loaded: {ex.Message}");
                MessageBox.Show($"Error al cargar la aplicación: {ex.Message}", 
                    "Error de Carga", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Evento que se dispara antes de cerrar la ventana
        /// </summary>
        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                // Lógica de limpieza al cerrar la aplicación
                Debug.WriteLine("MainWindow: Cerrando aplicación");
                
                // Guardar datos si es necesario
                if (MainContent.Content is MainDashboardView dashboard)
                {
                    if (dashboard.DataContext is ViewModels.MainDashboardViewModel viewModel)
                    {
                        viewModel.SaveDataBeforeExitAsync().ConfigureAwait(false);
                        Debug.WriteLine("Datos guardados antes de salir");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al cerrar la aplicación: {ex.Message}");
            }
        }

        /// <summary>
        /// Método público para navegar a una página específica
        /// </summary>
        /// <param name="page">Página a mostrar</param>
        public void NavigateToPage(UserControl page)
        {
            try
            {
                _navigationService.Navigate(page);
                Debug.WriteLine($"Navegación exitosa a: {page.GetType().Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al navegar: {ex.Message}");
                MessageBox.Show($"Error al navegar: {ex.Message}", 
                    "Error de Navegación", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Navega a la página predeterminada según el estado de la aplicación
        /// </summary>
        private void NavigateToDefaultPage()
        {
            try
            {
                // Por defecto, navegar al dashboard principal
                var dashboardView = new MainDashboardView();
                NavigateToPage(dashboardView);
                
                Debug.WriteLine("Navegación a dashboard predeterminado");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al navegar a página predeterminada: {ex.Message}");
                MessageBox.Show($"Error al cargar la vista inicial: {ex.Message}", 
                    "Error de Navegación", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Método para volver a la página anterior
        /// </summary>
        public void GoBack()
        {
            try
            {
                _navigationService.GoBack();
                Debug.WriteLine("Navegación hacia atrás");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al navegar hacia atrás: {ex.Message}");
                MessageBox.Show($"Error al navegar hacia atrás: {ex.Message}", 
                    "Error de Navegación", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}