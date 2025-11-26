# Guía de Configuración: Animación de Tirar Basura

## 📋 Resumen de Cambios

El sistema ahora diferencia entre dos acciones:
- **"Throw"** → Tirar basura en un basurero (nueva animación)
- **"Drop"** → Soltar objetos normalmente (animación existente)

## 🎮 Configuración del Animator Controller

### 1. Abrir el Animator Controller del Jugador

1. Selecciona el GameObject del jugador en la jerarquía
2. En el Inspector, busca el componente **Animator**
3. Haz doble clic en el **Controller** para abrirlo en la ventana Animator

### 2. Crear el Parámetro "Throw"

1. En la ventana **Animator**, ve a la pestaña **Parameters** (izquierda)
2. Click en el botón **+**
3. Selecciona **Trigger**
4. Nómbralo exactamente: **Throw** (con T mayúscula)

### 3. Crear el Estado de Animación "Throw"

**Opción A: Si ya tienes la animación de tirar**
1. Click derecho en el grid del Animator → **Create State** → **Empty**
2. Nómbralo: **Throw**
3. Selecciona el estado **Throw**
4. En el Inspector, asigna tu animación de tirar en **Motion**

**Opción B: Si aún no tienes la animación**
1. Crea una animación temporal o usa la misma que "Drop" por ahora
2. Más adelante puedes reemplazarla con una animación personalizada

### 4. Crear la Transición

1. **Desde "Idle" o "Any State" hacia "Throw":**
   - Click derecho en **Any State** → **Make Transition**
   - Arrastra la flecha hacia el estado **Throw**

2. **Configurar la transición:**
   - Selecciona la transición (la flecha)
   - En el Inspector:
     - **Conditions**: Agrega el trigger **Throw**
     - **Has Exit Time**: ❌ Desactivar
     - **Transition Duration**: 0.1 - 0.2 (transición rápida)

3. **Desde "Throw" de vuelta a "Idle":**
   - Click derecho en **Throw** → **Make Transition**
   - Arrastra hacia **Idle** (o el estado base)
   - Configurar:
     - **Has Exit Time**: ✅ Activar
     - **Exit Time**: 0.9 - 1.0 (espera a que termine la animación)
     - **Transition Duration**: 0.1

## 🎨 Ejemplo de Estructura del Animator

```
[Any State] ---(Trigger: Throw)---> [Throw State]
                                         |
                                         | (Exit Time)
                                         ↓
                                    [Idle State]
```

## 🔧 Parámetros del Animator Necesarios

Asegúrate de tener estos triggers en tu Animator:
- ✅ **Throw** (nuevo) - Para tirar basura en basurero
- ✅ **Drop** (existente) - Para soltar objetos normalmente
- ✅ **PickUp** (existente) - Para recoger objetos

## 🎬 Comportamiento del Sistema

### Cuando el jugador tira basura en un basurero:
1. Se activa el trigger **"Throw"** → Animación de tirar
2. El objeto se suelta de la mano
3. El basurero se abre
4. El objeto es absorbido hacia el basurero

### Cuando el jugador suelta un objeto (sin basurero):
1. Se activa el trigger **"Drop"** → Animación de soltar
2. El objeto cae al suelo con física

## 💡 Consejos para la Animación "Throw"

### Animación recomendada:
- **Duración**: 0.5 - 1.0 segundos
- **Movimiento**: Brazo hacia adelante/abajo (como tirar algo)
- **Timing**: El objeto debe soltarse a mitad de la animación

### Si no tienes animación personalizada:
Puedes usar una de estas opciones temporales:
1. Duplicar la animación "Drop" y ajustar la velocidad
2. Usar la misma animación que "PickUp" pero en reversa
3. Crear una animación simple con el Animation window de Unity

## 🧪 Prueba del Sistema

1. **Ejecuta el juego**
2. **Recoge un objeto de basura**
3. **Acércate a un basurero**
4. **Presiona E o Click**
   - Si es el basurero correcto: Verás la animación "Throw"
   - Si no hay basurero cerca: Verás la animación "Drop"

## 📝 Notas Importantes

- El trigger **"Throw"** solo se activa cuando:
  - ✅ Tienes un objeto en la mano
  - ✅ Estás mirando a un basurero
  - ✅ Presionas E o Click

- El trigger **"Drop"** se activa cuando:
  - ✅ Tienes un objeto en la mano
  - ✅ NO estás mirando a un basurero
  - ✅ Presionas E o Click

## 🔧 Troubleshooting

**Problema:** La animación no se reproduce
- Verifica que el trigger se llame exactamente **"Throw"** (con T mayúscula)
- Verifica que el Animator esté asignado en PlayerInteraction
- Revisa que la transición tenga el trigger correcto

**Problema:** La animación se reproduce pero se ve mal
- Ajusta el **Transition Duration** (más bajo = más rápido)
- Ajusta el **Exit Time** del estado Throw
- Verifica que la animación tenga la duración correcta

**Problema:** El objeto se suelta antes/después de la animación
- Actualmente el objeto se suelta inmediatamente
- Si quieres sincronizarlo con la animación, necesitarás usar Animation Events
  (puedo ayudarte con esto si lo necesitas)

## 🎯 Mejora Futura: Sincronización con Animation Events

Si quieres que el objeto se suelte exactamente en el momento correcto de la animación:

1. Abre la animación "Throw" en el Animation window
2. Agrega un **Animation Event** en el frame donde quieres soltar
3. Llama a una función en PlayerInteraction
4. Modifica el código para esperar el evento

¿Quieres que implemente esto? 🎬
