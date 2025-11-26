# Troubleshooting: Botella que no se Recoge Bien

## 🔍 Diagnóstico Rápido

### Paso 1: Verifica la Jerarquía de la Botella

Selecciona la botella problemática y verifica su estructura:

**Configuración CORRECTA:**
```
Botella_Problema (GameObject raíz)
├── Rigidbody ✅ DEBE estar aquí
├── TrashObject ✅ DEBE estar aquí o en un hijo
├── Collider ✅ Puede estar aquí o en hijos
├── Tag: "Recogible" ✅
└── [Modelo/Mesh] (hijo opcional)
    └── MeshRenderer
```

### Paso 2: Verifica los Componentes

**En el objeto raíz (Botella_Problema):**
- [ ] Tiene **Rigidbody**
  - Mass: 0.1 - 0.5
  - Use Gravity: ✅
  - Is Kinematic: ❌

- [ ] Tiene **TrashObject** script
  - Trash Type: (Amarillo/Azul/Verde/Rojo)
  - Score Value: 10

- [ ] Tiene **Collider** (BoxCollider, SphereCollider, etc.)
  - Is Trigger: ❌

- [ ] Tiene **Tag: "Recogible"**

- [ ] Tiene **MeshRenderer** (o en un hijo)

### Paso 3: Compara con la Botella que Funciona

1. Selecciona la botella que **SÍ funciona**
2. Anota su configuración exacta
3. Selecciona la botella que **NO funciona**
4. Haz que coincida con la que funciona

### Paso 4: Revisa los Logs de la Consola

Cuando recoges la botella, deberías ver en la consola:

```
🔍 PickupableObject en 'Botella': Rigidbody encontrado en 'Botella'
✅ PlayerInteraction: Recogiendo 'Botella' (script encontrado en 'Botella')
🎬 Animación de agacharse iniciada. Esperando 0.8 segundos...
📦 PickupableObject: Script en 'Botella' → Moviendo objeto raíz 'Botella' al HoldPoint
   Jerarquía: Botella
✅ Objeto 'Botella' ahora en la mano del jugador
Recogiste: Botella (Tipo: Amarillo)
```

## 🔧 Soluciones Comunes

### Problema: Solo se mueve el collider, no el modelo

**Causa**: El Rigidbody no está en el objeto raíz

**Solución**:
1. Selecciona el objeto raíz de la botella
2. Add Component → Rigidbody
3. Si ya tiene Rigidbody en un hijo, muévelo al raíz

### Problema: El objeto se recoge pero está invisible

**Causa**: El MeshRenderer está en un objeto que no es hijo del que tiene el Rigidbody

**Solución**:
1. Asegúrate de que toda la geometría visual sea hija del objeto que tiene el Rigidbody
2. Estructura correcta:
   ```
   Botella (Rigidbody aquí)
   └── Modelo (MeshRenderer aquí)
   ```

### Problema: No se puede recoger el objeto

**Causa**: Falta el tag "Recogible" o el collider

**Solución**:
1. Selecciona el objeto con el collider
2. En la parte superior del Inspector, cambia Tag a "Recogible"
3. Verifica que tenga un Collider activo (Is Trigger: OFF)

### Problema: El objeto atraviesa el suelo

**Causa**: El Rigidbody está en Kinematic o no tiene collider

**Solución**:
1. Rigidbody → Is Kinematic: ❌ (desactivado)
2. Asegúrate de que tenga un Collider
3. Collider → Is Trigger: ❌ (desactivado)

## 📋 Checklist Final

Antes de probar, verifica:

- [ ] Rigidbody en el objeto raíz
- [ ] TrashObject script presente
- [ ] Tag "Recogible" configurado
- [ ] Collider presente y activo
- [ ] MeshRenderer presente (para verlo)
- [ ] Trash Type configurado correctamente

## 🎮 Cómo Probar

1. Ejecuta el juego
2. Abre la **Consola** (Ctrl + Shift + C en Unity)
3. Acércate a la botella problemática
4. Presiona E para recogerla
5. **Lee los logs** en la consola
6. Copia y pega los logs si hay errores

## 💡 Información Útil para Debugging

Si me pasas esta información, puedo ayudarte mejor:

1. **Nombre del objeto** en la jerarquía
2. **Logs de la consola** cuando intentas recogerlo
3. **Estructura de la jerarquía** (qué hijos tiene)
4. **Qué componentes** tiene y en qué objetos están

---

## ⚙️ Sensibilidad de Cámara

La sensibilidad se redujo de **150** a **100**.

Si aún está muy sensible:
1. Selecciona la **Main Camera** en la jerarquía
2. En el Inspector, busca **PlayerCamera** script
3. Ajusta **Mouse Sensitivity**:
   - 50 = Muy lento
   - 100 = Normal (actual)
   - 150 = Rápido
   - 200 = Muy rápido

Prueba con valores entre **70-100** hasta encontrar el que te guste.
