using System;
using System.Collections.Generic;

namespace PulseLogin.Models
{
    /// <summary>
    /// Modelo que representa el estado del usuario que será persistido
    /// </summary>
    public class UserStateData
    {
        /// <summary>
        /// Identificador único del usuario
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// Estado actual seleccionado
        /// </summary>
        public string? CurrentState { get; set; }

        /// <summary>
        /// Marca de tiempo cuando se inició el estado actual
        /// </summary>
        public DateTime StateStartTime { get; set; }

        /// <summary>
        /// Historial de estados con sus tiempos de inicio y fin
        /// </summary>
        public List<StateHistoryEntry>? StateHistory { get; set; }

        /// <summary>
        /// Actividades planificadas del usuario
        /// </summary>
        public List<ActivityData>? Activities { get; set; }

        /// <summary>
        /// Última vez que se actualizaron los datos
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Configuración del usuario
        /// </summary>
        public UserSettings? Settings { get; set; }

        /// <summary>
        /// Crea una nueva instancia de UserStateData con valores predeterminados
        /// </summary>
        public UserStateData()
        {
            StateHistory = new List<StateHistoryEntry>();
            Activities = new List<ActivityData>();
            LastUpdated = DateTime.Now;
            Settings = new UserSettings();
        }
    }

    /// <summary>
    /// Representa una entrada en el historial de estados
    /// </summary>
    public class StateHistoryEntry
    {
        /// <summary>
        /// Estado seleccionado
        /// </summary>
        public string? State { get; set; }

        /// <summary>
        /// Tiempo de inicio del estado
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Tiempo de finalización del estado (null si está activo)
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Duración total en el estado
        /// </summary>
        public TimeSpan Duration => EndTime.HasValue 
            ? EndTime.Value - StartTime 
            : DateTime.Now - StartTime;

        /// <summary>
        /// Comentarios o notas asociadas a este estado
        /// </summary>
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Representa una actividad planificada
    /// </summary>
    public class ActivityData
    {
        /// <summary>
        /// Identificador único de la actividad
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Hora programada para la actividad
        /// </summary>
        public string? Time { get; set; }

        /// <summary>
        /// Descripción de la actividad
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Indica si la actividad ha sido completada
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Fecha asociada a la actividad (puede ser nula para actividades recurrentes)
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Prioridad de la actividad (1-3, donde 1 es alta)
        /// </summary>
        public int Priority { get; set; } = 2;

        /// <summary>
        /// Categoría de la actividad
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Crea una nueva instancia con un ID único
        /// </summary>
        public ActivityData()
        {
            Id = Guid.NewGuid().ToString();
            Date = DateTime.Today;
        }
    }

    /// <summary>
    /// Configuración del usuario
    /// </summary>
    public class UserSettings
    {
        /// <summary>
        /// Lista de estados disponibles para el usuario
        /// </summary>
        public List<string>? AvailableStates { get; set; }

        /// <summary>
        /// Indica si se debe mostrar notificaciones para actividades programadas
        /// </summary>
        public bool EnableNotifications { get; set; }

        /// <summary>
        /// Minutos antes de una actividad para mostrar notificación
        /// </summary>
        public int NotificationMinutesBefore { get; set; }

        /// <summary>
        /// Tema de la aplicación (claro, oscuro, sistema)
        /// </summary>
        public string? Theme { get; set; }

        /// <summary>
        /// Muestra u oculta las actividades completadas
        /// </summary>
        public bool ShowCompletedActivities { get; set; }

        /// <summary>
        /// Nombre de usuario
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Correo electrónico del usuario
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Color preferido para el estado actual
        /// </summary>
        public string? StateColor { get; set; }

        /// <summary>
        /// Crea una nueva instancia con valores predeterminados
        /// </summary>
        public UserSettings()
        {
            AvailableStates = new List<string>
            {
                "ESTADO 1", "ESTADO 2", "ESTADO 3", "ESTADO 4",
                "ESTADO 5", "ESTADO 6", "ESTADO 7", "ESTADO 8"
            };
            EnableNotifications = true;
            NotificationMinutesBefore = 5;
            Theme = "Default";
            ShowCompletedActivities = true;
            StateColor = "#4A76A8"; // Azul Pulse por defecto
        }
    }
}