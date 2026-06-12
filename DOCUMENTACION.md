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

- Hay textos con caracteres mal codificados en algunos scripts (`InformaciÃ³n`, `DaÃ±o`, etc.). No rompe logica, pero conviene normalizar encoding a UTF-8.
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
