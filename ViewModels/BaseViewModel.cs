using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace PulseLogin.ViewModels
{
    /// <summary>
    /// Clase base para todos los ViewModels que implementa INotifyPropertyChanged
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Evento que se dispara cuando una propiedad cambia su valor
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Método para notificar que una propiedad ha cambiado
        /// </summary>
        /// <param name="propertyName">Nombre de la propiedad que cambió</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                Debug.WriteLine($"Propiedad notificada: {propertyName}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al notificar propiedad {propertyName}: {ex.Message}");
            }
        }

        /// <summary>
        /// Método para establecer el valor de una propiedad y notificar si ha cambiado
        /// </summary>
        /// <typeparam name="T">Tipo de la propiedad</typeparam>
        /// <param name="storage">Referencia a la variable de almacenamiento</param>
        /// <param name="value">Nuevo valor</param>
        /// <param name="propertyName">Nombre de la propiedad</param>
        /// <returns>True si el valor ha cambiado</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            // Si son iguales, no hacemos nada
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }
            
            try
            {
                // Actualizar el valor
                storage = value;
                
                // Notificar el cambio
                OnPropertyChanged(propertyName);
                
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en SetProperty para {propertyName}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Método para establecer el valor de una propiedad y notificar si ha cambiado, además de ejecutar una acción
        /// </summary>
        /// <typeparam name="T">Tipo de la propiedad</typeparam>
        /// <param name="storage">Referencia a la variable de almacenamiento</param>
        /// <param name="value">Nuevo valor</param>
        /// <param name="action">Acción a ejecutar después del cambio</param>
        /// <param name="propertyName">Nombre de la propiedad</param>
        /// <returns>True si el valor ha cambiado</returns>
        protected bool SetProperty<T>(ref T storage, T value, Action action, [CallerMemberName] string? propertyName = null)
        {
            // Actualizar la propiedad
            if (SetProperty(ref storage, value, propertyName))
            {
                try
                {
                    // Ejecutar la acción adicional
                    action();
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error en acción después de SetProperty para {propertyName}: {ex.Message}");
                    return false;
                }
            }
            
            return false;
        }
    }
}