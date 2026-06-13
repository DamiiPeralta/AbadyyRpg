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

