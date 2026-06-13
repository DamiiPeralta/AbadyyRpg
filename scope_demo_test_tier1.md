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
