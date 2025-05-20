using System;
using System.Windows.Input;
using System.Diagnostics;

namespace PulseLogin.Helpers
{
    /// <summary>
    /// Implementación de ICommand que encapsula una acción y una condición de ejecución
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        /// <summary>
        /// Evento que se dispara cuando cambian las condiciones para la ejecución del comando
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// Constructor que inicializa el comando con una acción
        /// </summary>
        /// <param name="execute">Acción a ejecutar</param>
        public RelayCommand(Action execute) : this(execute, null)
        {
        }

        /// <summary>
        /// Constructor que inicializa el comando con una acción y una condición
        /// </summary>
        /// <param name="execute">Acción a ejecutar</param>
        /// <param name="canExecute">Función que determina si se puede ejecutar</param>
        public RelayCommand(Action execute, Func<bool>? canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Determina si el comando puede ejecutarse
        /// </summary>
        /// <param name="parameter">Parámetro para la ejecución (no utilizado)</param>
        /// <returns>True si el comando puede ejecutarse</returns>
        public bool CanExecute(object? parameter)
        {
            try
            {
                return _canExecute == null || _canExecute();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en CanExecute: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Ejecuta el comando
        /// </summary>
        /// <param name="parameter">Parámetro para la ejecución (no utilizado)</param>
        public void Execute(object? parameter)
        {
            try
            {
                _execute();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al ejecutar comando: {ex.Message}");
            }
        }

        /// <summary>
        /// Fuerza una reevaluación de las condiciones de ejecución
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }

    /// <summary>
    /// Implementación de ICommand que encapsula una acción con parámetro y una condición de ejecución
    /// </summary>
    /// <typeparam name="T">Tipo del parámetro</typeparam>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T?> _execute;
        private readonly Predicate<T?>? _canExecute;

        /// <summary>
        /// Evento que se dispara cuando cambian las condiciones para la ejecución del comando
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// Constructor que inicializa el comando con una acción que acepta un parámetro
        /// </summary>
        /// <param name="execute">Acción a ejecutar</param>
        public RelayCommand(Action<T?> execute) : this(execute, null)
        {
        }

        /// <summary>
        /// Constructor que inicializa el comando con una acción y una condición, ambas con parámetro
        /// </summary>
        /// <param name="execute">Acción a ejecutar</param>
        /// <param name="canExecute">Función que determina si se puede ejecutar</param>
        public RelayCommand(Action<T?> execute, Predicate<T?>? canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Determina si el comando puede ejecutarse
        /// </summary>
        /// <param name="parameter">Parámetro para la evaluación</param>
        /// <returns>True si el comando puede ejecutarse</returns>
        public bool CanExecute(object? parameter)
        {
            try
            {
                return _canExecute == null || _canExecute((T?)parameter);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en CanExecute<T>: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Ejecuta el comando
        /// </summary>
        /// <param name="parameter">Parámetro para la ejecución</param>
        public void Execute(object? parameter)
        {
            try
            {
                _execute((T?)parameter);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al ejecutar comando<T>: {ex.Message}");
            }
        }

        /// <summary>
        /// Fuerza una reevaluación de las condiciones de ejecución
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}