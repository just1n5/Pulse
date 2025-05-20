# Guía de Contribución para Pulse

¡Gracias por tu interés en contribuir a Pulse! Este documento proporciona las directrices y mejores prácticas para contribuir a este proyecto.

## Tabla de Contenido

1. [Código de Conducta](#código-de-conducta)
2. [Primeros Pasos](#primeros-pasos)
3. [Flujo de Trabajo](#flujo-de-trabajo)
4. [Estilo de Código](#estilo-de-código)
5. [Mensajes de Commit](#mensajes-de-commit)
6. [Pull Requests](#pull-requests)
7. [Reportar Bugs](#reportar-bugs)
8. [Proponer Nuevas Características](#proponer-nuevas-características)

## Código de Conducta

Este proyecto y todos sus contribuyentes están gobernados por nuestro [Código de Conducta](CODE_OF_CONDUCT.md). Al participar, se espera que respetes este código.

## Primeros Pasos

1. **Configuración del Entorno de Desarrollo**
   - Asegúrate de tener Visual Studio 2022 o posterior instalado
   - .NET 9.0 SDK instalado
   - Extensiones recomendadas para Visual Studio:
     - ReSharper o similar para análisis de código
     - Git Extensions o similar para gestionar Git desde VS

2. **Estructura del Proyecto**
   - **Models**: Clases de datos y entidades
   - **ViewModels**: Lógica de presentación siguiendo MVVM
   - **Views**: Interfaces de usuario XAML
   - **Services**: Servicios y lógica de negocio
   - **Repositories**: Acceso a datos y persistencia

## Flujo de Trabajo

Seguimos un flujo de trabajo basado en Git Flow:

1. **Ramas Principales**
   - `main`: Código en producción
   - `develop`: Rama principal de desarrollo

2. **Ramas de Soporte**
   - `feature/*`: Para nuevas características
   - `bugfix/*`: Para corrección de errores
   - `release/*`: Para preparar nuevas versiones
   - `hotfix/*`: Para correcciones urgentes en producción

3. **Proceso de Desarrollo**
   - Crea una rama desde `develop` usando la convención `feature/nombre-funcionalidad`
   - Desarrolla y prueba tu código
   - Realiza commits frecuentes con mensajes descriptivos
   - Cuando la funcionalidad esté completa, crea un Pull Request a `develop`

## Estilo de Código

Seguimos las convenciones de estilo de código de Microsoft para C#:

1. **Nomenclatura**
   - PascalCase para nombres de clases, propiedades y métodos
   - camelCase para variables locales y parámetros
   - _camelCase para campos privados
   - MAYÚSCULAS para constantes

2. **Formato**
   - Usa 4 espacios para indentación, no tabulaciones
   - Añade un espacio alrededor de operadores
   - Alinea las llaves en líneas separadas
   - Limita las líneas a 120 caracteres

3. **Comentarios**
   - Usa comentarios XML para documentación pública
   - Añade comentarios para explicar "por qué", no "qué"
   - Mantén comentarios actualizados con el código

4. **XAML**
   - Un atributo por línea para elementos complejos
   - Usa estilos y plantillas en lugar de propiedades repetitivas
   - Organiza los atributos de manera consistente

## Mensajes de Commit

Usa mensajes de commit descriptivos con el siguiente formato:

```
[Módulo] Breve descripción de los cambios (50 caracteres o menos)

Explicación más detallada de los cambios si es necesario. Mantén
cada línea a 72 caracteres o menos.

Referencia a issues o tickets relacionados:
Fixes #123
Related to #456
```

Ejemplos:
- `[Login] Corregir validación de contraseña`
- `[Dashboard] Añadir gráfico de tiempo por estado`
- `[Core] Mejorar rendimiento de carga de datos`

## Pull Requests

1. **Antes de Crear un PR**
   - Asegúrate de que tu código compila sin errores
   - Ejecuta todas las pruebas existentes
   - Añade pruebas para tu código nuevo
   - Revisa tu código para asegurarte de que sigue las guías de estilo

2. **Contenido del PR**
   - Título descriptivo
   - Descripción clara de los cambios
   - Referencias a issues relacionados
   - Capturas de pantalla o videos para cambios visuales

3. **Proceso de Revisión**
   - Todos los PRs requieren al menos una revisión
   - Responde a los comentarios de manera oportuna
   - Realiza los cambios solicitados en nuevos commits
   - Una vez aprobado, realiza un squash de tus commits si es necesario

## Reportar Bugs

Utiliza el sistema de issues de GitHub para reportar bugs. Incluye:

1. **Título**: Breve descripción del problema
2. **Descripción**: Detalle completo del problema
3. **Pasos para Reproducir**: Instrucciones paso a paso
4. **Comportamiento Esperado**: Lo que debería suceder
5. **Comportamiento Actual**: Lo que realmente sucede
6. **Capturas de Pantalla**: Si aplica
7. **Entorno**: Sistema operativo, versión del software, etc.

## Proponer Nuevas Características

Para proponer nuevas características:

1. Primero, verifica si ya existe una propuesta similar
2. Crea un issue con la etiqueta "enhancement"
3. Describe la funcionalidad en detalle
4. Explica por qué sería valiosa
5. Si es posible, incluye mockups o diagramas

---

Gracias por contribuir a Pulse. ¡Tu esfuerzo ayuda a mejorar el software para todos!
