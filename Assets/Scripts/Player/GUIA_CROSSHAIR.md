# 🎯 Guía: Crosshair (Punto de Mira)

## ✅ Script Creado: Crosshair.cs

Este script muestra un **punto de mira** en el centro de la pantalla que indica dónde apunta tu raycast de interacción.

---

## 🔧 Configuración en Unity

### Opción 1: Configuración Automática (Recomendada)

1. **Crea un GameObject vacío** en la escena
   - Click derecho en Hierarchy → Create Empty
   - Nombre: "CrosshairManager"

2. **Agrega el script**
   - Selecciona "CrosshairManager"
   - Add Component → Crosshair

3. **¡Listo!** El script creará automáticamente:
   - Canvas (si no existe)
   - Punto de mira en el centro de la pantalla

---

### Opción 2: Configuración Manual

Si ya tienes un Canvas:

1. **Crea un GameObject vacío** como hijo del Canvas
   - Click derecho en Canvas → Create Empty
   - Nombre: "Crosshair"

2. **Agrega el script**
   - Selecciona "Crosshair"
   - Add Component → Crosshair

---

## ⚙️ Configuración en el Inspector

```
Crosshair (Script)
├─ Crosshair Color: Blanco (o el color que quieras)
├─ Size: 8 (tamaño del punto)
├─ Alpha: 0.8 (opacidad)
├─ Dynamic Color: ✅ (marcado) ← RECOMENDADO
├─ Interactable Color: Verde
├─ Raycast Distance: 3 (debe coincidir con PlayerInteraction)
└─ Interactable Layers: Everything
```

---

## 🎨 Características

### Color Dinámico ✨

Si **Dynamic Color** está activado:
- **Blanco** = No hay nada interactuable
- **Verde** = Estás apuntando a algo que puedes recoger/interactuar

### Personalización

Puedes ajustar:
- **Color**: Cambia `Crosshair Color`
- **Tamaño**: Ajusta `Size` (2-20)
- **Opacidad**: Ajusta `Alpha` (0-1)
- **Color de interacción**: Cambia `Interactable Color`

---

## 🎮 Cómo Funciona

1. El script crea un **punto circular** en el centro de la pantalla
2. Hace un **raycast** desde la cámara hacia adelante
3. Si detecta un objeto interactuable, cambia a **verde**
4. Si no hay nada, se queda **blanco**

---

## 💡 Tips

### Hacer el punto más visible:
```
Size: 10-12
Alpha: 1.0
Crosshair Color: Amarillo o Cyan
```

### Hacer el punto más discreto:
```
Size: 4-6
Alpha: 0.5
Crosshair Color: Blanco
```

### Desactivar color dinámico:
```
Dynamic Color: ❌ (desmarcado)
```

---

## 🔧 Funciones Públicas

Puedes controlar el crosshair desde otros scripts:

```csharp
Crosshair crosshair = FindObjectOfType<Crosshair>();

// Mostrar/ocultar
crosshair.SetVisible(false); // Ocultar
crosshair.SetVisible(true);  // Mostrar

// Cambiar color
crosshair.SetColor(Color.red);

// Cambiar tamaño
crosshair.SetSize(12f);
```

---

## 🐛 Troubleshooting

### Problema: No veo el punto
**Solución**:
- Verifica que el GameObject con Crosshair esté activo
- Aumenta el `Size` a 15-20
- Cambia el color a algo más visible (amarillo, cyan)

### Problema: El punto no cambia de color
**Solución**:
- Verifica que `Dynamic Color` esté marcado
- Verifica que `Raycast Distance` coincida con PlayerInteraction (3)
- Verifica que `Interactable Layers` incluya los objetos

### Problema: El punto está en la esquina
**Solución**:
- El script debería centrarlo automáticamente
- Si no, verifica que el Canvas esté en modo "Screen Space - Overlay"

---

## 📋 Checklist

- [ ] GameObject con script Crosshair creado
- [ ] Canvas existe en la escena
- [ ] El punto se ve en el centro de la pantalla
- [ ] El punto cambia a verde al apuntar a objetos
- [ ] El tamaño y color son de tu agrado

---

## 🎯 Resultado Final

Deberías ver:
- ⚪ **Punto blanco** en el centro de la pantalla normalmente
- 🟢 **Punto verde** cuando apuntas a basura o basureros
- El punto se mueve con la cámara automáticamente

---

*Script creado: 2025-11-25*
