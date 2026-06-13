# AbadyyRpg - Ítems, armas y armaduras de la demo

Estado: borrador inicial de equipamiento.  
Objetivo: definir la estructura de ítems por tier para la demo, separando armas, armaduras y especiales.

---

## 1. Lógica general de tiers

El equipamiento de la demo se organiza en 4 categorías:

| Categoría | Lectura |
|---|---|
| Tier 1 | oxidado, roto, improvisado |
| Tier 2 | hierro, confiable, funcional |
| Tier 3 | acero, sólido, preparación de jefe |
| Especial | efecto único que modifica la build |

La idea no es tener muchísimos ítems, sino que cada mejora se sienta clara.

```text
Tier 1 = sobrevivir
Tier 2 = estabilizar build
Tier 3 = preparar jefe
Especial = cambiar comportamiento
```

---

## 2. Relación con el sistema de combate

El sistema defensivo trabaja con tres barras:

```text
HP real
Armadura física
Armadura mágica
```

La armadura funciona como vida extra por tipo de daño:

```text
Daño físico -> Armadura física -> HP
Daño mágico -> Armadura mágica -> HP
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
| Bastones / focos / grimorios | Mago del Círculo |
| Cetros / símbolos / campanas | Acólita |

---

# 4. Armas físicas

## 4.1 Espadas

Las espadas funcionan como arma física estable. Pueden servir tanto al Defensor como al Asesino, aunque el Defensor las usa mejor si busca balance entre daño y defensa.

| Tier | Ítem | Stats / efecto |
|---|---|---|
| Tier 1 | Espada Oxidada | +6 daño físico |
| Tier 2 | Espada de Hierro | +12 daño físico |
| Tier 3 | Espada de Acero | +20 daño físico |
| Especial | Espada de Luz | +16 daño físico, +8 daño mágico, bonus contra culto |
| Especial | Espada Chupasangre | +14 daño físico, vampirismo menor |

### Uso esperado

```text
Espada Oxidada -> inicio de demo
Espada de Hierro -> estabilización
Espada de Acero -> preparación contra jefes
Espada de Luz -> anti-culto / anti-aberración
Espada Chupasangre -> sustain ofensivo
```

---

## 4.2 Dagas

Las dagas son armas rápidas para el Asesino. Tienen menos daño bruto que las espadas, pero agregan speed o efectos.

| Tier | Ítem | Stats / efecto |
|---|---|---|
| Tier 1 | Daga Mellada | +5 daño físico, +1 speed |
| Tier 2 | Daga de Hierro | +10 daño físico, +2 speed |
| Tier 3 | Daga de Acero | +16 daño físico, +3 speed |
| Especial | Daga de Sangre Negra | +12 daño físico, aplica sangrado/poison |
| Especial | Daga del Silencio | +10 daño físico, baja taunt o chance de stun |

### Uso esperado

```text
Daga Mellada -> asesino inicial
Daga de Hierro -> asesino funcional
Daga de Acero -> asesino de cierre
Daga de Sangre Negra -> build de daño por turno
Daga del Silencio -> build de control / evasión de foco
```

---

## 4.3 Mazas y martillos

Las mazas y martillos son armas físicas pensadas para romper armadura física.

| Tier | Ítem | Stats / efecto |
|---|---|---|
| Tier 1 | Maza Astillada | +7 daño físico, daño extra a armadura física |
| Tier 2 | Maza de Hierro | +13 daño físico, daño extra a armadura física |
| Tier 3 | Martillo de Acero | +21 daño físico, daño extra fuerte a armadura física |
| Especial | Rompejuramentos | +18 daño físico, mucho daño a armadura física |
| Especial | Maza Bendita | +15 daño físico, restauración menor de armadura física al usuario |

### Uso esperado

```text
Mazas -> útiles contra Escudero Bandido, Capitán Desertor y enemigos con mucha armadura física.
```

---

# 5. Armas mágicas

## 5.1 Bastones

Los bastones son el arma mágica directa del Mago del Círculo.

| Tier | Ítem | Stats / efecto |
|---|---|---|
| Tier 1 | Bastón Partido | +6 daño mágico |
| Tier 2 | Bastón de Roble Marcado | +12 daño mágico |
| Tier 3 | Bastón del Círculo | +20 daño mágico |
| Especial | Bastón de Luz Fría | +16 daño mágico, bonus contra aberraciones |
| Especial | Bastón de Médula | +14 daño mágico, vampirismo mágico menor |

### Uso esperado

```text
Bastones -> daño mágico estable.
Bastón de Médula -> sustain del Mago del Círculo.
Bastón de Luz Fría -> herramienta contra jefe final o culto.
```

---

## 5.2 Focos y grimorios

Los focos y grimorios dan menos daño directo que un bastón, pero ofrecen mana o efectos especiales.

| Tier | Ítem | Stats / efecto |
|---|---|---|
| Tier 1 | Foco Agrietado | +4 daño mágico, +10 mana |
| Tier 2 | Foco de Hierro Ritual | +9 daño mágico, +20 mana |
| Tier 3 | Foco de Cristal | +15 daño mágico, +35 mana |
| Especial | Grimorio del Hambre | +12 daño mágico, mejora poison |
| Especial | Ojo del Círculo | +10 daño mágico, +speed o mejor target |

### Uso esperado

```text
Focos -> más recursos y consistencia.
Grimorio del Hambre -> build de poison.
Ojo del Círculo -> build rápida/táctica.
```

---

# 6. Armas de soporte

## 6.1 Cetros, símbolos y campanas

Estas armas son para la Acólita. Mejoran curación, soporte, regeneración o restauración defensiva.

| Tier | Ítem | Stats / efecto |
|---|---|---|
| Tier 1 | Símbolo Quebrado | +5 poder de curación |
| Tier 2 | Cetro de Hierro | +10 poder de curación, +10 mana |
| Tier 3 | Cetro de Acero Consagrado | +16 poder de curación, +25 mana |
| Especial | Cetro de Luz | +14 curación, +restauración de armadura mágica |
| Especial | Campana de los Caídos | +10 curación, aplica regeneration más fuerte |

### Uso esperado

```text
Símbolo Quebrado -> soporte inicial
Cetro de Hierro -> soporte estable
Cetro Consagrado -> preparación contra jefes
Cetro de Luz -> soporte defensivo mágico
Campana de los Caídos -> build de regeneración
```

---

# 7. Familias de armaduras

Para la demo se definen 4 familias principales:

| Tipo | Usuario principal |
|---|---|
| Armadura pesada | Defensor |
| Armadura ligera | Asesino |
| Túnica arcana | Mago del Círculo |
| Vestidura / armadura media | Acólita |

---

# 8. Armaduras pesadas - Defensor

Las armaduras pesadas dan mucha armadura física y poca/media armadura mágica. Pueden reducir speed.

| Tier | Ítem | Armadura física | Armadura mágica | Efecto |
|---|---|---:|---:|---|
| Tier 1 | Armadura de Placas Oxidada | 80 | 20 | -1 speed |
| Tier 2 | Armadura de Placas de Hierro | 115 | 30 | -1 speed |
| Tier 3 | Armadura de Placas de Acero | 155 | 45 | -2 speed |
| Especial | Armadura Bendecida Autorreparadora | 130 | 55 | restaura armadura física bajo condición |
| Especial | Coraza del Mártir | 145 | 35 | genera taunt extra |

## Efecto recomendado para Armadura Bendecida Autorreparadora

Opción balanceada:

```text
Si la armadura física baja de 40%, restaura 20 de armadura física una vez por combate.
```

Evitar que regenere demasiado por turno, porque puede alargar mucho los combates.

---

# 9. Armaduras ligeras - Asesino

Las armaduras ligeras dan menos defensa, pero pueden sumar speed, bajar taunt o mejorar el remate.

| Tier | Ítem | Armadura física | Armadura mágica | Efecto |
|---|---|---:|---:|---|
| Tier 1 | Cuero Gastado | 35 | 15 | +1 speed |
| Tier 2 | Cuero Hervido | 55 | 25 | +2 speed |
| Tier 3 | Cuero Reforzado con Acero | 80 | 35 | +2 speed |
| Especial | Manto del Acechador | 60 | 40 | baja taunt |
| Especial | Cuero de Sangre Negra | 70 | 30 | aumenta daño contra enemigos heridos |

### Uso esperado

```text
El Asesino no debería alcanzar la defensa del Defensor.
Su supervivencia viene de speed, bajo taunt y matar rápido.
```

---

# 10. Túnicas mágicas - Mago del Círculo

Las túnicas mágicas dan poca armadura física, mucha armadura mágica y mana.

| Tier | Ítem | Armadura física | Armadura mágica | Efecto |
|---|---|---:|---:|---|
| Tier 1 | Túnica Rasgada | 10 | 50 | +10 mana |
| Tier 2 | Túnica del Círculo | 20 | 80 | +20 mana |
| Tier 3 | Túnica de Cristal Bordado | 35 | 115 | +35 mana |
| Especial | Túnica de Ceniza Viva | 25 | 100 | mejora poison |
| Especial | Manto Antimagia | 20 | 145 | gran defensa mágica |

### Uso esperado

```text
El Mago del Círculo resiste bien el daño mágico, pero debe temer al daño físico.
```

---

# 11. Vestiduras / armadura media - Acólita

La Acólita usa protección mixta. No tan dura como el Defensor, no tan frágil como el Mago.

| Tier | Ítem | Armadura física | Armadura mágica | Efecto |
|---|---|---:|---:|---|
| Tier 1 | Vestidura Remendada | 25 | 35 | +10 mana |
| Tier 2 | Cota Liviana de Hierro | 40 | 60 | +15 mana |
| Tier 3 | Vestidura Consagrada | 65 | 90 | +25 mana |
| Especial | Hábito de la Llama Serena | 50 | 100 | mejora curaciones |
| Especial | Manto de la Vigilia | 60 | 75 | buffs duran más |

### Uso esperado

```text
La Acólita debe sobrevivir mejor que el Mago, pero no puede reemplazar al Defensor.
```

---

# 12. Accesorios opcionales

Los accesorios pueden quedar para después. Si se agregan, deberían ser simples.

| Ítem | Efecto |
|---|---|
| Anillo de Hierro | +20 armadura física |
| Medalla de Vidrio | +25 armadura mágica |
| Amuleto del Peregrino | +15 mana |
| Cordón de Guerra | +15 stamina |
| Talismán de Sangre | +5% vampirismo |
| Sello del Círculo | +daño mágico |
| Hebilla Pesada | +taunt |
| Broche del Cobarde | baja taunt |

Recomendación:

```text
No implementar accesorios todavía salvo que haga falta más personalización.
```

---

# 13. Especiales importantes

Los ítems especiales no deberían ser solo mejores números. Deben modificar el comportamiento o favorecer una build.

## Armas especiales

| Ítem | Rol |
|---|---|
| Espada de Luz | anti-culto / híbrida físico-mágica |
| Espada Chupasangre | sustain por vampirismo |
| Daga de Sangre Negra | sangrado/poison |
| Daga del Silencio | control / baja taunt |
| Rompejuramentos | destruye armadura física |
| Maza Bendita | sustain defensivo |
| Bastón de Luz Fría | anti-aberración |
| Bastón de Médula | vampirismo mágico |
| Grimorio del Hambre | mejora poison |
| Ojo del Círculo | velocidad o targeting |
| Cetro de Luz | soporte defensivo mágico |
| Campana de los Caídos | regeneration fuerte |

## Armaduras especiales

| Ítem | Rol |
|---|---|
| Armadura Bendecida Autorreparadora | tanque sostenido |
| Coraza del Mártir | tanque con taunt |
| Manto del Acechador | Asesino menos targeteado |
| Cuero de Sangre Negra | Asesino ejecutor |
| Túnica de Ceniza Viva | build de poison |
| Manto Antimagia | defensa contra brujas/culto |
| Hábito de la Llama Serena | mejor sanación |
| Manto de la Vigilia | mejor booster |

---

# 14. Loot conectado a enemigos

| Enemigo | Puede soltar |
|---|---|
| Bandido Raso | Espada Oxidada, Daga Mellada |
| Saqueador | Maza Astillada, Hierro |
| Ballestero | Cuero Gastado, Madera |
| Escudero Bandido | Placas Oxidadas, Escudo Astillado |
| Matón Desertor | Espada de Hierro, Cuero Hervido |
| Cultista Menor | Foco Agrietado, Tiza Ritual |
| Acólito Corrupto | Símbolo Quebrado, Vestidura Remendada |
| Bestia del Monte | Cuero Reforzado, Piel Gruesa |
| Capitán Desertor | Espada de Acero, Coraza del Mártir |
| Bruja del Círculo | Grimorio del Hambre, Túnica de Ceniza Viva |
| Señor del Camino | arma tier 3 garantizada, armadura pesada tier 3 |
| La Cosa Bajo la Abadía | item único de cierre |

---

# 15. Set mínimo para implementar primero

Para no explotar el scope, el primer paquete debería ser este.

## Armas mínimas

| Rol | Tier 1 | Tier 2 | Tier 3 | Especial |
|---|---|---|---|---|
| Defensor | Espada Oxidada | Espada de Hierro | Espada de Acero | Espada de Luz |
| Asesino | Daga Mellada | Daga de Hierro | Daga de Acero | Daga de Sangre Negra |
| Mago del Círculo | Bastón Partido | Bastón de Roble | Bastón del Círculo | Grimorio del Hambre |
| Acólita | Símbolo Quebrado | Cetro de Hierro | Cetro Consagrado | Campana de los Caídos |

## Armaduras mínimas

| Rol | Tier 1 | Tier 2 | Tier 3 | Especial |
|---|---|---|---|---|
| Defensor | Placas Oxidadas | Placas de Hierro | Placas de Acero | Armadura Autorreparadora |
| Asesino | Cuero Gastado | Cuero Hervido | Cuero Reforzado | Manto del Acechador |
| Mago del Círculo | Túnica Rasgada | Túnica del Círculo | Túnica de Cristal | Manto Antimagia |
| Acólita | Vestidura Remendada | Cota Liviana | Vestidura Consagrada | Hábito de la Llama Serena |

Este set mínimo contiene:

```text
16 armas
16 armaduras
```

Es suficiente para una demo y todavía es manejable.

---

# 16. Especiales disponibles en demo

No conviene que todos los especiales estén disponibles en la primera demo. Lo ideal es elegir pocos.

Recomendación inicial:

| Especial | Fuente |
|---|---|
| Espada Chupasangre o Espada de Luz | Capitán Desertor / Señor del Camino |
| Grimorio del Hambre | Bruja del Círculo |
| Armadura Bendecida Autorreparadora | Señor del Camino |
| Campana de los Caídos | evento de abadía / recompensa especial |

La idea es que lo especial se sienta especial.

---

# 17. Pendientes

Luego de esta definición falta:

1. Definir IDs técnicos de cada ítem.
2. Definir precio de tienda.
3. Definir materiales de crafting.
4. Definir recetas.
5. Definir qué ítems son loot directo y cuáles se craftean.
6. Definir valores finales de efectos especiales.
7. Crear los ScriptableObjects en Unity.
