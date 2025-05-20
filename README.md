# Pulse - Gestión de Estados y Actividades

![Logo Pulse](Assets/Images/Logo_pulse_blanco.png)

## Descripción
Pulse es una aplicación de escritorio desarrollada en C# con WPF que permite a los usuarios gestionar sus estados de trabajo y actividades diarias. Diseñada con una interfaz moderna y minimalista, Pulse facilita el seguimiento del tiempo dedicado a diferentes tareas y la planificación de actividades.

## Características
- **Gestión de Estados**: Define y cambia fácilmente entre diferentes estados de trabajo o actividad.
- **Seguimiento de Tiempo**: Registro automático del tiempo dedicado a cada estado.
- **Historial de Estados**: Visualiza un historial completo de tus cambios de estado.
- **Planificación de Actividades**: Gestiona las actividades planificadas para el día.
- **Persistencia de Datos**: Todos los datos se almacenan localmente para mantener tu información segura.
- **Interfaz Intuitiva**: Diseño limpio y funcional que facilita su uso diario.

## Tecnologías
- **Lenguaje**: C# (.NET 7.0)
- **Framework UI**: Windows Presentation Foundation (WPF)
- **Arquitectura**: Modelo-Vista-ViewModel (MVVM)
- **Persistencia**: Almacenamiento en JSON local

## Estructura del Proyecto
- **/Models**: Clases de modelo de datos
- **/ViewModels**: ViewModels que implementan la lógica de presentación
- **/Views**: Vistas WPF
- **/Services**: Servicios de la aplicación
- **/Repositories**: Implementaciones de acceso a datos
- **/Helpers**: Clases auxiliares
- **/Assets**: Recursos gráficos y fuentes

## Requisitos del Sistema
- Windows 10/11
- .NET 7.0 Runtime o superior

## Instalación
1. Descarga la última versión desde la sección de releases
2. Ejecuta el instalador y sigue las instrucciones
3. Inicia la aplicación desde el menú de inicio o el acceso directo creado

## Desarrollo
Para contribuir al desarrollo:

1. Clona el repositorio
```
git clone https://github.com/just1n5/Pulse.git
```

2. Abre la solución en Visual Studio 2022 o posterior
```
cd Pulse
start PulseLogin.sln
```

3. Compila y ejecuta la aplicación
```
dotnet build
dotnet run
```

## Licencia
Este proyecto está bajo la licencia MIT. Ver el archivo LICENSE para más detalles.

## Contacto
- Desarrollador: just1n5
- GitHub: [https://github.com/just1n5](https://github.com/just1n5)
