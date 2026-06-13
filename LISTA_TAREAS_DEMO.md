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

- [ ] Definir recursos iniciales reales.
- [ ] Definir equipo inicial real.
- [ ] Definir si el personaje arranca con habilidades base o debe desbloquearlas/craftearlas.
- [ ] Configurar party activa con 1 solo personaje.
- [ ] Congelar party de 4 para esta demo.
- [ ] Ajustar stats base del personaje al scope chico.
- [ ] Ajustar XP/levels al ritmo de 10 dias.

---

## 2. Completar habilidades genericas

- [ ] Definir numeros finales de cada habilidad.
- [ ] Crear/ajustar `AbilitySO` de ataque poderoso.
- [ ] Crear/ajustar `AbilitySO` de misil magico.
- [ ] Crear/ajustar `AbilitySO` de curar.
- [ ] Crear/ajustar `AbilitySO` de regeneracion.
- [ ] Crear/ajustar `AbilitySO` de cleanse.
- [ ] Crear/ajustar `AbilitySO` de rompebrazo.
- [ ] Crear/ajustar `AbilitySO` de guardia.
- [ ] Crear/ajustar `AbilitySO` de escudo magico.
- [ ] Revisar que habilidades son posibles con el sistema actual.
- [ ] Marcar habilidades que requieren codigo nuevo.

---

## 3. Alinear consumibles

- [ ] Pocion de salud cura 4 HP.
- [ ] Molotov hace 3 dano fijo o magico/directo.
- [ ] Pergamino revive automaticamente con 30% HP.
- [ ] Decidir si consumibles usan sistema actual de condiciones o tacticas.
- [ ] Ajustar uso automatico para que no contradiga el diseno.
- [ ] Confirmar consumo real del item al usarlo.

---

## 4. Alinear recompensas

- [ ] Agregar soporte de piedra a `RewardData`.
- [ ] Agregar soporte de cristales a `RewardData`.
- [ ] Agregar aplicacion de piedra en `RewardApplier`.
- [ ] Agregar aplicacion de cristales en `RewardApplier`.
- [ ] Definir recompensas por enemigo.
- [ ] Definir recompensas por nodo repetible.
- [ ] Definir recompensas de eventos.
- [ ] Evitar loops de farmeo infinito rentable.
- [ ] Confirmar que XP se aplica solo al personaje activo.

---

## 5. Alinear economia de desgaste

- [ ] Implementar o ajustar accion de buscar recursos.
- [ ] Buscar recursos cuesta 4 horas.
- [ ] Buscar recursos cuesta 3 stamina del personaje o decidir reemplazo.
- [ ] Buscar recursos da 1 recurso aleatorio basico o 1 comida.
- [ ] Buscar recursos da 0 XP.
- [ ] Revisar si eventos pueden consumir stamina del personaje.
- [ ] Si no se agrega stamina de personaje en eventos, decidir usar stamina de caravana.
- [ ] Hacer que descansar tenga costo de tiempo si corresponde.
- [ ] Hacer que reparar armaduras tenga costo de tiempo si corresponde.
- [ ] Confirmar que ganar mal genera coste real.

---

## 6. Tiempo y derrota

- [ ] Implementar regla de derrota por tiempo.
- [ ] Si empieza el dia 11 y `flag_jefe_derrotado` es falso, la expedicion fracasa.
- [ ] Decidir donde se muestra esa derrota.
- [ ] Decidir si vuelve a menu, pantalla final o estado bloqueado de demo.
- [ ] Asegurar que dormir al dia 11 dispare fracaso si no se derroto al jefe.
- [ ] Asegurar que viajar pasando al dia 11 dispare fracaso si no se derroto al jefe.

---

## 7. Caravana y descanso

- [ ] Revisar descanso parcial actual.
- [ ] Agregar costo de horas al descanso parcial si falta.
- [ ] Agregar costo de stamina de caravana al descanso parcial si se mantiene.
- [ ] Revisar descanso completo.
- [ ] Confirmar que descanso completo avanza dia.
- [ ] Confirmar que descanso completo cura 100% HP.
- [ ] Confirmar que descanso completo recupera 100% stamina.
- [ ] Confirmar que descanso completo recupera 100% mana.
- [ ] Revisar reparacion de armadura fisica.
- [ ] Revisar reparacion de armadura magica.
- [ ] Agregar costo de horas a reparaciones si falta.
- [ ] Alinear UI de hoguera con costos reales.

---

## 8. Items Tier 1

- [ ] Crear/ajustar daga mellada.
- [ ] Crear/ajustar espada oxidada.
- [ ] Crear/ajustar baston partido.
- [ ] Crear/ajustar simbolo quebrado.
- [ ] Crear/ajustar placas oxidadas.
- [ ] Crear/ajustar cuero gastado.
- [ ] Crear/ajustar tunica rasgada.
- [ ] Crear/ajustar vestidura remendada.
- [ ] Crear/ajustar pocion de salud.
- [ ] Crear/ajustar molotov.
- [ ] Crear/ajustar pergamino de revivir.
- [ ] Crear/ajustar recetas Tier 1.
- [ ] Revisar iconos/placeholders.
- [ ] Revisar valores de venta si se usa venta.

---

## 9. Enemigos Tier 1

- [ ] Crear/ajustar `EnemyDefinitionSO` de rata gigante.
- [ ] Crear/ajustar `EnemyDefinitionSO` de esqueleto.
- [ ] Crear/ajustar `EnemyDefinitionSO` de gusano.
- [ ] Crear/ajustar `EnemyDefinitionSO` de demonio menor.
- [ ] Asignar sprites/icons.
- [ ] Asignar habilidades.
- [ ] Asignar tacticas.
- [ ] Asignar recompensas.
- [ ] Crear encuentro de 1 rata.
- [ ] Crear encuentro de 2 ratas.
- [ ] Crear encuentro de rata + esqueleto.
- [ ] Crear encuentro de 1 esqueleto.
- [ ] Crear encuentro de 1 gusano.
- [ ] Crear encuentro de esqueleto + gusano.
- [ ] Crear encuentro de demonio menor.

---

## 10. Mapa demo

- [ ] Crear/ajustar nodo Valdoran.
- [ ] Crear/ajustar nodo Camino 01.
- [ ] Crear/ajustar nodo Camino 02.
- [ ] Crear/ajustar nodo Puente roto.
- [ ] Crear/ajustar nodo Cruce del Vigia.
- [ ] Crear/ajustar nodo Bosque de Aldheron.
- [ ] Crear/ajustar nodo Claro de las Rocas.
- [ ] Crear/ajustar nodo Torre Vigia.
- [ ] Crear/ajustar nodo Mina de Hierro Negro.
- [ ] Crear/ajustar nodo Collinasombra.
- [ ] Crear/ajustar nodo Portal de los Excavadores.
- [ ] Crear/ajustar nodo Frontera abierta.
- [ ] Crear 10 caminos.
- [ ] Asignar estados iniciales.
- [ ] Asignar flags requeridas.
- [ ] Ajustar colores de caminos.
- [ ] Acomodar posiciones en `worldmapconcept7`.

---

## 11. Eventos

- [ ] Crear evento Puente roto.
- [ ] Crear evento Pista del jefe.
- [ ] Crear evento Collinasombra.
- [ ] Crear evento Jefe Demonio menor.
- [ ] Configurar opciones.
- [ ] Configurar costos.
- [ ] Configurar flags.
- [ ] Configurar recompensas.
- [ ] Confirmar que aparecen solo los botones necesarios.

---

## 12. Battle flow

- [ ] Confirmar que `WorldMapScene` carga `BattleScene`.
- [ ] Confirmar que `BattleScene` vuelve a `WorldMapScene`.
- [ ] Confirmar que el encuentro correcto carga enemigos.
- [ ] Confirmar que se aplican recompensas al volver.
- [ ] Confirmar que se conserva estado del nodo.
- [ ] Confirmar que derrota en combate vuelve a Valdoran.
- [ ] Confirmar que consumibles gastados no vuelven.
- [ ] Confirmar que HP persiste como esperamos.
- [ ] Confirmar que stamina persiste como esperamos.
- [ ] Confirmar que mana persiste como esperamos.
- [ ] Confirmar que armaduras persisten como esperamos.

---

## 13. UI minima necesaria

- [ ] HUD de mapa muestra dia.
- [ ] HUD de mapa muestra hora.
- [ ] HUD de mapa muestra stamina de caravana.
- [ ] HUD de mapa muestra oro.
- [ ] HUD de mapa muestra ubicacion.
- [ ] Caravana muestra recursos.
- [ ] Caravana muestra personaje.
- [ ] Caravana muestra equipo.
- [ ] Caravana muestra HP/stamina/mana.
- [ ] Caravana muestra armaduras.
- [ ] Caravana permite crafting.
- [ ] Caravana permite descanso/reparacion.
- [ ] Combate muestra vida.
- [ ] Combate muestra armadura fisica.
- [ ] Combate muestra armadura magica.
- [ ] Combate muestra turnos.
- [ ] Combate muestra feedback de dano.
- [ ] Evitar agregar pantallas nuevas si una existente sirve.

---

## 14. Balance de ruta completa

- [ ] Simular ruta minima.
- [ ] Simular ruta con farmeo malo.
- [ ] Simular ruta optima.
- [ ] Ver en que dia llega al jefe cada ruta.
- [ ] Ajustar recompensas/costos.
- [ ] Ajustar HP/dano enemigos.
- [ ] Ajustar recetas.
- [ ] Ajustar XP.
- [ ] Confirmar que el jefe es dificil pero justo.

---

## 15. Limpieza de scope

- [ ] Marcar sistemas congelados para no tocarlos.
- [ ] No usar contratos.
- [ ] No usar tienda dinamica.
- [ ] No usar reclutamiento.
- [ ] No usar party de 4.
- [ ] No usar tiers 2/3.
- [ ] No agregar enemigos nuevos.
- [ ] No agregar eventos aleatorios.
- [ ] No agregar economia local.

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

