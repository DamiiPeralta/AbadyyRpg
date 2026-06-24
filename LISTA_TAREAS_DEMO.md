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
