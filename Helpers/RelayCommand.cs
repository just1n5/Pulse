using System;
using System.Windows.Input;

namespace PulseLogin.Helpers
{
    /// <summary>
    /// Implementación de ICommand que encapsula un delegado para la ejecución y, opcionalmente, un delegado para determinar si se puede ejecutar.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        /// <summary>
        /// Crea una nueva instancia de RelayCommand.
        /// </summary>
        /// <param name="execute">Acción a ejecutar cuando se invoca el comando.</param>
        /// <param name="canExecute">Función que determina si el comando puede ejecutarse.</param>
        /// <exception cref="ArgumentNullException">Si execute es null.</exception>
        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Evento que se dispara cuando cambian las condiciones que afectan si el comando puede ejecutarse.
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// Determina si este comando puede ejecutarse con los parámetros proporcionados.
        /// </summary>
        /// <param name="parameter">Parámetro de datos para el comando (no utilizado).</param>
        /// <returns>True si el comando puede ejecutarse; de lo contrario, false.</returns>
        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

        /// <summary>
        /// Ejecuta el comando con los parámetros proporcionados.
        /// </summary>
        /// <param name="parameter">Parámetro de datos para el comando (no utilizado).</param>
        public void Execute(object? parameter) => _execute();
    }

    /// <summary>
    /// Implementación genérica de ICommand que acepta un parámetro de tipo T.
    /// </summary>
    /// <typeparam name="T">Tipo del parámetro que acepta el comando.</typeparam>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T?> _execute;
        private readonly Predicate<T?>? _canExecute;

        /// <summary>
        /// Crea una nueva instancia de RelayCommand genérico.
        /// </summary>
        /// <param name="execute">Acción a ejecutar cuando se invoca el comando.</param>
        /// <param name="canExecute">Función que determina si el comando puede ejecutarse.</param>
        /// <exception cref="ArgumentNullException">Si execute es null.</exception>
        public RelayCommand(Action<T?> execute, Predicate<T?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Evento que se dispara cuando cambian las condiciones que afectan si el comando puede ejecutarse.
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// Determina si este comando puede ejecutarse con los parámetros proporcionados.
        /// </summary>
        /// <param name="parameter">Parámetro de datos para el comando. Se convierte al tipo T.</param>
        /// <returns>True si el comando puede ejecutarse; de lo contrario, false.</returns>
        public bool CanExecute(object? parameter)
        {
            if (_canExecute == null)
                return true;

            if (parameter == null)
                return _canExecute(default);

            if (parameter is T tParameter)
                return _canExecute(tParameter);

            // Intentar convertir el parámetro al tipo T
            try
            {
                var parameterT = (T?)Convert.ChangeType(parameter, typeof(T));
                return _canExecute(parameterT);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Ejecuta el comando con los parámetros proporcionados.
        /// </summary>
        /// <param name="parameter">Parámetro de datos para el comando. Se convierte al tipo T.</param>
        public void Execute(object? parameter)
        {
            if (parameter == null)
            {
                _execute(default);
                return;
            }

            if (parameter is T tParameter)
            {
                _execute(tParameter);
                return;
            }

            // Intentar convertir el parámetro al tipo T
            try
            {
                var parameterT = (T?)Convert.ChangeType(parameter, typeof(T));
                _execute(parameterT);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al convertir el parámetro: {ex.Message}");
            }
        }
    }
}