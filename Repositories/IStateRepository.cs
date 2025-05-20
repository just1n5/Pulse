using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PulseLogin.Models;

namespace PulseLogin.Repositories
{
    /// <summary>
    /// Interfaz para el repositorio que maneja la persistencia del estado del usuario
    /// </summary>
    public interface IStateRepository
    {
        /// <summary>
        /// Obtiene los datos del estado del usuario
        /// </summary>
        /// <param name="userId">Identificador del usuario</param>
        /// <returns>Datos del estado del usuario o null si no existe</returns>
        Task<UserStateData?> GetUserStateAsync(string userId);

        /// <summary>
        /// Guarda los datos de estado del usuario
        /// </summary>
        /// <param name="userData">Datos a guardar</param>
        /// <returns>True si se guardó correctamente</returns>
        Task<bool> SaveUserStateAsync(UserStateData userData);

        /// <summary>
        /// Actualiza solo el estado actual y su tiempo de inicio
        /// </summary>
        /// <param name="userId">Identificador del usuario</param>
        /// <param name="newState">Nuevo estado</param>
        /// <param name="startTime">Tiempo de inicio del nuevo estado</param>
        /// <returns>True si se actualizó correctamente</returns>
        Task<bool> UpdateCurrentStateAsync(string userId, string newState, DateTime startTime);

        /// <summary>
        /// Añade una entrada al historial de estados
        /// </summary>
        /// <param name="userId">Identificador del usuario</param>
        /// <param name="historyEntry">Entrada del historial a añadir</param>
        /// <returns>True si se añadió correctamente</returns>
        Task<bool> AddStateHistoryEntryAsync(string userId, StateHistoryEntry historyEntry);

        /// <summary>
        /// Actualiza la configuración del usuario
        /// </summary>
        /// <param name="userId">Identificador del usuario</param>
        /// <param name="settings">Nueva configuración</param>
        /// <returns>True si se actualizó correctamente</returns>
        Task<bool> UpdateUserSettingsAsync(string userId, UserSettings settings);

        /// <summary>
        /// Obtiene solo las actividades del usuario
        /// </summary>
        /// <param name="userId">Identificador del usuario</param>
        /// <returns>Lista de actividades o una lista vacía si no hay</returns>
        Task<List<ActivityData>> GetUserActivitiesAsync(string userId);

        /// <summary>
        /// Actualiza las actividades del usuario
        /// </summary>
        /// <param name="userId">Identificador del usuario</param>
        /// <param name="activities">Lista de actividades actualizada</param>
        /// <returns>True si se actualizó correctamente</returns>
        Task<bool> UpdateUserActivitiesAsync(string userId, List<ActivityData> activities);

        /// <summary>
        /// Elimina todos los datos del usuario
        /// </summary>
        /// <param name="userId">Identificador del usuario</param>
        /// <returns>True si se eliminaron correctamente</returns>
        Task<bool> DeleteUserDataAsync(string userId);

        /// <summary>
        /// Verifica si existen datos para un usuario específico
        /// </summary>
        /// <param name="userId">Identificador del usuario</param>
        /// <returns>True si existen datos del usuario</returns>
        Task<bool> UserExistsAsync(string userId);
    }
}