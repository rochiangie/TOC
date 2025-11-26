# Guía de Configuración: Sistema de Mensajes de Feedback

## 📋 Pasos para configurar el sistema de mensajes en Unity

### 1. Crear el Canvas y el TextMeshPro

1. **Crear Canvas:**
   - En la jerarquía: Click derecho → UI → Canvas
   - Nombre: "FeedbackCanvas"
   - Canvas Scaler:
     - UI Scale Mode: Scale With Screen Size
     - Reference Resolution: 1920 x 1080

2. **Crear TextMeshPro para mensajes:**
   - Click derecho en "FeedbackCanvas" → UI → Text - TextMeshPro
   - Nombre: "FeedbackMessageText"
   
3. **Configurar el TextMeshPro:**
   - **Rect Transform:**
     - Anchor: Middle Center
     - Pos X: 0, Pos Y: 200 (o donde prefieras)
     - Width: 800, Height: 200
   
   - **TextMeshPro - Text (UI):**
     - Font Size: 32
     - Alignment: Center (horizontal y vertical)
     - Color: Blanco (se cambiará dinámicamente)
     - Wrapping: Enabled
     - Overflow: Overflow
     - Auto Size: Optional (recomendado)
   
   - **Outline (Opcional pero recomendado):**
     - Add Component → TextMeshPro → Outline
     - Color: Negro
     - Size: 0.2

### 2. Agregar el script FeedbackMessageUI

1. **En el Canvas "FeedbackCanvas":**
   - Add Component → FeedbackMessageUI
   
2. **Configurar las referencias:**
   - Message Text: Arrastra "FeedbackMessageText" aquí
   - Default Duration: 3 (segundos)
   
3. **Colores (opcional, ya tienen valores por defecto):**
   - Error Color: Rojo suave (1, 0.3, 0.3)
   - Success Color: Verde suave (0.3, 1, 0.3)
   - Info Color: Blanco (1, 1, 1)
   - Warning Color: Amarillo (1, 0.8, 0.2)

### 3. Configurar el Canvas Order

Para asegurarte de que los mensajes aparezcan por encima de otros elementos UI:
- Selecciona "FeedbackCanvas"
- En Canvas component:
  - Sort Order: 100 (o un número alto)

### 4. Desactivar el TextMeshPro inicialmente

- Selecciona "FeedbackMessageText"
- Desmarca el checkbox en la parte superior del Inspector para desactivarlo
- (El script lo activará automáticamente cuando haya mensajes)

## 🎨 Personalización Avanzada (Opcional)

### Agregar sombra al texto:
1. Selecciona "FeedbackMessageText"
2. Add Component → Shadow
3. Effect Distance: (2, -2)
4. Color: Negro con alpha 0.5

### Agregar panel de fondo:
1. Click derecho en "FeedbackCanvas" → UI → Panel
2. Nombre: "FeedbackBackground"
3. Moverlo como hijo de "FeedbackMessageText" (arrastrarlo encima)
4. Configurar:
   - Color: Negro con alpha 0.7
   - Stretch to fill parent
   - Agregar padding si es necesario

### Agregar animación de entrada/salida más elaborada:
El script ya incluye fade in/out, pero puedes agregar:
- Animator component con animaciones de escala
- Efectos de partículas
- Sonidos de feedback

## 🧪 Prueba del Sistema

Para probar que funciona:
1. Ejecuta el juego
2. Recoge un objeto de basura
3. Intenta tirarlo en el basurero INCORRECTO
   - Deberías ver un mensaje ROJO diciendo que es incorrecto
4. Intenta tirarlo en el basurero CORRECTO
   - Deberías ver un mensaje VERDE diciendo que es correcto

## 📝 Notas Importantes

- El script usa un patrón Singleton, así que solo debe haber UNA instancia en la escena
- Si no ves mensajes, verifica la consola para errores
- Asegúrate de que el Canvas esté en modo "Screen Space - Overlay" o "Screen Space - Camera"
- El texto se desactiva automáticamente después de mostrar el mensaje

## 🎮 Uso desde otros scripts

Si quieres usar este sistema desde otros scripts:

```csharp
// Mensaje de error
FeedbackMessageUI.Instance.ShowError("¡Algo salió mal!", 3f);

// Mensaje de éxito
FeedbackMessageUI.Instance.ShowSuccess("¡Bien hecho!", 2f);

// Mensaje informativo
FeedbackMessageUI.Instance.ShowInfo("Presiona E para interactuar", 2f);

// Mensaje de advertencia
FeedbackMessageUI.Instance.ShowWarning("¡Cuidado!", 2f);

// Mensaje con color personalizado
FeedbackMessageUI.Instance.ShowMessage("Mensaje custom", Color.cyan, 3f);
```

## 🔧 Troubleshooting

**Problema:** No veo los mensajes
- Verifica que el Canvas esté activo
- Verifica que FeedbackMessageUI.Instance no sea null
- Revisa la consola para errores

**Problema:** Los mensajes aparecen muy pequeños/grandes
- Ajusta el Font Size en el TextMeshPro
- Verifica el Canvas Scaler

**Problema:** Los mensajes se cortan
- Aumenta el Width y Height del Rect Transform
- Activa Wrapping en el TextMeshPro
