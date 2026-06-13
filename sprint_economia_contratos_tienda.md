# Sprint — Economía jugable: contratos, recompensa visible y tienda reutilizable

## 1. Identidad del juego

El juego no debe presentarse solo como “mapa + combate automático”.

La identidad principal es:

```text
RPG de caravana, contratos, crafting y gestión.
```

El combate es una capa importante, pero no es el centro absoluto. Sirve para:

- conseguir recursos;
- completar contratos;
- conseguir loot;
- probar builds;
- validar el sistema de gambits/tácticas;
- abrir consecuencias en el mundo.

El loop principal buscado es:

```text
Aceptar contrato
→ preparar caravana
→ comprar / craftear / equipar
→ salir al mapa
→ resolver eventos o combates
→ ganar recursos, oro e ítems
→ volver a la caravana o pueblo
→ vender, craftear, descansar, reparar
→ tomar contratos más difíciles
```

La idea fuerte es que el juego se sienta como una compañía de mercenarios que sobrevive mediante gestión, preparación, trabajos, decisiones y crafting.

---

## 2. Sistemas próximos importantes

Los tres sistemas prioritarios para mostrar mejor la identidad del juego son:

```text
1. UI de recompensa post-combate
2. Contratos
3. Tienda reutilizable
```

Estos tres sistemas juntos forman el primer loop económico claro:

```text
Contrato
→ combate / evento
→ pantalla de recompensa
→ inventario
→ tienda / crafting
→ preparación
→ contrato más difícil
```

---

## 3. UI de recompensa post-combate

Hace falta una pantalla de victoria que muestre claramente lo ganado.

Ejemplo:

```text
Victoria

Recompensas:
+25 oro
+2 comida
+1 hierro
Rusty Sword x1

[Continuar]
```

### Objetivo

Que cada combate tenga cierre visual.

Que el jugador entienda qué ganó.

Que el loot tenga peso.

Que el sistema de recompensas no se sienta invisible o escondido en el inventario.

### Scope V1

Para la primera versión alcanza con:

- título de victoria;
- lista de oro / recursos obtenidos;
- lista de ítems obtenidos;
- botón continuar;
- aplicar recompensa al inventario runtime;
- volver al mapa después de continuar.

No hace falta todavía:

- animaciones complejas;
- rarezas visuales;
- pantalla de comparación de equipo;
- loot random profundo;
- sonidos finales;
- pantalla hermosa final.

---

## 4. Contratos

Los contratos son el motor que le da dirección al jugador.

No son solo quests genéricas. Son trabajos que la compañía acepta para conseguir oro, recursos, reputación o acceso a nuevos lugares.

### Estados posibles

```text
Disponible
Aceptado
Rechazado
Completado
Fallido
Expirado
```

### Datos mínimos de un contrato

```text
contractId
título
descripción
empleador
ubicación
deadline
recompensa
objetivo principal
objetivos secundarios
estado
```

### Ejemplo

```text
Contrato: Erradicar a los bandidos del bosque

Deadline: Día 4
Recompensa: 120 oro, 3 comida, 1 hierro

Objetivo principal:
- Derrotar al líder bandido

Objetivos secundarios:
- Encontrar el campamento
- Destruir el alijo
- Matar al capitán
```

### Función de gameplay

El contrato debe responder:

```text
¿Quién me paga?
¿Qué tengo que hacer?
¿Cuánto tiempo tengo?
¿Qué gano?
¿Qué pasa si fallo?
¿Dónde tengo que ir?
```

Esto le da dirección al mapa y hace que los nodos tengan propósito.

---

## 5. Tienda reutilizable

La tienda no debería ser una tienda hardcodeada, sino un sistema genérico reutilizable por diferentes pueblos, ciudades o NPCs.

Cada tienda debería poder tener su propio inventario, precios y reglas.

### Datos mínimos de tienda

```text
storeId
nombre
lista de ítems en venta
precios
modificador de precio
permite comprar
permite vender
```

### Regla base de precio

```text
Precio de tienda = valor base del ítem + 50%
```

Después cada ciudad o pueblo puede modificar esa regla.

### Ejemplos de tiendas por lugar

```text
Ciudad Central:
- armas
- armaduras
- pociones
- pergaminos de revivir
- comida a precio normal
```

```text
Pueblo de montaña:
- comida más cara
- armas más débiles
- hierbas
- cuero
- minerales
```

```text
Aldea pobre:
- comida simple
- cuero
- herramientas básicas
- mejor stock si la ayudaste
```

### Worldbuilding desde economía

La tienda debe contar cómo vive cada lugar.

Si un pueblo de montaña tiene comida cara, hierbas, armas malas y minerales, el jugador entiende algo del mundo sin necesidad de un diálogo largo.

Si la ciudad central vende pociones, pergaminos de revivir, armas y armaduras, se siente como centro comercial y político.

Si una aldea pobre solo vende comida simple y herramientas, se siente vulnerable.

---

## 6. Venta

Por ahora se puede dejar la venta disponible globalmente para testear el loop.

Más adelante, la venta debería depender del contexto.

Regla futura:

```text
Vender solo debería habilitarse si estás en un pueblo o ciudad con tienda.
```

Esto haría que el lugar actual importe más.

También permitiría que algunas zonas no permitan vender, obligando al jugador a planificar mejor qué cargar, qué guardar y cuándo volver.

---

## 7. Vistas dentro de CaravanScene

La CaravanScene debería funcionar como centro de gestión.

Vistas principales:

```text
Tienda actual
Contratos
Diario de misión
Registro / logs
```

La caravana no debe ser solamente descanso y roster. Tiene que ser el lugar donde el jugador entiende la run, prepara la próxima salida y administra lo ganado.

---

## 8. Contratos, diario y logs no son lo mismo

Conviene separar estos conceptos aunque en V1 puedan compartir datos.

### Contratos

Son trabajos formales.

Responden:

```text
Quién paga.
Qué hay que hacer.
Cuánto paga.
Deadline.
Estado del contrato.
```

### Diario de misión

Es la vista narrativa/progresiva.

Muestra:

```text
Qué se descubrió.
Qué pistas hay.
Qué objetivos se actualizaron.
Qué puede hacerse antes del final.
```

### Logs

Es el historial frío de lo ocurrido.

Ejemplo:

```text
Día 1 — Aceptaste el contrato: Erradicar a los bandidos del bosque.
Día 2 — Destruiste el alijo de suministros.
Día 3 — Mataste al capitán bandido.
Día 3 — El combate final tendrá un elite menos.
```

El log sirve para que el jugador entienda por qué el mundo cambió.

---

## 9. Idea fuerte: preparación narrativa del combate

Esta es una de las ideas principales del diseño.

Los eventos del mapa no solo dan recompensas. También pueden modificar el combate final.

El jugador no solo hace sidequests. Prepara el terreno.

### Ejemplo: contrato de bandidos del bosque

Objetivo principal:

```text
Derrotar al líder bandido.
```

Eventos opcionales:

```text
Encontraste el campamento
→ desbloquea ruta al jefe.
```

```text
Destruiste el alijo de suministros
→ los enemigos pierden consumibles.
→ el loot de comida puede ser menor porque quemaste parte del botín.
```

```text
Mataste al capitán bandido
→ el combate final tiene un elite menos.
→ loot extra: insignia del capitán.
```

```text
Ayudaste a un aldeano vengativo
→ se infiltra antes del ataque.
→ los enemigos empiezan con Quemadura.
```

```text
Aceptaste soborno de los bandidos
→ falla el contrato de la aldea.
→ se desbloquea recompensa alternativa.
```

Cada decisión debe tener dos capas:

```text
Narrativa: pasó algo en el mundo.
Mecánica: cambió una pelea, recompensa, tienda o estado de contrato.
```

Ese es el corazón del sistema.

---

## 10. Vista ideal de contratos

La UI de contratos podría organizarse así:

```text
[Contratos]

Tabs:
Disponibles | Aceptados | Completados | Fallidos

Lista izquierda:
- Erradicar bandidos del bosque
- Escoltar caravana minera
- Recuperar reliquia del pantano

Panel derecho:
Título
Empleador
Descripción
Deadline
Recompensas
Estado

Objetivo principal:
[ ] Derrotar al líder bandido

Avances:
[✓] Encontrar el campamento
[✓] Destruir el alijo
[✓] Matar al capitán
[ ] Enfrentar al líder

Consecuencias detectadas:
- El capitán no participará del combate final.
- Los enemigos empiezan con Quemadura.
- Los bandidos no tendrán consumibles.
```

La UI tiene que juntar gameplay y narrativa en un solo lugar.

---

## 11. Scope V1 recomendado

No hacer un sistema gigante de golpe.

Para V1 alcanza con:

```text
Contrato aceptable/completable
Deadline visible
Recompensa visible
Objetivo principal
Avances simples
Flags que modifican combate final
UI básica de contratos
UI básica de recompensa
Tienda básica reutilizable
```

No hacer todavía:

```text
contratos infinitos
generación procedural
reputación compleja
economía dinámica
múltiples pantallas hermosas
tienda con stock cambiante avanzado
sistema profundo de rumores
quest journal gigante
```

---

## 12. Sprint propuesto

Nombre:

```text
Sprint — Economía jugable: contratos, recompensa visible y tienda reutilizable
```

### Objetivo

Mostrar que el juego es de gestión, crafting y contratos, no solamente de combate.

### Resultado esperado

Al terminar este sprint, el jugador debería poder:

```text
Aceptar un contrato
→ salir al mapa
→ resolver eventos o combate
→ ver recompensas claramente
→ volver a gestionar recursos
→ comprar / vender / craftear
→ prepararse para otro contrato
```

Este sería el primer momento donde el juego empieza a mostrar su identidad real.

---

## 13. Orden sugerido de implementación

```text
1. Revisar sistema actual de recompensas.
2. Crear UI de recompensa post-combate.
3. Conectar RewardData con pantalla de victoria.
4. Crear estructura básica de ContractData.
5. Crear ContractRuntimeState o ContractManager runtime.
6. Crear panel de contratos en CaravanScene.
7. Permitir aceptar contrato.
8. Permitir completar contrato cuando se cumpla un objetivo.
9. Mostrar estado: disponible / aceptado / completado / fallido.
10. Agregar deadline visible.
11. Crear StoreInventory reutilizable.
12. Crear panel de tienda actual en CaravanScene.
13. Mostrar stock y precio.
14. Comprar ítems con oro.
15. Dejar venta global por ahora o conectarla al panel de tienda si es simple.
16. Crear un contrato de prueba: bandidos del bosque.
17. Crear eventos opcionales que agreguen flags/modificadores.
18. Hacer que el combate final lea esos modificadores.
19. Mostrar consecuencias en diario/log.
20. Testear loop completo.
```

---

## 14. Test final del sprint

El test mínimo debería ser:

```text
1. Entrar a CaravanScene.
2. Abrir panel de contratos.
3. Aceptar contrato de bandidos del bosque.
4. Salir al mapa.
5. Resolver evento para encontrar campamento.
6. Resolver evento para destruir alijo.
7. Resolver combate contra capitán.
8. Ver avances en diario/contrato.
9. Entrar al combate final.
10. Confirmar que el elite no aparece o que los enemigos tienen debuff.
11. Ganar combate.
12. Ver pantalla de recompensa.
13. Volver a CaravanScene.
14. Ver contrato completado.
15. Ver recursos en inventario.
16. Comprar algo en tienda.
17. Craftear o preparar la party para el siguiente contrato.
```

Si este test pasa, el juego ya muestra claramente su identidad.

---

## 15. Frase guía del diseño

```text
El jugador no solo pelea: acepta trabajos, prepara la caravana, administra recursos, toma decisiones, modifica el terreno antes del combate y convierte el riesgo en progreso.
```
