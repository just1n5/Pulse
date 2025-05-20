using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PulseLogin.Models;
using PulseLogin.Repositories;
using System.Diagnostics;

namespace PulseLogin.Services
{
    /// <summary>
    /// Servicio que actúa como intermediario entre los ViewModels y el repositorio
    /// </summary>
    public class StateService
    {
        private readonly IStateRepository _repository;
        private string _currentUserId = "default_user"; // Por defecto si no hay login

        /// <summary>
        /// Constructor que inicializa el servicio con su repositorio correspondiente
        /// </summary>
        /// <param name="repository">Repositorio para la persistencia de estados</param>
        public StateService(IStateRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Debug.WriteLine("StateService inicializado correctamente");
        }

        /// <summary>
        /// Establece el ID del usuario actual (llamar después del login)
        /// </summary>
        /// <param name="userId">ID único del usuario</param>
        public void SetCurrentUser(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                _currentUserId = userId;
                Debug.WriteLine($"Usuario actual establecido: {userId}");
            }
            else
            {
                Debug.WriteLine("Se intentó establecer un userId nulo o vacío. Se mantiene el valor por defecto.");
            }
        }

        /// <summary>
        /// Obtiene el estado actual y su tiempo de inicio
        /// </summary>
        /// <returns>Tupla con el estado actual y su tiempo de inicio</returns>
        public async Task<(string State, DateTime StartTime)> GetCurrentStateAsync()
        {
            try
            {
                var userData = await _repository.GetUserStateAsync(_currentUserId);
                
                if (userData != null && !string.IsNullOrEmpty(userData.CurrentState))
                {
                    Debug.WriteLine($"Estado actual obtenido: {userData.CurrentState} (inicio: {userData.StateStartTime})");
                    return (userData.CurrentState, userData.StateStartTime);
                }
                
                // Valores por defecto si no hay datos
                Debug.WriteLine("No se encontraron datos de estado, devolviendo valores por defecto");
                return ("ESTADO ACTUAL", DateTime.Now);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al obtener el estado actual: {ex.Message}");
                return ("ESTADO ACTUAL", DateTime.Now);
            }
        }

        /// <summary>
        /// Actualiza el estado actual y registra el cambio en el historial
        /// </summary>
        /// <param name="newState">Nuevo estado a establecer</param>
        /// <returns>True si se actualizó correctamente</returns>
        public async Task<bool> UpdateCurrentStateAsync(string newState)
        {
            if (string.IsNullOrEmpty(newState))
            {
                Debug.WriteLine("Error: Intento de actualizar con un estado vacío");
                return false;
            }

            try
            {
                var now = DateTime.Now;
                bool result = await _repository.UpdateCurrentStateAsync(_currentUserId, newState, now);
                Debug.WriteLine($"Estado actualizado a '{newState}': {result}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al actualizar el estado: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene la lista de estados disponibles
        /// </summary>
        /// <returns>Lista de estados configurados</returns>
        public async Task<List<string>> GetAvailableStatesAsync()
        {
            try
            {
                var userData = await _repository.GetUserStateAsync(_currentUserId);
                
                if (userData?.Settings?.AvailableStates != null && 
                    userData.Settings.AvailableStates.Count > 0)
                {
                    Debug.WriteLine($"Se encontraron {userData.Settings.AvailableStates.Count} estados disponibles");
                    return userData.Settings.AvailableStates;
                }
                
                // Lista por defecto si no hay datos
                Debug.WriteLine("No se encontraron estados configurados, devolviendo lista por defecto");
                return new List<string>
                {
                    "ESTADO 1", "ESTADO 2", "ESTADO 3", "ESTADO 4",
                    "ESTADO 5", "ESTADO 6", "ESTADO 7", "ESTADO 8"
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al obtener estados disponibles: {ex.Message}");
                
                // Lista por defecto en caso de error
                return new List<string>
                {
                    "ESTADO 1", "ESTADO 2", "ESTADO 3", "ESTADO 4",
                    "ESTADO 5", "ESTADO 6", "ESTADO 7", "ESTADO 8"
                };
            }
        }

        /// <summary>
        /// Actualiza la lista de estados disponibles
        /// </summary>
        /// <param name="states">Nueva lista de estados</param>
        /// <returns>True si se actualizó correctamente</returns>
        public async Task<bool> UpdateAvailableStatesAsync(List<string> states)
        {
            try
            {
                var userData = await _repository.GetUserStateAsync(_currentUserId);
                
                if (userData == null)
                {
                    userData = new UserStateData
                    {
                        UserId = _currentUserId,
                        Settings = new UserSettings()
                    };
                }
                
                if (userData.Settings == null)
                {
                    userData.Settings = new UserSettings();
                }
                
                userData.Settings.AvailableStates = states;
                
                bool result = await _repository.SaveUserStateAsync(userData);
                Debug.WriteLine($"Estados disponibles actualizados: {result} (total: {states.Count})");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al actualizar estados disponibles: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene la lista de actividades
        /// </summary>
        /// <returns>Lista de actividades del usuario</returns>
        public async Task<List<ActivityData>> GetActivitiesAsync()
        {
            try
            {
                var activities = await _repository.GetUserActivitiesAsync(_currentUserId);
                Debug.WriteLine($"Se encontraron {activities.Count} actividades");
                return activities;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al obtener actividades: {ex.Message}");
                return new List<ActivityData>();
            }
        }

        /// <summary>
        /// Actualiza las actividades
        /// </summary>
        /// <param name="activities">Nueva lista de actividades</param>
        /// <returns>True si se actualizó correctamente</returns>
        public async Task<bool> UpdateActivitiesAsync(List<ActivityData> activities)
        {
            try
            {
                bool result = await _repository.UpdateUserActivitiesAsync(_currentUserId, activities);
                Debug.WriteLine($"Actividades actualizadas: {result} (total: {activities.Count})");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al actualizar actividades: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Agrega una nueva actividad a la lista existente
        /// </summary>
        /// <param name="activity">Nueva actividad a agregar</param>
        /// <returns>True si se añadió correctamente</returns>
        public async Task<bool> AddActivityAsync(ActivityData activity)
        {
            try
            {
                // Obtener las actividades actuales
                var activities = await GetActivitiesAsync();
                
                // Asegurarse de que la nueva actividad tenga un ID único
                if (string.IsNullOrEmpty(activity.Id))
                {
                    activity.Id = Guid.NewGuid().ToString();
                }
                
                // Añadir la nueva actividad
                activities.Add(activity);
                
                // Actualizar la lista
                bool result = await UpdateActivitiesAsync(activities);
                Debug.WriteLine($"Actividad añadida: {result} (ID: {activity.Id})");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al añadir actividad: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina una actividad por su ID
        /// </summary>
        /// <param name="activityId">ID de la actividad a eliminar</param>
        /// <returns>True si se eliminó correctamente</returns>
        public async Task<bool> RemoveActivityAsync(string activityId)
        {
            try
            {
                // Obtener las actividades actuales
                var activities = await GetActivitiesAsync();
                
                // Encontrar la actividad a eliminar
                int initialCount = activities.Count;
                activities.RemoveAll(a => a.Id == activityId);
                
                // Verificar si se eliminó alguna actividad
                if (activities.Count == initialCount)
                {
                    Debug.WriteLine($"No se encontró la actividad con ID: {activityId}");
                    return false;
                }
                
                // Actualizar la lista
                bool result = await UpdateActivitiesAsync(activities);
                Debug.WriteLine($"Actividad eliminada: {result} (ID: {activityId})");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al eliminar actividad: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene el historial completo de estados
        /// </summary>
        /// <returns>Lista de entradas del historial de estados</returns>
        public async Task<List<StateHistoryEntry>> GetStateHistoryAsync()
        {
            try
            {
                var userData = await _repository.GetUserStateAsync(_currentUserId);
                var history = userData?.StateHistory ?? new List<StateHistoryEntry>();
                
                Debug.WriteLine($"Se encontraron {history.Count} entradas en el historial de estados");
                return history;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al obtener historial de estados: {ex.Message}");
                return new List<StateHistoryEntry>();
            }
        }

        /// <summary>
        /// Obtiene las configuraciones del usuario
        /// </summary>
        /// <returns>Configuraciones del usuario o valores predeterminados</returns>
        public async Task<UserSettings> GetUserSettingsAsync()
        {
            try
            {
                var userData = await _repository.GetUserStateAsync(_currentUserId);
                
                if (userData?.Settings != null)
                {
                    Debug.WriteLine("Configuraciones de usuario obtenidas correctamente");
                    return userData.Settings;
                }
                
                // Devolver configuraciones por defecto
                Debug.WriteLine("No se encontraron configuraciones, devolviendo valores por defecto");
                return new UserSettings();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al obtener configuraciones: {ex.Message}");
                return new UserSettings();
            }
        }

        /// <summary>
        /// Actualiza las configuraciones del usuario
        /// </summary>
        /// <param name="settings">Nuevas configuraciones</param>
        /// <returns>True si se actualizó correctamente</returns>
        public async Task<bool> UpdateUserSettingsAsync(UserSettings settings)
        {
            try
            {
                bool result = await _repository.UpdateUserSettingsAsync(_currentUserId, settings);
                Debug.WriteLine($"Configuraciones actualizadas: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al actualizar configuraciones: {ex.Message}");
                return false;
            }
        }
    }
}