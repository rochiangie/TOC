# 🎯 Guía: Visualizador de Raycast

## ✅ Script Creado: RaycastVisualizer.cs

Este script muestra **visualmente** dónde está golpeando tu raycast, tanto en el editor como en el juego.

---

## 🔧 Configuración en Unity

### Paso 1: Agregar el Script

1. **Selecciona** la Main Camera (o crea un GameObject vacío)
2. **Add Component** → RaycastVisualizer

### Paso 2: Configurar en el Inspector

```
RaycastVisualizer (Script)

┌─ Configuración Visual ─────────────┐
│ Miss Color: Rojo                   │ ← Color cuando NO golpea nada
│ Hit Color: Verde                   │ ← Color cuando SÍ golpea algo
│ Sphere Size: 0.1                   │ ← Tamaño de la esfera
└────────────────────────────────────┘

┌─ Configuración de Raycast ─────────┐
│ Raycast Distance: 3                │ ← Debe coincidir con PlayerInteraction
│ Interactable Layers: Everything    │
└────────────────────────────────────┘

┌─ Opciones de Visualización ────────┐
│ Show Ray Line: ✅                  │ ← Mostrar línea del raycast
│ Show Hit Sphere: ✅                │ ← Mostrar esfera en el punto
│ Show Object Info: ✅               │ ← Mostrar info en pantalla
└────────────────────────────────────┘
```

---

## 👁️ Qué Verás

### En el Editor (Scene View):

Cuando ejecutes el juego y tengas la ventana Scene abierta:

- 🔴 **Línea roja** desde la cámara → No golpea nada
- 🟢 **Línea verde** desde la cámara → Golpea algo
- 🔴 **Esfera roja** al final del raycast → No hay objeto
- 🟢 **Esfera verde** en el punto de impacto → Hay objeto

### En el Juego (Game View):

En la esquina superior izquierda verás:

```
Raycast Debug:
Estado: GOLPEANDO ✓
Distancia: 2.45m
Objeto: Botella_Plastico
Posición: (1.2, 0.5, 3.4)
```

O si no golpea nada:

```
Raycast Debug:
Estado: SIN IMPACTO ✗
Distancia: 3.00m
```

---

## 🎨 Personalización

### Cambiar Colores

```csharp
// Desde el Inspector:
Miss Color: Amarillo (cuando no golpea)
Hit Color: Cyan (cuando golpea)
```

### Cambiar Tamaño de la Esfera

```csharp
// Desde el Inspector:
Sphere Size: 0.05 (pequeña)
Sphere Size: 0.2 (grande)
```

### Ocultar Elementos

```csharp
// Solo mostrar la línea:
Show Ray Line: ✅
Show Hit Sphere: ❌
Show Object Info: ❌

// Solo mostrar la esfera:
Show Ray Line: ❌
Show Hit Sphere: ✅
Show Object Info: ❌

// Solo mostrar info en pantalla:
Show Ray Line: ❌
Show Hit Sphere: ❌
Show Object Info: ✅
```

---

## 🔍 Debugging

### Para Ver el Raycast en el Editor:

1. **Ejecuta el juego**
2. **Abre** la ventana Scene (junto a Game)
3. **Mueve** el mouse y verás la línea y esfera moverse
4. **Apunta** a objetos y verás cambiar de rojo a verde

### Para Ver Info en el Juego:

1. **Ejecuta el juego**
2. **Mira** la esquina superior izquierda
3. Verás información en tiempo real del raycast

---

## 💡 Casos de Uso

### Debugging de Interacciones

Úsalo para:
- ✅ Verificar que el raycast está apuntando correctamente
- ✅ Ver la distancia exacta a los objetos
- ✅ Confirmar qué objeto está golpeando
- ✅ Ajustar la distancia de interacción

### Desarrollo

Úsalo mientras desarrollas para:
- ✅ Testear colisiones
- ✅ Ajustar capas (layers)
- ✅ Ver si los objetos tienen colliders
- ✅ Verificar que los tags están correctos

### Producción

Para la versión final del juego:
- ❌ Desactiva `Show Object Info` (no mostrar debug en pantalla)
- ✅ Mantén `Show Ray Line` y `Show Hit Sphere` solo en el editor

---

## 🎯 Combinación con Crosshair

Puedes usar **ambos** scripts juntos:

- **Crosshair**: Punto de mira en 2D (centro de pantalla)
- **RaycastVisualizer**: Punto de impacto en 3D (mundo del juego)

Configuración recomendada:
```
Crosshair:
├─ Dynamic Color: ✅
└─ Size: 8

RaycastVisualizer:
├─ Show Ray Line: ✅ (solo en editor)
├─ Show Hit Sphere: ✅ (solo en editor)
└─ Show Object Info: ❌ (desactivado en producción)
```

---

## 🐛 Troubleshooting

### Problema: No veo la línea ni la esfera
**Solución**:
- Abre la ventana **Scene** (no Game)
- Verifica que el juego esté **ejecutándose**
- Verifica que `Show Ray Line` y `Show Hit Sphere` estén marcados

### Problema: La línea siempre es roja
**Solución**:
- Verifica que `Raycast Distance` sea suficiente (3 o más)
- Verifica que `Interactable Layers` incluya los objetos
- Verifica que los objetos tengan **colliders**

### Problema: No veo la info en pantalla
**Solución**:
- Verifica que `Show Object Info` esté marcado
- Mira en la **esquina superior izquierda** de la pantalla Game
- Verifica que el script esté en un GameObject activo

---

## 📊 Comparación de Visualización

| Método | Dónde se Ve | Cuándo Usar |
|--------|-------------|-------------|
| **Gizmos (Línea/Esfera)** | Scene View | Desarrollo/Debugging |
| **OnGUI (Texto)** | Game View | Desarrollo/Testing |
| **Crosshair** | Game View | Producción |

---

## 🎮 Workflow Recomendado

### Durante Desarrollo:
```
RaycastVisualizer:
├─ Show Ray Line: ✅
├─ Show Hit Sphere: ✅
└─ Show Object Info: ✅

Crosshair:
└─ Activo: ✅
```

### Para Testing:
```
RaycastVisualizer:
├─ Show Ray Line: ✅
├─ Show Hit Sphere: ✅
└─ Show Object Info: ❌

Crosshair:
└─ Activo: ✅
```

### Para Producción:
```
RaycastVisualizer:
└─ GameObject: ❌ Desactivado

Crosshair:
└─ Activo: ✅
```

---

*Script creado: 2025-11-26*
*Útil para debugging y desarrollo*
