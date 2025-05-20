using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using PulseLogin.Models;
using System.Diagnostics;
using System.Linq;

namespace PulseLogin.Repositories
{
    /// <summary>
    /// Implementación del repositorio que guarda los datos en archivos JSON
    /// </summary>
    public class JsonStateRepository : IStateRepository
    {
        private readonly string _baseDirectory;
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// Constructor que inicializa el repositorio
        /// </summary>
        public JsonStateRepository()
        {
            // Directorio base para los archivos de datos
            _baseDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PulseApp",
                "UserData");

            // Asegurarse de que el directorio existe
            if (!Directory.Exists(_baseDirectory))
            {
                Directory.CreateDirectory(_baseDirectory);
                Debug.WriteLine($"Directorio de datos creado: {_baseDirectory}");
            }

            // Configurar opciones de serialización JSON
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,              // Formato legible
                PropertyNameCaseInsensitive = true // Insensible a mayúsculas/minúsculas
            };
            
            Debug.WriteLine("JsonStateRepository inicializado correctamente");
        }

        /// <summary>
        /// Constructor con directorio personalizado (útil para pruebas)
        /// </summary>
        /// <param name="baseDirectory">Directorio base para almacenar los archivos</param>
        public JsonStateRepository(string baseDirectory)
        {
            // Validar directorio
            if (string.IsNullOrEmpty(baseDirectory))
            {
                throw new ArgumentNullException(nameof(baseDirectory));
            }
            
            _baseDirectory = baseDirectory;
            
            // Asegurarse de que el directorio existe
            if (!Directory.Exists(_baseDirectory))
            {
                Directory.CreateDirectory(_baseDirectory);
                Debug.WriteLine($"Directorio de datos personalizado creado: {_baseDirectory}");
            }
            
            // Configurar opciones de serialización
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
            
            Debug.WriteLine("JsonStateRepository inicializado con directorio personalizado");
        }

        /// <summary>
        /// Obtiene la ruta de archivo para un usuario específico
        /// </summary>
        private string GetUserFilePath(string userId)
        {
            // Sanitizar el ID de usuario para usarlo como nombre de archivo
            string safeUserId = string.Join("_", userId.Split(Path.GetInvalidFileNameChars()));
            return Path.Combine(_baseDirectory, $"{safeUserId}.json");
        }

        /// <inheritdoc />
        public async Task<UserStateData?> GetUserStateAsync(string userId)
        {
            try
            {
                string filePath = GetUserFilePath(userId);
                
                // Verificar si el archivo existe
                if (!File.Exists(filePath))
                {
                    Debug.WriteLine($"Archivo de datos no encontrado para el usuario {userId}");
                    return null;
                }

                // Leer el contenido del archivo
                string jsonContent = await File.ReadAllTextAsync(filePath);
                
                // Deserializar el contenido
                var userData = JsonSerializer.Deserialize<UserStateData>(jsonContent, _jsonOptions);
                
                Debug.WriteLine($"Datos cargados correctamente para el usuario {userId}");
                return userData;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al obtener datos del usuario {userId}: {ex.Message}");
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<bool> SaveUserStateAsync(UserStateData userData)
        {
            if (userData == null || string.IsNullOrEmpty(userData.UserId))
            {
                Debug.WriteLine("Error: Datos de usuario nulos o ID de usuario vacío");
                return false;
            }

            try
            {
                // Actualizar la marca de tiempo
                userData.LastUpdated = DateTime.Now;
                
                // Serializar los datos
                string jsonContent = JsonSerializer.Serialize(userData, _jsonOptions);
                
                // Guardar en el archivo
                string filePath = GetUserFilePath(userData.UserId);
                await File.WriteAllTextAsync(filePath, jsonContent);
                
                Debug.WriteLine($"Datos guardados correctamente para el usuario {userData.UserId}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al guardar datos del usuario {userData.UserId}: {ex.Message}");
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> UpdateCurrentStateAsync(string userId, string newState, DateTime startTime)
        {
            try
            {
                // Obtener los datos actuales
                var userData = await GetUserStateAsync(userId);
                
                // Si no existen, crear nuevos datos
                if (userData == null)
                {
                    userData = new UserStateData
                    {
                        UserId = userId,
                        StateHistory = new List<StateHistoryEntry>()
                    };
                    Debug.WriteLine($"Creando nuevos datos para el usuario {userId}");
                }

                // Si hay un estado actual, finalizar su entrada en el historial
                if (!string.IsNullOrEmpty(userData.CurrentState))
                {
                    var lastEntry = userData.StateHistory?.FirstOrDefault(e => 
                        e.State == userData.CurrentState && !e.EndTime.HasValue);
                    
                    if (lastEntry != null)
                    {
                        lastEntry.EndTime = startTime;
                        Debug.WriteLine($"Estado anterior finalizado: {userData.CurrentState}");
                    }
                }

                // Actualizar el estado actual
                userData.CurrentState = newState;
                userData.StateStartTime = startTime;
                
                // Añadir nueva entrada al historial
                userData.StateHistory?.Add(new StateHistoryEntry
                {
                    State = newState,
                    StartTime = startTime,
                    EndTime = null
                });
                
                Debug.WriteLine($"Nuevo estado registrado: {newState}");

                // Guardar los cambios
                return await SaveUserStateAsync(userData);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al actualizar el estado actual del usuario {userId}: {ex.Message}");
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> AddStateHistoryEntryAsync(string userId, StateHistoryEntry historyEntry)
        {
            try
            {
                // Obtener los datos actuales
                var userData = await GetUserStateAsync(userId);
                
                // Si no existen, crear nuevos datos
                if (userData == null)
                {
                    userData = new UserStateData
                    {
                        UserId = userId,
                        StateHistory = new List<StateHistoryEntry>()
                    };
                    Debug.WriteLine($"Creando nuevos datos para el usuario {userId}");
                }

                // Añadir la entrada al historial
                userData.StateHistory?.Add(historyEntry);
                Debug.WriteLine($"Entrada añadida al historial: {historyEntry.State}");
                
                // Guardar los cambios
                return await SaveUserStateAsync(userData);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al añadir entrada al historial del usuario {userId}: {ex.Message}");
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> UpdateUserSettingsAsync(string userId, UserSettings settings)
        {
            try
            {
                // Obtener los datos actuales
                var userData = await GetUserStateAsync(userId);
                
                // Si no existen, crear nuevos datos
                if (userData == null)
                {
                    userData = new UserStateData
                    {
                        UserId = userId,
                        Settings = settings
                    };
                    Debug.WriteLine($"Creando nuevos datos para el usuario {userId}");
                }
                else
                {
                    // Actualizar la configuración
                    userData.Settings = settings;
                }
                
                Debug.WriteLine($"Configuración actualizada para el usuario {userId}");
                
                // Guardar los cambios
                return await SaveUserStateAsync(userData);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al actualizar la configuración del usuario {userId}: {ex.Message}");
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<List<ActivityData>> GetUserActivitiesAsync(string userId)
        {
            try
            {
                // Obtener los datos completos
                var userData = await GetUserStateAsync(userId);
                
                // Devolver las actividades o una lista vacía si no hay datos
                var activities = userData?.Activities ?? new List<ActivityData>();
                Debug.WriteLine($"Se recuperaron {activities.Count} actividades para el usuario {userId}");
                return activities;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al obtener actividades del usuario {userId}: {ex.Message}");
                return new List<ActivityData>();
            }
        }

        /// <inheritdoc />
        public async Task<bool> UpdateUserActivitiesAsync(string userId, List<ActivityData> activities)
        {
            try
            {
                // Obtener los datos actuales
                var userData = await GetUserStateAsync(userId);
                
                // Si no existen, crear nuevos datos
                if (userData == null)
                {
                    userData = new UserStateData
                    {
                        UserId = userId,
                        Activities = activities
                    };
                    Debug.WriteLine($"Creando nuevos datos para el usuario {userId}");
                }
                else
                {
                    // Actualizar las actividades
                    userData.Activities = activities;
                }
                
                Debug.WriteLine($"Actividades actualizadas para el usuario {userId}: {activities.Count} actividades");
                
                // Guardar los cambios
                return await SaveUserStateAsync(userData);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al actualizar actividades del usuario {userId}: {ex.Message}");
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> DeleteUserDataAsync(string userId)
        {
            try
            {
                string filePath = GetUserFilePath(userId);
                
                // Verificar si el archivo existe
                if (!File.Exists(filePath))
                {
                    Debug.WriteLine($"No se encontraron datos para eliminar del usuario {userId}");
                    return true; // Considerar éxito si no hay archivos para eliminar
                }
                
                // Eliminar el archivo
                File.Delete(filePath);
                Debug.WriteLine($"Datos del usuario {userId} eliminados correctamente");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al eliminar datos del usuario {userId}: {ex.Message}");
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> UserExistsAsync(string userId)
        {
            try
            {
                string filePath = GetUserFilePath(userId);
                bool exists = File.Exists(filePath);
                Debug.WriteLine($"Verificación de existencia del usuario {userId}: {exists}");
                return exists;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al verificar existencia del usuario {userId}: {ex.Message}");
                return false;
            }
        }
    }
}