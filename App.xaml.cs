using System;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;
using System.Diagnostics;
using PulseLogin.Repositories;
using PulseLogin.Services;

namespace PulseLogin;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    // Temporizador global a nivel de aplicación
    public static DispatcherTimer GlobalTimer { get; private set; } = new DispatcherTimer();
    
    // Evento para notificar actualizaciones de tiempo
    public static event EventHandler? TimeUpdated;

    // Servicios de la aplicación (accesibles globalmente)
    public static IStateRepository StateRepository { get; private set; } = new JsonStateRepository();
    public static StateService StateService { get; private set; } = null!; // Inicializada en InitializeServices
    
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Debug.WriteLine("Aplicación iniciada");
        
        // Inicializar los servicios
        InitializeServices();
        
        // Configurar e iniciar el temporizador global
        GlobalTimer = new DispatcherTimer(DispatcherPriority.Normal);
        GlobalTimer.Interval = TimeSpan.FromSeconds(1);
        GlobalTimer.Tick += (s, args) => 
        {
            // Disparar el evento de actualización de tiempo
            TimeUpdated?.Invoke(s, args);
        };
        GlobalTimer.Start();
        
        Debug.WriteLine("Temporizador global iniciado");
    }

    private void InitializeServices()
    {
        try
        {
            // Crear instancias de los servicios
            StateRepository = new JsonStateRepository();
            StateService = new StateService(StateRepository);
            
            Debug.WriteLine("Servicios inicializados correctamente");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error al inicializar los servicios: {ex.Message}");
            MessageBox.Show($"Error al inicializar la aplicación: {ex.Message}", 
                "Error de inicialización", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}