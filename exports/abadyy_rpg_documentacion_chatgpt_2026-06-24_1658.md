# AbadyyRpg - Documentacion para ChatGPT

Generado: 2026-06-24 16:58:52

Archivos incluidos:
- DOCUMENTACION.md
- SCOPE_TEST_SIMPLIFICADO.md
- scope_demo_test_tier1.md
- LISTA_TAREAS_DEMO.md
- BALANCE_RUTAS_DEMO.md
- SISTEMAS_CONGELADOS_DEMO.md
- definicion_clases_combate_demo.md
- enemigos_dificultad_xp_recompensas_demo.md
- items_armas_armaduras_demo.md
- sprint_economia_contratos_tienda.md
- Assets\Auditoria\AUDITORIA_LIMPIEZA_DEMO.md
- ProjectSettings\ProjectVersion.txt


---

# Fuente: DOCUMENTACION.md

# Documentacion del proyecto AbadyyRpg

Estado documentado: codigo actual del proyecto Unity en `Assets/`.

## Resumen general

AbadyyRpg es un RPG por escenas construido en Unity 2022.3.62f3. El codigo actual se organiza alrededor de estos sistemas:

- Mapa del mundo: nodos navegables, eventos, combates por nodo y desbloqueos.
- Combate automatico por turnos acumulativos: unidades, habilidades, taunt, estados, consumibles y recompensas.
- Party persistente: roster, miembros activos, salud, mana, stamina, equipo y consumibles.
- Caravana: descanso, reparacion de armaduras, recursos e interfaz generada.
- Inventario/equipamiento/crafting: items ScriptableObject, base de datos, recursos, recetas y paneles UI.
- UI de gestion: roster, equipo, suministros, crafting, tacticas y seleccion de habilidades/condiciones.

## Version y paquetes

- Version de Unity: `2022.3.62f3`.
- Paquetes principales:
  - `com.unity.ugui`
  - `com.unity.textmeshpro`
  - `com.unity.test-framework`
  - `com.unity.ai.navigation`
  - paquetes 2D Sprite/Tilemap y herramientas IDE.

## Escenas principales

Las escenas actuales estan en `Assets/Scenes/`:

- `WorldMapScene.unity`: mapa de nodos. Usa `WorldMapManager`, `WorldMapNode`, `GameRunState` y `EventPanelUI`.
- `BattleScene.unity`: combate. Usa `BattleManager`, `BattleSetup`, `GameManager`, `PartyRuntimeState`.
- `CaravanScene.unity`: gestion de caravana. Usa `CaravanManager`, `CaravanState`, inventario, party y UI de gestion.

Tambien existe `Assets/escena_abadia.unity`, aparentemente una escena anterior o experimental.

## Flujo de juego actual

### Inicio y mapa

1. `WorldMapManager` inicia el mapa.
2. Busca o usa la lista `allNodes`.
3. Aplica estado persistente desde `GameRunState`: nodo actual, visitados, desbloqueados, completados, eventos completados y flags.
4. Marca el nodo actual como visitado y actual.

Archivo principal: `Assets/WorldMap/Managers/WorldMapManager.cs`.

### Movimiento entre nodos

El jugador hace click sobre un `WorldMapNode`.

`WorldMapNode.OnMouseDown()` llama a:

```csharp
WorldMapManager.Instance.TryMoveToNode(this);
```

`TryMoveToNode` valida:

- que no haya un evento activo;
- que el nodo no sea el actual;
- que este desbloqueado;
- que este conectado al nodo actual.

Si pasa las validaciones, `MoveToNode` actualiza el estado visual y persistente.

Archivo principal: `Assets/WorldMap/Nodes/WorldMapNode.cs`.

### Combate desde mapa

Si el nodo actual es de combate (`isCombatNode`) y no esta completado:

1. `WorldMapManager.StartCombatFromNode` llama a `GameRunState.RequestBattle`.
2. Se guarda:
   - `battleGroupId`
   - `returnNodeId`
   - escena de retorno
3. Se carga `BattleScene`.

En `BattleScene`, `BattleManager.StartBattleFromSetup`:

1. Lee `PartyRuntimeState.Instance`.
2. Carga la party activa.
3. Instancia las vistas aliadas en posiciones de `BattleSetup`.
4. Crea enemigos desde el grupo pedido en `GameRunState.currentBattleGroupId`.
5. Inicia el combate.

Cuando termina el combate:

1. `BattleManager.EndBattle` registra el resultado en `GameRunState`.
2. Vuelve a `returnSceneName`, normalmente `WorldMapScene`.
3. `WorldMapManager.ApplyReturnedCombatResult` marca el nodo completado, aplica recompensas y desbloquea nodos.

## Persistencia runtime

El proyecto usa varios singletons con `DontDestroyOnLoad` para conservar estado entre escenas.

### GameRunState

Archivo: `Assets/WorldMap/Managers/GameRunState.cs`

Responsabilidades:

- Guardar la solicitud de combate actual.
- Recordar a que escena/nodo volver.
- Registrar resultado del ultimo combate.
- Mantener estado persistente del mapa:
  - `currentNodeId`
  - `completedNodeIds`
  - `unlockedNodeIds`
  - `visitedNodeIds`
  - `eventCompletedNodeIds`
  - `flags`
- Guardar una recompensa pendiente si un evento inicia combate.

### PartyRuntimeState

Archivo: `Assets/Scripts/Core/PartyRuntimeState.cs`

Responsabilidades:

- Mantener el roster completo.
- Mantener la party activa.
- Limitar roster y miembros activos:
  - `maxRosterMembers`
  - `maxActiveMembers`
- Crear unidades runtime desde `BattleSetup.allyPrefabs`.
- Agregar, despedir, activar y desactivar mercenarios.
- Curar/restaurar recursos de todos los miembros vivos.
- Limpiar referencias visuales al salir de escenas de combate.
- Emitir el evento `PartyChanged` para que la UI se refresque.

### InventoryRuntimeState

Archivo: `Assets/Scripts/Items/InventoryRuntimeState.cs`

Responsabilidades:

- Guardar recursos basicos:
  - oro
  - comida
  - madera
  - piedra
  - hierro
  - cuero
  - cristales
- Guardar items por `itemId` y cantidad.
- Agregar, remover y consultar items.
- Gastar/agregar recursos.

### ItemDatabase

Archivo: `Assets/Scripts/Items/ItemDatabase.cs`

Responsabilidades:

- Registrar assets `ItemBase` asignados en el inspector.
- Construir un diccionario por `itemId`.
- Resolver items por ID.
- Devolver tipos concretos:
  - `Weapon`
  - `Armor`
  - `ConsumableItem`

## Sistema de combate

### BattleSetup

Archivo: `Assets/Scripts/Core/BattleSetup.cs`

Contenedor de configuracion de combate:

- prefabs aliados;
- posiciones aliadas;
- prefabs enemigos por defecto;
- posiciones enemigas;
- grupos de enemigos por `groupId`.

`BattleManager` usa `GetEnemyPrefabsForGroup` para elegir enemigos segun el nodo/evento que inicio el combate.

### BattleManager

Archivo: `Assets/Scripts/Core/BattleManager.cs`

Responsabilidades:

- Crear la batalla desde `BattleSetup`.
- Asociar unidades runtime con `UnitView`.
- Crear unidades enemigas desde prefabs con `UnitData`.
- Administrar turnos acumulativos.
- Ejecutar turnos automaticos.
- Resolver ataques basicos, habilidades, consumibles, estados y taunt.
- Detectar victoria/derrota.
- Volver a la escena de mapa al terminar.

#### Turnos acumulativos

Cada ronda:

1. Baja el taunt de unidades vivas (`tauntDecayPerRound`).
2. Cada unidad suma `speed` a `turnMeter`.
3. Cuando `turnMeter >= turnThreshold`, entra en cola de turno.
4. Se ordena por mayor `turnMeter` y luego por mayor `speed`.
5. La cola se limita por `maxTurnsPerRound`.

Valores configurables:

- `turnThreshold`
- `maxTurnsPerRound`
- `frontMaxTaunt`
- `backMaxTaunt`
- `tauntDecayPerRound`

#### Prioridad de accion

En el turno de una unidad:

1. Recalcula stats.
2. Procesa estados.
3. Si debe saltar turno, termina.
4. Intenta usar el primer consumible valido.
5. Busca habilidad segun tacticas o rotacion.
6. Si no hay habilidad, hace ataque basico.

### Unit

Archivo: `Assets/Scripts/Units/Unit.cs`

Modelo runtime de una unidad. No es `MonoBehaviour`.

Contiene:

- identidad (`unitName`);
- atributos base;
- nivel, experiencia, stamina, mana;
- HP, dano, velocidad, armaduras;
- equipo por slot;
- consumibles;
- habilidades;
- tacticas;
- estados;
- taunt;
- referencias a `UnitData` y `UnitView`.

Responsabilidades principales:

- Recalcular stats derivados.
- Aplicar equipo.
- Equipar y desequipar items.
- Gastar/restaurar stamina y mana.
- Elegir habilidad para el turno.
- Tomar dano y curarse.
- Restaurar armaduras.
- Procesar estados.
- Gestionar taunt.

### UnitData

Archivo: `Assets/Scripts/Units/UnitData.cs`

Componente de prefab que define los datos iniciales de una unidad:

- nombre, descripcion, icono, sprite de batalla;
- atributos;
- nivel y experiencia inicial;
- stamina y mana;
- armaduras base;
- HP/armadura inicial opcional;
- habilidades;
- tacticas;
- equipo inicial;
- consumibles iniciales.

`CreateUnit()` convierte esos datos de inspector en una instancia runtime de `Unit`.

### Habilidades

Archivo: `Assets/Scripts/Core/AbilitySO.cs`

`AbilitySO` es un `ScriptableObject` configurable desde el inspector.

Puede definir:

- objetivo: usuario, aliado o enemigo;
- modo: individual o todos;
- costo de stamina/mana;
- dano fisico/magico plano y escalado;
- curacion;
- restauracion de armadura fisica/magica;
- vampirismo;
- estados;
- generacion o modificacion de taunt.

El metodo `Execute` selecciona objetivos, cobra costos, aplica efectos y actualiza visuales.

### Tacticas

Archivo: `Assets/Scripts/Core/TacticRule.cs`

Cada `TacticRule` contiene:

- prioridad;
- si esta activa;
- condicion;
- porcentaje umbral;
- habilidad a usar.

Condiciones disponibles:

- siempre;
- HP propio bajo cierto porcentaje;
- HP de aliado bajo cierto porcentaje;
- HP de enemigo bajo cierto porcentaje;
- armadura fisica propia baja;
- armadura fisica de aliado baja;
- stamina propia sobre cierto porcentaje.

`Unit.GetAbilityForTurn` ordena reglas activas por prioridad y usa la primera que cumpla condicion y pueda pagarse.

### Estados

Archivo: `Assets/Scripts/StatusEffect.cs`

Estados actuales:

- `Poison`: hace dano por turno.
- `Regeneration`: cura por turno.
- `Stun`: marca `skipNextTurn`.
- `Invisibility`: reduce el maximo temporal de taunt.

### Consumibles

Archivos:

- `Assets/Scripts/Items/ConsumableItem.cs`
- `Assets/Scripts/Core/ConsumableResolver.cs`

Cada consumible define:

- condicion de uso;
- umbral de HP si aplica;
- tipo de efecto;
- valor;
- estado opcional.

Efectos actuales:

- curarse a si mismo;
- revivir un aliado;
- danar a todos los enemigos;
- aplicar estado al usuario;
- limpiar un estado del usuario.

`BattleManager` intenta usar consumibles antes de elegir habilidades.

## Sistema de items

### Jerarquia

Archivo base: `Assets/Scripts/Items/ItemBase.cs`

Tipos:

- `Weapon`
- `Armor`
- `ConsumableItem`
- `MaterialItem`

Enums principales: `Assets/Scripts/Items/ENUMS.cs`

- `ItemType`
- `EffectType`
- `StatType`
- `EquipmentSlot`
- `ConsumableSlot`
- `WeaponType`
- `ArmorType`

### Equipo

Archivo base: `Assets/Scripts/Items/EquipmentItem.cs`

Un item equipable tiene:

- slot;
- efectos;
- datos comunes heredados de `ItemBase`.

`Weapon` agrega rangos de dano fisico y magico.

`Armor` agrega armadura fisica y magica.

Cuando una unidad equipa/desequipa:

1. Se guarda el item por slot.
2. Se recalculan stats.
3. Se conserva la armadura actual de forma proporcional al cambio de maximo.

### Inventario

Archivo: `Assets/Scripts/Items/InventoryEntry.cs`

El inventario guarda pares:

- `itemId`
- `amount`

Los datos concretos del item se resuelven con `ItemDatabase`.

### Seeder de inventario

Archivo: `Assets/Scripts/Items/InventoryTestSeeder.cs`

Sirve para poblar inventario de prueba desde assets o IDs. Es util para escenas de test y desarrollo.

## Crafting

Archivo principal: `Assets/Scripts/Items/CraftingRecipeSO.cs`

Una receta define:

- `recipeId`
- nombre/descripicion/icono;
- estacion:
  - herrero;
  - cocina;
  - alquimista;
  - mago runico;
- item de preview;
- inputs;
- outputs.

Cada `RecipeStack` puede representar:

- item;
- oro;
- comida;
- madera;
- piedra;
- hierro;
- cuero;
- cristales.

UI relacionada:

- `Assets/Scripts/UI/CraftingPanelUI.cs`
- `Assets/Scripts/UI/RecipeRowUI.cs`
- `Assets/Scripts/UI/RecipeStackSlotUI.cs`

## Caravana

### CaravanState

Archivo: `Assets/Scripts/Caravan/CaravanState.cs`

Mantiene estado simple de viaje:

- dia;
- moral;
- stamina de caravana;
- carga actual/maxima.

### CaravanManager

Archivo: `Assets/Scripts/Caravan/CaravanManager.cs`

Responsabilidades:

- Cambiar secciones de UI:
  - Hoguera;
  - Barracas;
  - Suministros;
  - Artesanos.
- Refrescar textos de caravana, suministros y roster.
- Ejecutar acciones:
  - descanso parcial;
  - dormir;
  - reparar armadura fisica;
  - reparar armadura magica;
  - volver al mapa.
- Consumir recursos desde `InventoryRuntimeState`.
- Curar/restaurar party desde `PartyRuntimeState`.
- Construir una UI runtime si `buildGeneratedUI` esta activo.

Costos y porcentajes importantes:

- `restHealPercent`
- `armorRepairPercent`
- `partialRestFoodPerLivingMember`
- `partialRestWoodCost`
- `sleepFoodPerLivingMember`
- costos de reparacion fisica/magica.

## Mapa del mundo y eventos

### WorldMapNode

Archivo: `Assets/WorldMap/Nodes/WorldMapNode.cs`

Cada nodo contiene:

- `nodeId`
- `nodeName`
- descripcion;
- conexiones;
- estado visual:
  - desbloqueado;
  - visitado;
  - actual;
- configuracion de combate:
  - `isCombatNode`
  - `battleGroupId`
  - nodos a desbloquear por victoria
  - recompensa por victoria;
- configuracion de evento.

### WorldMapEvent

Archivo: `Assets/WorldMap/Connections/WorldMapEvent.cs`

Un evento contiene texto y opciones. Cada opcion puede:

- desbloquear nodos;
- agregar flags;
- completar o no el evento;
- iniciar combate;
- aplicar recompensa;
- volver al nodo inicial.

### EventPanelUI

Archivo: `Assets/WorldMap/Connections/EventPanelUI.cs`

Muestra el evento actual y genera botones para sus opciones. Al elegir opcion llama a `WorldMapManager.ResolveOption`.

## Recompensas

Archivos:

- `Assets/Scripts/Reward/RewardData.cs`
- `Assets/Scripts/Reward/RewardApplier.cs`

`RewardData` puede incluir:

- oro;
- comida;
- madera;
- hierro;
- cuero;
- items por ID y cantidad.

`RewardApplier.ApplyReward` aplica la recompensa sobre `InventoryRuntimeState`.

## UI

La UI esta repartida principalmente en `Assets/Scripts/UI/`.

Areas principales:

- Gestion de mercenarios:
  - `MercenaryManagementUI`
  - `MercenaryCardUI`
  - `MercenaryDetailPanelUI`
  - `RosterSelectionPanelUI`
- Equipo:
  - `EquipmentPanelUI`
  - `EquipmentSlotButtonUI`
  - `EquipmentItemRowUI`
  - `ItemDetailPanelUI`
- Crafting:
  - `CraftingPanelUI`
  - `RecipeRowUI`
  - `RecipeStackSlotUI`
- Suministros:
  - `SuppliesPanelUI`
  - `SuppliesItemRowUI`
  - `SuppliesResourcesPanelUI`
  - `SuppliesScrollViewSetup`
- Tacticas:
  - `TacticsPanelUI`
  - `TacticRuleRowUI`
  - `AbilityPickerModalUI`
  - `ConditionPickerModalUI`
  - `ConfirmActionModalUI`
- Combate:
  - `HealthBarUI`
  - `StatusIconUI`
  - `TurnOrderUI`
  - `FloatingCombatText`

## Herramientas de editor

Carpeta: `Assets/Editor/`

- `StarterItemCreator`: crea assets iniciales de items.
- `StarterAbilityCreator`: crea assets iniciales de habilidades.
- `EnemyPrefabGeneratorWindow`: ventana para generar prefabs de enemigos.

Estas herramientas ayudan a poblar contenido sin crear cada asset manualmente.

## Guia rapida para agregar contenido

### Agregar un item

1. Crear un asset desde el menu correspondiente:
   - `Items/Weapon`
   - `Items/Armor`
   - `Items/Consumable`
   - `Items/Material`
2. Completar `itemId`, `itemName`, icono y stats.
3. Registrar el asset en `ItemDatabase.items`.
4. Si debe aparecer en inventario inicial, agregarlo al seeder o a una recompensa.

### Agregar una habilidad

1. Crear asset desde `Abilities/GenericAbility`.
2. Configurar objetivo y modo.
3. Definir costos.
4. Configurar dano, cura, armadura, estado o taunt.
5. Asignarla en `UnitData.abilities` o en una `TacticRule`.

### Agregar una unidad

1. Crear o duplicar un prefab de aliado/enemigo.
2. Agregar/configurar `UnitData`.
3. Asignar stats, habilidades, tacticas, equipo y consumibles.
4. Verificar que tenga o pueda recibir `UnitView`.
5. Para aliados: asignar prefab en `BattleSetup.allyPrefabs`.
6. Para enemigos: asignar en `BattleSetup.enemyPrefabs` o en un `EnemyGroup`.

### Agregar un combate a un nodo

1. En el nodo, activar `isCombatNode`.
2. Definir `battleGroupId`.
3. En `BattleSetup.enemyGroups`, crear un grupo con el mismo `groupId`.
4. Asignar prefabs enemigos.
5. Opcional: definir `combatVictoryReward`.
6. Opcional: asignar nodos a `unlockOnCombatVictory`.

### Agregar un evento de mapa

1. En el nodo, activar `hasEvent`.
2. Configurar `nodeEvent`.
3. Agregar opciones.
4. En cada opcion, definir desbloqueos, flags, recompensa o combate.

### Agregar una receta

1. Crear asset `Caravan/Crafting Recipe`.
2. Elegir `stationType`.
3. Agregar inputs y outputs.
4. Si usa items por ID, asegurarse de que existan en `ItemDatabase`.
5. Vincular la receta con el panel/estacion correspondiente.

## Convenciones actuales

- Los IDs (`itemId`, `nodeId`, `groupId`) son strings y deben mantenerse unicos.
- El estado persistente de sesion vive en singletons `DontDestroyOnLoad`.
- Las unidades runtime son instancias de `Unit`, generadas desde `UnitData`.
- Los assets configurables usan `ScriptableObject`.
- La mayoria de sistemas se refrescan desde eventos o llamadas explicitas de UI.
- El combate esta pensado para resolverse de forma automatica.

## Riesgos o puntos a revisar

- Hay textos con caracteres mal codificados en algunos scripts (`InformaciÃƒÂ³n`, `DaÃƒÂ±o`, etc.). No rompe logica, pero conviene normalizar encoding a UTF-8.
- La persistencia actual es solo runtime; no hay guardado a disco.
- `GameRunState`, `PartyRuntimeState`, `InventoryRuntimeState` e `ItemDatabase` dependen de estar presentes en escena antes de usarse.
- Algunos sistemas tienen fallback por inspector, pero faltan validaciones visuales mas fuertes para prefabs incompletos.
- `WorldMapManager.ResolveOption` tiene un comentario pendiente para mejorar `returnToStartNode`.
- No se detectaron tests automatizados propios; existe el paquete de Unity Test Framework.

## Indice de archivos de codigo

### Core

- `Assets/Scripts/Core/AbilitySO.cs`
- `Assets/Scripts/Core/BattleManager.cs`
- `Assets/Scripts/Core/BattleSceneStarter.cs`
- `Assets/Scripts/Core/BattleSetup.cs`
- `Assets/Scripts/Core/ConsumableResolver.cs`
- `Assets/Scripts/Core/GameManager.cs`
- `Assets/Scripts/Core/PartyInitializer.cs`
- `Assets/Scripts/Core/PartyRuntimeState.cs`
- `Assets/Scripts/Core/TacticRule.cs`
- `Assets/Scripts/Core/TurnOrderUI.cs`

### Units

- `Assets/Scripts/Units/Unit.cs`
- `Assets/Scripts/Units/UnitData.cs`
- `Assets/Scripts/Units/UnitHealthBar.cs`
- `Assets/Scripts/Units/UnitView.cs`
- `Assets/Scripts/Units/StarterPartyPresetApplier.cs`

### Items

- `Assets/Scripts/Items/Armor.cs`
- `Assets/Scripts/Items/ConsumableItem.cs`
- `Assets/Scripts/Items/CraftingRecipeSO.cs`
- `Assets/Scripts/Items/ENUMS.cs`
- `Assets/Scripts/Items/EquipmentItem.cs`
- `Assets/Scripts/Items/InventoryEntry.cs`
- `Assets/Scripts/Items/InventoryRuntimeState.cs`
- `Assets/Scripts/Items/InventoryTestSeeder.cs`
- `Assets/Scripts/Items/ItemBase.cs`
- `Assets/Scripts/Items/ItemDatabase.cs`
- `Assets/Scripts/Items/ItemEffect.cs`
- `Assets/Scripts/Items/MaterialItem.cs`
- `Assets/Scripts/Items/Weapon.cs`

### Caravan

- `Assets/Scripts/Caravan/CaravanManager.cs`
- `Assets/Scripts/Caravan/CaravanState.cs`

### WorldMap

- `Assets/WorldMap/Managers/GameRunState.cs`
- `Assets/WorldMap/Managers/WorldMapManager.cs`
- `Assets/WorldMap/Nodes/WorldMapNode.cs`
- `Assets/WorldMap/Connections/EventPanelUI.cs`
- `Assets/WorldMap/Connections/PartySetup.cs`
- `Assets/WorldMap/Connections/WorldMapEvent.cs`

### Reward

- `Assets/Scripts/Reward/RewardApplier.cs`
- `Assets/Scripts/Reward/RewardData.cs`

### UI

- `Assets/Scripts/UI/AbilityPickerModalUI.cs`
- `Assets/Scripts/UI/ConditionPickerModalUI.cs`
- `Assets/Scripts/UI/ConfirmActionModalUI.cs`
- `Assets/Scripts/UI/CraftingPanelUI.cs`
- `Assets/Scripts/UI/EquipmentItemRowUI.cs`
- `Assets/Scripts/UI/EquipmentPanelUI.cs`
- `Assets/Scripts/UI/EquipmentSlotButtonUI.cs`
- `Assets/Scripts/UI/HealthBarUI.cs`
- `Assets/Scripts/UI/ItemDetailPanelUI.cs`
- `Assets/Scripts/UI/MercenaryCardUI.cs`
- `Assets/Scripts/UI/MercenaryDetailPanelUI.cs`
- `Assets/Scripts/UI/MercenaryManagementUI.cs`
- `Assets/Scripts/UI/RecipeRowUI.cs`
- `Assets/Scripts/UI/RecipeStackSlotUI.cs`
- `Assets/Scripts/UI/RosterSelectionPanelUI.cs`
- `Assets/Scripts/UI/StatusIconUI.cs`
- `Assets/Scripts/UI/SuppliesItemRowUI.cs`
- `Assets/Scripts/UI/SuppliesPanelUI.cs`
- `Assets/Scripts/UI/SuppliesResourcesPanelUI.cs`
- `Assets/Scripts/UI/SuppliesScrollViewSetup.cs`
- `Assets/Scripts/UI/TacticRuleRowUI.cs`
- `Assets/Scripts/UI/TacticsPanelUI.cs`

### Otros

- `Assets/Scripts/StatusEffect.cs`
- `Assets/Scripts/Utils/FloatingCombatText.cs`
- `Assets/Editor/EnemyPrefabGeneratorWindow.cs`
- `Assets/Editor/StarterAbilityCreator.cs`
- `Assets/Editor/StarterItemCreator.cs`


---

# Fuente: SCOPE_TEST_SIMPLIFICADO.md

# AbadyyRpg - Scope test simplificado

Estado: documento vivo de scope.
Objetivo: reducir la demo test a una expedicion Tier 1 con un solo personaje flexible, caravana como taller y combates que premian la preparacion.

---

## 1. Decision principal

La demo test simplificada no intenta probar el RPG completo.

La demo test intenta probar una sola idea:

```text
preparar bien la caravana cambia el resultado de una expedicion.
```

El jugador controla un solo personaje flexible.

La party de 4 queda congelada para una fase posterior, cuando el combate, las tacticas, el equipo y la economia esten bien probados.

---

## 2. Fantasia de juego

El jugador no gana por tener muchos personajes.

Gana porque:

- lee al enemigo;
- fabrica equipo adecuado;
- equipa habilidades utiles;
- configura tacticas;
- administra recursos;
- decide cuando gastar consumibles;
- vuelve mejor preparado.

La caravana funciona como base, taller y centro economico.

---

## 3. Sistemas activos en la demo test

La demo test usa:

- un personaje principal;
- equipo Tier 1;
- habilidades genericas equipables;
- tacticas;
- consumibles como parte de tacticas;
- crafting con recursos + oro;
- viaje por nodos de camino;
- eventos puntuales;
- combates repetibles;
- jefe Tier 1;
- recompensas de recursos;
- progresion de nivel.

---

## 4. Sistemas congelados

Estos sistemas pueden existir en codigo, pero no se usan como foco de la demo test:

- party de 4;
- reclutamiento;
- tienda dinamica;
- contratos;
- tiers 2 y 3;
- items especiales;
- economia compleja;
- generacion procedural;
- eventos aleatorios;
- muchos tipos de enemigos;
- balance de largo plazo.

Regla:

```text
Si no ayuda a probar la expedicion Tier 1, queda fuera.
```

---

## 5. Personaje unico flexible

El personaje puede convertirse temporalmente en distintas builds segun su equipo y habilidades.

No hay clases fijas para la demo test.

La identidad de build sale de:

- arma equipada;
- armadura equipada;
- habilidades equipadas;
- tacticas activas;
- consumibles disponibles.

La idea es mantener abierta la fantasia de:

```text
un mismo mercenario puede prepararse distinto para cada amenaza.
```

---

## 6. Escala numerica Tier 1

La demo usa numeros chicos.

Cada punto debe importar:

- 1 punto de dano importa;
- 1 punto de armadura importa;
- 1 punto de stamina importa;
- 1 punto de mana importa;
- una pocion que cura 4 HP importa.

Si el balance funciona, los numeros pueden multiplicarse mas adelante sin cambiar la logica.

---

## 7. Formulas base propuestas

```text
HP = constitucion * 2

Dano fisico minimo = fuerza
Dano fisico maximo = ceil(fuerza * 1.5)

Dano magico minimo = inteligencia
Dano magico maximo = ceil(inteligencia * 1.5)

Speed = 10 + destreza
```

---

## 8. Personaje base Tier 1

| Stat | Valor |
|---|---:|
| Fuerza | 2 |
| Destreza | 2 |
| Inteligencia | 2 |
| Constitucion | 5 |
| Stamina maxima | 10 |
| Mana maximo | 10 |
| Armadura fisica base | 0 |
| Armadura magica base | 0 |

Resultado sin equipo:

| Derivado | Valor |
|---|---:|
| HP | 10 |
| Dano fisico | 2-3 |
| Dano magico | 2-3 |
| Speed | 12 |
| Armadura fisica | 0 |
| Armadura magica | 0 |

---

## 9. Progresion por nivel

La subida de nivel mejora el personaje de forma general.

El equipo define la build.

| Nivel | Bonus |
|---|---|
| 1 | base |
| 2 | +1 fuerza, +1 constitucion, +1 stamina, +1 mana |
| 3 | +1 inteligencia, +1 destreza, +1 stamina, +1 mana |
| 4 | +1 fuerza, +1 inteligencia, +1 constitucion |
| 5 | +1 destreza, +1 stamina, +1 mana |

XP propuesta:

| Nivel | XP total requerida |
|---|---:|
| 1 | 0 |
| 2 | 40 |
| 3 | 100 |
| 4 | 180 |
| 5 | 300 |

---

## 10. Items Tier 1

El nivel da crecimiento general.

El equipo define la preparacion.

Cada item Tier 1 debe empujar una identidad clara con bonus chicos.

Reglas de items Tier 1:

- ningun item debe resolver todos los problemas;
- cada item tiene una funcion clara;
- los bonus de stats son chicos;
- las recetas usan recursos base + oro;
- el oro representa pago a artesanos.

### Armas Tier 1

#### Daga mellada

```text
ID: weapon_daga_mellada_t1
Tipo: arma
Slot: mano principal
Tier: 1
Dano fisico: 1-2
Dano magico: 0
Bonus: +1 destreza
Coste recomendado de habilidades fisicas: bajo
Uso: build rapida, economica y de desgaste.
Lectura: no pega fuerte, pero ayuda a actuar antes y gastar menos recursos.
Receta: 8 oro, 1 hierro, 1 cuero
```

#### Espada oxidada

```text
ID: weapon_espada_oxidada_t1
Tipo: arma
Slot: mano principal
Tier: 1
Dano fisico: 2-3
Dano magico: 0
Bonus: +1 fuerza
Coste recomendado de habilidades fisicas: medio
Uso: dano fisico estable.
Lectura: buena contra enemigos con poca armadura fisica o enemigos que conviene matar rapido por HP.
Receta: 12 oro, 2 hierro, 1 madera
```

#### Baston partido

```text
ID: weapon_baston_partido_t1
Tipo: arma
Slot: mano principal
Tier: 1
Dano fisico: 0
Dano magico: 2-3
Bonus: +1 inteligencia
Coste recomendado de habilidades magicas: medio
Uso: dano magico estable.
Lectura: respuesta clara contra enemigos con armadura fisica alta y armadura magica baja.
Receta: 12 oro, 2 madera, 1 cristal
```

#### Simbolo quebrado

```text
ID: weapon_simbolo_quebrado_t1
Tipo: arma
Slot: mano principal
Tier: 1
Dano fisico: 0
Dano magico: 1-2
Bonus: +1 constitucion
Coste recomendado de habilidades de soporte: medio
Uso: sustain, curacion y seguridad.
Lectura: baja el dano ofensivo, pero mejora supervivencia y builds defensivas.
Receta: 10 oro, 1 piedra, 1 cristal
```

### Armaduras Tier 1

#### Placas oxidadas

```text
ID: armor_placas_oxidadas_t1
Tipo: armadura
Slot: pecho
Tier: 1
Armadura fisica: +5
Armadura magica: +1
Bonus: +1 constitucion, -1 destreza
Uso: aguantar dano fisico.
Lectura: ideal contra rata gigante en grupo, esqueleto o enemigos que pegan fisico.
Debilidad: baja velocidad y no protege bien contra magia.
Receta: 16 oro, 3 hierro, 1 piedra
```

#### Cuero gastado

```text
ID: armor_cuero_gastado_t1
Tipo: armadura
Slot: pecho
Tier: 1
Armadura fisica: +3
Armadura magica: +1
Bonus: +1 destreza
Uso: velocidad y defensa fisica ligera.
Lectura: buena para peleas faciles o repetibles donde importa gastar poco y actuar rapido.
Debilidad: mala contra dano magico sostenido.
Receta: 12 oro, 3 cuero
```

#### Tunica rasgada

```text
ID: armor_tunica_rasgada_t1
Tipo: armadura
Slot: pecho
Tier: 1
Armadura fisica: +1
Armadura magica: +4
Bonus: +1 inteligencia
Uso: aguantar magia y potenciar build magica.
Lectura: respuesta clara contra demonio menor o enemigos que atacan armadura magica.
Debilidad: fragil contra golpes fisicos.
Receta: 14 oro, 2 cuero, 1 cristal
```

#### Vestidura remendada

```text
ID: armor_vestidura_remendada_t1
Tipo: armadura
Slot: pecho
Tier: 1
Armadura fisica: +2
Armadura magica: +3
Bonus: +1 constitucion
Uso: equilibrio y sustain.
Lectura: buena cuando no se conoce bien la amenaza o se quiere jugar a curacion/regeneracion.
Debilidad: no destaca contra amenazas extremas.
Receta: 14 oro, 1 cuero, 1 madera, 1 cristal
```

---

## 11. Consumibles Tier 1

Los consumibles no se usan automaticamente desde el inventario libre.

Para la demo actual se usan desde los 2 slots equipados y con las condiciones existentes del item.
Esto mantiene la preparacion previa sin abrir UI nueva de combate.

El sistema ideal futuro es usarlos como acciones elegibles dentro de tacticas.

Ejemplo:

```text
Si HP < 40% -> usar pocion de salud
```

Lectura de implementacion actual:

```text
Pocion: se consume si esta equipada y HP <= 40%.
Molotov: se consume al primer turno valido si esta equipada.
Pergamino: se consume automaticamente al caer.
```

### Pocion de salud

```text
ID: consumable_pocion_salud_t1
Tipo: consumible
Tier: 1
Efecto: cura 4 HP
Uso: emergencia.
Regla tactica esperada: si HP < 40% -> usar pocion.
Consume turno: si
Se consume al usar: si
Receta: 6 oro, 1 comida, 1 cristal
```

### Molotov

```text
ID: consumable_molotov_t1
Tipo: consumible
Tier: 1
Efecto: hace 3 dano fijo
Tipo de dano propuesto: mixto o fuego generico
Uso: opcion ofensiva sin depender de fuerza/inteligencia.
Regla tactica esperada: si enemigo HP < 35% -> usar molotov, o siempre contra enemigo peligroso.
Consume turno: si
Se consume al usar: si
Receta: 8 oro, 1 madera, 1 cuero, 1 cristal
```

Nota de implementacion futura:

```text
Si todavia no existe dano fuego, usar dano fijo fisico o dano fijo directo para la demo.
No crear un sistema nuevo de elementos solo por la molotov.
```

### Pergamino de revivir

```text
ID: consumable_pergamino_revivir_t1
Tipo: consumible
Tier: 1
Efecto: revive automaticamente con 30% de HP maximo al morir
Uso: seguro de emergencia.
Regla tactica esperada: no requiere regla activa; se dispara al caer.
Consume turno: no
Se consume al activar: si
Receta: 16 oro, 1 piedra, 2 cristal
```

Regla del pergamino:

```text
Si el personaje cae y tiene pergamino equipado/disponible:
- revive automaticamente con 30% de HP maximo;
- consume 1 pergamino;
- continua el combate.
```

---

## 12. Enemigos Tier 1

Enemigos definidos para la demo test:

- rata gigante;
- esqueleto;
- gusano;
- demonio menor.

Tabla inicial:

| Enemigo | Rol | Nivel | HP | Armadura fisica | Armadura magica | Dano principal |
|---|---|---:|---:|---:|---:|---|
| Rata gigante | debil | 1 | 7 | 1 | 0 | fisico 1-2 |
| Esqueleto | medio | 2 | 11 | 5 | 1 | fisico 2-3 |
| Gusano | peligroso | 3 | 16 | 3 | 4 | fisico 3-5 |
| Demonio menor | jefe Tier 1 | 4 | 28 | 5 | 8 | magico 4-6 |

### Rata gigante

```text
ID: enemy_rata_gigante_t1
Rol: enemigo debil / desgaste
Nivel: 1
HP: 7
Stamina: 6
Mana: 0
Fuerza: 1
Destreza: 3
Inteligencia: 0
Constitucion: 4
Armadura fisica: 1
Armadura magica: 0
Speed esperado: 13
```

Habilidades:

```text
Mordida
- Tipo: fisico
- Dano: 1-2
- Coste: 1 stamina
- Funcion: ataque basico de desgaste.

Roer
- Tipo: fisico
- Dano: 1
- Coste: 1 stamina
- Efecto: baja 1 armadura fisica actual si existe soporte tecnico.
- Fallback si no existe: dano fisico 1.
```

Tacticas:

```text
1. Si tiene stamina -> Mordida
2. Siempre -> ataque basico
```

Recompensa:

```text
XP: 8
Oro: 3
Comida: 1
Cuero: 0-1
Cristal: 0
Uso economico: recompensa chica para sostener pociones y primeras recetas.
```

Encuentros sugeridos:

```text
1 rata = tutorial.
2 ratas = desgaste real.
3 ratas = peligro si el jugador no preparo curacion.
```

### Esqueleto

```text
ID: enemy_esqueleto_t1
Rol: enemigo medio / ensena armadura fisica
Nivel: 2
HP: 11
Stamina: 8
Mana: 0
Fuerza: 2
Destreza: 1
Inteligencia: 0
Constitucion: 6
Armadura fisica: 5
Armadura magica: 1
Speed esperado: 11
```

Habilidades:

```text
Golpe Oxidado
- Tipo: fisico
- Dano: 2-3
- Coste: 2 stamina
- Funcion: amenaza fisica media.

Guardia Osea
- Tipo: defensa
- Coste: 2 stamina
- Efecto: recupera 2 armadura fisica.
- Fallback si no existe: no usar todavia.
```

Tacticas:

```text
1. Si armadura fisica < 40% -> Guardia Osea
2. Siempre -> Golpe Oxidado
```

Recompensa:

```text
XP: 16
Oro: 6
Piedra: 1
Hierro: 1
Cristal: 0
Uso economico: empuja crafting de armas/armaduras fisicas.
```

Encuentros sugeridos:

```text
1 esqueleto = test de baston/misil magico.
1 esqueleto + 1 rata = prueba de prioridad y desgaste.
2 esqueletos = pelea dura, no usar temprano.
```

### Gusano

```text
ID: enemy_gusano_t1
Rol: enemigo peligroso / sostenimiento
Nivel: 3
HP: 16
Stamina: 10
Mana: 0
Fuerza: 3
Destreza: 1
Inteligencia: 1
Constitucion: 8
Armadura fisica: 3
Armadura magica: 4
Speed esperado: 11
```

Habilidades:

```text
Mordida Profunda
- Tipo: fisico
- Dano: 3-5
- Coste: 3 stamina
- Funcion: castiga entrar sin armadura fisica.

Corrosion
- Tipo: debuff
- Dano: 1
- Coste: 2 stamina
- Efecto: baja ataque fisico del jugador o aplica dano por turno leve.
- Fallback si no existe debuff/dot: dano fisico 2.
```

Tacticas:

```text
1. Si tiene stamina >= 3 -> Mordida Profunda
2. Si HP del jugador > 50% -> Corrosion
3. Siempre -> ataque basico
```

Recompensa:

```text
XP: 28
Oro: 10
Comida: 1
Cuero: 1
Cristal: 1
Uso economico: recompensa buena para reponer consumibles y preparar jefe.
```

Encuentros sugeridos:

```text
1 gusano = mini-check de preparacion.
1 gusano + 1 rata = pelea peligrosa.
2 gusanos = no usar en Tier 1 salvo desafio opcional.
```

### Demonio menor

```text
ID: enemy_demonio_menor_t1
Rol: jefe Tier 1 / examen de preparacion
Nivel: 4
HP: 28
Stamina: 8
Mana: 14
Fuerza: 2
Destreza: 2
Inteligencia: 4
Constitucion: 14
Armadura fisica: 5
Armadura magica: 8
Speed esperado: 12
```

Habilidades:

```text
Latigo Infernal
- Tipo: magico
- Dano: 4-6
- Coste: 3 mana
- Funcion: castiga entrar sin armadura magica.

Garra
- Tipo: fisico
- Dano: 2-3
- Coste: 2 stamina
- Funcion: evita que la tunica sea una respuesta perfecta.

Rugido Menor
- Tipo: debuff
- Coste: 3 mana
- Efecto: baja ataque fisico del jugador por pocos turnos.
- Fallback si no existe debuff: Latigo Infernal.
```

Tacticas:

```text
1. Si mana >= 3 -> Latigo Infernal
2. Si jugador usa build fisica y existe soporte tecnico -> Rugido Menor
3. Si stamina >= 2 -> Garra
4. Siempre -> ataque basico
```

Recompensa:

```text
XP: 60
Oro: 25
Cristal: 3
Hierro: 1
Cuero: 1
Flag: frontera_abierta
Uso economico: cierre de demo, recompensa para indicar progreso posterior.
```

Encuentro sugerido:

```text
1 demonio menor solo.
No acompanar con adds en la primera version.
La dificultad debe venir de preparar mal o bien, no de saturar la pantalla.
```

---

## 13. Lectura de counterplay

Cada enemigo debe tener una lectura clara.

### Rata gigante

```text
Amenaza: dano fisico bajo, puede aparecer en grupo.
Defensa: casi sin armadura.
Respuesta: cualquier build sirve, pero gastar demasiados recursos contra ratas es mala economia.
Objetivo de diseno: ensenar combate basico y desgaste.
```

### Esqueleto

```text
Amenaza: dano fisico medio.
Defensa: armadura fisica alta, armadura magica baja.
Respuesta: armadura fisica para aguantar + baston/misil magico para atravesarlo mejor.
Objetivo de diseno: ensenar que pegar fisico contra armadura fisica es ineficiente.
```

### Gusano

```text
Amenaza: dano peligroso y pelea mas larga.
Defensa: defensas mixtas.
Respuesta: preparacion equilibrada, pocion, regeneracion o debuff ofensivo.
Objetivo de diseno: ensenar sostenimiento y tacticas defensivas.
```

### Demonio menor

```text
Amenaza: dano magico alto y presion sostenida.
Defensa: armadura magica alta, armadura fisica media.
Respuesta: tunica o vestidura para aguantar magia + espada/ataque poderoso para presionar por fisico.
Objetivo de diseno: examen final de preparacion Tier 1.
```

---

## 14. Habilidades genericas

Pendiente de definir.

La idea es que las habilidades no pertenezcan todavia a clases fijas.

El personaje puede equipar habilidades genericas y combinarlas con su equipo.

Primeras funciones candidatas:

- dano fisico alto con coste de stamina;
- dano magico medio con coste de mana;
- curacion directa;
- regeneracion;
- reducir ataque enemigo;
- restaurar armadura magica;
- defensa fisica o guardia;
- uso de consumibles desde tacticas.

Regla:

```text
No agregar habilidades porque suenan lindas.
Agregar habilidades porque un enemigo o una decision de preparacion las necesita.
```

---

## 15. Tacticas

Pendiente de definir.

La demo conserva la idea de 5 espacios de tacticas, pero no necesita exigir que el jugador use los 5 desde el inicio.

Ejemplos de reglas:

```text
Si HP < 40% -> usar pocion
Si HP < 50% -> curar
Si enemigo tiene mucha armadura fisica -> misil magico
Si enemigo pega fuerte -> romperbrazo
Siempre -> ataque poderoso
```

Los consumibles dejan de ser automaticos.

Se usan como parte de las tacticas.

---

## 16. Crafting

La demo no usa tienda.

La demo no usa contratos.

La caravana fabrica usando:

- oro;
- comida;
- madera;
- piedra;
- hierro;
- cuero;
- cristal.

El oro representa pago a artesanos, mano de obra, mantenimiento y herramientas menores.

### Recetas activas Tier 1

| Item | Oro | Comida | Madera | Piedra | Hierro | Cuero | Cristal |
|---|---:|---:|---:|---:|---:|---:|---:|
| Daga mellada | 8 | 0 | 0 | 0 | 1 | 1 | 0 |
| Espada oxidada | 12 | 0 | 1 | 0 | 2 | 0 | 0 |
| Baston partido | 12 | 0 | 2 | 0 | 0 | 0 | 1 |
| Simbolo quebrado | 10 | 0 | 0 | 1 | 0 | 0 | 1 |
| Placas oxidadas | 16 | 0 | 0 | 1 | 3 | 0 | 0 |
| Cuero gastado | 12 | 0 | 0 | 0 | 0 | 3 | 0 |
| Tunica rasgada | 14 | 0 | 0 | 0 | 0 | 2 | 1 |
| Vestidura remendada | 14 | 0 | 1 | 0 | 0 | 1 | 1 |
| Pocion de salud | 6 | 1 | 0 | 0 | 0 | 0 | 1 |
| Molotov | 8 | 0 | 1 | 0 | 0 | 1 | 1 |
| Pergamino de revivir | 16 | 0 | 0 | 1 | 0 | 0 | 2 |

### Regla de venta

Pendiente de definir con cuidado.

Propuesta inicial:

```text
Vender un item devuelve oro, pero no recursos.
El valor de venta debe ser menor que el coste de fabricacion.
```

Valores tentativos:

| Item | Venta oro |
|---|---:|
| Armas Tier 1 | 5-8 |
| Armaduras Tier 1 | 7-10 |
| Pocion de salud | 3 |
| Molotov | 4 |
| Pergamino de revivir | 8 |

Nota:

```text
La venta existe para sostener la expedicion, no para crear una economia infinita.
Si vender/craftear genera ganancia segura, rompe la demo.
```

---

## 17. Mapa y eventos

La demo usa una mini zona cerrada.

No intenta representar todo el mapa.

El objetivo del mapa es probar este loop:

```text
viajar -> leer obstaculo -> preparar/craftear -> combatir -> desbloquear ruta -> vencer jefe
```

### Nodos a usar

| ID | Nombre visible | Tipo | Funcion |
|---|---|---|---|
| node_valdoran | Ciudad de Valdoran | inicio/caravana | base, crafting, preparacion |
| node_camino_01 | Camino a Claravalle I | camino | movimiento inicial |
| node_camino_02 | Camino a Claravalle II | camino | movimiento inicial |
| node_puente_roto | Puente roto | evento | primer bloqueo real |
| node_cruce_central | Cruce del Vigia | cruce | abre ramas de la demo |
| node_bosque_aldheron | Bosque de Aldheron | combate repetible | farm temprano con ratas/esqueletos |
| node_claro_rocas | Claro de las Rocas | combate repetible | farm medio con esqueletos/gusano |
| node_torre_vigia | Torre vigia abandonada | evento | encuentra pista del jefe |
| node_mina_hierro | Mina de Hierro Negro | combate repetible | farm de hierro/cristal |
| node_collinasombra | Aldea de Collinasombra | descanso/evento menor | punto previo al jefe |
| node_portal_excavadores | Portal de los Excavadores | jefe | demonio menor |
| node_frontera_abierta | Frontera abierta | final | cierre de demo |

Nota:

```text
Estos son nodos logicos.
No todos necesitan una UI especial.
Los nodos de camino pueden ser simples paradas que consumen tiempo/stamina y muestran estado de ruta.
```

### Caminos de conexion

La demo usa 10 caminos/tramos.

| ID | Desde | Hacia | Estado inicial | Desbloqueo | Funcion |
|---|---|---|---|---|---|
| road_01 | node_valdoran | node_camino_01 | abierto | ninguno | salida segura |
| road_02 | node_camino_01 | node_camino_02 | abierto | ninguno | viaje inicial |
| road_03 | node_camino_02 | node_puente_roto | abierto | ninguno | llegada al obstaculo |
| road_04 | node_puente_roto | node_cruce_central | bloqueado | flag_puente_reparado | primera prueba de recursos |
| road_05 | node_cruce_central | node_bosque_aldheron | abierto | flag_puente_reparado | ruta de farm facil |
| road_06 | node_bosque_aldheron | node_claro_rocas | abierto | flag_puente_reparado | farm medio |
| road_07 | node_cruce_central | node_torre_vigia | abierto | flag_puente_reparado | ruta de pista |
| road_08 | node_torre_vigia | node_mina_hierro | abierto | flag_pista_jefe_encontrada | farm de preparacion final |
| road_09 | node_cruce_central | node_collinasombra | bloqueado | flag_pista_jefe_encontrada | acceso al jefe |
| road_10 | node_collinasombra | node_portal_excavadores | bloqueado | flag_ruta_jefe_desbloqueada | combate final |

### Estados visuales de caminos

Los caminos deben poder comunicar estado sin texto largo.

| Estado | Color sugerido | Significado |
|---|---|---|
| oculto | gris oscuro/transparente | todavia no descubierto |
| abierto | dorado suave | se puede viajar |
| bloqueado | rojo oscuro | existe, pero falta condicion |
| completado | dorado fuerte o blanco calido | ya fue recorrido |
| peligroso | naranja | combate/recompensa relevante |

Regla:

```text
Si una ruta esta bloqueada, el jugador debe entender por que.
No usar bloqueos mudos.
```

### Eventos principales

#### Evento 1 - Puente roto

```text
ID: event_puente_roto_t1
Nodo: node_puente_roto
Tipo: evento de camino
Objetivo: ensenar que algunos obstaculos se resuelven con recursos, no con combate.
Estado inicial: activo
```

Texto funcional:

```text
El puente cruje bajo el peso de la caravana.
Cruzarlo sin repararlo puede dejar la expedicion varada.
```

Opciones:

| Opcion | Requisito | Resultado |
|---|---|---|
| Reparar el puente | 1 madera, 1 hierro, 2 stamina, 2 horas | activa flag_puente_reparado, abre road_04 |
| Forzar el cruce | sin coste de recursos | pierde 3 stamina, 1 comida, abre road_04 igual |
| Volver a Valdoran | ninguno | no cambia flags |

Lectura:

```text
Reparar es mejor si el jugador preparo recursos.
Forzar existe para no bloquear la demo, pero debe doler.
```

#### Evento 2 - Pista del jefe

```text
ID: event_pista_jefe_t1
Nodo: node_torre_vigia
Tipo: evento de descubrimiento
Objetivo: desbloquear la ubicacion del jefe.
Estado inicial: disponible despues de reparar el puente.
```

Texto funcional:

```text
Desde la torre se ven marcas recientes hacia las montanas.
Alguien esta usando el viejo portal como refugio.
```

Opciones:

| Opcion | Requisito | Resultado |
|---|---|---|
| Revisar la torre | 1 hora | activa flag_pista_jefe_encontrada, revela road_08 y desbloquea road_09 |
| Saquear suministros | 1 hora | +1 madera, +1 piedra, no activa pista |
| Irse | ninguno | no cambia flags |

Lectura:

```text
El jugador puede tomar recursos, pero la opcion importante es encontrar la pista.
Si saquea primero, debe poder volver y revisar despues.
```

#### Evento 3 - Aldea de Collinasombra

```text
ID: event_collinasombra_prejefe_t1
Nodo: node_collinasombra
Tipo: evento menor / advertencia
Objetivo: avisar que el jefe pega magico y permitir preparacion final.
Estado inicial: disponible con flag_pista_jefe_encontrada.
```

Texto funcional:

```text
Los aldeanos hablan de fuego azul en las ruinas.
Las heridas no parecen hechas por acero.
```

Opciones:

| Opcion | Requisito | Resultado |
|---|---|---|
| Escuchar advertencias | ninguno | activa flag_ruta_jefe_desbloqueada, road_10 abierto |
| Pedir ayuda menor | 5 oro | +1 comida |
| Volver a preparar | ninguno | no cambia flags |

Lectura:

```text
Este evento existe para comunicar counterplay.
No debe ser un bloqueo caro antes del jefe.
```

#### Evento 4 - Demonio menor

```text
ID: event_jefe_demonio_menor_t1
Nodo: node_portal_excavadores
Tipo: combate jefe
Objetivo: examen final Tier 1.
Estado inicial: bloqueado por flag_ruta_jefe_desbloqueada.
Encuentro: 1 demonio menor.
```

Resultado:

| Resultado | Efecto |
|---|---|
| Victoria | activa flag_jefe_derrotado, activa flag_frontera_abierta, desbloquea node_frontera_abierta |
| Derrota | vuelve a Valdoran, mantiene progreso salvo consumibles gastados |

Lectura:

```text
El jefe debe ser dificil si el jugador llega con equipo incorrecto.
Debe ser ganable si llega con armadura magica, consumibles y tacticas razonables.
```

### Flags de demo

| Flag | Se activa en | Efecto |
|---|---|---|
| flag_demo_iniciada | inicio de run | habilita nodos iniciales |
| flag_puente_reparado | event_puente_roto_t1 | abre road_04 y cruce central |
| flag_pista_jefe_encontrada | event_pista_jefe_t1 | desbloquea ruta hacia Collinasombra y Mina |
| flag_ruta_jefe_desbloqueada | event_collinasombra_prejefe_t1 | abre road_10 |
| flag_jefe_derrotado | event_jefe_demonio_menor_t1 | marca victoria de combate |
| flag_frontera_abierta | event_jefe_demonio_menor_t1 | cierre de demo |

### Combates repetibles por nodo

| Nodo | Encuentros posibles | Uso |
|---|---|---|
| node_bosque_aldheron | 1 rata, 2 ratas, 1 rata + 1 esqueleto | XP y recursos basicos |
| node_claro_rocas | 1 esqueleto, 1 esqueleto + 1 rata, 1 gusano | test de build fisica/magica |
| node_mina_hierro | 1 esqueleto + 1 gusano, 1 gusano | hierro, cristal, preparacion jefe |

Regla de repetibles:

```text
Los repetibles son para prepararse, no para grind infinito perfecto.
Si hay recompensas, debe haber coste de tiempo/stamina/consumibles.
```

### Ruta critica esperada

```text
Valdoran
-> Camino 01
-> Camino 02
-> Puente roto
-> reparar o forzar
-> Cruce del Vigia
-> Bosque/Claro para levear y conseguir recursos
-> Torre Vigia para encontrar pista
-> Mina opcional para preparar equipo
-> Collinasombra para advertencia
-> Portal de los Excavadores
-> Demonio menor
-> Frontera abierta
```

### Limite de scope del mapa

No agregar todavia:

- eventos aleatorios;
- mas jefes;
- reputacion de pueblos;
- economia local;
- multiples finales;
- rutas secretas;
- decisiones permanentes complejas.

El mapa existe para validar preparacion, no para simular una region completa.

---

## 18. Recompensas

Las recompensas deben alimentar el loop de preparacion.

No deben abrir sistemas nuevos.

Recompensas activas:

- oro;
- comida;
- madera;
- piedra;
- hierro;
- cuero;
- cristal;
- XP;
- ingredientes para consumibles;
- ingredientes para equipo Tier 1.

Regla:

```text
Siempre conviene luchar optimamente.
Buscar recursos existe como ultima opcion, no como estrategia principal.
```

### Buscar recursos

Accion disponible para evitar bloqueos duros si el jugador queda corto de materiales.

```text
Costo:
- 4 horas
- 3 stamina del personaje

Resultado:
- 1 recurso aleatorio basico o 1 comida
- 0 XP
```

Lectura:

```text
Es una red de seguridad.
Si el jugador la usa demasiado, pierde tiempo de expedicion y llega peor al jefe.
```

### Recompensa y desgaste

La demo no mide solo si el jugador gana.

Tambien mide cuanto costo ganar.

```text
Ganar un combate mal puede consumir:
- HP;
- stamina;
- mana;
- consumibles;
- armadura;
- horas;
- recursos de reparacion;
- dias de expedicion.
```

Frase de diseno:

```text
El combate no solo se gana o se pierde.
Tambien se mide por cuanto costo ganarlo.
```

---

## 19. Tiempo, descanso y derrota

La demo tiene limite de tiempo.

```text
Si empieza el dia 11 y flag_jefe_derrotado es falso:
- la expedicion fracasa.
```

Esto permite farmear, pero no permite farmear infinito.

El jugador tiene 10 dias para:

- aprender enemigos;
- conseguir recursos;
- craftear equipo;
- preparar consumibles;
- reparar armaduras;
- derrotar al jefe.

### Descanso parcial

Uso:

```text
Fuera de combate.
En ruta o campamento.
```

Costo:

```text
- stamina de caravana
- comida
- madera/lena
- horas
```

Efecto:

```text
- recupera 20% HP
- recupera 20% stamina
- recupera 20% mana
```

Lectura:

```text
Sirve para estirar una expedicion.
No borra gratis los errores de combate.
```

### Descanso completo en caravana

Uso:

```text
Solo en Valdoran/caravana.
```

Costo:

```text
- avanza al dia siguiente
```

Efecto:

```text
- recupera 100% HP
- recupera 100% stamina
- recupera 100% mana
```

Lectura:

```text
Curar todo esta permitido porque el costo real es perder un dia.
Con limite de 10 dias, dormir no es gratis.
```

### Reparar armaduras

Uso:

```text
En caravana.
```

Costo:

```text
- horas
- pocos recursos segun tipo de armadura
```

Efecto:

```text
- restaura armadura fisica/magica del equipo
```

Lectura:

```text
No optimizar un combate reduce la recompensa real.
Si el jugador recibe demasiado dano, paga despues con tiempo y recursos.
```

---

## 20. Regla de balance

Si una pelea se siente injusta, primero revisar:

1. si el enemigo comunica bien su amenaza;
2. si existe una respuesta clara en Tier 1;
3. si el jugador puede craftear esa respuesta;
4. si el coste de recursos permite prepararse.

No subir numeros grandes para arreglar confusion.

Primero arreglar lectura y preparacion.

---

## 21. Frase guia

```text
No estamos haciendo el RPG completo.
Estamos demostrando que preparar la caravana cambia el resultado de una expedicion.
```


---

# Fuente: scope_demo_test_tier1.md

# AbadyyRpg - Scope demo test Tier 1

Estado: documento de recorte de scope.
Objetivo: cerrar una demo test jugable sin intentar construir todavia el juego completo.

---

## 1. Decision principal

La demo test no intenta probar todo el RPG.

La demo test intenta probar una sola expedicion:

```text
preparar caravana -> viajar -> gastar recursos -> combatir -> conseguir recursos -> volver a preparar -> vencer jefe -> abrir frontera
```

El juego completo puede tener tienda, contratos, economia dinamica, mas tiers, mas enemigos y mas rutas.

La demo test no.

---

## 2. Principio de scope

```text
Una expedicion buena antes que un mundo grande.
```

Todo lo que no ayude directamente a probar esa expedicion queda fuera del scope activo, aunque ya exista codigo parcial.

No se borra necesariamente.
No se balancea.
No se pule.
No se usa en la demo test.

---

## 3. Fantasia economica de la demo

La caravana es el centro de preparacion.

No hay tienda activa.
No hay contratos activos.

Los artesanos de la caravana fabrican equipo y consumibles usando:

- oro como pago/mano de obra;
- comida;
- madera;
- piedra;
- hierro;
- cuero;
- cristal.

El oro no representa un material fisico. Representa pago a artesanos, herramientas menores, mantenimiento y trabajo especializado.

---

## 4. Loop activo

### 4.1 Preparacion

El jugador entra a la caravana y decide que fabricar con los recursos disponibles.

Debe poder fabricar:

- armas tier 1;
- armaduras tier 1;
- pocion;
- molotov;
- pergamino de revivir.

### 4.2 Viaje

El jugador viaja por nodos de camino.

Los nodos de camino no son contenido especial. Son distancia, coste y ritmo.

Cada nodo de camino puede consumir:

- stamina de caravana;
- horas.

### 4.3 Eventos

Los eventos deben ser pocos y funcionales.

Eventos de la demo test:

- reparar puente;
- encontrar informacion sobre el jefe bandido;
- abrir frontera despues del jefe.

### 4.4 Combates

Los combates deben ensenar lectura y preparacion, no solo gastar tiempo.

La demo test usa pocos enemigos repetidos para probar counterplay.

### 4.5 Recompensa

Los combates y eventos entregan recursos para volver a preparar la caravana.

La recompensa debe alimentar el crafting, no abrir nuevos sistemas.

---

## 5. Mapa de demo test

El mapa no necesita muchos eventos.

Estructura propuesta:

```text
Pueblo
  |
Camino 1
  |
Camino 2
  |
Camino 3
  |
Evento: puente roto
  |
Cruce post-puente
 /        |        \
Bosque   Norte    Este bloqueado
 |        |        |
Encuentros repetibles
          |
Evento: pista del jefe
          |
Desbloquea camino este
          |
Jefe bandido
          |
Evento: abrir frontera
```

Los nodos de camino existen para:

- dar sensacion de viaje;
- consumir tiempo/stamina;
- separar decisiones;
- ubicar zonas de peligro.

No deben tener UI o logica especial si no hace falta.

---

## 6. Contenido incluido

### 6.1 Party

Cuatro mercenarios fijos:

- Defensor;
- Asesino;
- Mago del Circulo;
- Acolita.

No hay reclutamiento en la demo test.
No hay rotacion amplia de roster.

### 6.2 Items craftables

Armas tier 1:

- espada oxidada;
- daga mellada;
- baston partido;
- simbolo quebrado.

Armaduras tier 1:

- placas oxidadas;
- cuero gastado;
- tunica rasgada;
- vestidura remendada.

Consumibles:

- pocion de salud;
- molotov;
- pergamino de revivir.

Accesorios:

- opcionales.

Si entran, maximo dos. No cuatro, salvo que exista una necesidad concreta de balance.

### 6.3 Enemigos

Enemigos normales:

- rata: enemigo debil, prueba dano basico y consumo de recursos;
- esqueleto: armadura fisica alta, ensena romper armadura o preparar arma adecuada;
- demonio/cultista: dano o armadura magica, ensena proteccion magica y curacion.

Jefe:

- bandido fuerte.

El jefe debe combinar lo aprendido:

- dano fisico peligroso;
- armadura fisica;
- presion sobre la stamina/curacion;
- recompensa de cierre.

---

## 7. Sistemas activos

Estos sistemas si se usan en la demo test:

- caravana;
- crafting basico;
- inventario de recursos;
- equipo de mercenarios;
- consumibles;
- nodos de camino;
- eventos puntuales;
- combates;
- recompensas;
- flags minimos para desbloquear jefe/frontera.

---

## 8. Sistemas congelados

Estos sistemas pueden existir en codigo, pero quedan fuera de la demo test:

- tienda;
- contratos;
- economia dinamica;
- multiples contratos simultaneos;
- tiers 2 y 3;
- items especiales;
- accesorios amplios;
- generacion procedural;
- eventos aleatorios;
- muchas ramas de mapa;
- balance fino de largo plazo;
- narrativa extensa;
- mas clases;
- reclutamiento avanzado.

Regla:

```text
Si una feature no ayuda a probar la expedicion tier 1, no entra.
```

---

## 9. Criterio de exito

La demo test funciona si el jugador puede:

1. ver su party;
2. fabricar 1 o 2 preparaciones utiles;
3. viajar hasta el puente;
4. resolver el puente;
5. entrar a una zona de encuentros repetibles;
6. conseguir recursos;
7. volver/prepararse mejor;
8. encontrar la pista del jefe;
9. vencer al jefe bandido;
10. abrir la frontera.

No hace falta que el juego sea grande.
Hace falta que el loop se entienda y tenga tension.

---

## 10. Reglas anti-scope creep

Antes de agregar algo, preguntar:

```text
Esto hace mas clara la expedicion tier 1?
Esto mejora la preparacion antes del combate?
Esto ayuda a entender un enemigo o una decision?
Esto reduce confusion?
```

Si la respuesta es no, va a despues.

Tambien preguntar:

```text
Esto pide UI nueva?
Esto pide balance nuevo?
Esto pide contenido adicional para justificarlo?
Esto abre otra tabla de datos?
```

Si la respuesta es si, probablemente no entra ahora.

---

## 11. Prioridad tecnica inmediata

Orden sugerido:

1. Congelar datos tier 1.
2. Simplificar crafting a recetas tier 1.
3. Desactivar tienda/contratos de la demo test.
4. Reducir mapa a expedicion de prueba.
5. Crear encuentros repetibles del bosque.
6. Crear evento de pista del jefe.
7. Bloquear/desbloquear jefe por flag.
8. Crear evento de abrir frontera.
9. Probar una run completa.
10. Balancear solo lo que bloquee la run.

---

## 12. Frase guia

```text
No estamos haciendo el RPG completo.
Estamos demostrando que preparar la caravana cambia el resultado de una expedicion.
```


---

# Fuente: LISTA_TAREAS_DEMO.md

# AbadyyRpg - Lista de tareas demo Tier 1

Estado: hoja de ruta de implementacion.
Base de diseno: `SCOPE_TEST_SIMPLIFICADO.md`.

Objetivo:

```text
Convertir el scope simplificado en una demo jugable de una expedicion Tier 1.
```

La demo prueba una idea:

```text
preparar bien la caravana cambia el resultado de una expedicion.
```

---

## 1. Alinear datos base de la demo

- [x] Definir recursos iniciales reales.
- [x] Definir equipo inicial real.
- [x] Definir si el personaje arranca con habilidades base o debe desbloquearlas/craftearlas.
- [x] Configurar party activa con 1 solo personaje.
- [x] Congelar party de 4 para esta demo.
- [x] Ajustar stats base del personaje al scope chico.
- [x] Ajustar XP/levels al ritmo de 10 dias.

Notas:

- El personaje base arranca con habilidades genericas para probar builds desde equipo/tacticas sin agregar desbloqueos.
- XP demo: nivel 1 a 5 con umbrales `0/40/100/180/300`.
- Crecimiento demo por tabla: nivel 2 fuerza/constitucion, nivel 3 destreza/inteligencia, nivel 4 fuerza/inteligencia/constitucion, nivel 5 destreza.

---

## 2. Completar habilidades genericas

- [x] Definir numeros iniciales de cada habilidad.
- [x] Crear/ajustar `AbilitySO` de ataque poderoso.
- [x] Crear/ajustar `AbilitySO` de misil magico.
- [x] Crear/ajustar `AbilitySO` de curar.
- [x] Crear/ajustar `AbilitySO` de regeneracion.
- [x] Crear/ajustar `AbilitySO` de cleanse.
- [x] Crear/ajustar `AbilitySO` de rompebrazo.
- [x] Crear/ajustar `AbilitySO` de guardia.
- [x] Crear/ajustar `AbilitySO` de escudo magico.
- [x] Revisar que habilidades son posibles con el sistema actual.
- [x] Marcar habilidades que requieren codigo nuevo.
- [x] Asignar habilidades genericas al personaje base actual.
- [ ] Probar habilidades genericas en combate.

Notas:

- `cleanse` limpia estados negativos desde `AbilitySO`: veneno, stun y reduccion de ataque.
- `rompebrazo` aplica `AttackDown`, reduciendo dano fisico y magico por valor plano mientras dura.

---

## 3. Alinear consumibles

- [x] Pocion de salud cura 4 HP.
- [x] Molotov hace 3 dano fijo o magico/directo.
- [x] Pergamino revive automaticamente con 30% HP.
- [x] Decidir si consumibles usan sistema actual de condiciones o tacticas.
- [x] Ajustar uso automatico para que no contradiga el diseno.
- [x] Confirmar consumo real del item al usarlo.

Notas:

- Demo actual: consumibles desde slots equipados + condiciones del item.
- No hay uso desde inventario libre en combate.
- Tacticas con consumibles queda congelado para despues; hacerlo ahora abre UI y serializacion nueva.
- Molotov equipada se usa en el primer turno valido; pocion equipada espera HP bajo; pergamino equipado revive al caer.

---

## 4. Alinear recompensas

- [x] Agregar soporte de piedra a `RewardData`.
- [x] Agregar soporte de cristales a `RewardData`.
- [x] Agregar aplicacion de piedra en `RewardApplier`.
- [x] Agregar aplicacion de cristales en `RewardApplier`.
- [x] Definir recompensas por enemigo.
- [x] Definir recompensas por nodo repetible.
- [x] Definir recompensas de eventos.
- [x] Alinear generador de enemigos para no perder piedra al regenerar assets.
- [x] Evitar loops de farmeo infinito rentable.
- [x] Confirmar que XP se aplica solo al personaje activo.

Notas:

- Los nodos de farm de la demo son repetibles, pero obligan a viajar entre nodos para repetir combate.
- El viaje consume horas/stamina y el dia 11 corta la expedicion, asi que el farmeo no es gratis.
- `RewardApplier` entrega XP mediante `PartyRuntimeState.AddExperienceToActiveParty`; con `maxActiveMembers = 1`, solo sube el expedicionario activo.

---

## 5. Alinear economia de desgaste

- [x] Implementar o ajustar accion de buscar recursos.
- [x] Buscar recursos cuesta 4 horas.
- [x] Buscar recursos cuesta 3 stamina del personaje o decidir reemplazo.
- [x] Buscar recursos da 1 recurso aleatorio basico o 1 comida.
- [x] Buscar recursos da 0 XP.
- [x] Revisar si eventos pueden consumir stamina del personaje.
- [x] Si no se agrega stamina de personaje en eventos, decidir usar stamina de caravana.
- [x] Hacer que descansar tenga costo de tiempo si corresponde.
- [x] Hacer que reparar armaduras tenga costo de tiempo si corresponde.
- [ ] Confirmar que ganar mal genera coste real en Play Mode.
- [ ] Probar buscar recursos en Play Mode.

Notas:

- Eventos siguen usando stamina de caravana por ahora.
- Buscar recursos usa stamina del personaje porque es una accion de emergencia lenta y poco rentable.

---

## 6. Tiempo y derrota

- [x] Implementar regla de derrota por tiempo.
- [x] Si empieza el dia 11 y `flag_jefe_derrotado` es falso, la expedicion fracasa.
- [x] Decidir donde se muestra esa derrota.
- [ ] Decidir si vuelve a menu, pantalla final o estado bloqueado de demo.
- [x] Asegurar que dormir al dia 11 dispare fracaso si no se derroto al jefe.
- [x] Asegurar que viajar pasando al dia 11 dispare fracaso si no se derroto al jefe.

---

## 7. Caravana y descanso

- [x] Revisar descanso parcial actual.
- [x] Agregar costo de horas al descanso parcial si falta.
- [x] Agregar costo de stamina de caravana al descanso parcial si se mantiene.
- [x] Revisar descanso completo.
- [x] Confirmar que descanso completo avanza dia.
- [x] Confirmar que descanso completo cura 100% HP.
- [x] Confirmar que descanso completo recupera 100% stamina.
- [x] Confirmar que descanso completo recupera 100% mana.
- [x] Revisar reparacion de armadura fisica.
- [x] Revisar reparacion de armadura magica.
- [x] Agregar costo de horas a reparaciones si falta.
- [x] Alinear UI de hoguera con costos reales.

---

## 8. Items Tier 1

- [x] Crear/ajustar daga mellada.
- [x] Crear/ajustar espada oxidada.
- [x] Crear/ajustar baston partido.
- [x] Crear/ajustar simbolo quebrado.
- [x] Crear/ajustar placas oxidadas.
- [x] Crear/ajustar cuero gastado.
- [x] Crear/ajustar tunica rasgada.
- [x] Crear/ajustar vestidura remendada.
- [x] Crear/ajustar pocion de salud.
- [x] Crear/ajustar molotov.
- [x] Crear/ajustar pergamino de revivir.
- [x] Crear/ajustar recetas Tier 1.
- [x] Implementar auto-revive del pergamino para demo de un solo personaje.
- [ ] Revisar iconos/placeholders.
- [x] Revisar valores de venta si se usa venta.

---

## 9. Enemigos Tier 1

- [x] Crear/ajustar `EnemyDefinitionSO` de rata gigante.
- [x] Crear/ajustar `EnemyDefinitionSO` de esqueleto.
- [x] Crear/ajustar `EnemyDefinitionSO` de gusano.
- [x] Crear/ajustar `EnemyDefinitionSO` de demonio menor.
- [x] Asignar sprites/icons.
- [x] Asignar habilidades.
- [x] Asignar tacticas.
- [x] Asignar recompensas.
- [x] Crear encuentro de 1 rata.
- [x] Crear encuentro de 2 ratas.
- [x] Crear encuentro de rata + esqueleto.
- [x] Crear encuentro de 1 esqueleto.
- [x] Crear encuentro de 1 gusano.
- [x] Crear encuentro de esqueleto + gusano.
- [x] Crear encuentro de demonio menor.

---

## 10. Mapa demo

- [x] Crear/ajustar nodo Valdoran.
- [x] Crear/ajustar nodo Camino 01.
- [x] Crear/ajustar nodo Camino 02.
- [x] Crear/ajustar nodo Puente roto.
- [x] Crear/ajustar nodo Cruce del Vigia.
- [x] Crear/ajustar nodo Bosque de Aldheron.
- [x] Crear/ajustar nodo Claro de las Rocas.
- [x] Crear/ajustar nodo Torre Vigia.
- [x] Crear/ajustar nodo Mina de Hierro Negro.
- [x] Crear/ajustar nodo Collinasombra.
- [x] Crear/ajustar nodo Portal de los Excavadores.
- [x] Crear/ajustar nodo Frontera abierta.
- [x] Crear 10 caminos.
- [x] Asignar estados iniciales.
- [x] Asignar flags requeridas.
- [x] Ajustar colores de caminos.
- [x] Acomodar posiciones en `worldmapconcept7`.
- [ ] Ejecutar generador en Unity y guardar `WorldMapScene`.

---

## 11. Eventos

- [x] Crear evento Puente roto.
- [x] Crear evento Pista del jefe.
- [x] Crear evento Collinasombra.
- [x] Crear evento Jefe Demonio menor.
- [x] Configurar opciones.
- [x] Configurar costos.
- [x] Configurar flags.
- [x] Configurar recompensas.
- [ ] Confirmar que aparecen solo los botones necesarios.

---

## 12. Battle flow

- [x] Confirmar que `WorldMapScene` carga `BattleScene`.
- [x] Confirmar que `BattleScene` vuelve a `WorldMapScene`.
- [x] Confirmar que el encuentro correcto carga enemigos.
- [x] Confirmar que se aplican recompensas al volver.
- [x] Confirmar que se conserva estado del nodo.
- [x] Confirmar que derrota en combate vuelve a Valdoran.
- [x] Confirmar que consumibles gastados no vuelven.
- [x] Confirmar que HP persiste como esperamos.
- [x] Confirmar que stamina persiste como esperamos.
- [x] Confirmar que mana persiste como esperamos.
- [x] Confirmar que armaduras persisten como esperamos.
- [ ] Probar manualmente una run mapa -> combate -> mapa en Play Mode.

---

## 13. UI minima necesaria

- [x] HUD de mapa muestra dia.
- [x] HUD de mapa muestra hora.
- [x] HUD de mapa muestra stamina de caravana.
- [x] HUD de mapa muestra oro.
- [x] HUD de mapa muestra ubicacion.
- [x] Caravana muestra recursos.
- [x] Caravana muestra personaje.
- [x] Caravana muestra equipo.
- [x] Caravana muestra HP/stamina/mana.
- [x] Caravana muestra armaduras.
- [x] Caravana permite crafting.
- [x] Caravana permite descanso/reparacion.
- [x] Combate muestra vida.
- [x] Combate muestra armadura fisica.
- [x] Combate muestra armadura magica.
- [x] Combate muestra turnos.
- [x] Combate muestra feedback de dano.
- [x] Evitar agregar pantallas nuevas si una existente sirve.
- [ ] Probar manualmente UI minima en Play Mode.

---

## 14. Balance de ruta completa

- [x] Simular ruta minima.
- [x] Simular ruta con farmeo malo.
- [x] Simular ruta optima.
- [ ] Ver en que dia llega al jefe cada ruta en Play Mode.
- [ ] Ajustar recompensas/costos.
- [ ] Ajustar HP/dano enemigos.
- [ ] Ajustar recetas.
- [ ] Ajustar XP.
- [ ] Confirmar que el jefe es dificil pero justo.
- [x] Documentar simulacion inicial en `BALANCE_RUTAS_DEMO.md`.

---

## 15. Limpieza de scope

- [x] Marcar sistemas congelados para no tocarlos.
- [x] No usar contratos.
- [x] No usar tienda dinamica.
- [x] No usar reclutamiento.
- [x] No usar party de 4.
- [x] No usar tiers 2/3.
- [x] No agregar enemigos nuevos.
- [x] No agregar eventos aleatorios.
- [x] No agregar economia local.
- [x] Documentar reglas en `SISTEMAS_CONGELADOS_DEMO.md`.

---

## 16. Orden recomendado de implementacion

1. Alineacion tecnica minima:
   - recursos iniciales;
   - party de 1;
   - `RewardData` con piedra/cristales;
   - derrota dia 11;
   - descanso/reparacion con costos reales.

2. Contenido Tier 1:
   - items;
   - recetas;
   - habilidades;
   - enemigos;
   - encuentros.

3. Mapa y eventos:
   - nodos;
   - caminos;
   - flags;
   - eventos;
   - jefe.

4. Balance y prueba:
   - ruta minima;
   - ruta mala;
   - ruta optima;
   - ajustes finales.

---

## 17. Regla de trabajo

```text
Si una tarea no ayuda a probar la expedicion Tier 1, no entra en esta demo.
```


---

# Fuente: BALANCE_RUTAS_DEMO.md

# AbadyyRpg - Balance rutas demo Tier 1

Estado: simulacion de papel con datos actuales.
Base: `SCOPE_TEST_SIMPLIFICADO.md` y assets Tier 1 actuales.

Objetivo:

```text
Ver si la ruta de 10 dias permite aprender, farmear, craftear y llegar al jefe sin abrir sistemas grandes.
```

---

## Supuestos actuales

- El jugador empieza dia 1, hora 8.
- La caravana empieza con 100 stamina.
- Inventario inicial: 0 oro, 0 comida, 0 madera, 0 piedra, 0 hierro, 0 cuero, 0 cristal.
- El personaje empieza sin equipo.
- El puente no debe bloquear la demo si el jugador no tiene recursos.
- El combate repetible consume horas/stamina indirectamente: para repetir un nodo hay que viajar de ida/vuelta.
- El coste real de combate viene por HP, stamina, mana, armadura y consumibles.
- La derrota por tiempo ocurre al llegar al dia 11 sin derrotar al jefe.

---

## Observacion critica encontrada

Con inventario inicial en cero, la opcion de reparar puente no puede usarse al llegar por primera vez.

Eso esta bien si existe alternativa.

La alternativa correcta es:

```text
Forzar el cruce:
- 3 stamina de caravana
- 2 horas
- 0 recursos
```

Se corrigio el generador para que no pida comida en esa opcion.

Motivo:

```text
Si forzar tambien pide recursos, la demo puede quedar bloqueada antes del primer combate.
```

---

## Recompensas activas por encuentro

| Encuentro | Oro | Comida | Madera | Piedra | Hierro | Cuero | Cristal | XP |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 rata | 3 | 1 | 0 | 0 | 0 | 1 | 0 | 8 |
| 2 ratas | 6 | 2 | 0 | 0 | 0 | 2 | 0 | 16 |
| rata + esqueleto | 9 | 1 | 0 | 1 | 1 | 1 | 0 | 24 |
| 1 esqueleto | 6 | 0 | 0 | 1 | 1 | 0 | 0 | 16 |
| 1 gusano | 10 | 1 | 0 | 0 | 0 | 1 | 1 | 28 |
| esqueleto + gusano | 16 | 1 | 0 | 1 | 1 | 1 | 1 | 44 |
| jefe demonio menor | 25 | 0 | 0 | 0 | 1 | 1 | 3 | 60 |

---

## Ruta minima

```text
Valdoran
-> Camino 01
-> Camino 02
-> Puente roto
-> Forzar cruce
-> Cruce del Vigia
-> Torre vigia
-> Revisar torre
-> Collinasombra
-> Escuchar advertencias
-> Portal
-> Jefe
```

Resultado esperado:

- Llega muy rapido al jefe.
- No craftea nada importante.
- No gana XP relevante.
- Deberia perder o quedar como intento claramente imprudente.

Conclusion:

```text
La ruta minima sirve como leccion, no como ruta ganadora.
```

---

## Ruta mala con farmeo ineficiente

```text
Forzar puente.
Farmear ratas muchas veces.
Descansar/reparar de mas.
Llegar tarde al jefe con equipo incompleto.
```

Resultado esperado:

- Junta comida y cuero.
- Junta poco hierro, piedra y cristal.
- Sube XP lento.
- Puede quedarse sin dias si compensa malos combates con muchos descansos.

Riesgo de balance:

```text
Si las ratas dan demasiada seguridad economica, el jugador puede ignorar enemigos que ensenan counterplay.
```

Control recomendado:

- Mantener ratas como recompensa chica.
- Hacer que hierro/cristal vengan de esqueleto/gusano/mina.
- No permitir que solo ratas preparen al jefe.

---

## Ruta optima tentativa

```text
Forzar puente.
Ganar 2 ratas.
Ganar rata + esqueleto.
Ir a torre y revisar pista.
Saquear torre si falta madera.
Ganar esqueleto + gusano en mina.
Craftear tunica rasgada + espada oxidada.
Preparar pocion o molotov si sobran recursos.
Ir a Collinasombra.
Ir al portal.
Pelear jefe.
```

Recursos aproximados antes de craftear, si hace 2 ratas + rata/esqueleto + esqueleto/gusano + saqueo torre:

| Recurso | Total |
|---|---:|
| Oro | 31 |
| Comida | 4 |
| Madera | 1 |
| Piedra | 3 |
| Hierro | 2 |
| Cuero | 4 |
| Cristal | 1 |
| XP | 84 |

Craft recomendado:

| Item | Oro | Madera | Hierro | Cuero | Cristal |
|---|---:|---:|---:|---:|---:|
| Espada oxidada | 12 | 1 | 2 | 0 | 0 |
| Tunica rasgada | 14 | 0 | 0 | 2 | 1 |

Sobrante aproximado:

| Recurso | Sobra |
|---|---:|
| Oro | 5 |
| Comida | 4 |
| Piedra | 3 |
| Cuero | 2 |

Lectura:

```text
Esta ruta arma una respuesta clara contra el jefe:
- tunica para aguantar magia;
- espada para pegar por fisico;
- algo de comida/cuero/piedra como margen economico.
```

Problema:

```text
Con XP 84 el personaje queda cerca de nivel 3, pero no llega.
```

Recomendacion:

- Una pelea extra de 1 esqueleto o 2 ratas deberia alcanzar nivel 3.
- Si nivel 3 se siente obligatorio para jefe, bajar requisito de nivel 3 o subir un poco XP de mina.
- Si nivel 3 es opcional, dejarlo asi.

---

## Ajustes que parecen necesarios antes de balance fino

1. Implementar o resolver `Buscar recursos`.
   - Sin esta accion, falta la red de seguridad documentada.

2. Definir habilidades genericas del personaje.
   - El balance real depende mucho de ataque poderoso, misil magico, curar, guardia y escudo magico.

3. Implementar auto-revive del pergamino.
   - El item existe, pero la regla especial de demo todavia falta.

4. Ejecutar el generador en Unity y guardar `WorldMapScene`.
   - Los cambios del generador no viven en escena hasta regenerar.

5. Probar una run real.
   - La simulacion no reemplaza ver turnos, desgaste, IA y tacticas en Play Mode.

---

## Estado de balance

```text
La estructura de ruta parece viable.
El bloqueo inicial del puente quedo identificado y corregido en generador.
Todavia no conviene tocar HP/dano/recompensas hasta cerrar habilidades y buscar recursos.
```


---

# Fuente: SISTEMAS_CONGELADOS_DEMO.md

# AbadyyRpg - Sistemas congelados para demo Tier 1

Estado: regla de scope activa.
Base: `SCOPE_TEST_SIMPLIFICADO.md` y `LISTA_TAREAS_DEMO.md`.

Objetivo:

```text
Evitar que la demo vuelva a crecer mientras estamos cerrando el loop Tier 1.
```

---

## Regla principal

```text
Si una tarea no ayuda a probar preparacion -> viaje -> combate -> recompensa -> preparacion, no entra.
```

La demo prueba una sola idea:

```text
preparar bien la caravana cambia el resultado de una expedicion.
```

---

## Sistemas congelados

Estos sistemas pueden existir en codigo o assets viejos, pero no se usan como objetivo de la demo.

| Sistema | Estado demo | Regla |
|---|---|---|
| Contratos | congelado | No crear UI, datos ni balance de contratos. |
| Tienda dinamica | congelado | No usar tienda; la caravana craftea. |
| Reclutamiento | congelado | No sumar mercenarios nuevos durante la demo. |
| Party de 4 | congelado | La party activa queda limitada a 1 personaje. |
| Tiers 2/3 de items | congelado | Solo items Tier 1 en recetas y base de datos activa. |
| Enemigos nuevos | congelado | Usar solo rata gigante, esqueleto, gusano y demonio menor. |
| Eventos aleatorios | congelado | Usar solo eventos definidos de ruta. |
| Economia local | congelado | No pueblos con precios, reputacion ni mercados propios. |

---

## Aclaracion sobre tiers

La demo es Tier 1 por contenido:

- items Tier 1;
- recetas Tier 1;
- zona Tier 1;
- jefe Tier 1;
- escala numerica chica.

El enum `EnemyTier` de `EnemyDefinitionSO` se interpreta como rol interno de amenaza:

- `Basic`;
- `Common`;
- `Dangerous`;
- `Boss`.

Eso no habilita contenido Tier 2/3.

Ejemplo:

```text
Gusano puede ser EnemyTier.Dangerous dentro de una demo Tier 1.
Demonio menor puede ser EnemyTier.Boss dentro de una demo Tier 1.
```

---

## Datos activos esperados

### Items activos

Solo deben estar activos en la base de datos de demo:

- daga mellada;
- espada oxidada;
- baston partido;
- simbolo quebrado;
- placas oxidadas;
- cuero gastado;
- tunica rasgada;
- vestidura remendada;
- pocion de salud;
- molotov;
- pergamino de revivir.

### Encuentros activos

Solo deben estar activos en la base de datos de demo:

- 1 rata;
- 2 ratas;
- rata + esqueleto;
- 1 esqueleto;
- 1 gusano;
- esqueleto + gusano;
- demonio menor.

### Mapa activo

Solo debe probar:

- Valdoran;
- puente roto;
- farm temprano;
- pista del jefe;
- preparacion final;
- jefe;
- frontera abierta.

---

## Cosas que no se tocan hasta cerrar la demo

- Nuevas clases.
- Mas personajes jugables.
- Contratos.
- Venta/compra dinamica.
- Mas biomas.
- Mas jefes.
- Mas rarezas de item.
- Generacion procedural.
- Reputacion.
- Economia por ciudad.
- Eventos aleatorios.

---

## Excepcion permitida

Se puede tocar un sistema congelado solo si:

```text
esta rompiendo el loop minimo de la demo.
```

Ejemplo valido:

```text
Si una UI vieja de party de 4 impide equipar al personaje unico, se ajusta lo minimo.
```

Ejemplo invalido:

```text
Agregar reclutamiento porque ya existe parte del codigo.
```


---

# Fuente: definicion_clases_combate_demo.md

# AbadyyRpg - Definicion inicial de clases y armaduras

Estado: base de diseno para balanceo de demo.
Objetivo: definir las 4 clases fijas de la demo y la lectura correcta del sistema defensivo antes de balancear enemigos, experiencia, armas, armaduras y habilidades.

---

## 1. Premisa de combate

El combate de la demo se piensa como un sistema automatico por tacticas/gambits. El jugador no elige acciones manualmente durante cada turno, sino que prepara unidades con:

- stats base;
- equipo;
- habilidades;
- consumibles;
- reglas de tactica;
- prioridades de uso.

Por eso, una clase no debe definirse solo como una lista de habilidades, sino como un paquete completo de comportamiento automatico.

```text
Clase = stats base + equipo esperado + habilidades + tacticas iniciales + rol dentro del autocombate
```

---

## 2. Lectura correcta de vida y armaduras

El sistema defensivo se define con tres barras principales:

```text
HP real
Armadura fisica
Armadura magica
```

Ejemplo:

```text
HP: 100
Armadura fisica: 100
Armadura magica: 50
```

La armadura no funciona como reduccion porcentual de dano. Funciona como vida extra separada por tipo de dano.

### Dano fisico

El dano fisico golpea primero la armadura fisica.

```text
Dano fisico -> Armadura fisica -> HP
```

Ejemplo:

```text
HP: 100
Armadura fisica: 100
Recibe 30 dano fisico

Resultado:
HP: 100
Armadura fisica: 70
```

Si el dano excede la armadura fisica restante, el sobrante pasa a HP.

```text
HP: 100
Armadura fisica: 20
Recibe 60 dano fisico

Resultado:
HP: 60
Armadura fisica: 0
```

### Dano magico

El dano magico golpea primero la armadura magica.

```text
Dano magico -> Armadura magica -> HP
```

Ejemplo:

```text
HP: 100
Armadura magica: 50
Recibe 30 dano magico

Resultado:
HP: 100
Armadura magica: 20
```

Si el dano excede la armadura magica restante, el sobrante pasa a HP.

---

## 3. Origen de las armaduras

Las armaduras vienen principalmente de:

- equipo;
- habilidades que restauran o refuerzan armadura;
- consumibles, si se agregan;
- descanso/reparacion fuera de combate.

Las clases no deberian depender de armadura base alta. La clase define el rol, pero la defensa concreta viene del equipo y de las habilidades.

Ejemplo:

```text
Defensor sin equipo:
HP alto
Armadura fisica: 0
Armadura magica: 0

Defensor con equipo pesado inicial:
HP alto
Armadura fisica alta
Armadura magica baja/media
```

---

## 4. Reglas generales de armadura

Para mantener el combate legible:

1. La armadura no se regenera sola en combate.
2. La armadura vuelve por habilidades, consumibles o reparacion/descanso.
3. El equipo define gran parte de la supervivencia.
4. Las habilidades defensivas deben restaurar armadura, no reducir dano de forma abstracta.
5. El tipo de dano importa porque cada enemigo puede presionar una barra distinta.

Esto permite que un enemigo fisico y uno magico tengan identidades claras.

Ejemplo:

```text
Bandido con hacha:
Amenaza armadura fisica.
Es peligroso para Mago del Circulo y Acolita si logra pasar el taunt.

Cultista:
Amenaza armadura magica.
Es mas peligroso para Defensor que para Mago del Circulo.
```

---

## 5. Clases fijas de la demo

La demo usara 4 clases fijas:

| Rol | Clase |
|---|---|
| Tanque | Defensor |
| DPS fisico | Asesino |
| DPS magico | Mago del Circulo |
| Sanador / booster | Acolita |

Estas clases buscan ser claras de leer, faciles de balancear y utiles para probar los sistemas centrales del combate automatico.

---

# 6. Defensor

## Rol

Tanque principal. Su funcion es sostener la linea, atraer ataques y mantener su armadura fisica activa.

## Identidad

El Defensor no es tanque porque tenga armadura base alta. Es tanque porque:

- tiene HP alto;
- usa equipo pesado;
- genera taunt;
- puede restaurar armadura fisica;
- protege indirectamente al resto de la party.

## Stats base sugeridos sin equipo

| Stat | Valor |
|---|---:|
| HP | 130 |
| Dano base | 8 |
| Speed | 7 |
| Stamina | 110 |
| Mana | 20 |
| Armadura fisica | 0 |
| Armadura magica | 0 |

## Equipo esperado

| Etapa | Armadura fisica | Armadura magica |
|---|---:|---:|
| Inicial | 80 | 20 |
| Media | 110 | 30 |
| Final demo | 150 | 45 |

## Habilidades tipo

### Golpe de Escudo

```text
Costo: stamina
Objetivo: enemigo individual
Efecto: dano fisico bajo + genera taunt propio
```

### Postura Defensiva

```text
Costo: stamina
Objetivo: self
Efecto: restaura armadura fisica propia
```

### Provocar

```text
Costo: stamina
Objetivo: self / enemigos
Efecto: genera mucho taunt
```

### Romper Guardia

```text
Costo: stamina
Objetivo: enemigo individual
Efecto: dano fisico medio + dano extra a armadura fisica
```

## Tacticas iniciales sugeridas

```text
1. Si armadura fisica propia < 40% -> Postura Defensiva
2. Si HP de aliado < 50% -> Provocar
3. Si enemigo tiene armadura fisica alta -> Romper Guardia
4. Siempre -> Golpe de Escudo
```

---

# 7. Asesino

## Rol

DPS fisico rapido. Su funcion es eliminar objetivos vulnerables, rematar enemigos heridos y aplicar presion fisica.

## Identidad

El Asesino no aguanta por defensa alta. Sobrevive porque:

- actua rapido;
- mata antes de recibir demasiado dano;
- puede bajar su taunt o evitar foco;
- usa armadura ligera;
- prioriza objetivos heridos.

## Stats base sugeridos sin equipo

| Stat | Valor |
|---|---:|
| HP | 85 |
| Dano base | 14 |
| Speed | 15 |
| Stamina | 120 |
| Mana | 10 |
| Armadura fisica | 0 |
| Armadura magica | 0 |

## Equipo esperado

| Etapa | Armadura fisica | Armadura magica |
|---|---:|---:|
| Inicial | 35 | 15 |
| Media | 50 | 25 |
| Final demo | 75 | 35 |

## Habilidades tipo

### Corte Rapido

```text
Costo: stamina
Objetivo: enemigo individual
Efecto: dano fisico medio
```

### Ejecucion

```text
Costo: stamina
Objetivo: enemigo individual con HP bajo
Efecto: dano fisico alto
```

### Sangrado

```text
Costo: stamina
Objetivo: enemigo individual
Efecto: dano fisico bajo/medio + estado de dano por turno
```

Nota: si el sistema actual usa Poison como estado generico de dano por turno, puede reutilizarse visualmente como sangrado.

### Desaparecer

```text
Costo: stamina
Objetivo: self
Efecto: baja taunt / aplica invisibilidad temporal
```

## Tacticas iniciales sugeridas

```text
1. Si HP propio < 35% -> Desaparecer
2. Si enemigo HP < 35% -> Ejecucion
3. Si enemigo fuerte no tiene dano por turno -> Sangrado
4. Siempre -> Corte Rapido
```

---

# 8. Mago del Circulo

## Rol

DPS magico. Su funcion es presionar armadura magica, aplicar estados y resolver grupos con dano en area.

## Identidad

El Mago del Circulo representa magia organizada pero peligrosa. No es un mago generico: pertenece o pertenecio a una tradicion, escuela, orden o circulo arcano.

Sobrevive porque:

- usa mana alto;
- tiene buena armadura magica por equipo;
- puede drenar vida o protegerse magicamente;
- evita el dano fisico directo.

## Stats base sugeridos sin equipo

| Stat | Valor |
|---|---:|
| HP | 70 |
| Dano base | 6 |
| Speed | 10 |
| Stamina | 60 |
| Mana | 130 |
| Armadura fisica | 0 |
| Armadura magica | 0 |

## Equipo esperado

| Etapa | Armadura fisica | Armadura magica |
|---|---:|---:|
| Inicial | 10 | 50 |
| Media | 18 | 75 |
| Final demo | 30 | 110 |

## Habilidades tipo

### Dardo Arcano

```text
Costo: mana
Objetivo: enemigo individual
Efecto: dano magico medio
```

### Marca del Circulo

```text
Costo: mana
Objetivo: enemigo individual
Efecto: dano magico bajo + estado de dano por turno o vulnerabilidad
```

### Pulso Magico

```text
Costo: mana
Objetivo: todos los enemigos
Efecto: dano magico bajo/medio en area
```

### Drenar Esencia

```text
Costo: mana
Objetivo: enemigo individual
Efecto: dano magico + vampirismo
```

## Tacticas iniciales sugeridas

```text
1. Si HP propio < 45% -> Drenar Esencia
2. Si hay 3+ enemigos vivos -> Pulso Magico
3. Si enemigo fuerte no tiene estado de dano por turno -> Marca del Circulo
4. Siempre -> Dardo Arcano
```

---

# 9. Acolita

## Rol

Sanadora y booster. Su funcion es mantener viva a la party, reparar defensas y aumentar el margen de error.

## Identidad

La Acolita no es solo una healer. Es una unidad de soporte que puede representar fe, rito, medicina de campamento o entrenamiento religioso. Su poder principal esta en sostener al grupo durante peleas largas.

## Stats base sugeridos sin equipo

| Stat | Valor |
|---|---:|
| HP | 90 |
| Dano base | 5 |
| Speed | 12 |
| Stamina | 70 |
| Mana | 130 |
| Armadura fisica | 0 |
| Armadura magica | 0 |

## Equipo esperado

| Etapa | Armadura fisica | Armadura magica |
|---|---:|---:|
| Inicial | 25 | 35 |
| Media | 35 | 55 |
| Final demo | 60 | 80 |

## Habilidades tipo

### Curar Herida

```text
Costo: mana
Objetivo: aliado individual
Efecto: cura HP
```

### Bendicion Menor

```text
Costo: mana
Objetivo: aliado individual
Efecto: regeneration o pequeno buff defensivo/ofensivo
```

### Remendar Armadura

```text
Costo: mana
Objetivo: aliado individual
Efecto: restaura armadura fisica
```

### Oracion de Vigor

```text
Costo: mana
Objetivo: aliado individual o todos los aliados
Efecto: restaura stamina, aumenta speed o mejora temporalmente el dano
```

## Tacticas iniciales sugeridas

```text
1. Si HP de aliado < 35% -> Curar Herida
2. Si armadura fisica de aliado < 35% -> Remendar Armadura
3. Si HP de aliado < 70% -> Bendicion Menor
4. Siempre -> Oracion de Vigor o ataque basico/debil
```

---

# 10. Lectura de roles por recurso

| Clase | Recurso principal | Funcion |
|---|---|---|
| Defensor | Stamina | taunt, armadura fisica, proteccion |
| Asesino | Stamina | dano fisico, velocidad, remate |
| Mago del Circulo | Mana | dano magico, area, estados, drenaje |
| Acolita | Mana | curacion, reparacion, buffs |

Esto separa bien los roles y ayuda a que cada clase pruebe una parte distinta del sistema.

---

# 11. Progresion sugerida de habilidades en demo

Para no saturar al jugador, cada clase puede empezar con 2 habilidades y desbloquear mas durante la demo.

## Nivel 1

| Clase | Habilidades iniciales |
|---|---|
| Defensor | Golpe de Escudo, Postura Defensiva |
| Asesino | Corte Rapido, Ejecucion |
| Mago del Circulo | Dardo Arcano, Marca del Circulo |
| Acolita | Curar Herida, Bendicion Menor |

## Nivel 2

| Clase | Nueva habilidad |
|---|---|
| Defensor | Provocar |
| Asesino | Sangrado |
| Mago del Circulo | Drenar Esencia |
| Acolita | Remendar Armadura |

## Nivel 3

| Clase | Nueva habilidad |
|---|---|
| Defensor | Romper Guardia |
| Asesino | Desaparecer |
| Mago del Circulo | Pulso Magico |
| Acolita | Oracion de Vigor |

## Nivel 4 y 5

No hace falta desbloquear mas habilidades en la demo. Es preferible mejorar stats, equipo y valores de habilidades existentes.

---

# 12. Reglas de tactica por progresion

Para que el sistema se aprenda de forma gradual:

## Nivel 1

2 reglas activas.

```text
1. Regla de emergencia
2. Regla por defecto
```

Ejemplo Acolita:

```text
1. Si HP de aliado < 35% -> Curar Herida
2. Siempre -> Bendicion Menor / ataque debil
```

## Nivel 2

3 reglas activas.

```text
1. Emergencia
2. Condicion tactica
3. Accion por defecto
```

## Nivel 3+

4 reglas activas.

A partir de este punto la clase ya expresa su comportamiento completo.

---

# 13. Principio de balance para seguir

La demo debe balancearse alrededor de esta idea:

```text
El nivel y el equipo no deben reemplazar a las tacticas.
Deben dar margen de error.
```

Si una party con malas tacticas gana todo solo por numeros, el sistema pierde gracia.
Si una party con buenas tacticas no puede ganar por falta de numeros, el progreso pierde valor.

El punto ideal es:

```text
Buenas tacticas + equipo medio = victoria justa
Malas tacticas + equipo bueno = victoria posible pero desprolija
Buenas tacticas + mal equipo = victoria dificil
Malas tacticas + mal equipo = derrota probable
```

---

# 14. Pendientes de balanceo

A partir de esta base, los proximos pasos son:

1. Definir valores finales de stats por nivel.
2. Definir armas iniciales, medias y finales de demo.
3. Definir armaduras iniciales, medias y finales de demo.
4. Definir valores numericos de cada habilidad.
5. Definir enemigos normales.
6. Definir elites.
7. Definir jefe normal.
8. Definir jefe final.
9. Definir experiencia por combate.
10. Definir curva de nivel maximo de demo.


---

# Fuente: enemigos_dificultad_xp_recompensas_demo.md

# AbadyyRpg - Enemigos, dificultad, experiencia y recompensas de demo

Estado: borrador inicial de balance.  
Objetivo: asignar dificultad, experiencia, progresiÃ³n por niveles y recompensas a los enemigos definidos para la demo.

---

## 1. Base del sistema

La demo trabaja con una party de 4 clases fijas:

| Rol | Clase |
|---|---|
| Tanque | Defensor |
| DPS fÃ­sico | Asesino |
| DPS mÃ¡gico | Mago del CÃ­rculo |
| Sanador / booster | AcÃ³lita |

El combate se piensa como automÃ¡tico por tÃ¡cticas/gambits. El jugador prepara la party antes de combatir mediante equipo, habilidades, consumibles y reglas de prioridad.

El sistema defensivo se lee asÃ­:

```text
DaÃ±o fÃ­sico -> Armadura fÃ­sica -> HP
DaÃ±o mÃ¡gico -> Armadura mÃ¡gica -> HP
```

La armadura funciona como vida extra separada por tipo de daÃ±o. No es reducciÃ³n porcentual.

---

## 2. Nivel mÃ¡ximo de demo

La demo se balancea con nivel mÃ¡ximo 5.

| Nivel | XP total requerida |
|---|---:|
| 1 | 0 |
| 2 | 100 |
| 3 | 250 |
| 4 | 450 |
| 5 | 700 |

Curva por tramo:

| Subida | XP necesaria |
|---|---:|
| Nivel 1 -> 2 | 100 |
| Nivel 2 -> 3 | 150 |
| Nivel 3 -> 4 | 200 |
| Nivel 4 -> 5 | 250 |

La idea es que el jugador llegue a nivel 5 solamente si completa buena parte del contenido de la regiÃ³n.

---

## 3. Tiers de dificultad

Los enemigos se dividen en tiers.

| Tier | Nombre | Uso |
|---|---|---|
| Tier 1 | BÃ¡sico | primeros combates, tutorial natural |
| Tier 2 | ComÃºn | nÃºcleo de encuentros normales |
| Tier 3 | Peligroso | enemigos normales fuertes |
| Tier 4 | Ã‰lite | encuentros especiales/opcionales |
| Tier 5 | Jefe normal | cierre del conflicto regional |
| Tier 6 | Jefe final | cierre de demo |

---

## 4. Valores base por tier

| Tier | Dificultad | XP individual aproximada | Recompensa esperada |
|---|---|---:|---|
| Tier 1 | Baja | 15-25 | recursos chicos |
| Tier 2 | Media | 30-40 | oro + recurso Ãºtil |
| Tier 3 | Alta normal | 45-60 | recurso bueno / item comÃºn |
| Tier 4 | Ã‰lite | 120-150 | item fuerte / recurso raro |
| Tier 5 | Jefe normal | 220-260 | item clave / desbloqueo |
| Tier 6 | Jefe final | 0 o cierre | recompensa narrativa |

---

# 5. Enemigos normales

## 5.1 Bandido Raso

| Campo | Valor |
|---|---|
| Tier | 1 |
| Dificultad | Baja |
| Tipo | Humano |
| DaÃ±o principal | FÃ­sico |
| Rol | Atacante bÃ¡sico |
| XP | 20 |

### FunciÃ³n de combate

Introduce el flujo bÃ¡sico del combate:

```text
daÃ±o fÃ­sico -> armadura fÃ­sica -> HP
```

No deberÃ­a tener mecÃ¡nicas complejas.

### Recompensas

| Recompensa | Cantidad |
|---|---:|
| Oro | 5-8 |
| Cuero | 0-1 |
| Hierro | 0 |
| Comida | 0 |
| Cristales | 0 |

Loot opcional:

```text
Daga oxidada
Trapo viejo
```

---

## 5.2 Perro Hambriento

| Campo | Valor |
|---|---|
| Tier | 1 |
| Dificultad | Baja |
| Tipo | Bestia |
| DaÃ±o principal | FÃ­sico |
| Rol | Atacante rÃ¡pido/frÃ¡gil |
| XP | 15 |

### FunciÃ³n de combate

Prueba speed y presiÃ³n por mÃºltiples turnos. Pega poco, pero actÃºa seguido.

### Recompensas

| Recompensa | Cantidad |
|---|---:|
| Oro | 0 |
| Cuero | 1 |
| Comida | 0-1 |
| Hierro | 0 |
| Cristales | 0 |

Loot opcional:

```text
Piel daÃ±ada
Colmillo
```

---

## 5.3 Cuervo CarroÃ±ero

| Campo | Valor |
|---|---|
| Tier | 1 |
| Dificultad | Baja |
| Tipo | Bestia |
| DaÃ±o principal | FÃ­sico/control menor |
| Rol | Molestia rÃ¡pida |
| XP | 15 |

### FunciÃ³n de combate

Introduce interrupciones leves, reducciÃ³n de speed o stun corto si se decide usarlo.

### Recompensas

| Recompensa | Cantidad |
|---|---:|
| Oro | 0 |
| Cuero | 0 |
| Comida | 0 |
| Hierro | 0 |
| Cristales | 0-1 |

Loot opcional:

```text
Pluma negra
Ojo de cuervo
```

---

## 5.4 Saqueador

| Campo | Valor |
|---|---|
| Tier | 2 |
| Dificultad | Media |
| Tipo | Humano |
| DaÃ±o principal | FÃ­sico |
| Rol | Golpeador lento |
| XP | 35 |

### FunciÃ³n de combate

Prueba si el Defensor estÃ¡ absorbiendo correctamente el daÃ±o fÃ­sico.

Si el Saqueador golpea a una unidad blanda, deberÃ­a sentirse peligroso.

### Recompensas

| Recompensa | Cantidad |
|---|---:|
| Oro | 8-14 |
| Cuero | 0-1 |
| Hierro | 1 |
| Comida | 0 |
| Cristales | 0 |

Loot opcional:

```text
Hacha mellada
Chatarra de hierro
```

---

## 5.5 Ballestero

| Campo | Valor |
|---|---|
| Tier | 2 |
| Dificultad | Media |
| Tipo | Humano |
| DaÃ±o principal | FÃ­sico |
| Rol | PresiÃ³n a backline / bajo taunt |
| XP | 35 |

### FunciÃ³n de combate

Prueba el taunt. Si el Defensor no genera suficiente amenaza, el Ballestero castiga a la backline.

### Recompensas

| Recompensa | Cantidad |
|---|---:|
| Oro | 8-12 |
| Madera | 1 |
| Cuero | 0-1 |
| Hierro | 0-1 |
| Cristales | 0 |

Loot opcional:

```text
Ballesta rota
Virotes usados
```

---

## 5.6 Escudero Bandido

| Campo | Valor |
|---|---|
| Tier | 2 |
| Dificultad | Media |
| Tipo | Humano |
| DaÃ±o principal | FÃ­sico bajo |
| Rol | Tanque enemigo |
| XP | 40 |

### FunciÃ³n de combate

Tiene armadura fÃ­sica alta. EnseÃ±a que no todo se resuelve con daÃ±o fÃ­sico.

El Mago del CÃ­rculo deberÃ­a ser Ãºtil contra este enemigo.

### Recompensas

| Recompensa | Cantidad |
|---|---:|
| Oro | 10-16 |
| Hierro | 1-2 |
| Cuero | 0-1 |
| Madera | 0 |
| Cristales | 0 |

Loot opcional:

```text
Escudo astillado
Placas daÃ±adas
```

---

## 5.7 Cultista Menor

| Campo | Valor |
|---|---|
| Tier | 2 |
| Dificultad | Media |
| Tipo | Culto |
| DaÃ±o principal | MÃ¡gico |
| Rol | Caster bÃ¡sico |
| XP | 40 |

### FunciÃ³n de combate

Introduce daÃ±o mÃ¡gico:

```text
daÃ±o mÃ¡gico -> armadura mÃ¡gica -> HP
```

Debe castigar especialmente al Defensor si no tiene buena armadura mÃ¡gica.

### Recompensas

| Recompensa | Cantidad |
|---|---:|
| Oro | 4-10 |
| Cristales | 1 |
| Cuero | 0 |
| Hierro | 0 |
| Comida | 0 |

Loot opcional:

```text
Tiza ritual
Amuleto roto
```

---

## 5.8 MatÃ³n Desertor

| Campo | Valor |
|---|---|
| Tier | 3 |
| Dificultad | Alta normal |
| Tipo | Humano |
| DaÃ±o principal | FÃ­sico |
| Rol | Enemigo balanceado fuerte |
| XP | 55 |

### FunciÃ³n de combate

Primer enemigo normal serio. Tiene daÃ±o, aguante y poca debilidad obvia.

### Recompensas

| Recompensa | Cantidad |
|---|---:|
| Oro | 15-24 |
| Hierro | 1-2 |
| Cuero | 1 |
| Comida | 0 |
| Cristales | 0 |

Loot opcional:

```text
Espada gastada
Armadura remendada
```

---

## 5.9 Bestia del Monte

| Campo | Valor |
|---|---|
| Tier | 3 |
| Dificultad | Alta normal |
| Tipo | Bestia |
| DaÃ±o principal | FÃ­sico |
| Rol | Bruto fÃ­sico |
| XP | 60 |

### FunciÃ³n de combate

Prueba si el Defensor y la AcÃ³lita pueden sostener una pelea pesada.

Debe ser el enemigo normal fÃ­sico mÃ¡s amenazante.

### Recompensas

| Recompensa | Cantidad |
|---|---:|
| Oro | 0 |
| Cuero | 2-3 |
| Comida | 1-2 |
| Hierro | 0 |
| Cristales | 0 |

Loot opcional:

```text
Piel gruesa
Garra rota
```

---

## 5.10 AcÃ³lito Corrupto

| Campo | Valor |
|---|---|
| Tier | 3 |
| Dificultad | Alta normal |
| Tipo | Culto |
| DaÃ±o principal | MÃ¡gico bajo / soporte |
| Rol | Healer / buffer enemigo |
| XP | 60 |

### FunciÃ³n de combate

Prueba prioridad de objetivo. Si no se lo elimina, alarga las peleas.

### Recompensas

| Recompensa | Cantidad |
|---|---:|
| Oro | 8-14 |
| Cristales | 2 |
| Cuero | 0 |
| Hierro | 0 |
| Comida | 0 |

Loot opcional:

```text
Rosario quebrado
Fragmento ritual
```

---

# 6. Ã‰lites

## 6.1 CapitÃ¡n Desertor

| Campo | Valor |
|---|---|
| Tier | 4 |
| Dificultad | Ã‰lite fÃ­sico |
| Tipo | Humano |
| DaÃ±o principal | FÃ­sico |
| Rol | LÃ­der tÃ¡ctico |
| XP | 140 |

### FunciÃ³n de combate

Examen de la rama humana/bandida.

Prueba:

```text
taunt
armadura fÃ­sica
focus al backline
peleas largas
```

### Recompensas

| Recompensa | Cantidad |
|---|---:|
| Oro | 40-60 |
| Hierro | 3 |
| Cuero | 2 |
| Cristales | 0 |
| Comida | 0 |

Loot garantizado sugerido:

```text
Item de arma fÃ­sica tier 2 o tier 3 inicial
```

Loot opcional:

```text
Insignia de desertor
Espada de capitÃ¡n
```

---

## 6.2 Bruja del CÃ­rculo

| Campo | Valor |
|---|---|
| Tier | 4 |
| Dificultad | Ã‰lite mÃ¡gico |
| Tipo | Culto |
| DaÃ±o principal | MÃ¡gico / poison |
| Rol | Desgaste mÃ¡gico |
| XP | 140 |

### FunciÃ³n de combate

Examen de magia, estados y supervivencia larga.

Prueba:

```text
armadura mÃ¡gica
curaciÃ³n
regeneraciÃ³n
manejo de poison
```

### Recompensas

| Recompensa | Cantidad |
|---|---:|
| Oro | 25-40 |
| Cristales | 4 |
| Cuero | 0 |
| Hierro | 0 |
| Comida | 0 |

Loot garantizado sugerido:

```text
Item mÃ¡gico tier 2 o tier 3 inicial
```

Loot opcional:

```text
BastÃ³n torcido
Velo ritual
```

---

# 7. Jefes

## 7.1 SeÃ±or del Camino

| Campo | Valor |
|---|---|
| Tier | 5 |
| Dificultad | Jefe normal |
| Tipo | Humano |
| DaÃ±o principal | FÃ­sico |
| Rol | Jefe regional |
| XP | 240 |

### FunciÃ³n de combate

Cierre del conflicto humano de la regiÃ³n.

Prueba:

```text
armadura fÃ­sica
taunt
remate
soporte
focus
resistencia de recursos
```

### Recompensas

| Recompensa | Cantidad |
|---|---:|
| Oro | 100 |
| Hierro | 4 |
| Cuero | 3 |
| Cristales | 1 |
| Comida | 2 |

Loot garantizado sugerido:

```text
Arma tier 3
Armadura pesada tier 3
```

Recompensa narrativa:

```text
Desbloquea acceso a la AbadÃ­a / jefe final
```

---

## 7.2 La Cosa Bajo la AbadÃ­a

| Campo | Valor |
|---|---|
| Tier | 6 |
| Dificultad | Jefe final |
| Tipo | AberraciÃ³n / culto |
| DaÃ±o principal | FÃ­sico y mÃ¡gico |
| Rol | Cierre de demo |
| XP | 0 |

### FunciÃ³n de combate

Examen final del sistema.

Prueba:

```text
armadura fÃ­sica
armadura mÃ¡gica
curaciÃ³n
poison
taunt
daÃ±o en Ã¡rea
resistencia de recursos
```

### Recompensas

Como es cierre de demo, no necesita XP.

Recompensa sugerida:

```text
Cierre narrativo
Objeto Ãºnico de demo
Pantalla de victoria
Desbloqueo simbÃ³lico para futura regiÃ³n
```

Loot opcional:

| Recompensa | Cantidad |
|---|---:|
| Oro | 150 |
| Cristales | 6 |
| Hierro | 3 |
| Cuero | 3 |

Loot Ãºnico:

```text
CorazÃ³n Negro de la AbadÃ­a
```

---

# 8. CuÃ¡ntos enemigos hacen falta para subir de nivel

La progresiÃ³n no deberÃ­a medirse solamente en enemigos sueltos, sino en grupos de combate. Sin embargo, esta tabla sirve como referencia.

## Nivel 1 -> 2

XP necesaria: 100

Ejemplos:

| Enemigos derrotados | XP |
|---|---:|
| 3 Bandidos Rasos + 2 Perros | 90 |
| 4 Bandidos Rasos + 1 Perro | 95 |
| 2 Bandidos Rasos + 1 Saqueador + 1 Ballestero | 110 |

Objetivo real:

```text
2 combates fÃ¡ciles o 1 combate fÃ¡cil + 1 medio
```

---

## Nivel 2 -> 3

XP necesaria adicional: 150  
XP total requerida: 250

Ejemplos:

| Enemigos derrotados | XP |
|---|---:|
| 2 Saqueadores + 2 Ballesteros + 1 Bandido | 160 |
| 1 Escudero + 1 Cultista + 2 Bandidos + 1 Perro | 135 |
| 1 MatÃ³n + 1 Saqueador + 1 Ballestero + 1 Bandido | 145 |

Objetivo real:

```text
2 combates medios
```

---

## Nivel 3 -> 4

XP necesaria adicional: 200  
XP total requerida: 450

Ejemplos:

| Enemigos derrotados | XP |
|---|---:|
| 1 Bestia + 1 MatÃ³n + 1 AcÃ³lito + 1 Cultista | 215 |
| 1 CapitÃ¡n Desertor + 1 Bandido + 1 Ballestero | 195 |
| 1 Bruja del CÃ­rculo + 2 Cuervos + 1 Cultista | 210 |

Objetivo real:

```text
1 combate difÃ­cil + 1 elite
o
3 combates medios/difÃ­ciles
```

---

## Nivel 4 -> 5

XP necesaria adicional: 250  
XP total requerida: 700

Ejemplos:

| Enemigos derrotados | XP |
|---|---:|
| SeÃ±or del Camino | 240 |
| CapitÃ¡n Desertor + Bruja del CÃ­rculo | 280 |
| Bestia + AcÃ³lito + MatÃ³n + Cultista + Saqueador | 255 |

Objetivo real:

```text
jefe normal casi sube un nivel completo
o
2 elites completan la subida
o
varios combates difÃ­ciles
```

---

# 9. ProgresiÃ³n ideal de una regiÃ³n demo

Esta serÃ­a una ruta balanceada de ejemplo.

| Momento | Encuentro | XP aproximada | Nivel esperado |
|---|---|---:|---|
| 1 | 2 Bandidos Rasos | 40 | 1 |
| 2 | Bandido Raso + 2 Perros | 50 | 1 |
| 3 | Saqueador + Ballestero | 70 | 2 |
| 4 | Escudero + Bandido + Cuervo | 75 | 2 |
| 5 | Cultista + AcÃ³lito | 100 | 3 |
| 6 | MatÃ³n + Saqueador + Ballestero | 125 | 3 |
| 7 | Bestia + 2 Perros | 90 | 3-4 |
| 8 | CapitÃ¡n Desertor + apoyo | 200 aprox | 4 |
| 9 | Bruja del CÃ­rculo + apoyo | 210 aprox | 4-5 |
| 10 | SeÃ±or del Camino | 240 | 5 |
| 11 | La Cosa Bajo la AbadÃ­a | 0 | 5 |

La ruta completa da mÃ¡s de 700 XP, pero eso estÃ¡ bien si algunos combates son opcionales o si se ajusta la experiencia entregada por grupo.

---

# 10. Regla importante de balance

La experiencia no tiene que premiar solo matar enemigos. TambiÃ©n puede premiar completar combates/nodos.

RecomendaciÃ³n:

```text
70% de XP viene de enemigos
30% de XP viene de completar contratos, eventos o nodos importantes
```

Esto evita que el jugador sienta que debe farmear.

---

# 11. Tabla compacta de XP y recompensas

| Enemigo | Tier | XP | Recompensa principal |
|---|---:|---:|---|
| Bandido Raso | 1 | 20 | oro chico, cuero |
| Perro Hambriento | 1 | 15 | cuero, comida |
| Cuervo CarroÃ±ero | 1 | 15 | componente menor |
| Saqueador | 2 | 35 | oro, hierro |
| Ballestero | 2 | 35 | madera, cuero |
| Escudero Bandido | 2 | 40 | hierro, placas |
| Cultista Menor | 2 | 40 | cristal |
| MatÃ³n Desertor | 3 | 55 | oro, hierro, cuero |
| Bestia del Monte | 3 | 60 | cuero, comida |
| AcÃ³lito Corrupto | 3 | 60 | cristales |
| CapitÃ¡n Desertor | 4 | 140 | oro, hierro, item fÃ­sico |
| Bruja del CÃ­rculo | 4 | 140 | cristales, item mÃ¡gico |
| SeÃ±or del Camino | 5 | 240 | oro, item tier 3, desbloqueo |
| La Cosa Bajo la AbadÃ­a | 6 | 0 | cierre demo, item Ãºnico |

---

# 12. Pendientes

DespuÃ©s de esta base falta definir:

1. Stats numÃ©ricos de cada enemigo.
2. Habilidades exactas de cada enemigo.
3. Grupos de combate definitivos.
4. Recompensas por nodo/contrato.
5. Items concretos que pueden caer.
6. EconomÃ­a de tienda y crafting.


---

# Fuente: items_armas_armaduras_demo.md

# AbadyyRpg - Ãtems, armas y armaduras de la demo

Estado: borrador inicial de equipamiento.  
Objetivo: definir la estructura de Ã­tems por tier para la demo, separando armas, armaduras y especiales.

---

## 1. LÃ³gica general de tiers

El equipamiento de la demo se organiza en 4 categorÃ­as:

| CategorÃ­a | Lectura |
|---|---|
| Tier 1 | oxidado, roto, improvisado |
| Tier 2 | hierro, confiable, funcional |
| Tier 3 | acero, sÃ³lido, preparaciÃ³n de jefe |
| Especial | efecto Ãºnico que modifica la build |

La idea no es tener muchÃ­simos Ã­tems, sino que cada mejora se sienta clara.

```text
Tier 1 = sobrevivir
Tier 2 = estabilizar build
Tier 3 = preparar jefe
Especial = cambiar comportamiento
```

---

## 2. RelaciÃ³n con el sistema de combate

El sistema defensivo trabaja con tres barras:

```text
HP real
Armadura fÃ­sica
Armadura mÃ¡gica
```

La armadura funciona como vida extra por tipo de daÃ±o:

```text
DaÃ±o fÃ­sico -> Armadura fÃ­sica -> HP
DaÃ±o mÃ¡gico -> Armadura mÃ¡gica -> HP
```

Por eso, las armaduras no son simples reducciones porcentuales. Son reservas defensivas concretas.

El equipo define gran parte de la supervivencia de cada clase.

---

## 3. Familias de armas

Para la demo se definen 4 familias principales de armas:

| Familia | Usuario principal |
|---|---|
| Espadas / mazas / escudos | Defensor |
| Dagas / espadas cortas | Asesino |
| Bastones / focos / grimorios | Mago del CÃ­rculo |
| Cetros / sÃ­mbolos / campanas | AcÃ³lita |

---

# 4. Armas fÃ­sicas

## 4.1 Espadas

Las espadas funcionan como arma fÃ­sica estable. Pueden servir tanto al Defensor como al Asesino, aunque el Defensor las usa mejor si busca balance entre daÃ±o y defensa.

| Tier | Ãtem | Stats / efecto |
|---|---|---|
| Tier 1 | Espada Oxidada | +6 daÃ±o fÃ­sico |
| Tier 2 | Espada de Hierro | +12 daÃ±o fÃ­sico |
| Tier 3 | Espada de Acero | +20 daÃ±o fÃ­sico |
| Especial | Espada de Luz | +16 daÃ±o fÃ­sico, +8 daÃ±o mÃ¡gico, bonus contra culto |
| Especial | Espada Chupasangre | +14 daÃ±o fÃ­sico, vampirismo menor |

### Uso esperado

```text
Espada Oxidada -> inicio de demo
Espada de Hierro -> estabilizaciÃ³n
Espada de Acero -> preparaciÃ³n contra jefes
Espada de Luz -> anti-culto / anti-aberraciÃ³n
Espada Chupasangre -> sustain ofensivo
```

---

## 4.2 Dagas

Las dagas son armas rÃ¡pidas para el Asesino. Tienen menos daÃ±o bruto que las espadas, pero agregan speed o efectos.

| Tier | Ãtem | Stats / efecto |
|---|---|---|
| Tier 1 | Daga Mellada | +5 daÃ±o fÃ­sico, +1 speed |
| Tier 2 | Daga de Hierro | +10 daÃ±o fÃ­sico, +2 speed |
| Tier 3 | Daga de Acero | +16 daÃ±o fÃ­sico, +3 speed |
| Especial | Daga de Sangre Negra | +12 daÃ±o fÃ­sico, aplica sangrado/poison |
| Especial | Daga del Silencio | +10 daÃ±o fÃ­sico, baja taunt o chance de stun |

### Uso esperado

```text
Daga Mellada -> asesino inicial
Daga de Hierro -> asesino funcional
Daga de Acero -> asesino de cierre
Daga de Sangre Negra -> build de daÃ±o por turno
Daga del Silencio -> build de control / evasiÃ³n de foco
```

---

## 4.3 Mazas y martillos

Las mazas y martillos son armas fÃ­sicas pensadas para romper armadura fÃ­sica.

| Tier | Ãtem | Stats / efecto |
|---|---|---|
| Tier 1 | Maza Astillada | +7 daÃ±o fÃ­sico, daÃ±o extra a armadura fÃ­sica |
| Tier 2 | Maza de Hierro | +13 daÃ±o fÃ­sico, daÃ±o extra a armadura fÃ­sica |
| Tier 3 | Martillo de Acero | +21 daÃ±o fÃ­sico, daÃ±o extra fuerte a armadura fÃ­sica |
| Especial | Rompejuramentos | +18 daÃ±o fÃ­sico, mucho daÃ±o a armadura fÃ­sica |
| Especial | Maza Bendita | +15 daÃ±o fÃ­sico, restauraciÃ³n menor de armadura fÃ­sica al usuario |

### Uso esperado

```text
Mazas -> Ãºtiles contra Escudero Bandido, CapitÃ¡n Desertor y enemigos con mucha armadura fÃ­sica.
```

---

# 5. Armas mÃ¡gicas

## 5.1 Bastones

Los bastones son el arma mÃ¡gica directa del Mago del CÃ­rculo.

| Tier | Ãtem | Stats / efecto |
|---|---|---|
| Tier 1 | BastÃ³n Partido | +6 daÃ±o mÃ¡gico |
| Tier 2 | BastÃ³n de Roble Marcado | +12 daÃ±o mÃ¡gico |
| Tier 3 | BastÃ³n del CÃ­rculo | +20 daÃ±o mÃ¡gico |
| Especial | BastÃ³n de Luz FrÃ­a | +16 daÃ±o mÃ¡gico, bonus contra aberraciones |
| Especial | BastÃ³n de MÃ©dula | +14 daÃ±o mÃ¡gico, vampirismo mÃ¡gico menor |

### Uso esperado

```text
Bastones -> daÃ±o mÃ¡gico estable.
BastÃ³n de MÃ©dula -> sustain del Mago del CÃ­rculo.
BastÃ³n de Luz FrÃ­a -> herramienta contra jefe final o culto.
```

---

## 5.2 Focos y grimorios

Los focos y grimorios dan menos daÃ±o directo que un bastÃ³n, pero ofrecen mana o efectos especiales.

| Tier | Ãtem | Stats / efecto |
|---|---|---|
| Tier 1 | Foco Agrietado | +4 daÃ±o mÃ¡gico, +10 mana |
| Tier 2 | Foco de Hierro Ritual | +9 daÃ±o mÃ¡gico, +20 mana |
| Tier 3 | Foco de Cristal | +15 daÃ±o mÃ¡gico, +35 mana |
| Especial | Grimorio del Hambre | +12 daÃ±o mÃ¡gico, mejora poison |
| Especial | Ojo del CÃ­rculo | +10 daÃ±o mÃ¡gico, +speed o mejor target |

### Uso esperado

```text
Focos -> mÃ¡s recursos y consistencia.
Grimorio del Hambre -> build de poison.
Ojo del CÃ­rculo -> build rÃ¡pida/tÃ¡ctica.
```

---

# 6. Armas de soporte

## 6.1 Cetros, sÃ­mbolos y campanas

Estas armas son para la AcÃ³lita. Mejoran curaciÃ³n, soporte, regeneraciÃ³n o restauraciÃ³n defensiva.

| Tier | Ãtem | Stats / efecto |
|---|---|---|
| Tier 1 | SÃ­mbolo Quebrado | +5 poder de curaciÃ³n |
| Tier 2 | Cetro de Hierro | +10 poder de curaciÃ³n, +10 mana |
| Tier 3 | Cetro de Acero Consagrado | +16 poder de curaciÃ³n, +25 mana |
| Especial | Cetro de Luz | +14 curaciÃ³n, +restauraciÃ³n de armadura mÃ¡gica |
| Especial | Campana de los CaÃ­dos | +10 curaciÃ³n, aplica regeneration mÃ¡s fuerte |

### Uso esperado

```text
SÃ­mbolo Quebrado -> soporte inicial
Cetro de Hierro -> soporte estable
Cetro Consagrado -> preparaciÃ³n contra jefes
Cetro de Luz -> soporte defensivo mÃ¡gico
Campana de los CaÃ­dos -> build de regeneraciÃ³n
```

---

# 7. Familias de armaduras

Para la demo se definen 4 familias principales:

| Tipo | Usuario principal |
|---|---|
| Armadura pesada | Defensor |
| Armadura ligera | Asesino |
| TÃºnica arcana | Mago del CÃ­rculo |
| Vestidura / armadura media | AcÃ³lita |

---

# 8. Armaduras pesadas - Defensor

Las armaduras pesadas dan mucha armadura fÃ­sica y poca/media armadura mÃ¡gica. Pueden reducir speed.

| Tier | Ãtem | Armadura fÃ­sica | Armadura mÃ¡gica | Efecto |
|---|---|---:|---:|---|
| Tier 1 | Armadura de Placas Oxidada | 80 | 20 | -1 speed |
| Tier 2 | Armadura de Placas de Hierro | 115 | 30 | -1 speed |
| Tier 3 | Armadura de Placas de Acero | 155 | 45 | -2 speed |
| Especial | Armadura Bendecida Autorreparadora | 130 | 55 | restaura armadura fÃ­sica bajo condiciÃ³n |
| Especial | Coraza del MÃ¡rtir | 145 | 35 | genera taunt extra |

## Efecto recomendado para Armadura Bendecida Autorreparadora

OpciÃ³n balanceada:

```text
Si la armadura fÃ­sica baja de 40%, restaura 20 de armadura fÃ­sica una vez por combate.
```

Evitar que regenere demasiado por turno, porque puede alargar mucho los combates.

---

# 9. Armaduras ligeras - Asesino

Las armaduras ligeras dan menos defensa, pero pueden sumar speed, bajar taunt o mejorar el remate.

| Tier | Ãtem | Armadura fÃ­sica | Armadura mÃ¡gica | Efecto |
|---|---|---:|---:|---|
| Tier 1 | Cuero Gastado | 35 | 15 | +1 speed |
| Tier 2 | Cuero Hervido | 55 | 25 | +2 speed |
| Tier 3 | Cuero Reforzado con Acero | 80 | 35 | +2 speed |
| Especial | Manto del Acechador | 60 | 40 | baja taunt |
| Especial | Cuero de Sangre Negra | 70 | 30 | aumenta daÃ±o contra enemigos heridos |

### Uso esperado

```text
El Asesino no deberÃ­a alcanzar la defensa del Defensor.
Su supervivencia viene de speed, bajo taunt y matar rÃ¡pido.
```

---

# 10. TÃºnicas mÃ¡gicas - Mago del CÃ­rculo

Las tÃºnicas mÃ¡gicas dan poca armadura fÃ­sica, mucha armadura mÃ¡gica y mana.

| Tier | Ãtem | Armadura fÃ­sica | Armadura mÃ¡gica | Efecto |
|---|---|---:|---:|---|
| Tier 1 | TÃºnica Rasgada | 10 | 50 | +10 mana |
| Tier 2 | TÃºnica del CÃ­rculo | 20 | 80 | +20 mana |
| Tier 3 | TÃºnica de Cristal Bordado | 35 | 115 | +35 mana |
| Especial | TÃºnica de Ceniza Viva | 25 | 100 | mejora poison |
| Especial | Manto Antimagia | 20 | 145 | gran defensa mÃ¡gica |

### Uso esperado

```text
El Mago del CÃ­rculo resiste bien el daÃ±o mÃ¡gico, pero debe temer al daÃ±o fÃ­sico.
```

---

# 11. Vestiduras / armadura media - AcÃ³lita

La AcÃ³lita usa protecciÃ³n mixta. No tan dura como el Defensor, no tan frÃ¡gil como el Mago.

| Tier | Ãtem | Armadura fÃ­sica | Armadura mÃ¡gica | Efecto |
|---|---|---:|---:|---|
| Tier 1 | Vestidura Remendada | 25 | 35 | +10 mana |
| Tier 2 | Cota Liviana de Hierro | 40 | 60 | +15 mana |
| Tier 3 | Vestidura Consagrada | 65 | 90 | +25 mana |
| Especial | HÃ¡bito de la Llama Serena | 50 | 100 | mejora curaciones |
| Especial | Manto de la Vigilia | 60 | 75 | buffs duran mÃ¡s |

### Uso esperado

```text
La AcÃ³lita debe sobrevivir mejor que el Mago, pero no puede reemplazar al Defensor.
```

---

# 12. Accesorios opcionales

Los accesorios pueden quedar para despuÃ©s. Si se agregan, deberÃ­an ser simples.

| Ãtem | Efecto |
|---|---|
| Anillo de Hierro | +20 armadura fÃ­sica |
| Medalla de Vidrio | +25 armadura mÃ¡gica |
| Amuleto del Peregrino | +15 mana |
| CordÃ³n de Guerra | +15 stamina |
| TalismÃ¡n de Sangre | +5% vampirismo |
| Sello del CÃ­rculo | +daÃ±o mÃ¡gico |
| Hebilla Pesada | +taunt |
| Broche del Cobarde | baja taunt |

RecomendaciÃ³n:

```text
No implementar accesorios todavÃ­a salvo que haga falta mÃ¡s personalizaciÃ³n.
```

---

# 13. Especiales importantes

Los Ã­tems especiales no deberÃ­an ser solo mejores nÃºmeros. Deben modificar el comportamiento o favorecer una build.

## Armas especiales

| Ãtem | Rol |
|---|---|
| Espada de Luz | anti-culto / hÃ­brida fÃ­sico-mÃ¡gica |
| Espada Chupasangre | sustain por vampirismo |
| Daga de Sangre Negra | sangrado/poison |
| Daga del Silencio | control / baja taunt |
| Rompejuramentos | destruye armadura fÃ­sica |
| Maza Bendita | sustain defensivo |
| BastÃ³n de Luz FrÃ­a | anti-aberraciÃ³n |
| BastÃ³n de MÃ©dula | vampirismo mÃ¡gico |
| Grimorio del Hambre | mejora poison |
| Ojo del CÃ­rculo | velocidad o targeting |
| Cetro de Luz | soporte defensivo mÃ¡gico |
| Campana de los CaÃ­dos | regeneration fuerte |

## Armaduras especiales

| Ãtem | Rol |
|---|---|
| Armadura Bendecida Autorreparadora | tanque sostenido |
| Coraza del MÃ¡rtir | tanque con taunt |
| Manto del Acechador | Asesino menos targeteado |
| Cuero de Sangre Negra | Asesino ejecutor |
| TÃºnica de Ceniza Viva | build de poison |
| Manto Antimagia | defensa contra brujas/culto |
| HÃ¡bito de la Llama Serena | mejor sanaciÃ³n |
| Manto de la Vigilia | mejor booster |

---

# 14. Loot conectado a enemigos

| Enemigo | Puede soltar |
|---|---|
| Bandido Raso | Espada Oxidada, Daga Mellada |
| Saqueador | Maza Astillada, Hierro |
| Ballestero | Cuero Gastado, Madera |
| Escudero Bandido | Placas Oxidadas, Escudo Astillado |
| MatÃ³n Desertor | Espada de Hierro, Cuero Hervido |
| Cultista Menor | Foco Agrietado, Tiza Ritual |
| AcÃ³lito Corrupto | SÃ­mbolo Quebrado, Vestidura Remendada |
| Bestia del Monte | Cuero Reforzado, Piel Gruesa |
| CapitÃ¡n Desertor | Espada de Acero, Coraza del MÃ¡rtir |
| Bruja del CÃ­rculo | Grimorio del Hambre, TÃºnica de Ceniza Viva |
| SeÃ±or del Camino | arma tier 3 garantizada, armadura pesada tier 3 |
| La Cosa Bajo la AbadÃ­a | item Ãºnico de cierre |

---

# 15. Set mÃ­nimo para implementar primero

Para no explotar el scope, el primer paquete deberÃ­a ser este.

## Armas mÃ­nimas

| Rol | Tier 1 | Tier 2 | Tier 3 | Especial |
|---|---|---|---|---|
| Defensor | Espada Oxidada | Espada de Hierro | Espada de Acero | Espada de Luz |
| Asesino | Daga Mellada | Daga de Hierro | Daga de Acero | Daga de Sangre Negra |
| Mago del CÃ­rculo | BastÃ³n Partido | BastÃ³n de Roble | BastÃ³n del CÃ­rculo | Grimorio del Hambre |
| AcÃ³lita | SÃ­mbolo Quebrado | Cetro de Hierro | Cetro Consagrado | Campana de los CaÃ­dos |

## Armaduras mÃ­nimas

| Rol | Tier 1 | Tier 2 | Tier 3 | Especial |
|---|---|---|---|---|
| Defensor | Placas Oxidadas | Placas de Hierro | Placas de Acero | Armadura Autorreparadora |
| Asesino | Cuero Gastado | Cuero Hervido | Cuero Reforzado | Manto del Acechador |
| Mago del CÃ­rculo | TÃºnica Rasgada | TÃºnica del CÃ­rculo | TÃºnica de Cristal | Manto Antimagia |
| AcÃ³lita | Vestidura Remendada | Cota Liviana | Vestidura Consagrada | HÃ¡bito de la Llama Serena |

Este set mÃ­nimo contiene:

```text
16 armas
16 armaduras
```

Es suficiente para una demo y todavÃ­a es manejable.

---

# 16. Especiales disponibles en demo

No conviene que todos los especiales estÃ©n disponibles en la primera demo. Lo ideal es elegir pocos.

RecomendaciÃ³n inicial:

| Especial | Fuente |
|---|---|
| Espada Chupasangre o Espada de Luz | CapitÃ¡n Desertor / SeÃ±or del Camino |
| Grimorio del Hambre | Bruja del CÃ­rculo |
| Armadura Bendecida Autorreparadora | SeÃ±or del Camino |
| Campana de los CaÃ­dos | evento de abadÃ­a / recompensa especial |

La idea es que lo especial se sienta especial.

---

# 17. Pendientes

Luego de esta definiciÃ³n falta:

1. Definir IDs tÃ©cnicos de cada Ã­tem.
2. Definir precio de tienda.
3. Definir materiales de crafting.
4. Definir recetas.
5. Definir quÃ© Ã­tems son loot directo y cuÃ¡les se craftean.
6. Definir valores finales de efectos especiales.
7. Crear los ScriptableObjects en Unity.


---

# Fuente: sprint_economia_contratos_tienda.md

# Sprint â€” EconomÃ­a jugable: contratos, recompensa visible y tienda reutilizable

## 1. Identidad del juego

El juego no debe presentarse solo como â€œmapa + combate automÃ¡ticoâ€.

La identidad principal es:

```text
RPG de caravana, contratos, crafting y gestiÃ³n.
```

El combate es una capa importante, pero no es el centro absoluto. Sirve para:

- conseguir recursos;
- completar contratos;
- conseguir loot;
- probar builds;
- validar el sistema de gambits/tÃ¡cticas;
- abrir consecuencias en el mundo.

El loop principal buscado es:

```text
Aceptar contrato
â†’ preparar caravana
â†’ comprar / craftear / equipar
â†’ salir al mapa
â†’ resolver eventos o combates
â†’ ganar recursos, oro e Ã­tems
â†’ volver a la caravana o pueblo
â†’ vender, craftear, descansar, reparar
â†’ tomar contratos mÃ¡s difÃ­ciles
```

La idea fuerte es que el juego se sienta como una compaÃ±Ã­a de mercenarios que sobrevive mediante gestiÃ³n, preparaciÃ³n, trabajos, decisiones y crafting.

---

## 2. Sistemas prÃ³ximos importantes

Los tres sistemas prioritarios para mostrar mejor la identidad del juego son:

```text
1. UI de recompensa post-combate
2. Contratos
3. Tienda reutilizable
```

Estos tres sistemas juntos forman el primer loop econÃ³mico claro:

```text
Contrato
â†’ combate / evento
â†’ pantalla de recompensa
â†’ inventario
â†’ tienda / crafting
â†’ preparaciÃ³n
â†’ contrato mÃ¡s difÃ­cil
```

---

## 3. UI de recompensa post-combate

Hace falta una pantalla de victoria que muestre claramente lo ganado.

Ejemplo:

```text
Victoria

Recompensas:
+25 oro
+2 comida
+1 hierro
Rusty Sword x1

[Continuar]
```

### Objetivo

Que cada combate tenga cierre visual.

Que el jugador entienda quÃ© ganÃ³.

Que el loot tenga peso.

Que el sistema de recompensas no se sienta invisible o escondido en el inventario.

### Scope V1

Para la primera versiÃ³n alcanza con:

- tÃ­tulo de victoria;
- lista de oro / recursos obtenidos;
- lista de Ã­tems obtenidos;
- botÃ³n continuar;
- aplicar recompensa al inventario runtime;
- volver al mapa despuÃ©s de continuar.

No hace falta todavÃ­a:

- animaciones complejas;
- rarezas visuales;
- pantalla de comparaciÃ³n de equipo;
- loot random profundo;
- sonidos finales;
- pantalla hermosa final.

---

## 4. Contratos

Los contratos son el motor que le da direcciÃ³n al jugador.

No son solo quests genÃ©ricas. Son trabajos que la compaÃ±Ã­a acepta para conseguir oro, recursos, reputaciÃ³n o acceso a nuevos lugares.

### Estados posibles

```text
Disponible
Aceptado
Rechazado
Completado
Fallido
Expirado
```

### Datos mÃ­nimos de un contrato

```text
contractId
tÃ­tulo
descripciÃ³n
empleador
ubicaciÃ³n
deadline
recompensa
objetivo principal
objetivos secundarios
estado
```

### Ejemplo

```text
Contrato: Erradicar a los bandidos del bosque

Deadline: DÃ­a 4
Recompensa: 120 oro, 3 comida, 1 hierro

Objetivo principal:
- Derrotar al lÃ­der bandido

Objetivos secundarios:
- Encontrar el campamento
- Destruir el alijo
- Matar al capitÃ¡n
```

### FunciÃ³n de gameplay

El contrato debe responder:

```text
Â¿QuiÃ©n me paga?
Â¿QuÃ© tengo que hacer?
Â¿CuÃ¡nto tiempo tengo?
Â¿QuÃ© gano?
Â¿QuÃ© pasa si fallo?
Â¿DÃ³nde tengo que ir?
```

Esto le da direcciÃ³n al mapa y hace que los nodos tengan propÃ³sito.

---

## 5. Tienda reutilizable

La tienda no deberÃ­a ser una tienda hardcodeada, sino un sistema genÃ©rico reutilizable por diferentes pueblos, ciudades o NPCs.

Cada tienda deberÃ­a poder tener su propio inventario, precios y reglas.

### Datos mÃ­nimos de tienda

```text
storeId
nombre
lista de Ã­tems en venta
precios
modificador de precio
permite comprar
permite vender
```

### Regla base de precio

```text
Precio de tienda = valor base del Ã­tem + 50%
```

DespuÃ©s cada ciudad o pueblo puede modificar esa regla.

### Ejemplos de tiendas por lugar

```text
Ciudad Central:
- armas
- armaduras
- pociones
- pergaminos de revivir
- comida a precio normal
```

```text
Pueblo de montaÃ±a:
- comida mÃ¡s cara
- armas mÃ¡s dÃ©biles
- hierbas
- cuero
- minerales
```

```text
Aldea pobre:
- comida simple
- cuero
- herramientas bÃ¡sicas
- mejor stock si la ayudaste
```

### Worldbuilding desde economÃ­a

La tienda debe contar cÃ³mo vive cada lugar.

Si un pueblo de montaÃ±a tiene comida cara, hierbas, armas malas y minerales, el jugador entiende algo del mundo sin necesidad de un diÃ¡logo largo.

Si la ciudad central vende pociones, pergaminos de revivir, armas y armaduras, se siente como centro comercial y polÃ­tico.

Si una aldea pobre solo vende comida simple y herramientas, se siente vulnerable.

---

## 6. Venta

Por ahora se puede dejar la venta disponible globalmente para testear el loop.

MÃ¡s adelante, la venta deberÃ­a depender del contexto.

Regla futura:

```text
Vender solo deberÃ­a habilitarse si estÃ¡s en un pueblo o ciudad con tienda.
```

Esto harÃ­a que el lugar actual importe mÃ¡s.

TambiÃ©n permitirÃ­a que algunas zonas no permitan vender, obligando al jugador a planificar mejor quÃ© cargar, quÃ© guardar y cuÃ¡ndo volver.

---

## 7. Vistas dentro de CaravanScene

La CaravanScene deberÃ­a funcionar como centro de gestiÃ³n.

Vistas principales:

```text
Tienda actual
Contratos
Diario de misiÃ³n
Registro / logs
```

La caravana no debe ser solamente descanso y roster. Tiene que ser el lugar donde el jugador entiende la run, prepara la prÃ³xima salida y administra lo ganado.

---

## 8. Contratos, diario y logs no son lo mismo

Conviene separar estos conceptos aunque en V1 puedan compartir datos.

### Contratos

Son trabajos formales.

Responden:

```text
QuiÃ©n paga.
QuÃ© hay que hacer.
CuÃ¡nto paga.
Deadline.
Estado del contrato.
```

### Diario de misiÃ³n

Es la vista narrativa/progresiva.

Muestra:

```text
QuÃ© se descubriÃ³.
QuÃ© pistas hay.
QuÃ© objetivos se actualizaron.
QuÃ© puede hacerse antes del final.
```

### Logs

Es el historial frÃ­o de lo ocurrido.

Ejemplo:

```text
DÃ­a 1 â€” Aceptaste el contrato: Erradicar a los bandidos del bosque.
DÃ­a 2 â€” Destruiste el alijo de suministros.
DÃ­a 3 â€” Mataste al capitÃ¡n bandido.
DÃ­a 3 â€” El combate final tendrÃ¡ un elite menos.
```

El log sirve para que el jugador entienda por quÃ© el mundo cambiÃ³.

---

## 9. Idea fuerte: preparaciÃ³n narrativa del combate

Esta es una de las ideas principales del diseÃ±o.

Los eventos del mapa no solo dan recompensas. TambiÃ©n pueden modificar el combate final.

El jugador no solo hace sidequests. Prepara el terreno.

### Ejemplo: contrato de bandidos del bosque

Objetivo principal:

```text
Derrotar al lÃ­der bandido.
```

Eventos opcionales:

```text
Encontraste el campamento
â†’ desbloquea ruta al jefe.
```

```text
Destruiste el alijo de suministros
â†’ los enemigos pierden consumibles.
â†’ el loot de comida puede ser menor porque quemaste parte del botÃ­n.
```

```text
Mataste al capitÃ¡n bandido
â†’ el combate final tiene un elite menos.
â†’ loot extra: insignia del capitÃ¡n.
```

```text
Ayudaste a un aldeano vengativo
â†’ se infiltra antes del ataque.
â†’ los enemigos empiezan con Quemadura.
```

```text
Aceptaste soborno de los bandidos
â†’ falla el contrato de la aldea.
â†’ se desbloquea recompensa alternativa.
```

Cada decisiÃ³n debe tener dos capas:

```text
Narrativa: pasÃ³ algo en el mundo.
MecÃ¡nica: cambiÃ³ una pelea, recompensa, tienda o estado de contrato.
```

Ese es el corazÃ³n del sistema.

---

## 10. Vista ideal de contratos

La UI de contratos podrÃ­a organizarse asÃ­:

```text
[Contratos]

Tabs:
Disponibles | Aceptados | Completados | Fallidos

Lista izquierda:
- Erradicar bandidos del bosque
- Escoltar caravana minera
- Recuperar reliquia del pantano

Panel derecho:
TÃ­tulo
Empleador
DescripciÃ³n
Deadline
Recompensas
Estado

Objetivo principal:
[ ] Derrotar al lÃ­der bandido

Avances:
[âœ“] Encontrar el campamento
[âœ“] Destruir el alijo
[âœ“] Matar al capitÃ¡n
[ ] Enfrentar al lÃ­der

Consecuencias detectadas:
- El capitÃ¡n no participarÃ¡ del combate final.
- Los enemigos empiezan con Quemadura.
- Los bandidos no tendrÃ¡n consumibles.
```

La UI tiene que juntar gameplay y narrativa en un solo lugar.

---

## 11. Scope V1 recomendado

No hacer un sistema gigante de golpe.

Para V1 alcanza con:

```text
Contrato aceptable/completable
Deadline visible
Recompensa visible
Objetivo principal
Avances simples
Flags que modifican combate final
UI bÃ¡sica de contratos
UI bÃ¡sica de recompensa
Tienda bÃ¡sica reutilizable
```

No hacer todavÃ­a:

```text
contratos infinitos
generaciÃ³n procedural
reputaciÃ³n compleja
economÃ­a dinÃ¡mica
mÃºltiples pantallas hermosas
tienda con stock cambiante avanzado
sistema profundo de rumores
quest journal gigante
```

---

## 12. Sprint propuesto

Nombre:

```text
Sprint â€” EconomÃ­a jugable: contratos, recompensa visible y tienda reutilizable
```

### Objetivo

Mostrar que el juego es de gestiÃ³n, crafting y contratos, no solamente de combate.

### Resultado esperado

Al terminar este sprint, el jugador deberÃ­a poder:

```text
Aceptar un contrato
â†’ salir al mapa
â†’ resolver eventos o combate
â†’ ver recompensas claramente
â†’ volver a gestionar recursos
â†’ comprar / vender / craftear
â†’ prepararse para otro contrato
```

Este serÃ­a el primer momento donde el juego empieza a mostrar su identidad real.

---

## 13. Orden sugerido de implementaciÃ³n

```text
1. Revisar sistema actual de recompensas.
2. Crear UI de recompensa post-combate.
3. Conectar RewardData con pantalla de victoria.
4. Crear estructura bÃ¡sica de ContractData.
5. Crear ContractRuntimeState o ContractManager runtime.
6. Crear panel de contratos en CaravanScene.
7. Permitir aceptar contrato.
8. Permitir completar contrato cuando se cumpla un objetivo.
9. Mostrar estado: disponible / aceptado / completado / fallido.
10. Agregar deadline visible.
11. Crear StoreInventory reutilizable.
12. Crear panel de tienda actual en CaravanScene.
13. Mostrar stock y precio.
14. Comprar Ã­tems con oro.
15. Dejar venta global por ahora o conectarla al panel de tienda si es simple.
16. Crear un contrato de prueba: bandidos del bosque.
17. Crear eventos opcionales que agreguen flags/modificadores.
18. Hacer que el combate final lea esos modificadores.
19. Mostrar consecuencias en diario/log.
20. Testear loop completo.
```

---

## 14. Test final del sprint

El test mÃ­nimo deberÃ­a ser:

```text
1. Entrar a CaravanScene.
2. Abrir panel de contratos.
3. Aceptar contrato de bandidos del bosque.
4. Salir al mapa.
5. Resolver evento para encontrar campamento.
6. Resolver evento para destruir alijo.
7. Resolver combate contra capitÃ¡n.
8. Ver avances en diario/contrato.
9. Entrar al combate final.
10. Confirmar que el elite no aparece o que los enemigos tienen debuff.
11. Ganar combate.
12. Ver pantalla de recompensa.
13. Volver a CaravanScene.
14. Ver contrato completado.
15. Ver recursos en inventario.
16. Comprar algo en tienda.
17. Craftear o preparar la party para el siguiente contrato.
```

Si este test pasa, el juego ya muestra claramente su identidad.

---

## 15. Frase guÃ­a del diseÃ±o

```text
El jugador no solo pelea: acepta trabajos, prepara la caravana, administra recursos, toma decisiones, modifica el terreno antes del combate y convierte el riesgo en progreso.
```


---

# Fuente: Assets\Auditoria\AUDITORIA_LIMPIEZA_DEMO.md

# Auditoria de limpieza - Demo Tier 1

Estado: auditoria inicial.
Rama: `codex/demo-tier1-cleanup`.
Checkpoint previo: `2ae2592 checkpoint: demo tier1 pre-cleanup`.

Objetivo:

```text
Reducir ruido del proyecto antes de implementar la demo Tier 1.
No borrar directo: mover a cuarentena, probar, y borrar mas adelante por tandas.
```

---

## Regla de seguridad

- No borrar assets directamente durante la primera pasada.
- Mover con `.meta` para conservar GUIDs.
- Si Unity sigue compilando y la demo funciona, recien ahi considerar borrado por tandas.
- Los scripts de editor deben quedar dentro de una carpeta `Editor`, incluso si estan en cuarentena.

---

## Movido a cuarentena en esta pasada

### Datos viejos duplicados

Destino:

```text
Assets/Auditoria/Cuarentena/GameDataViejo
```

Movido:

- `Assets/Abilities`
- `Assets/Items`
- `Assets/Crafting`

Motivo:

```text
Esos datos conviven con `Assets/GameData`, que es la estructura mas nueva.
Para la demo Tier 1 conviene trabajar desde `Assets/GameData` y no mantener dos fuentes paralelas.
```

Riesgo:

```text
Medio.
Si alguna escena o prefab referenciaba esos assets, los GUIDs se conservaron al moverlos con `.meta`.
Si algun editor script viejo usaba paths fijos hacia `Assets/Items`, ese script ya no deberia ser usado para la demo nueva.
```

---

### Scripts de test

Destino:

```text
Assets/Auditoria/Cuarentena/Scripts/TestScripts
```

Movido:

- `Assets/Scripts/Core/TestScripts/BattleLogic.cs`
- `Assets/Scripts/Core/TestScripts/BattleWaveTester.cs`

Motivo:

```text
Son scripts de prueba directa de combate.
No forman parte del flujo demo Tier 1 WorldMap -> BattleScene -> WorldMap.
```

Riesgo:

```text
Bajo/medio.
Siguen dentro de `Assets`, por lo que Unity todavia puede compilarlos.
Si no se usan en escenas, luego se pueden borrar.
```

---

### Generadores viejos de editor

Destino:

```text
Assets/Auditoria/Editor/Cuarentena/GeneratorsViejos
```

Movido:

- `Assets/Editor/EnemyPrefabGeneratorWindow.cs`
- `Assets/Editor/StarterAbilityCreator.cs`
- `Assets/Editor/StarterItemCreator.cs`

Motivo:

```text
Fueron reemplazados por generadores mas nuevos orientados a `Assets/GameData`.
Mantenerlos visibles aumenta el riesgo de generar contenido viejo por accidente.
```

Riesgo:

```text
Bajo.
Siguen dentro de una carpeta `Editor`, asi que no deberian romper compilacion runtime.
```

---

### Archivo no utilizable en Unity

Destino:

```text
Assets/Auditoria/Cuarentena/Sprites
```

Movido:

- `Assets/Sprites/GeneratedUI/enemigos/Mount.and.Blade.II.Bannerlord.v1.4.6.115628-P2P.torrent`

Motivo:

```text
No es un asset util del proyecto ni de la demo.
Queda en cuarentena para borrado posterior.
```

Riesgo:

```text
Muy bajo.
```

---

## Mantener activo por ahora

### Carpetas principales

- `Assets/GameData`
- `Assets/Scripts`
- `Assets/WorldMap`
- `Assets/Scenes`
- `Assets/Prefabs`
- `Assets/Sprites`
- `Assets/Pixel Monster Pack`
- `Assets/Fonts`
- `Assets/TextMesh Pro`

Motivo:

```text
Contienen sistemas activos, escenas, prefabs, sprites o dependencias que todavia pueden estar referenciadas.
```

---

## Candidatos para revisar despues

### Arte conceptual viejo

- `Assets/abadia`
- `Assets/mapas`
- `Assets/Sprites/worldmapconcept6.png`

Lectura:

```text
Probablemente no entra en la demo Tier 1 actual, pero conviene revisar referencias visuales antes de mover.
```

### Prefabs viejos de unidades

- `Assets/Prefabs/Enemies`
- `Assets/Prefabs/Mercs`

Lectura:

```text
El sistema nuevo puede crear enemigos desde `EnemyDefinitionSO`, pero todavia puede haber prefabs usados por BattleSetup o escenas.
No mover hasta confirmar flujo de batalla.
```

### GameData viejo dentro de GameData

- clases de 4 personajes;
- enemigos fuera de rata/esqueleto/gusano/demonio menor;
- encuentros que no son de la demo Tier 1;
- armas/armaduras Tier 2 o especiales.

Lectura:

```text
No mover todavia.
Primero crear `GameData/DemoTier1` o identificar exactamente que SOs usa la demo final.
```

### UI y sprites genericos

- sprites de `GeneratedUI` no usados por el panel de eventos;
- portraits y sheets no usados;
- ejemplos de UI.

Lectura:

```text
Moverlos despues de probar escenas, porque las referencias visuales en Unity son faciles de romper silenciosamente.
```

---

## Proxima pasada recomendada

1. Compilar C#.
2. Abrir Unity y dejar que reimporte.
3. Revisar errores de consola.
4. Probar escenas principales:
   - `WorldMapScene`;
   - `BattleScene`;
   - `CaravanScene`.
5. Si todo funciona, hacer commit de cuarentena inicial.
6. Recien despues seguir con segunda tanda:
   - arte conceptual viejo;
   - prefabs viejos;
   - SOs fuera del scope Tier 1.



---

# Fuente: ProjectSettings\ProjectVersion.txt

m_EditorVersion: 2022.3.62f3
m_EditorVersionWithRevision: 2022.3.62f3 (96770f904ca7)

