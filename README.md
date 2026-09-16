# EpicLootRaritySets

Convierte EpicLoot en una progresion de clases por equipo para Valheim.

Este mod anade sets personalizados por rareza, drops por etapa de boss, piezas especiales, bonus de set y habilidades activas al completar sets. La idea es que el jugador no solo encuentre objetos mejores: que pueda construir un personaje completo alrededor de una clase de equipo.

Si te gusta EpicLoot pero quieres objetivos claros de farmeo, builds reconocibles y habilidades nuevas al cerrar un set completo, este mod es para eso.

## Que aporta

- Sets por rareza: `Magic`, `Rare`, `Epic`, `Legendary`, `Mythic` y `Ancient`.
- Clases por set: tanque, berserker, arquero, ballestero, asesino, mago elemental, mago de sangre, arquero magico y spellblade.
- Habilidades activas y pasivas al completar el set.
- Bonus de set crecientes segun rareza y numero de piezas.
- Drops naturales de piezas de set desde EpicLoot.
- Drops garantizados de boss por etapa cuando hay una tirada valida.
- Configs de EpicLoot y NorseDemigods embebidas en la DLL y sincronizadas al arrancar.
- Entrada nueva en el compendio/textos del juego: `Epic Loot Rarity Sets`, con descripcion de cada clase y habilidades detalladas.
- Integracion opcional con BetterArchery para `LeatherQuiver`.
- Integracion con Bestiary para criaturas `RDB_*` en loot y contratos.
- Compatibilidad opcional con WolfPack para controlar invocaciones de Hraesvelgr.

## Dependencias

Obligatorias:

- `RandyKnapp-EpicLoot`
- `Alpus-NorseDemigods`
- `Radamanto-Bestiary`

Opcionales:

- `BetterArchery`: integra `LeatherQuiver` como pieza encantable en sets de arquero desde Epic en adelante.
- `WolfPack`: el mod puede ajustar su config para que las bestias invocadas por Hraesvelgr sean entrenables/controlables.

Aunque NorseDemigods aparece como dependencia, no tienes que jugar una clase de NorseDemigods. Este mod suprime su UI, energia, seleccion de clase e input cuando usa sus efectos como puente visual/mecanico.

## Como funciona

El mod crea arquetipos de set. Al equipar suficientes piezas de una misma linea, se activa un buff con el nombre base del set:

- `MagicFrostbrand`, `Frostbrand`, `MythicFrostbrand` y `AncientFrostbrand` activan `Frostbrand`.
- `RareSolomonKane`, `SolomonKane`, `MythicSolomonKane` y `AncientSolomonKane` activan `SolomonKane`.

Ese buff sirve como bandera para habilitar las habilidades de clase.

Los buffs de set completo no muestran nivel debajo del icono. Las habilidades con cooldown si muestran su propio buff de cooldown.

## Probabilidades de drops naturales

Las tiradas magicas normales de EpicLoot pueden convertirse en piezas de set:

- Magic: `12%`
- Rare: `10%`
- Epic: `8%`
- Ancient: `5%`

Legendary y Mythic se gestionan desde las secciones generadas de EpicLoot y sus pools propios.

`bosssetdrops.json` fuerza `GuaranteedSetDrops = 1`: cada boss convierte una tirada valida en una pieza de set aleatoria de la rareza de su etapa.

## Configs gestionadas

Con `Generate Managed Config Files = true`, la DLL escribe/sincroniza:

- `config/EpicLoot/raritysets.json`
- `config/EpicLoot/baseconfig/legendaries.json`
- `config/EpicLoot/baseconfig/loottables.json`
- `config/EpicLoot/baseconfig/iteminfo.json`
- `config/EpicLoot/baseconfig/magiceffects.json`
- `config/EpicLoot/baseconfig/adventuredata.json`
- `config/EpicLoot/bosssetdrops.json`
- `config/NorseDemigods.cfg`

Si un archivo existe y es distinto, primero crea backup `.bak-fran-managed-...` y despues escribe la version embebida.

Para servidor, normalmente basta con subir la DLL nueva y reiniciar.

## Compendio dentro del juego

El mod anade una entrada al panel de textos/compendio del inventario:

`Epic Loot Rarity Sets`

Incluye:

- Explicacion del sistema de rarezas.
- Descripcion de cada clase por set.
- Habilidades, teclas, coste, cooldown, rango, escalado, duracion y dano.
- Valores leidos desde la config actual cuando se abre el panel.

## Clases por set

Los valores siguientes son los valores por defecto.

### Heimdall - tanque de escudo

Tanque defensivo de escudo, bloqueo y control de amenaza.

Equipo y bonus principales:

- Escudo, arma de una mano y armadura defensiva.
- Poder de bloqueo, menor coste de stamina al bloquear, fuerza de bloqueo y salud.
- En rarezas altas gana efectos como `Bulwark`, `ReflectDamage`, `Undying` e `Immovable`.

Habilidades con set completo:

- Pasiva `Heimdall Guard`: bloquear ataques da `+10%` reduccion de dano y `+10%` dano por stack durante `6s`; maximo `10` stacks.
- `Mouse3` - `Lightning Storm`: cooldown `30s`; invoca tormenta fija durante `10s`, radio `8m`, tick cada `1s`; dano rayo `24 + 0.60` por nivel de Bloqueo; cada golpe redirige amenaza hacia Heimdall.
- `Mouse4` - `Stone Shield`: coste `35` vigor, cooldown `24s`, duracion `8s`; reduce dano plano `20 + 0.55` por nivel de Bloqueo; refleja `25% + 0.3%` por nivel de Bloqueo del dano mitigado, maximo `75%`.

### Ragnar - berserker de hachas

Berserker de hachas centrado en sostenerse pegando.

Equipo y bonus principales:

- Hachas como arma principal.
- `LifeSteal` y reduccion de coste de vida de ataque.
- En Ancient, el ultimo bonus pasa a `Undying`.

Habilidades con set completo:

- Pasiva `Fury`: cada golpe melee contra enemigo da `+3%` velocidad de ataque y `0.5%` robo de vida durante `6s`; maximo `10` stacks.
- Pasiva `Blood Surge`: cada `3` golpes melee contra enemigos cura `5%` de salud maxima.
- `Mouse3` - `Decay Aura`: toggle; consume `8` vigor/s; radio `6m`; tick cada `1s`; dano veneno + espiritu `8 + 0.35` por nivel de Hachas, aplicado por tipo.

### Hraesvelgr - arquero fisico

Arquero puro de sigilo, bestias, trampas, headshots y rafaga.

Equipo y bonus principales:

- Arco, QuickDraw, coste de tensado, velocidad de proyectil y tasa de fuego.
- `AddBowsSkill`, `HeadHunter`, `TripleBowShot` y dano fisico en rarezas altas.
- `LeatherQuiver` entra como pieza extra desde Epic si BetterArchery esta instalado.

Habilidades con set completo:

- Pasiva `Sneaky`: al agacharte/en sigilo eres invisible para monstruos; ruido `x0.5`, deteccion `x0.3`, velocidad `+15%`.
- Pasiva `Headshot`: cada `3` disparos con arco Hraesvelgr, la flecha cuenta como punto debil/headshot y aplica al menos `x1.5` dano.
- `Mouse3` - `Summon Beasts`: toggle sin cooldown; coste `35` vigor al invocar; invoca lobo y bjorn/bear normales, permanentes hasta morir, perder set, limpiar sesion o guardarlos manualmente.
- `Ctrl + Mouse3` - trampa armada: coste `20` vigor; maximo `5` cargas, recarga `1` cada `60s`; al atrapar un enemigo lo inmoviliza y da `30s` para que la siguiente flecha haga `+50%` dano.
- `Mouse4` - `Rapid Volley`: canaliza `3s`, cooldown `18s`; mientras mantienes ataque y estas quieto, dispara `8` flechas/s; cada flecha hace `x0.5` dano normal, velocidad `90`; coste de vigor del arco `x0.05`; al terminar restaura todo el vigor.
- Ataque secundario - Dash de Freyja: coste `20` eitr, cooldown `10s`, sin dano propio.

### Solomon Kane - ballestero

Ballestero cazador de brujas con virotes imbuidos, bombas y ejecuciones encadenadas.

Equipo y bonus principales:

- Ballesta, `AddCrossbowsSkill`, `ExplosiveArrows`, `TripleBowShot`, `QuickDraw`, tasa de fuego y velocidad de proyectil.
- Empieza en Rare; no tiene version Magic.

Habilidades con set completo:

- `Mouse3` - `Infused Bolt`: coste `25` vigor, cooldown `12s`; arma el siguiente virote durante `8s`; al impactar explota en `3.5m`; dano total `24 + 0.55` por nivel de Ballestas, repartido entre frost y espiritu; aplica slow `35%` durante `4s`.
- `Mouse4` - `Blackpowder Bomb`: coste `35` vigor, cooldown `18s`; rango `35m`, radio `5m`, empuje `120`; dano total `38 + 0.65` por nivel de Ballestas, repartido en `40%` fuego y `60%` contundente.
- `Ctrl + Mouse4` - `Bat Form`: coste `40` vigor, cooldown `45s`; transforma temporalmente al personaje durante `12s`, oculta el cuerpo/equipo visible y activa vuelo temporal a velocidad base `9`. Si `Shawesome's Dark Gift` esta instalado, usa el visual `vampireformIV` incluido en `shwsmDarkGiftIV`; si no, vuelve al visual de murcielago disponible. Pulsar de nuevo cancela la forma y restaura visibilidad, gravedad y estado de vuelo previo.
- Pasiva `Witchmark`: impactos directos con ballesta Solomon Kane marcan enemigos durante `8s`.
- Pasiva `Silver Verdict`: matar a un marcado o golpearle punto debil arma el siguiente virote; suma dano espiritu `30 + 0.60` por nivel de Ballestas, encadena hasta `2` enemigos en `15m`, pierde `30%` por salto y devuelve `15` vigor al consumirse.

### Nott - duelista/asesino

Duelista de cuchillos, veneno, movilidad y sigilo.

Equipo y bonus principales:

- Cuchillos, ruido reducido, `Duelist`, dano/duracion de stagger, `Opportunist`, movimiento y esquiva.

Habilidades con set completo:

- Pasiva `Sneaky`: al agacharte/en sigilo eres invisible para monstruos; ruido `x0.4`, deteccion `x0.2`, velocidad `+20%`.
- Pasiva `Shadow Momentum`: golpear enemigos da `+30%` velocidad durante `5s`; no stackea, se refresca.
- Pasiva `Poison Edge`: cada golpe contra enemigos anade veneno `8 + 0.35` por nivel de Cuchillos.
- `Mouse3` - `Warp`: coste `25` vigor, cooldown `18s`, rango `18m`; teletransporta detras del enemigo apuntado, al mas cercano si no hay objetivo apuntado, o hacia delante si no hay objetivo valido.
- Despues de `Warp`, `Warp Strike` hace que el siguiente ataque contra enemigo pegue `x2.5` dano.
- Si muere un enemigo a `18m` mientras Warp esta en cooldown, se reinicia el cooldown y recuperas vigor.

### Seidr - mago elemental

Mago de eitr, control de zona e invocacion elemental.

Equipo y bonus principales:

- Bastones, eitr maximo, regeneracion de eitr, menor coste de eitr, tasa de magia.
- `DoubleMagicShot`, `AddElementalMagicSkill` y `ModifyElementalDamage` en rarezas altas.
- Empieza en Rare; no tiene version Magic.

Habilidades con set completo:

- `Mouse3` - `Nanocube`: coste `50` eitr, cooldown `30s`, duracion `10s`; empuja enemigos fuera de `5m`; dentro del cubo tu dano magico se multiplica `x1.30`.
- `Mouse4` - `Elemental Shield`: toggle; coste inicial `25` eitr; sin cooldown; eres inmune al dano mientras haya eitr; consume `2%` de eitr maximo por segundo, minimo `1` eitr/s.
- Ataque secundario - `Stone Golem`: coste `80` eitr, cooldown `60s`, duracion `60s`; invoca un golem friendly de Brokkr con stats de criatura.
- `Mouse4 + bloquear` - `Frost Nova`: coste `45` eitr, cooldown `12s`, radio `8m`; dano frost `45 + 0.85` por nivel de Magia elemental; slow `40%` durante `5s`.

### Helveig - mago de sangre

Mago de sangre centrado en curacion, espiritu/fuego e invocaciones no muertas.

Equipo y bonus principales:

- `AddBloodMagicSkill`, coste de eitr reducido, salud maxima, regeneracion, dano/vida de invocaciones.
- El baston de sangre reduce coste de vida de ataque.

Habilidades con set completo:

- `Mouse3` - `Holy Heal`: coste `25` eitr, cooldown `10s`; cura `35 + 0.75` por nivel de Magia de sangre.
- `Mouse4` - `Blood Rite`: coste `45` eitr, cooldown `20s`; canaliza `10s`, radio `30m`, tick cada `1s`; cura a ti, jugadores, aliados y domesticados `12 + 0.30` por nivel de Magia de sangre por tick; moverte mas de `0.65m` cancela; el cooldown empieza al terminar o romperse.
- Ataque secundario - `Holy Strike`: coste `25` eitr, cooldown `6s`, rango `30m`; dano fuego `25 + 0.45` y espiritu `35 + 0.70` por nivel de Magia de sangre.
- `Mouse3 + bloquear` - `Summon Undead`: coste `55` eitr, cooldown `60s`, duracion `60s`; la invocacion depende de Magia de sangre: `0-29 Skeleton Hildir`, `30-59 Fallen Warrior`, `60-89 Unbjorn`, `90-100 Charred Dyrnwyn`.

### Moonvein - arquero magico

Arquero de eitr que mezcla arco, `SpellSword` y magia elemental.

Equipo y bonus principales:

- Moonbow, `SpellSword`, `EitrLeech`, `AddBowsSkill`, `IncreaseEitr` y `AmmoConservation = 100`.
- `LeatherQuiver` entra como pieza extra desde Epic si BetterArchery esta instalado.
- Cada disparo con Moonbow consume eitr antes de reducciones de `ModifyAttackEitrUse`: Magic `6`, Rare `8`, Epic `10`, Legendary `12`, Mythic `14`, Ancient `16`.

Habilidades con set completo:

- Pasiva `Disparos cargados`: cada tercer disparo consecutivo lanza un hechizo aleatorio desde el arco.
- Probabilidades: Acid Bolt `40%`, Lightning Bolt `40%`, Fireball `20%`.
- Acid Bolt: veneno `20 + 0.50` por nivel de Magia elemental.
- Lightning Bolt: rayo `28 + 0.65` por nivel de Magia elemental.
- Fireball: fuego `38 + 0.80` por nivel de Magia elemental.
- `Mouse3` - `Meteor`: coste `45` eitr, cooldown `10s`, rango `60m`; dano fuego + contundente, cada tipo `70 + 1.20` por nivel de Magia elemental; empuje `80`.
- `Mouse4` - `Tornado Shot`: coste `35` eitr, cooldown `12s`; arma la siguiente flecha; al impactar invoca tornado de Njord durante `6s`; slow `40%` durante `6s`; fallback de dano en `4m`: rayo `1 + 0.39` por nivel de Magia elemental cada `0.5s`.

### Frostbrand - spellblade

Espadachin magico de arma a dos manos. Usa eitr, fuego, escudo elemental y ataques de Surt.

Equipo y bonus principales:

- Espada a dos manos, `SpellSword`, `EitrLeech`, `ModifyAttackEitrUse`, `AddSwordsSkill`, dano elemental/frost.
- El arma da `IncreaseEitr = 100` para que `SpellSword` pueda atacar aunque el personaje no tenga eitr base.

Habilidades con set completo:

- `Mouse3` - `Water Sphere`: coste `35` eitr, cooldown `20s`, duracion `8s`; usa la esfera de Njord y atrae enemigos en `20m`; no anade dano propio.
- Ataque secundario - `Slash`: coste `30` eitr, cooldown `8s`; dano fuego + slash `50 + 0.90` por nivel de Magia elemental.
- `Mouse4` - `Crush`: coste `45` eitr, cooldown `14s`; salto/impacto con dano fuego + contundente `65 + 1.10` por nivel de Magia elemental; deja `Burning Ground` durante `10s`, tick cada `1s`, fuego `16 + 0.32` por nivel de Magia elemental.
- `Mouse4 + bloquear` - `Elemental Shield`: coste `50` eitr, cooldown `24s`, duracion `8s`; anula fuego y mitiga todo dano `25% + 0.3%` por nivel de Magia elemental, maximo `70%`.
- Pasiva `Fire Ball`: cada `3` ataques iniciados con arma Frostbrand lanza Fire Ball donde apuntas; dano fuego `45 + 0.85` por nivel de Magia elemental; fallback en radio `3.5m` si el proyectil no esta disponible.

### Thor - set especial Epic

Set especial de hacha arrojadiza y tormenta.

No tiene controlador de hotkeys propio. Su identidad viene de:

- Hacha arrojadiza.
- Recall del arma.
- Dano de rayo.
- `ChainLightning`.

### Floki - set especial Epic

Set especial de constructor.

No tiene controlador de hotkeys propio. Su identidad viene de:

- Martillo de construccion.
- `FreeBuild`.
- Distancia de construccion.
- Peso/carga.
- Stamina y herramientas duraderas.

## LeatherQuiver de BetterArchery

`LeatherQuiver` se trata como pieza `Utility` para EpicLoot y entra en loot desde Epic en adelante.

Sets que lo usan:

- `EpicHraesvelgr`
- `Hraesvelgr`
- `MythicHraesvelgr`
- `AncientHraesvelgr`
- `EpicMoonvein`
- `Moonvein`
- `MythicMoonvein`
- `AncientMoonvein`

No sustituye piezas: se suma como pieza extra desde Epic.

- Epic: `6` piezas y `6` bonus.
- Legendary: `7` piezas y `7` bonus.
- Mythic: `8` piezas y `8` bonus.
- Ancient: `9` piezas y `9` bonus, manteniendo tambien el trinket final.

El bonus extra final es `HeadHunter` para Hraesvelgr y `ModifyElementalDamage` para Moonvein.

## Cambios de EpicLoot incluidos

- Efectos de arco/ballesta extendidos para `Bows` y `Crossbows`.
- `ModifyAttackEitrUse` disponible en Magic con valores bajos.
- `LifeSteal` permitido en Magic para balance de Ragnar.
- `IncreaseEitr` permitido en armas de una y dos manos.
- `FreeBuild` permitido en Epic para Floki.
- Ajustes de exclusividad entre `SpellSword`, `Duelist` y `EitrWeave`.
- Pools de loot por rareza para piezas de set.
- Pools de bosses para empujar progresion por etapa.
- Bestiary: las 21 criaturas `RDB_*` heredan tiers de loot y aparecen como objetivos de bounty.
- Tienda/adventure data con piezas generadas.

## Configuracion principal

Archivo:

`config/fran.mods.epiclootraritysets.cfg`

Opciones importantes:

- `Generate Managed Config Files`: escribe las configs gestionadas desde la DLL.
- `Enable Natural Drops`: permite conversion de tiradas normales de EpicLoot en piezas de set.
- `Enable Set Activation Buffs`: muestra buffs de set completo.
- `Enable Frostbrand Abilities`
- `Enable Hraesvelgr Abilities`
- `Enable Solomon Kane Abilities`: activa `Infused Bolt`, `Blackpowder Bomb`, `Bat Form` y la pasiva `Witchmark`/`Silver Verdict`.
- `Enable Moonvein Abilities`
- `Enable Nott Abilities`
- `Enable Ragnar Abilities`
- `Enable Helveig Abilities`
- `Enable Heimdall Abilities`
- `Enable Seidr Abilities`
- `Read Extra Sections From Legendaries Json`
- `Read Rarity Sets Json`
- `Read Boss Set Drop Rules`
- `Configure WolfPack`

Cada seccion de habilidades permite cambiar teclas, costes, cooldowns, radios, duraciones y escalados.

## Desarrollo

Las configs embebidas viven en:

`src/GeneratedConfig/`

Cuando cambies JSON/CFG manualmente y quieras que viajen dentro del mod, actualiza esos archivos y recompila la DLL.

Build:

`tools/Build-EpicLootRaritySets.ps1`

Verificacion:

`tools/Verify-RecoveredRaritySets.ps1`
