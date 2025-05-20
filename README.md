# Pulse

Aplicación de gestión de tiempo y actividades desarrollada en C# WPF.

## Descripción

Pulse es una aplicación de escritorio diseñada para ayudar a los usuarios a gestionar su tiempo y actividades. Ofrece un sistema de estados que permite a los usuarios registrar en qué están trabajando, cuánto tiempo llevan en cada tarea y mantener un historial de sus actividades.

## Características

- **Gestión de estados**: Registra diferentes estados de actividad (trabajo, descanso, reunión, etc.)
- **Seguimiento de tiempo**: Controla el tiempo invertido en cada estado o actividad
- **Planificación de actividades**: Programa actividades y recibe recordatorios
- **Historial de estados**: Consulta el historial de estados y actividades
- **Interfaz moderna**: Diseño limpio y fácil de usar con WPF

## Tecnologías utilizadas

- C# .NET 9.0
- Windows Presentation Foundation (WPF)
- Patrón MVVM
- Serialización JSON para almacenamiento de datos

## Requisitos del sistema

- Windows 10/11
- .NET 9.0 Runtime

## Instalación

1. Descarga la última versión desde la página de releases
2. Ejecuta el instalador y sigue las instrucciones
3. Inicia la aplicación desde el menú de inicio o el acceso directo creado

## Desarrollo

### Requisitos previos

- Visual Studio 2022 o superior
- .NET 9.0 SDK

### Configuración del entorno de desarrollo

1. Clona el repositorio
```
git clone https://github.com/just1n5/Pulse.git
```

2. Abre la solución en Visual Studio
```
PulseLogin.sln
```

3. Restaura los paquetes NuGet si es necesario

4. Compila y ejecuta la aplicación

### Estructura del proyecto

- **Models**: Definición de datos
- **ViewModels**: Lógica de presentación
- **Views**: Interfaz de usuario
- **Services**: Servicios y lógica de negocio
- **Repositories**: Acceso a datos y persistencia

## Contribución

Si deseas contribuir al proyecto, consulta nuestra guía de contribución en el archivo CONTRIBUTING.md.

## Licencia

Este proyecto está licenciado bajo los términos de la licencia MIT. Consulta el archivo LICENSE para más detalles.

## Contacto

Para cualquier consulta o sugerencia, no dudes en crear un issue en este repositorio.