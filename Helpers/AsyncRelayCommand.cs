using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PulseLogin.Helpers
{
    /// <summary>
    /// Implementación asíncrona de ICommand que permite ejecutar métodos asíncronos.
    /// </summary>
    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool>? _canExecute;
        private bool _isExecuting;

        /// <summary>
        /// Crea una nueva instancia de AsyncRelayCommand.
        /// </summary>
        /// <param name="execute">Función asíncrona a ejecutar cuando se invoca el comando.</param>
        /// <param name="canExecute">Función que determina si el comando puede ejecutarse.</param>
        /// <exception cref="ArgumentNullException">Si execute es null.</exception>
        public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
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
        /// Determina si este comando puede ejecutarse.
        /// </summary>
        /// <param name="parameter">Parámetro de datos para el comando (no utilizado).</param>
        /// <returns>True si el comando puede ejecutarse; de lo contrario, false.</returns>
        public bool CanExecute(object? parameter)
        {
            return !_isExecuting && (_canExecute?.Invoke() ?? true);
        }

        /// <summary>
        /// Ejecuta el comando asíncrono.
        /// </summary>
        /// <param name="parameter">Parámetro de datos para el comando (no utilizado).</param>
        public async void Execute(object? parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    _isExecuting = true;
                    RaiseCanExecuteChanged();
                    
                    await _execute();
                }
                finally
                {
                    _isExecuting = false;
                    RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Notifica a los oyentes que el valor de CanExecute ha cambiado.
        /// </summary>
        protected void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }

    /// <summary>
    /// Implementación asíncrona genérica de ICommand que acepta un parámetro de tipo T.
    /// </summary>
    /// <typeparam name="T">Tipo del parámetro que acepta el comando.</typeparam>
    public class AsyncRelayCommand<T> : ICommand
    {
        private readonly Func<T?, Task> _execute;
        private readonly Predicate<T?>? _canExecute;
        private bool _isExecuting;

        /// <summary>
        /// Crea una nueva instancia de AsyncRelayCommand genérico.
        /// </summary>
        /// <param name="execute">Función asíncrona a ejecutar cuando se invoca el comando.</param>
        /// <param name="canExecute">Función que determina si el comando puede ejecutarse.</param>
        /// <exception cref="ArgumentNullException">Si execute es null.</exception>
        public AsyncRelayCommand(Func<T?, Task> execute, Predicate<T?>? canExecute = null)
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
            if (_isExecuting)
                return false;

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
        /// Ejecuta el comando asíncrono con los parámetros proporcionados.
        /// </summary>
        /// <param name="parameter">Parámetro de datos para el comando. Se convierte al tipo T.</param>
        public async void Execute(object? parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    _isExecuting = true;
                    RaiseCanExecuteChanged();

                    T? parameterT = default;

                    if (parameter != null)
                    {
                        if (parameter is T tParameter)
                        {
                            parameterT = tParameter;
                        }
                        else
                        {
                            // Intentar convertir el parámetro al tipo T
                            try
                            {
                                parameterT = (T?)Convert.ChangeType(parameter, typeof(T));
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error al convertir el parámetro: {ex.Message}");
                            }
                        }
                    }

                    await _execute(parameterT);
                }
                finally
                {
                    _isExecuting = false;
                    RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Notifica a los oyentes que el valor de CanExecute ha cambiado.
        /// </summary>
        protected void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}