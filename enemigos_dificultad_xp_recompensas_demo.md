# AbadyyRpg - Enemigos, dificultad, experiencia y recompensas de demo

Estado: borrador inicial de balance.  
Objetivo: asignar dificultad, experiencia, progresión por niveles y recompensas a los enemigos definidos para la demo.

---

## 1. Base del sistema

La demo trabaja con una party de 4 clases fijas:

| Rol | Clase |
|---|---|
| Tanque | Defensor |
| DPS físico | Asesino |
| DPS mágico | Mago del Círculo |
| Sanador / booster | Acólita |

El combate se piensa como automático por tácticas/gambits. El jugador prepara la party antes de combatir mediante equipo, habilidades, consumibles y reglas de prioridad.

El sistema defensivo se lee así:

```text
Daño físico -> Armadura física -> HP
Daño mágico -> Armadura mágica -> HP
```

La armadura funciona como vida extra separada por tipo de daño. No es reducción porcentual.

---

## 2. Nivel máximo de demo

La demo se balancea con nivel máximo 5.

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

La idea es que el jugador llegue a nivel 5 solamente si completa buena parte del contenido de la región.

---

## 3. Tiers de dificultad

Los enemigos se dividen en tiers.

| Tier | Nombre | Uso |
|---|---|---|
| Tier 1 | Básico | primeros combates, tutorial natural |
| Tier 2 | Común | núcleo de encuentros normales |
| Tier 3 | Peligroso | enemigos normales fuertes |
| Tier 4 | Élite | encuentros especiales/opcionales |
| Tier 5 | Jefe normal | cierre del conflicto regional |
| Tier 6 | Jefe final | cierre de demo |

---

## 4. Valores base por tier

| Tier | Dificultad | XP individual aproximada | Recompensa esperada |
|---|---|---:|---|
| Tier 1 | Baja | 15-25 | recursos chicos |
| Tier 2 | Media | 30-40 | oro + recurso útil |
| Tier 3 | Alta normal | 45-60 | recurso bueno / item común |
| Tier 4 | Élite | 120-150 | item fuerte / recurso raro |
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
| Daño principal | Físico |
| Rol | Atacante básico |
| XP | 20 |

### Función de combate

Introduce el flujo básico del combate:

```text
daño físico -> armadura física -> HP
```

No debería tener mecánicas complejas.

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
| Daño principal | Físico |
| Rol | Atacante rápido/frágil |
| XP | 15 |

### Función de combate

Prueba speed y presión por múltiples turnos. Pega poco, pero actúa seguido.

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
Piel dañada
Colmillo
```

---

## 5.3 Cuervo Carroñero

| Campo | Valor |
|---|---|
| Tier | 1 |
| Dificultad | Baja |
| Tipo | Bestia |
| Daño principal | Físico/control menor |
| Rol | Molestia rápida |
| XP | 15 |

### Función de combate

Introduce interrupciones leves, reducción de speed o stun corto si se decide usarlo.

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
| Daño principal | Físico |
| Rol | Golpeador lento |
| XP | 35 |

### Función de combate

Prueba si el Defensor está absorbiendo correctamente el daño físico.

Si el Saqueador golpea a una unidad blanda, debería sentirse peligroso.

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
| Daño principal | Físico |
| Rol | Presión a backline / bajo taunt |
| XP | 35 |

### Función de combate

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
| Daño principal | Físico bajo |
| Rol | Tanque enemigo |
| XP | 40 |

### Función de combate

Tiene armadura física alta. Enseña que no todo se resuelve con daño físico.

El Mago del Círculo debería ser útil contra este enemigo.

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
Placas dañadas
```

---

## 5.7 Cultista Menor

| Campo | Valor |
|---|---|
| Tier | 2 |
| Dificultad | Media |
| Tipo | Culto |
| Daño principal | Mágico |
| Rol | Caster básico |
| XP | 40 |

### Función de combate

Introduce daño mágico:

```text
daño mágico -> armadura mágica -> HP
```

Debe castigar especialmente al Defensor si no tiene buena armadura mágica.

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

## 5.8 Matón Desertor

| Campo | Valor |
|---|---|
| Tier | 3 |
| Dificultad | Alta normal |
| Tipo | Humano |
| Daño principal | Físico |
| Rol | Enemigo balanceado fuerte |
| XP | 55 |

### Función de combate

Primer enemigo normal serio. Tiene daño, aguante y poca debilidad obvia.

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
| Daño principal | Físico |
| Rol | Bruto físico |
| XP | 60 |

### Función de combate

Prueba si el Defensor y la Acólita pueden sostener una pelea pesada.

Debe ser el enemigo normal físico más amenazante.

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

## 5.10 Acólito Corrupto

| Campo | Valor |
|---|---|
| Tier | 3 |
| Dificultad | Alta normal |
| Tipo | Culto |
| Daño principal | Mágico bajo / soporte |
| Rol | Healer / buffer enemigo |
| XP | 60 |

### Función de combate

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

# 6. Élites

## 6.1 Capitán Desertor

| Campo | Valor |
|---|---|
| Tier | 4 |
| Dificultad | Élite físico |
| Tipo | Humano |
| Daño principal | Físico |
| Rol | Líder táctico |
| XP | 140 |

### Función de combate

Examen de la rama humana/bandida.

Prueba:

```text
taunt
armadura física
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
Item de arma física tier 2 o tier 3 inicial
```

Loot opcional:

```text
Insignia de desertor
Espada de capitán
```

---

## 6.2 Bruja del Círculo

| Campo | Valor |
|---|---|
| Tier | 4 |
| Dificultad | Élite mágico |
| Tipo | Culto |
| Daño principal | Mágico / poison |
| Rol | Desgaste mágico |
| XP | 140 |

### Función de combate

Examen de magia, estados y supervivencia larga.

Prueba:

```text
armadura mágica
curación
regeneración
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
Item mágico tier 2 o tier 3 inicial
```

Loot opcional:

```text
Bastón torcido
Velo ritual
```

---

# 7. Jefes

## 7.1 Señor del Camino

| Campo | Valor |
|---|---|
| Tier | 5 |
| Dificultad | Jefe normal |
| Tipo | Humano |
| Daño principal | Físico |
| Rol | Jefe regional |
| XP | 240 |

### Función de combate

Cierre del conflicto humano de la región.

Prueba:

```text
armadura física
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
Desbloquea acceso a la Abadía / jefe final
```

---

## 7.2 La Cosa Bajo la Abadía

| Campo | Valor |
|---|---|
| Tier | 6 |
| Dificultad | Jefe final |
| Tipo | Aberración / culto |
| Daño principal | Físico y mágico |
| Rol | Cierre de demo |
| XP | 0 |

### Función de combate

Examen final del sistema.

Prueba:

```text
armadura física
armadura mágica
curación
poison
taunt
daño en área
resistencia de recursos
```

### Recompensas

Como es cierre de demo, no necesita XP.

Recompensa sugerida:

```text
Cierre narrativo
Objeto único de demo
Pantalla de victoria
Desbloqueo simbólico para futura región
```

Loot opcional:

| Recompensa | Cantidad |
|---|---:|
| Oro | 150 |
| Cristales | 6 |
| Hierro | 3 |
| Cuero | 3 |

Loot único:

```text
Corazón Negro de la Abadía
```

---

# 8. Cuántos enemigos hacen falta para subir de nivel

La progresión no debería medirse solamente en enemigos sueltos, sino en grupos de combate. Sin embargo, esta tabla sirve como referencia.

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
2 combates fáciles o 1 combate fácil + 1 medio
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
| 1 Matón + 1 Saqueador + 1 Ballestero + 1 Bandido | 145 |

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
| 1 Bestia + 1 Matón + 1 Acólito + 1 Cultista | 215 |
| 1 Capitán Desertor + 1 Bandido + 1 Ballestero | 195 |
| 1 Bruja del Círculo + 2 Cuervos + 1 Cultista | 210 |

Objetivo real:

```text
1 combate difícil + 1 elite
o
3 combates medios/difíciles
```

---

## Nivel 4 -> 5

XP necesaria adicional: 250  
XP total requerida: 700

Ejemplos:

| Enemigos derrotados | XP |
|---|---:|
| Señor del Camino | 240 |
| Capitán Desertor + Bruja del Círculo | 280 |
| Bestia + Acólito + Matón + Cultista + Saqueador | 255 |

Objetivo real:

```text
jefe normal casi sube un nivel completo
o
2 elites completan la subida
o
varios combates difíciles
```

---

# 9. Progresión ideal de una región demo

Esta sería una ruta balanceada de ejemplo.

| Momento | Encuentro | XP aproximada | Nivel esperado |
|---|---|---:|---|
| 1 | 2 Bandidos Rasos | 40 | 1 |
| 2 | Bandido Raso + 2 Perros | 50 | 1 |
| 3 | Saqueador + Ballestero | 70 | 2 |
| 4 | Escudero + Bandido + Cuervo | 75 | 2 |
| 5 | Cultista + Acólito | 100 | 3 |
| 6 | Matón + Saqueador + Ballestero | 125 | 3 |
| 7 | Bestia + 2 Perros | 90 | 3-4 |
| 8 | Capitán Desertor + apoyo | 200 aprox | 4 |
| 9 | Bruja del Círculo + apoyo | 210 aprox | 4-5 |
| 10 | Señor del Camino | 240 | 5 |
| 11 | La Cosa Bajo la Abadía | 0 | 5 |

La ruta completa da más de 700 XP, pero eso está bien si algunos combates son opcionales o si se ajusta la experiencia entregada por grupo.

---

# 10. Regla importante de balance

La experiencia no tiene que premiar solo matar enemigos. También puede premiar completar combates/nodos.

Recomendación:

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
| Cuervo Carroñero | 1 | 15 | componente menor |
| Saqueador | 2 | 35 | oro, hierro |
| Ballestero | 2 | 35 | madera, cuero |
| Escudero Bandido | 2 | 40 | hierro, placas |
| Cultista Menor | 2 | 40 | cristal |
| Matón Desertor | 3 | 55 | oro, hierro, cuero |
| Bestia del Monte | 3 | 60 | cuero, comida |
| Acólito Corrupto | 3 | 60 | cristales |
| Capitán Desertor | 4 | 140 | oro, hierro, item físico |
| Bruja del Círculo | 4 | 140 | cristales, item mágico |
| Señor del Camino | 5 | 240 | oro, item tier 3, desbloqueo |
| La Cosa Bajo la Abadía | 6 | 0 | cierre demo, item único |

---

# 12. Pendientes

Después de esta base falta definir:

1. Stats numéricos de cada enemigo.
2. Habilidades exactas de cada enemigo.
3. Grupos de combate definitivos.
4. Recompensas por nodo/contrato.
5. Items concretos que pueden caer.
6. Economía de tienda y crafting.
