# ✅ CHECKLIST DE MEJORAS - Proyecto TOC

## 🔴 PRIORIDAD CRÍTICA (Hacer HOY)

### Bug Fixes
- [ ] **Corregir PlayerAnimation.cs** (2 minutos)
  - [ ] Cambiar `public CharacterController controller;` por `private Rigidbody rb;`
  - [ ] Agregar `rb = GetComponent<Rigidbody>();` en Start()
  - [ ] Cambiar `controller.velocity.magnitude` por `rb.linearVelocity.magnitude`
  - [ ] Probar que las animaciones funcionen

### Optimización de Rendimiento
- [ ] **Cachear referencias en PlayerInteraction.cs** (10 minutos)
  - [ ] Mover `cameraTransform = Camera.main.transform` a Start()
  - [ ] Eliminar búsquedas repetidas en Update()
  - [ ] Probar que la interacción siga funcionando

- [ ] **Cachear referencias en TrashManager.cs** (10 minutos)
  - [ ] Crear `List<TrashItem> trashItems`
  - [ ] Cachear FindObjectsOfType en Start()
  - [ ] Agregar método RegisterTrash() para auto-registro

### Funcionalidad Core
- [ ] **Implementar ScoreManager.cs** (30 minutos)
  - [ ] Crear archivo en `Assets/Scripts/Systems/Managers/ScoreManager.cs`
  - [ ] Copiar código de `ANALISIS_Y_MEJORAS_RECOMENDADAS.md` - Sección 1
  - [ ] Crear GameObject "ScoreManager" en la escena
  - [ ] Agregar componente ScoreManager
  - [ ] Configurar puntos base y multiplicador de combo
  - [ ] Probar que los puntos se sumen correctamente

**Tiempo total**: ~1 hora  
**Impacto**: 🔥 ALTO - Arregla bugs críticos y agrega funcionalidad esencial

---

## 🟡 PRIORIDAD ALTA (Hacer Esta Semana)

### Feedback Visual
- [ ] **Mejorar Crosshair.cs** (45 minutos)
  - [ ] Agregar sprites para diferentes estados
  - [ ] Implementar detección de basurero correcto/incorrecto
  - [ ] Cambiar color según estado (verde/rojo/amarillo)
  - [ ] Agregar animación de hover
  - [ ] Probar con todos los tipos de basura

### UI/UX
- [ ] **Expandir UIManager.cs** (30 minutos)
  - [ ] Agregar TextMeshProUGUI para score
  - [ ] Agregar TextMeshProUGUI para combo
  - [ ] Agregar panel de combo con animación
  - [ ] Suscribirse a eventos de score
  - [ ] Crear animación "Pop" para combos
  - [ ] Probar que se actualice correctamente

- [ ] **Crear TutorialManager.cs** (1 hora)
  - [ ] Crear archivo en `Assets/Scripts/Systems/Managers/TutorialManager.cs`
  - [ ] Copiar código de `ANALISIS_Y_MEJORAS_RECOMENDADAS.md` - Sección 4
  - [ ] Crear GameObject "TutorialManager" en la escena
  - [ ] Configurar mensajes de tutorial
  - [ ] Probar que aparezcan en el momento correcto

### Efectos Visuales
- [ ] **Agregar partículas a TrashCan.cs** (30 minutos)
  - [ ] Crear ParticleSystem para clasificación correcta (verde)
  - [ ] Crear ParticleSystem para clasificación incorrecta (rojo)
  - [ ] Agregar referencias en TrashCan.cs
  - [ ] Llamar a Play() en momentos correctos
  - [ ] Ajustar colores y tamaño de partículas

**Tiempo total**: ~3 horas  
**Impacto**: 🔥 MEDIO-ALTO - Mejora significativamente la experiencia del jugador

---

## 🟢 PRIORIDAD MEDIA (Hacer Próximas 2 Semanas)

### Persistencia
- [ ] **Implementar SaveSystem.cs** (45 minutos)
  - [ ] Crear archivo en `Assets/Scripts/Systems/SaveSystem.cs`
  - [ ] Copiar código de `ANALISIS_Y_MEJORAS_RECOMENDADAS.md` - Sección 6
  - [ ] Crear clase GameData serializable
  - [ ] Implementar métodos Save/Load/Delete
  - [ ] Integrar con GameManager
  - [ ] Probar guardado y carga de high scores

### Refactoring
- [ ] **Migrar TrashCan.OnGUI() a Canvas** (30 minutos)
  - [ ] Crear Canvas WorldSpace para cada basurero
  - [ ] Agregar TextMeshProUGUI para label
  - [ ] Eliminar método OnGUI()
  - [ ] Actualizar Update() para mostrar/ocultar canvas
  - [ ] Probar que los labels se vean correctamente

### Audio
- [ ] **Agregar sonidos faltantes a AudioManager** (20 minutos)
  - [ ] Agregar AudioClip para bolsa llena
  - [ ] Agregar AudioClip para combo
  - [ ] Agregar AudioClip para advertencia de tiempo
  - [ ] Implementar métodos PlayBagFullSFX(), PlayComboSFX(), etc.
  - [ ] Suscribirse a eventos correspondientes
  - [ ] Asignar clips en el Inspector
  - [ ] Probar que se reproduzcan correctamente

### Documentación
- [ ] **Actualizar README.md** (15 minutos)
  - [ ] Ya creado ✅
  - [ ] Agregar capturas de pantalla del juego
  - [ ] Agregar GIF de gameplay
  - [ ] Actualizar sección de créditos con tu nombre

**Tiempo total**: ~2 horas  
**Impacto**: 🔥 MEDIO - Pulido y profesionalización del proyecto

---

## 🔵 PRIORIDAD BAJA (Futuro)

### Nuevas Funcionalidades
- [ ] **Sistema de Niveles** (3-4 horas)
  - [ ] Crear SceneManager para transiciones
  - [ ] Diseñar múltiples niveles con dificultad creciente
  - [ ] Implementar sistema de desbloqueo
  - [ ] Agregar pantalla de selección de nivel

- [ ] **Power-ups y Bonus** (2-3 horas)
  - [ ] Diseñar power-ups (tiempo extra, multiplicador, etc.)
  - [ ] Implementar lógica de aparición aleatoria
  - [ ] Crear efectos visuales para power-ups
  - [ ] Balancear dificultad

- [ ] **Leaderboards** (4-5 horas)
  - [ ] Integrar con servicio online (PlayFab, Firebase)
  - [ ] Crear UI de leaderboard
  - [ ] Implementar sistema de nombres
  - [ ] Agregar filtros (diario, semanal, global)

### Testing
- [ ] **Unit Tests** (2-3 horas)
  - [ ] Configurar Unity Test Framework
  - [ ] Crear tests para ScoreManager
  - [ ] Crear tests para TrashManager
  - [ ] Crear tests para SaveSystem

**Tiempo total**: ~10-15 horas  
**Impacto**: 🔥 BAJO - Expansión del proyecto

---

## 📊 PROGRESO GENERAL

### Completado
- [x] Sistema de eventos desacoplado
- [x] AudioManager con persistencia
- [x] Sistema de clasificación de basura
- [x] Documentación de patrones
- [x] Análisis completo del proyecto

### En Progreso
- [ ] Sistema de puntuación (0%)
- [ ] Feedback visual mejorado (0%)
- [ ] Tutorial (0%)

### Total Estimado
- **Crítico**: 1 hora
- **Alto**: 3 horas
- **Medio**: 2 horas
- **Bajo**: 10-15 horas
- **TOTAL**: ~16-21 horas para proyecto completo

---

## 🎯 PLAN DE TRABAJO SUGERIDO

### Día 1 (1 hora)
```
✅ Todas las tareas CRÍTICAS
- Corregir PlayerAnimation
- Cachear referencias
- Implementar ScoreManager
```

### Día 2 (2 horas)
```
✅ Mejorar Crosshair
✅ Expandir UIManager
```

### Día 3 (1.5 horas)
```
✅ Crear TutorialManager
✅ Agregar partículas
```

### Día 4 (2 horas)
```
✅ Implementar SaveSystem
✅ Migrar OnGUI a Canvas
✅ Agregar sonidos faltantes
```

### Día 5 (30 min)
```
✅ Testing completo
✅ Documentación final
✅ Capturas de pantalla
```

**Total**: 7 horas distribuidas en 5 días

---

## 📝 NOTAS

### Antes de Empezar
1. Haz backup del proyecto
2. Crea una nueva rama en Git (si usas control de versiones)
3. Lee el documento completo `ANALISIS_Y_MEJORAS_RECOMENDADAS.md`

### Durante el Desarrollo
1. Marca cada checkbox al completar
2. Haz commit después de cada sección completada
3. Prueba cada cambio antes de continuar

### Después de Completar
1. Juega el juego completo de principio a fin
2. Pide feedback a alguien más
3. Actualiza la documentación con cambios realizados

---

## 🎉 CELEBRA TUS LOGROS

Cada sección completada es un paso hacia un proyecto más profesional. ¡No olvides celebrar tus avances!

- ✅ Sección Crítica → 🎊 ¡Proyecto funcional sin bugs!
- ✅ Sección Alta → 🎊 ¡Experiencia de usuario mejorada!
- ✅ Sección Media → 🎊 ¡Proyecto pulido y profesional!
- ✅ Sección Baja → 🎊 ¡Proyecto completo y escalable!

---

*Checklist creado: 26 de Noviembre de 2025*  
*Última actualización: Pendiente*
