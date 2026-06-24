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
