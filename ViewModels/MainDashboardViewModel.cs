using PulseLogin.Models;
using PulseLogin.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;
using PulseLogin.Helpers;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PulseLogin.ViewModels
{
    public class MainDashboardViewModel : BaseViewModel
    {
        private readonly StateService _stateService;
        private readonly DispatcherTimer _timer;

        // Propiedades para mostrar el estado actual y su duración
        private string _currentState = "ESTADO ACTUAL";
        public string CurrentState
        {
            get => _currentState;
            set => SetProperty(ref _currentState, value);
        }

        private DateTime _stateStartTime = DateTime.Now;
        public DateTime StateStartTime
        {
            get => _stateStartTime;
            set => SetProperty(ref _stateStartTime, value);
        }

        private TimeSpan _elapsedTime;
        public TimeSpan ElapsedTime
        {
            get => _elapsedTime;
            set => SetProperty(ref _elapsedTime, value);
        }

        // Lista de estados disponibles
        private ObservableCollection<string> _availableStates = new ObservableCollection<string>();
        public ObservableCollection<string> AvailableStates
        {
            get => _availableStates;
            set => SetProperty(ref _availableStates, value);
        }

        // Comandos
        public ICommand ChangeStateCommand { get; }
        public ICommand ViewHistoryCommand { get; }
        public ICommand ViewCalendarCommand { get; }
        public ICommand LogoutCommand { get; }

        // Constructor
        public MainDashboardViewModel()
        {
            _stateService = App.StateService;

            // Inicializar los comandos
            ChangeStateCommand = new RelayCommand<string>(ExecuteChangeState);
            ViewHistoryCommand = new RelayCommand(ExecuteViewHistory);
            ViewCalendarCommand = new RelayCommand(ExecuteViewCalendar);
            LogoutCommand = new RelayCommand(ExecuteLogout);

            // Inicializar el temporizador para actualizar el tiempo transcurrido
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) => UpdateElapsedTime();
            _timer.Start();

            // Cargar los datos iniciales
            LoadInitialDataAsync();
        }

        // Método para cargar los datos iniciales
        private async void LoadInitialDataAsync()
        {
            try
            {
                // Cargar el estado actual y su tiempo de inicio
                var (state, startTime) = await _stateService.GetCurrentStateAsync();
                CurrentState = state;
                StateStartTime = startTime;
                UpdateElapsedTime();

                // Cargar los estados disponibles
                var states = await _stateService.GetAvailableStatesAsync();
                AvailableStates.Clear();
                foreach (var stateItem in states)
                {
                    AvailableStates.Add(stateItem);
                }
            }
            catch (Exception ex)
            {
                // Manejar errores de carga
                System.Diagnostics.Debug.WriteLine($"Error al cargar datos iniciales: {ex.Message}");
            }
        }

        // Método para actualizar el tiempo transcurrido
        private void UpdateElapsedTime()
        {
            ElapsedTime = DateTime.Now - StateStartTime;
        }

        // Implementaciones de comandos
        private async void ExecuteChangeState(string newState)
        {
            if (string.IsNullOrEmpty(newState))
            {
                return;
            }

            try
            {
                // Actualizar el estado en el servicio
                var success = await _stateService.UpdateCurrentStateAsync(newState);
                if (success)
                {
                    // Actualizar la interfaz de usuario
                    CurrentState = newState;
                    StateStartTime = DateTime.Now;
                    UpdateElapsedTime();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cambiar el estado: {ex.Message}");
            }
        }

        private void ExecuteViewHistory()
        {
            // Implementar la navegación a la vista de historial
            System.Diagnostics.Debug.WriteLine("Navegando a la vista de historial");
        }

        private void ExecuteViewCalendar()
        {
            // Implementar la navegación a la vista de calendario
            System.Diagnostics.Debug.WriteLine("Navegando a la vista de calendario");
        }

        private void ExecuteLogout()
        {
            // Implementar el cierre de sesión
            System.Diagnostics.Debug.WriteLine("Cerrando sesión");
        }
    }
}