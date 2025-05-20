using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;
using System.Linq;
using PulseLogin.Helpers;
using PulseLogin.Models;
using PulseLogin.Services;

namespace PulseLogin.ViewModels
{
    /// <summary>
    /// ViewModel principal para la vista del dashboard
    /// </summary>
    public class MainDashboardViewModel : BaseViewModel
    {
        private string _currentTime = "--:--";
        private string _currentDate = "--/--";
        private string _currentYear = "----";
        private string _currentDay = "-----";
        private string _currentState = "ESTADO ACTUAL";
        private string _elapsedTime = "00:00:00";
        private DateTime _startTime;
        private bool _isSubscribed = false;

        // Servicio para la persistencia de estado
        private readonly StateService _stateService;

        // Servicio de navegación
        private readonly INavigationService _navigationService;

        /// <summary>
        /// Constructor del ViewModel
        /// </summary>
        /// <param name="navigationService">Servicio de navegación</param>
        public MainDashboardViewModel(INavigationService? navigationService = null)
        {
            try
            {
                // Establecer el tiempo inicial
                _startTime = DateTime.Now;

                // Asignar la navegación; si es null, crear una nueva instancia
                _navigationService = navigationService ?? new NavigationService();

                // Obtener instancia del servicio de estado
                _stateService = App.StateService;

                // Inicializar comandos
                ConfiguracionCommand = new RelayCommand(NavigateToConfiguracion);

                // Cargar datos iniciales de forma asíncrona
                LoadInitialDataAsync();

                // Actualizar fecha y hora inicialmente
                UpdateDateTime();
                UpdateElapsedTime();

                // Suscribirse al evento global de actualización de tiempo
                if (!_isSubscribed)
                {
                    App.TimeUpdated += App_TimeUpdated;
                    _isSubscribed = true;
                }

                Debug.WriteLine("MainDashboardViewModel inicializado correctamente");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al inicializar el dashboard: {ex.Message}");
            }
        }

        /// <summary>
        /// Hora actual formateada
        /// </summary>
        public string CurrentTime
        {
            get => _currentTime;
            set => SetProperty(ref _currentTime, value);
        }

        /// <summary>
        /// Fecha actual formateada
        /// </summary>
        public string CurrentDate
        {
            get => _currentDate;
            set => SetProperty(ref _currentDate, value);
        }

        /// <summary>
        /// Año actual
        /// </summary>
        public string CurrentYear
        {
            get => _currentYear;
            set => SetProperty(ref _currentYear, value);
        }

        /// <summary>
        /// Día de la semana actual
        /// </summary>
        public string CurrentDay
        {
            get => _currentDay;
            set => SetProperty(ref _currentDay, value);
        }

        /// <summary>
        /// Estado actual seleccionado
        /// </summary>
        public string CurrentState
        {
            get => _currentState;
            set
            {
                try
                {
                    Debug.WriteLine($"Estableciendo estado: '{value}' (anterior: '{_currentState}')");
                    
                    // Solo reinicia el temporizador si cambia el estado
                    if (_currentState != value)
                    {
                        // Guardar valor anterior para debuggear
                        string oldValue = _currentState;
                        
                        // Establecer el nuevo valor
                        SetProperty(ref _currentState, value);
                        Debug.WriteLine($"Estado cambiado de '{oldValue}' a '{_currentState}'");
                        
                        // Reiniciar timer
                        Debug.WriteLine("Reiniciando timer desde CurrentState setter");
                        ResetTimer();
                        
                        // Persistir el cambio de estado
                        PersistCurrentState();
                    }
                    else
                    {
                        Debug.WriteLine("El estado no ha cambiado, manteniendo valor actual");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error al cambiar el estado: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Tiempo transcurrido en el estado actual
        /// </summary>
        public string ElapsedTime
        {
            get => _elapsedTime;
            set => SetProperty(ref _elapsedTime, value);
        }

        /// <summary>
        /// Lista de estados disponibles
        /// </summary>
        public ObservableCollection<string> States { get; private set; } = new ObservableCollection<string>();

        /// <summary>
        /// Lista de actividades del día
        /// </summary>
        public ObservableCollection<ActivityItem> Activities { get; private set; } = new ObservableCollection<ActivityItem>();

        /// <summary>
        /// Comando para navegar a la configuración
        /// </summary>
        public ICommand? ConfiguracionCommand { get; private set; }

        /// <summary>
        /// Método público para forzar la notificación de cambio de estado
        /// </summary>
        public void NotifyStateChanged()
        {
            try
            {
                Debug.WriteLine("NotifyStateChanged: Forzando notificación de cambio de estado");
                OnPropertyChanged(nameof(CurrentState));
                Debug.WriteLine($"Estado actual notificado: {CurrentState}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en NotifyStateChanged: {ex.Message}");
            }
        }

        /// <summary>
        /// Método para reiniciar el temporizador
        /// </summary>
        public void ResetTimer()
        {
            try
            {
                Debug.WriteLine("Reiniciando temporizador en el ViewModel...");
                
                // Reiniciar la hora de inicio
                _startTime = DateTime.Now;
                
                // Actualizar la visualización del tiempo transcurrido
                UpdateElapsedTime();
                
                // Notificar explicitamente que el tiempo ha cambiado
                OnPropertyChanged(nameof(ElapsedTime));
                
                Debug.WriteLine($"Temporizador reiniciado correctamente. Nuevo ElapsedTime: {ElapsedTime}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al reiniciar el temporizador: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Método para persistir el estado actual
        /// </summary>
        private async void PersistCurrentState()
        {
            try
            {
                bool result = await _stateService.UpdateCurrentStateAsync(CurrentState);
                Debug.WriteLine($"Estado persistido: {result}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al persistir el estado: {ex.Message}");
            }
        }

        /// <summary>
        /// Carga los datos iniciales del servicio
        /// </summary>
        private async void LoadInitialDataAsync()
        {
            try
            {
                // Cargar el estado actual y su tiempo de inicio
                var (state, startTime) = await _stateService.GetCurrentStateAsync();
                _currentState = state;
                _startTime = startTime;
                OnPropertyChanged(nameof(CurrentState));

                // Cargar los estados disponibles
                var states = await _stateService.GetAvailableStatesAsync();
                
                // Usar el dispatcher para actualizar la colección en el hilo de UI
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    States.Clear();
                    foreach (var s in states)
                    {
                        States.Add(s);
                    }
                });

                // Cargar las actividades
                var activities = await _stateService.GetActivitiesAsync();
                
                // Usar el dispatcher para actualizar la colección en el hilo de UI
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    Activities.Clear();
                    foreach (var activity in activities)
                    {
                        Activities.Add(new ActivityItem
                        {
                            Time = activity.Time ?? "00:00",
                            Description = activity.Description ?? "ACTIVIDAD PLANEADA"
                        });
                    }

                    // Si no hay actividades, añadir algunas por defecto
                    if (Activities.Count == 0)
                    {
                        Activities.Add(new ActivityItem { Time = "00:00", Description = "ACTIVIDAD PLANEADA" });
                        Activities.Add(new ActivityItem { Time = "00:00", Description = "ACTIVIDAD PLANEADA" });
                        Activities.Add(new ActivityItem { Time = "13:00", Description = "ALMUERZO" });
                        Activities.Add(new ActivityItem { Time = "00:00", Description = "ACTIVIDAD PLANEADA" });
                    }
                });
                
                Debug.WriteLine("Datos iniciales cargados correctamente");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al cargar datos iniciales: {ex.Message}");
                // En caso de error, inicializar con valores por defecto
                InitializeDefaultValues();
            }
        }

        /// <summary>
        /// Inicializa valores por defecto si no se pueden cargar los datos
        /// </summary>
        private void InitializeDefaultValues()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                // Estados por defecto
                States.Clear();
                foreach (var state in new[] { "ESTADO 1", "ESTADO 2", "ESTADO 3", "ESTADO 4",
                                            "ESTADO 5", "ESTADO 6", "ESTADO 7", "ESTADO 8" })
                {
                    States.Add(state);
                }

                // Actividades por defecto
                Activities.Clear();
                Activities.Add(new ActivityItem { Time = "09:00", Description = "ACTIVIDAD PLANEADA" });
                Activities.Add(new ActivityItem { Time = "10:30", Description = "ACTIVIDAD PLANEADA" });
                Activities.Add(new ActivityItem { Time = "13:00", Description = "ALMUERZO" });
                Activities.Add(new ActivityItem { Time = "15:00", Description = "ACTIVIDAD PLANEADA" });
            });
            
            Debug.WriteLine("Valores predeterminados inicializados");
        }

        /// <summary>
        /// Guarda los datos antes de salir de la aplicación
        /// </summary>
        public async Task SaveDataBeforeExitAsync()
        {
            try
            {
                if (Activities.Any())
                {
                    var activityList = Activities.Select(a => new ActivityData
                    {
                        Time = a.Time,
                        Description = a.Description,
                        IsCompleted = false,
                        Date = DateTime.Today
                    }).ToList();

                    await _stateService.UpdateActivitiesAsync(activityList);
                    Debug.WriteLine("Actividades guardadas antes de salir");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al guardar datos antes de salir: {ex.Message}");
            }
        }

        /// <summary>
        /// Manejador para el evento de actualización de tiempo
        /// </summary>
        private void App_TimeUpdated(object? sender, EventArgs e)
        {
            try
            {
                // Usar Dispatcher para asegurar que la actualización se realiza en el hilo UI
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    // Actualizar la hora y fecha cada segundo
                    UpdateDateTime();
                    UpdateElapsedTime();
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en App_TimeUpdated: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza la fecha y hora actuales
        /// </summary>
        private void UpdateDateTime()
        {
            var now = DateTime.Now;
            CurrentTime = now.ToString("HH:mm");
            CurrentDate = now.ToString("dd/MM");
            CurrentYear = now.ToString("yyyy");
            CurrentDay = GetDayName(now.DayOfWeek);
        }

        /// <summary>
        /// Actualiza el tiempo transcurrido desde el inicio del estado
        /// </summary>
        private void UpdateElapsedTime()
        {
            var elapsed = DateTime.Now - _startTime;
            ElapsedTime = string.Format("{0:00}:{1:00}:{2:00}",
                elapsed.Hours, elapsed.Minutes, elapsed.Seconds);
        }

        /// <summary>
        /// Navega a la vista de configuración
        /// </summary>
        private void NavigateToConfiguracion()
        {
            try
            {
                // Guardar datos antes de navegar
                SaveDataBeforeExitAsync().ConfigureAwait(false);

                // Crear la vista de configuración
                var configView = new Views.ConfiguracionView();
                
                // Navegar a la nueva vista
                _navigationService.Navigate(configView);
                
                Debug.WriteLine("Navegación a configuración");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al navegar a configuración: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene el nombre del día de la semana
        /// </summary>
        private string GetDayName(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday: return "LUNES";
                case DayOfWeek.Tuesday: return "MARTES";
                case DayOfWeek.Wednesday: return "MIÉRCOLES";
                case DayOfWeek.Thursday: return "JUEVES";
                case DayOfWeek.Friday: return "VIERNES";
                case DayOfWeek.Saturday: return "SÁBADO";
                case DayOfWeek.Sunday: return "DOMINGO";
                default: return string.Empty;
            }
        }

        /// <summary>
        /// Destructor para liberar recursos
        /// </summary>
        ~MainDashboardViewModel()
        {
            if (_isSubscribed)
            {
                App.TimeUpdated -= App_TimeUpdated;
            }

            // Guardar datos antes de destruir el ViewModel
            SaveDataBeforeExitAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Modelo para representar una actividad en la interfaz
    /// </summary>
    public class ActivityItem : BaseViewModel
    {
        private string _time = string.Empty;
        private string _description = string.Empty;

        /// <summary>
        /// Hora de la actividad
        /// </summary>
        public string Time
        {
            get => _time;
            set => SetProperty(ref _time, value);
        }

        /// <summary>
        /// Descripción de la actividad
        /// </summary>
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }
    }
}