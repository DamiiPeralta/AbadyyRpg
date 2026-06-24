# AbadyyRpg - Balance rutas demo Tier 1

Estado: simulacion de papel con datos actuales.
Base: `SCOPE_TEST_SIMPLIFICADO.md` y assets Tier 1 actuales.

Objetivo:

```text
Ver si la ruta de 10 dias permite aprender, farmear, craftear y llegar al jefe sin abrir sistemas grandes.
```

---

## Supuestos actuales

- El jugador empieza dia 1, hora 8.
- La caravana empieza con 100 stamina.
- Inventario inicial: 0 oro, 0 comida, 0 madera, 0 piedra, 0 hierro, 0 cuero, 0 cristal.
- El personaje empieza sin equipo.
- El puente no debe bloquear la demo si el jugador no tiene recursos.
- El combate repetible consume horas/stamina indirectamente: para repetir un nodo hay que viajar de ida/vuelta.
- El coste real de combate viene por HP, stamina, mana, armadura y consumibles.
- La derrota por tiempo ocurre al llegar al dia 11 sin derrotar al jefe.

---

## Observacion critica encontrada

Con inventario inicial en cero, la opcion de reparar puente no puede usarse al llegar por primera vez.

Eso esta bien si existe alternativa.

La alternativa correcta es:

```text
Forzar el cruce:
- 3 stamina de caravana
- 2 horas
- 0 recursos
```

Se corrigio el generador para que no pida comida en esa opcion.

Motivo:

```text
Si forzar tambien pide recursos, la demo puede quedar bloqueada antes del primer combate.
```

---

## Recompensas activas por encuentro

| Encuentro | Oro | Comida | Madera | Piedra | Hierro | Cuero | Cristal | XP |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 rata | 3 | 1 | 0 | 0 | 0 | 1 | 0 | 8 |
| 2 ratas | 6 | 2 | 0 | 0 | 0 | 2 | 0 | 16 |
| rata + esqueleto | 9 | 1 | 0 | 1 | 1 | 1 | 0 | 24 |
| 1 esqueleto | 6 | 0 | 0 | 1 | 1 | 0 | 0 | 16 |
| 1 gusano | 10 | 1 | 0 | 0 | 0 | 1 | 1 | 28 |
| esqueleto + gusano | 16 | 1 | 0 | 1 | 1 | 1 | 1 | 44 |
| jefe demonio menor | 25 | 0 | 0 | 0 | 1 | 1 | 3 | 60 |

---

## Ruta minima

```text
Valdoran
-> Camino 01
-> Camino 02
-> Puente roto
-> Forzar cruce
-> Cruce del Vigia
-> Torre vigia
-> Revisar torre
-> Collinasombra
-> Escuchar advertencias
-> Portal
-> Jefe
```

Resultado esperado:

- Llega muy rapido al jefe.
- No craftea nada importante.
- No gana XP relevante.
- Deberia perder o quedar como intento claramente imprudente.

Conclusion:

```text
La ruta minima sirve como leccion, no como ruta ganadora.
```

---

## Ruta mala con farmeo ineficiente

```text
Forzar puente.
Farmear ratas muchas veces.
Descansar/reparar de mas.
Llegar tarde al jefe con equipo incompleto.
```

Resultado esperado:

- Junta comida y cuero.
- Junta poco hierro, piedra y cristal.
- Sube XP lento.
- Puede quedarse sin dias si compensa malos combates con muchos descansos.

Riesgo de balance:

```text
Si las ratas dan demasiada seguridad economica, el jugador puede ignorar enemigos que ensenan counterplay.
```

Control recomendado:

- Mantener ratas como recompensa chica.
- Hacer que hierro/cristal vengan de esqueleto/gusano/mina.
- No permitir que solo ratas preparen al jefe.

---

## Ruta optima tentativa

```text
Forzar puente.
Ganar 2 ratas.
Ganar rata + esqueleto.
Ir a torre y revisar pista.
Saquear torre si falta madera.
Ganar esqueleto + gusano en mina.
Craftear tunica rasgada + espada oxidada.
Preparar pocion o molotov si sobran recursos.
Ir a Collinasombra.
Ir al portal.
Pelear jefe.
```

Recursos aproximados antes de craftear, si hace 2 ratas + rata/esqueleto + esqueleto/gusano + saqueo torre:

| Recurso | Total |
|---|---:|
| Oro | 31 |
| Comida | 4 |
| Madera | 1 |
| Piedra | 3 |
| Hierro | 2 |
| Cuero | 4 |
| Cristal | 1 |
| XP | 84 |

Craft recomendado:

| Item | Oro | Madera | Hierro | Cuero | Cristal |
|---|---:|---:|---:|---:|---:|
| Espada oxidada | 12 | 1 | 2 | 0 | 0 |
| Tunica rasgada | 14 | 0 | 0 | 2 | 1 |

Sobrante aproximado:

| Recurso | Sobra |
|---|---:|
| Oro | 5 |
| Comida | 4 |
| Piedra | 3 |
| Cuero | 2 |

Lectura:

```text
Esta ruta arma una respuesta clara contra el jefe:
- tunica para aguantar magia;
- espada para pegar por fisico;
- algo de comida/cuero/piedra como margen economico.
```

Problema:

```text
Con XP 84 el personaje queda cerca de nivel 3, pero no llega.
```

Recomendacion:

- Una pelea extra de 1 esqueleto o 2 ratas deberia alcanzar nivel 3.
- Si nivel 3 se siente obligatorio para jefe, bajar requisito de nivel 3 o subir un poco XP de mina.
- Si nivel 3 es opcional, dejarlo asi.

---

## Ajustes que parecen necesarios antes de balance fino

1. Implementar o resolver `Buscar recursos`.
   - Sin esta accion, falta la red de seguridad documentada.

2. Definir habilidades genericas del personaje.
   - El balance real depende mucho de ataque poderoso, misil magico, curar, guardia y escudo magico.

3. Implementar auto-revive del pergamino.
   - El item existe, pero la regla especial de demo todavia falta.

4. Ejecutar el generador en Unity y guardar `WorldMapScene`.
   - Los cambios del generador no viven en escena hasta regenerar.

5. Probar una run real.
   - La simulacion no reemplaza ver turnos, desgaste, IA y tacticas en Play Mode.

---

## Estado de balance

```text
La estructura de ruta parece viable.
El bloqueo inicial del puente quedo identificado y corregido en generador.
Todavia no conviene tocar HP/dano/recompensas hasta cerrar habilidades y buscar recursos.
```
