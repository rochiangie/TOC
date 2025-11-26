# 🗑️ TOC - Trash Organization Challenge

## 📝 Descripción
Juego educativo de clasificación de basura donde el jugador debe recoger y clasificar correctamente diferentes tipos de residuos en sus basureros correspondientes.

## 🎮 Controles
- **WASD**: Movimiento
- **Mouse**: Mirar alrededor
- **E / Click Izquierdo**: Interactuar / Recoger / Soltar
- **Shift Izquierdo**: Correr
- **Espacio**: Saltar

## 🎯 Objetivo
Clasifica toda la basura en el tiempo límite. Cada clasificación correcta suma puntos. ¡Haz combos para multiplicar tu puntuación!

## 🎨 Tipos de Basura
- 🟡 **Amarillo**: Plástico y Envases
- 🔵 **Azul**: Papel y Cartón
- 🟢 **Verde**: Vidrio
- 🔴 **Rojo**: Residuos Peligrosos

## 🏗️ Arquitectura del Proyecto

### Estructura de Carpetas
```
Assets/
├── Scripts/
│   ├── Player/              # Movimiento, interacción, cámara, animación
│   ├── Systems/
│   │   ├── Managers/        # GameManager, TrashManager, UIManager, AudioManager
│   │   └── GameEvents.cs    # Sistema de eventos desacoplado
│   ├── Environment/         # TrashCan, TrashItem, Dumpster
│   ├── Interaction/         # PickupableObject, TrashObject
│   └── Tools/               # Utilidades
├── Scenes/
│   ├── Menu.unity
│   ├── PrimerNivel.unity
│   └── Creditos.unity
└── Prefabs/
```

### Patrones de Diseño Implementados
- ✅ **Singleton Pattern**: GameManager, AudioManager
- ✅ **Event System**: GameEvents para desacoplamiento
- ✅ **Properties**: Encapsulación con acceso controlado
- ✅ **Coroutines**: Para delays y animaciones

## 🔧 Características Técnicas

### Sistema de Eventos
El proyecto usa un sistema de eventos centralizado (`GameEvents.cs`) que permite la comunicación entre sistemas sin dependencias directas:

```csharp
// Disparar evento
GameEvents.TrashPickedUp();

// Suscribirse a evento
void OnEnable()
{
    GameEvents.OnTrashPickedUp += HandlePickup;
}
```

### Managers Principales
- **GameManager**: Control del estado del juego y tiempo
- **TrashManager**: Gestión de basura recolectada y capacidad de bolsa
- **UIManager**: Actualización de interfaz de usuario
- **AudioManager**: Reproducción de música y efectos de sonido

## 📊 Estado del Proyecto

### ✅ Implementado
- Sistema de movimiento con Rigidbody
- Sistema de interacción con objetos
- Clasificación de basura por colores
- Sistema de eventos desacoplado
- AudioManager con persistencia
- Feedback visual y auditivo básico
- Animaciones de absorción de basura

### 🚧 En Desarrollo
- Sistema de puntuación y combos
- Tutorial interactivo
- Mejoras de UI/UX
- Sistema de guardado de progreso
- Optimizaciones de rendimiento

### 📋 Próximas Mejoras
Ver `ANALISIS_Y_MEJORAS_RECOMENDADAS.md` para detalles completos

## 🐛 Problemas Conocidos

1. **PlayerAnimation.cs**: Referencia a CharacterController en lugar de Rigidbody
2. **TrashCan.cs**: Uso de OnGUI() (obsoleto, migrar a Canvas)
3. **Optimización**: Algunos FindObjectOfType en runtime

Ver `ANALISIS_Y_MEJORAS_RECOMENDADAS.md` para soluciones detalladas.

## 📚 Documentación Adicional

- **PATRONES_Y_MEJORES_PRACTICAS.md**: Guía de patrones aplicados
- **RESUMEN_MEJORAS_IMPLEMENTADAS.md**: Historial de mejoras
- **ANALISIS_Y_MEJORAS_RECOMENDADAS.md**: Análisis completo y roadmap

## 🛠️ Requisitos

- Unity 2022.3 o superior
- TextMeshPro (incluido en Unity)
- Input System (nuevo o legacy)

## 🚀 Cómo Ejecutar

1. Abrir el proyecto en Unity
2. Abrir la escena `Menu.unity`
3. Presionar Play
4. Seleccionar nivel y ¡jugar!

## 🎓 Proyecto Educativo

Desarrollado como parte del programa **TalentoTech 3D**, aplicando:
- Programación orientada a objetos
- Patrones de diseño
- Arquitectura de software
- Buenas prácticas de Unity

## 👥 Créditos

**Desarrollador**: [Tu Nombre]  
**Programa**: TalentoTech 3D  
**Año**: 2025

## 📄 Licencia

Proyecto educativo - Uso libre para aprendizaje

---

*Para más información sobre mejoras y optimizaciones, consulta `ANALISIS_Y_MEJORAS_RECOMENDADAS.md`*
