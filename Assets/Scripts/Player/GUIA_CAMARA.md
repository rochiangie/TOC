# 🎮 Guía de Configuración: Nueva Cámara PlayerCamera

## ✅ Sistema Implementado

He reemplazado el sistema de cámara anterior con uno nuevo basado en el **MouseLookController** de tu proyecto anterior de TalentoTech 3D.

---

## 🎯 Cómo Funciona Ahora

### **Mouse Horizontal** → Rota al JUGADOR
- Mueves el mouse a la izquierda/derecha
- El cuerpo del jugador rota
- La cámara lo sigue automáticamente

### **Mouse Vertical** → Mueve la CÁMARA arriba/abajo
- Mueves el mouse arriba/abajo
- Solo la cámara se mueve verticalmente
- El jugador no se inclina

---

## 🔧 Configuración en Unity

### Paso 1: Configurar la Main Camera

1. **Selecciona** la Main Camera en la jerarquía
2. **Verifica** que tenga el componente `PlayerCamera` (debería estar automáticamente)
3. **Configura** los siguientes valores en el Inspector:

#### Sensibilidad
- **Mouse Sensitivity**: `100` (ajusta a tu gusto)
  - 50-80 = Lento
  - 100-120 = Normal
  - 150-200 = Rápido

#### Límites de Rotación Vertical
- **Up Limit**: `60` (cuánto puedes mirar hacia arriba)
- **Down Limit**: `-40` (cuánto puedes mirar hacia abajo)

#### Referencias
- **Player Body**: Arrastra aquí el GameObject del jugador (el que tiene PlayerMovement)
  - Si no lo asignas, el script lo buscará automáticamente por el tag "Player"

#### Camera Offset
- **X**: `0` (izquierda/derecha)
- **Y**: `1.5` (altura sobre el jugador)
- **Z**: `-3` (distancia detrás del jugador)

#### Colisión de Cámara
- **Collision Layers**: `Default` (o las capas con las que quieres que la cámara colisione)
- **Camera Radius**: `0.2` (radio de la esfera de colisión)

---

## 🎮 Controles

### Durante el Juego:
- **Mouse**: Controla la cámara y rotación del jugador
- **ESC**: Libera el cursor (si implementas menú de pausa)

### Funciones Públicas Disponibles:

```csharp
// Activar/desactivar controles (para pausas)
PlayerCamera camera = FindObjectOfType<PlayerCamera>();
camera.SetControlsActive(false); // Pausa
camera.SetControlsActive(true);  // Reanudar

// Bloquear/desbloquear cámara (para menús flotantes)
camera.SetLockState(true);  // Bloquear
camera.SetLockState(false); // Desbloquear

// Ajustar sensibilidad en tiempo de ejecución
camera.SetSensitivity(150f);
```

---

## ⚙️ Ajustes Recomendados

### Si la cámara se siente muy sensible:
1. Reduce **Mouse Sensitivity** a `70-80`
2. O ajusta desde código: `camera.SetSensitivity(70f);`

### Si la cámara atraviesa paredes:
1. Aumenta **Camera Radius** a `0.3` o `0.4`
2. Verifica que **Collision Layers** incluya las paredes

### Si quieres la cámara más cerca/lejos:
1. Ajusta **Camera Offset Z**:
   - `-2` = Más cerca
   - `-4` = Más lejos

### Si quieres la cámara más alta/baja:
1. Ajusta **Camera Offset Y**:
   - `1.0` = Más baja
   - `2.0` = Más alta

---

## 🔍 Diferencias con el Sistema Anterior

### ANTES (PlayerCamera viejo):
- ❌ Cámara orbital independiente
- ❌ Mouse horizontal orbitaba la cámara
- ❌ Confuso y poco intuitivo

### AHORA (PlayerCamera nuevo):
- ✅ Mouse horizontal rota al jugador
- ✅ Cámara sigue al jugador automáticamente
- ✅ Más intuitivo (como la mayoría de juegos)
- ✅ Basado en tu proyecto anterior (código probado)
- ✅ Detección de colisiones mejorada
- ✅ Control de estado (pausas, menús)

---

## 🐛 Troubleshooting

### Problema: La cámara no se mueve
**Solución**: 
- Verifica que **Player Body** esté asignado
- Verifica que el jugador tenga el tag "Player"
- Revisa la consola para logs de error

### Problema: El jugador no rota con el mouse
**Solución**:
- Asegúrate de que **Player Body** apunte al GameObject correcto
- Verifica que `PlayerMovement.rotateWithCamera` esté en `false`

### Problema: La cámara atraviesa paredes
**Solución**:
- Aumenta **Camera Radius**
- Verifica **Collision Layers**
- Asegúrate de que las paredes tengan colliders

### Problema: El cursor no se bloquea
**Solución**:
- Verifica que `_controlsActive` esté en `true` en el Inspector
- Llama a `camera.SetControlsActive(true)` desde código

---

## 📊 Comparación de Sensibilidad

| Valor | Sensación | Recomendado Para |
|-------|-----------|------------------|
| 50-70 | Muy lento | Precisión extrema |
| 80-100 | Normal | Mayoría de jugadores |
| 120-150 | Rápido | Jugadores experimentados |
| 180-250 | Muy rápido | Solo para expertos |

---

## 💡 Tips

1. **Prueba diferentes sensibilidades** hasta encontrar la que te guste
2. **Ajusta los límites verticales** si quieres más/menos rango de visión
3. **Usa Gizmos** en el editor para ver el punto focal de la cámara
4. **Revisa los logs** en la consola para debugging

---

## 🎯 Próximos Pasos

1. **Prueba el juego** y ajusta la sensibilidad
2. **Configura los límites** de rotación vertical a tu gusto
3. **Ajusta el offset** de la cámara para la distancia perfecta
4. **Implementa menú de pausa** usando `SetControlsActive()`

---

*Sistema basado en MouseLookController del proyecto anterior de TalentoTech 3D*
*Fecha: 2025-11-25*
