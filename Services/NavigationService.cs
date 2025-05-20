using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;

namespace PulseLogin.Services
{
    /// <summary>
    /// Implementación del servicio de navegación que manipula el contenido de un ContentControl
    /// </summary>
    public class NavigationService : INavigationService
    {
        // Pila para mantener el historial de navegación
        private readonly Stack<UserControl> _navigationStack = new Stack<UserControl>();
        
        // Referencia al contenedor principal de la aplicación
        private ContentControl _contentContainer;

        /// <summary>
        /// Constructor que inicializa el servicio de navegación
        /// </summary>
        public NavigationService()
        {
            // El contenedor se establecerá posteriormente usando SetContentContainer
        }

        /// <summary>
        /// Establece el contenedor principal donde se mostrarán las páginas
        /// </summary>
        /// <param name="contentContainer">Control de contenido principal</param>
        public void SetContentContainer(ContentControl contentContainer)
        {
            _contentContainer = contentContainer;
            Debug.WriteLine($"Contenedor de navegación establecido: {contentContainer != null}");
        }

        /// <summary>
        /// Navega a una nueva página
        /// </summary>
        /// <param name="page">Control de usuario a mostrar</param>
        public void Navigate(UserControl page)
        {
            try
            {
                if (_contentContainer == null)
                {
                    // Si no hay contenedor configurado, intentar encontrar uno automáticamente
                    var mainWindow = Application.Current.MainWindow;
                    if (mainWindow != null)
                    {
                        var foundContainer = FindContentContainer(mainWindow);
                        if (foundContainer != null)
                        {
                            _contentContainer = foundContainer;
                            Debug.WriteLine("Contenedor encontrado automáticamente");
                        }
                        else
                        {
                            Debug.WriteLine("No se pudo encontrar un contenedor automáticamente");
                            throw new InvalidOperationException("No se ha establecido un contenedor para la navegación");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("No hay ventana principal disponible");
                        throw new InvalidOperationException("No hay ventana principal disponible");
                    }
                }

                // Guardar la página actual en la pila de navegación
                if (_contentContainer.Content is UserControl currentPage)
                {
                    _navigationStack.Push(currentPage);
                    Debug.WriteLine($"Página actual añadida a la pila: {currentPage.GetType().Name}");
                }

                // Navegar a la nueva página
                _contentContainer.Content = page;
                Debug.WriteLine($"Navegación exitosa a: {page.GetType().Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en navegación: {ex.Message}");
                MessageBox.Show($"Error al navegar: {ex.Message}", "Error de Navegación", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Navega a la página anterior
        /// </summary>
        public void GoBack()
        {
            try
            {
                if (_navigationStack.Count > 0)
                {
                    // Obtener la página anterior de la pila
                    var previousPage = _navigationStack.Pop();
                    
                    // Establecerla como el contenido actual
                    _contentContainer.Content = previousPage;
                    Debug.WriteLine($"Navegación hacia atrás exitosa a: {previousPage.GetType().Name}");
                }
                else
                {
                    Debug.WriteLine("No hay páginas en la pila para navegar hacia atrás");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al navegar hacia atrás: {ex.Message}");
                MessageBox.Show($"Error al navegar hacia atrás: {ex.Message}", 
                    "Error de Navegación", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Intenta encontrar un ContentControl adecuado para la navegación
        /// </summary>
        private ContentControl FindContentContainer(DependencyObject parent)
        {
            // Si el padre es un ContentControl, lo devolvemos
            if (parent is ContentControl contentControl && 
                contentControl.Name != null && 
                (contentControl.Name.Contains("Content") || contentControl.Name.Contains("MainFrame")))
            {
                return contentControl;
            }

            // Número de hijos
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            
            // Búsqueda en profundidad
            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                
                // Búsqueda recursiva
                ContentControl result = FindContentContainer(child);
                if (result != null)
                {
                    return result;
                }
            }

            // No se encontró ningún contenedor adecuado
            return null;
        }
    }
}