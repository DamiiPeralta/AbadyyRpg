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
