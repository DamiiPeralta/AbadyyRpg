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
