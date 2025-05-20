using System;
using System.Windows;
using System.Windows.Threading;
using PulseLogin.Services;
using PulseLogin.Repositories;
using System.Diagnostics;

namespace PulseLogin
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly DispatcherTimer _timer;
        
        // Evento para notificar actualizaciones de tiempo a los componentes
        public static event EventHandler? TimeUpdated;
        
        // Servicios de la aplicación como propiedades estáticas para acceso global
        public static StateService StateService { get; private set; }
        public static IStateRepository StateRepository { get; private set; }
        
        /// <summary>
        /// Constructor de la aplicación
        /// </summary>
        public App()
        {
            try
            {
                // Inicializar servicios y repositorios
                StateRepository = new JsonStateRepository();
                StateService = new StateService(StateRepository);
                
                Debug.WriteLine("App: Servicios inicializados correctamente");
                
                // Configurar timer para actualizar el reloj cada segundo
                _timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                _timer.Tick += Timer_Tick;
                
                // Capturar excepciones no manejadas
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
                
                Debug.WriteLine("App: Configuración completada");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en constructor de App: {ex.Message}");
                MessageBox.Show($"Error al inicializar la aplicación: {ex.Message}", 
                    "Error de Inicialización", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Manejador para el evento de inicio de la aplicación
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            try
            {
                // Iniciar el timer
                _timer.Start();
                Debug.WriteLine("App: Timer iniciado");
                
                // Otras tareas de inicialización pueden ir aquí
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en OnStartup: {ex.Message}");
                MessageBox.Show($"Error al iniciar la aplicación: {ex.Message}", 
                    "Error de Inicio", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Manejador para el evento de apagado de la aplicación
        /// </summary>
        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                // Detener el timer
                _timer.Stop();
                Debug.WriteLine("App: Timer detenido");
                
                // Otras tareas de limpieza pueden ir aquí
                
                base.OnExit(e);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en OnExit: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Manejador para el evento tick del timer
        /// </summary>
        private void Timer_Tick(object? sender, EventArgs e)
        {
            try
            {
                // Notificar a los componentes que el tiempo ha cambiado
                TimeUpdated?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en Timer_Tick: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Manejador para excepciones no manejadas en el dominio de la aplicación
        /// </summary>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var ex = e.ExceptionObject as Exception;
                Debug.WriteLine($"Excepción no manejada: {ex?.Message}");
                
                // Registrar la excepción (podría implementarse logging en archivo)
                
                if (e.IsTerminating)
                {
                    MessageBox.Show($"La aplicación debe cerrarse debido a un error crítico: {ex?.Message}", 
                        "Error Fatal", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {
                // Último recurso para evitar recursión
            }
        }
        
        /// <summary>
        /// Manejador para excepciones no manejadas en el dispatcher
        /// </summary>
        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                Debug.WriteLine($"Excepción no manejada en UI: {e.Exception.Message}");
                
                // Registrar la excepción
                
                // Marcar como manejada para evitar que la app se cierre
                e.Handled = true;
                
                // Mostrar mensaje amigable al usuario
                MessageBox.Show($"Ha ocurrido un error inesperado: {e.Exception.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch
            {
                // Último recurso para evitar recursión
            }
        }
    }
}