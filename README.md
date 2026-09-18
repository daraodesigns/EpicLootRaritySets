# EpicLootRaritySets

Convierte EpicLoot en una progresion estable de clases por equipo para Valheim. Desde la version `1.0.0`, el mod se considera version estable y preparada para jugar disfrutando de la experiencia completa.

Este mod anade sets personalizados por rareza, drops por etapa de boss, piezas especiales, bonus de set y habilidades activas al completar sets. La idea es que el jugador no solo encuentre objetos mejores: que pueda construir un personaje completo alrededor de una clase de equipo.

Si te gusta EpicLoot pero quieres objetivos claros de farmeo, builds reconocibles y habilidades nuevas al cerrar un set completo, este mod es para eso.

Dejo mi ko-fi por si te animas a dejarme una propina que me ayude a crecer. Muchas gracias y un abrazo por el apoyo a todos: https://ko-fi.com/daraodesigns

## Agradecimientos

Agradecimientos a Alpus, Radamanto y RandyKnapp por sus mods, de los cuales he partido o utilizo para mejorar la jugabilidad de valheim.

## Que aporta

- Sets por rareza: `Magic`, `Rare`, `Epic`, `Legendary`, `Mythic` y `Ancient`.
- Clases por set: tanque, berserker, arquero, ballestero, asesino, mago elemental, mago de sangre, arquero magico y spellblade.
- Habilidades activas y pasivas al completar el set.
- Buffs visibles para set activo, pasivas, estados temporales y cargas.
- Panel movible de habilidades activas con icono, nombre, tecla y cooldown.
- Panel movible de pasivas estaticas con placa y 2 orbes de carga para Moonvein, Frostbrand, Hraesvelgr y Oleada de sangre de Ragnar.
- Bonus de set crecientes segun rareza y numero de piezas.
- Drops naturales de piezas de set desde EpicLoot.
- Drops garantizados de boss por etapa cuando hay una tirada valida.
- Configs de EpicLoot y NorseDemigods embebidas en la DLL y sincronizadas al arrancar.
- Entrada nueva en el compendio/textos del juego: `Epic Loot Rarity Sets`, con descripcion de cada clase, habilidades, buffs y pasivas traducidos.
- Textos, tooltips y compendio en espanol o ingles segun el idioma activo del juego.
- Ordenes globales de mascotas con combinaciones `Ctrl`.
- Integracion opcional con BetterArchery para `LeatherQuiver`.
- Integracion con Bestiary para criaturas `RDB_*` en loot y contratos.
- Compatibilidad opcional con WolfPack para controlar invocaciones de Hraesvelgr.
- Compatibilidad opcional con Wires Enemy HUD: fuerza `HealthDisplay = Current HP/Max HP` si el mod esta instalado.

## Dependencias

Obligatorias:

- `RandyKnapp-EpicLoot`
- `Alpus-NorseDemigods`
- `Radamanto-Bestiary`

Opcionales:

- `BetterArchery`: integra `LeatherQuiver` como pieza encantable en sets de arquero desde Epic en adelante.
- `WolfPack`: el mod puede ajustar su config para que las bestias invocadas por Hraesvelgr sean entrenables/controlables.
- `WiresEnemyHUD`: el mod puede ajustar su config para mostrar vida actual y maxima sobre enemigos.

Aunque NorseDemigods aparece como dependencia, no tienes que jugar una clase de NorseDemigods. Este mod suprime su UI, energia, seleccion de clase e input cuando usa sus efectos como puente visual/mecanico.

## Como funciona

El mod crea arquetipos de set. Al equipar todas las piezas no-arma de una misma linea, se activa un buff con el nombre base del set. La unica pieza que puede faltar para activar la clase es el arma del set; no vale que falte cualquier otra pieza.

Ese buff sirve como bandera para habilitar las habilidades de clase. Si tienes el buff de clase activo, puedes usar sus habilidades aunque el arma del set sea la pieza que falta; cuando una habilidad necesita arma, valida el tipo real equipado, no que sea exactamente el arma del set.

Lineas de clase activas:

| Clase | Set IDs que activan la clase |
| --- | --- |
| `Heimdall` | `MagicHeimdall`, `RareHeimdall`, `EpicHeimdall`, `Heimdall`, `MythicHeimdall`, `AncientHeimdall` |
| `Ragnar` | `MagicRagnar`, `RareRagnar`, `EpicRagnar`, `Ragnar`, `MythicRagnar`, `AncientRagnar` |
| `Hraesvelgr` | `MagicHraesvelgr`, `RareHraesvelgr`, `EpicHraesvelgr`, `Hraesvelgr`, `MythicHraesvelgr`, `AncientHraesvelgr` |
| `Hellsyng` | `RareHellsyng`, `EpicHellsyng`, `Hellsyng`, `MythicHellsyng`, `AncientHellsyng` |
| `Nott` | `MagicNott`, `RareNott`, `EpicNott`, `Nott`, `MythicNott`, `AncientNott` |
| `Seidr` | `RareSeidr`, `EpicSeidr`, `Seidr`, `MythicSeidr`, `AncientSeidr` |
| `Helveig` | `MagicHelveig`, `RareHelveig`, `EpicHelveig`, `Helveig`, `MythicHelveig`, `AncientHelveig` |
| `Moonvein` | `MagicMoonvein`, `RareMoonvein`, `EpicMoonvein`, `Moonvein`, `MythicMoonvein`, `AncientMoonvein` |
| `Frostbrand` | `MagicFrostbrand`, `RareFrostbrand`, `EpicFrostbrand`, `Frostbrand`, `MythicFrostbrand`, `AncientFrostbrand` |

Los buffs de set completo muestran el nombre de clase y el nivel de habilidad (`Nivel`/`Level`). Las habilidades activas no usan la barra de buffs para sus cooldowns antiguos: se muestran en el panel de habilidades con nombre, tecla y contador. Las pasivas y estados temporales van en el area de buffs; las pasivas estaticas con cargas de Moonvein, Frostbrand, Hraesvelgr y Oleada de sangre de Ragnar se muestran ademas en el panel de orbes.

## Sistemas comunes

- `Panel de habilidades`: aparece al tener un buff de clase activo; muestra solo habilidades activas, su nombre, tecla y cooldown. Se puede arrastrar mientras el inventario esta abierto con `Tab`. Si hay mascotas activas, anade una seccion `Pets` con `Atacar` (`Ctrl + Mouse4`), `Seguir` (`Ctrl + Mouse3`) y `Libre` (`Ctrl + ataque secundario`); el estado actual queda resaltado.
- `Panel de pasivas`: muestra placas de 2 orbes para pasivas estaticas con carga (`Disparos cargados`, `Bola de fuego`, `Disparo certero`, `Oleada de sangre`). Al llenar los 2 orbes, la placa completa se ilumina con borde pulsante y etiqueta `LISTO` para indicar que el siguiente disparo/ataque esta cargado. Se puede arrastrar con `Tab`.
- `Ctrl + Mouse4`: ordena a las mascotas activas atacar al enemigo apuntado. Si el enemigo muere o deja de ser valido, pasan a seguir al jugador.
- `Ctrl + Mouse3`: fuerza a las mascotas activas a seguir al jugador aunque haya enemigos cerca.
- `Ctrl + ataque secundario`: libera a las mascotas al modo normal, sin seguimiento forzado.
- Las ordenes de mascotas se aplican a invocaciones persistentes o controladas de `Hraesvelgr`, `Hellsyng`, `Helveig` y `Seidr`, incluido el `Golem de piedra`.
- Las mascotas persistentes desaparecen al perder el buff de clase. Si mueren mientras siguen activas, usan un cooldown de reaparicion de `30s` cuando su clase lo permite.
- Las explosiones propias de `T.N.T.`, `Fuego rapido` y efectos equivalentes evitan dano a aliados, jugadores friendly y mascotas.
- Los buffs de habilidades, invocaciones y estados temporales usan los iconos generados del mod cuando existe icono especifico o icono de habilidad equivalente.
- El compendio, los nombres de habilidades, tooltips y buffs usan espanol o ingles segun el idioma activo de Valheim.

## Probabilidades de drops naturales

Las tiradas magicas normales de EpicLoot pueden convertirse en piezas de set:

- Magic: `12%`
- Rare: `10%`
- Epic: `8%`
- Ancient: `5%`

Legendary y Mythic se gestionan desde las secciones generadas de EpicLoot y sus pools propios.

`bosssetdrops.json` fija la rareza de cada boss y convierte las tiradas validas de equipo en piezas aleatorias de set de esa etapa. No hereda una rareza aleatoria de la tirada base: cada boss usa su rareza asignada, valida que el prefab base pertenezca al pool permitido (`ForceSetDropItems`) y despues elige al azar entre los IDs reales de set de esa misma rareza (`ForceSetItemIds`).

| Boss | Rareza de set |
| --- | --- |
| Eikthyr | `Magic` |
| Anciano | `Rare` |
| Bonemass | `Epic` |
| Moder | `Legendary` |
| Yagluth | `Mythic` |
| Reina | `Mythic` |
| Fader | `Ancient` |
| FrozenKing | `Ancient` |

## Configs gestionadas

Con `Generate Managed Config Files = true`, la DLL escribe/sincroniza:

- `config/EpicLoot/baseconfig/abilities.json`
- `config/EpicLoot/raritysets.json`
- `config/EpicLoot/baseconfig/legendaries.json`
- `config/EpicLoot/baseconfig/loottables.json`
- `config/EpicLoot/baseconfig/iteminfo.json`
- `config/EpicLoot/baseconfig/magiceffects.json`
- `config/EpicLoot/baseconfig/adventuredata.json`
- `config/EpicLoot/bosssetdrops.json`
- `config/NorseDemigods.cfg`

Si un archivo existe y es distinto, primero crea backup `.bak-fran-managed-...` y despues escribe la version embebida. Por eso, los cambios manuales en JSON no son persistentes si no se actualizan tambien los recursos embebidos del DLL principal.

Para servidor, normalmente basta con subir los DLLs incluidos en el paquete y reiniciar:

- `EpicLootRaritySets.dll`: DLL principal con sets, configs embebidas, compendio, clases, paneles, iconos y controladores base.
- `EpicLootRaritySetsHotfix.dll`: capa incluida para habilidades y ajustes que siguen separados del DLL principal cuando el paquete la incluya.

## Bonus custom de EpicLoot

- `Last Hope`: magic effect con ability propia. Cuando la salud entra en estado critico, evita el golpe entrante si lo hay, cura el `100%` de la salud maxima y activa un cooldown de `60s`.
- Efectos custom de clase como `AddFrostbrandSkill`, `AddHellsyngSkill`, `AddSeidrSkill`, etc. aumentan el nivel efectivo de la skill de clase y pueden hacer que el escalado supere nivel `100`.
- Los efectos usados en sets de rarezas bajas tambien tienen `ValuesPerRarity` configurados para evitar bonus sin valor, como `FrostDamageAOE` y `Bulwark`.
- En los sets de Ragnar/Frostbrand, cada bonus `FrostDamageAOE` va emparejado en el mismo contador con `AddFrostDamage`, porque EpicLoot solo dispara el area de hielo si el golpe del arma ya contiene dano frost.

## Compendio dentro del juego

El mod anade una entrada al panel de textos/compendio del inventario:

`Epic Loot Rarity Sets`

Incluye:

- Explicacion del sistema de rarezas.
- Descripcion de cada clase por set.
- Habilidades, teclas, coste, cooldown, rango, escalado, duracion y dano.
- Valores leidos desde la config actual cuando se abre el panel.
- Secciones de efectos activos con buffs, cargas, tiempos de reutilizacion, pasivas y estados temporales.
- Nombres de habilidades, buffs y pasivas localizados al idioma activo, igual que en el juego.
- La pagina nativa de EpicLoot `Conjuntos legendarios` se amplia para comparar tambien `Magic`, `Rare`, `Epic` y `Ancient` junto a las rarezas especiales.

## Clases por set

Los valores siguientes son los valores por defecto.

### Heimdall - tanque de escudo

Tanque defensivo de escudo, bloqueo y control de amenaza.

Equipo y bonus principales:

- Escudo, arma de una mano y armadura defensiva.
- Poder de bloqueo, menor coste de stamina al bloquear, fuerza de bloqueo y salud.
- En rarezas altas gana efectos como `Bulwark`, `ReflectDamage`, `Undying` e `Immovable`.

Habilidades con set completo:

- Pasiva `Guardia de Heimdall`: recibir dano da `5%` de reduccion de todo el dano y `+10%` poder de bloqueo por carga durante `6s`; maximo `3` cargas.
- `Mouse3` - `Tormenta de rayos`: cooldown `30s`; invoca tormenta fija durante `10s`, radio `8m`, pulso cada `1s`; dano rayo `24 + 0.60` por nivel de Heimdall; cada golpe redirige amenaza hacia Heimdall.
- `Mouse4` - `Escudo de piedra`: coste `35` vigor, cooldown `24s`, duracion `8s`; reduce dano plano `20 + 0.55` por nivel de Heimdall; refleja `25% + 0.3%` por nivel de Heimdall del dano mitigado, maximo `75%`.
- `Bloqueo + Mouse3` - `Arpon abisal`: cooldown `12s`; lanza una cuerda visible con aura al enemigo apuntado y lo atrae durante `5s`.

### Ragnar - berserker de hachas

Berserker de clase Ragnar centrado en sostenerse pegando.

Equipo y bonus principales:

- Hachas como arma principal.
- `LifeSteal` y reduccion de coste de vida de ataque.
- En Ancient, el ultimo bonus pasa a `Undying`.

Habilidades con set completo:

- Pasiva `Furia de Ragnar`: cada golpe cuerpo a cuerpo contra enemigo activa un buff visible de `6s` con cargas; cada carga da `+3%` velocidad de ataque y `0.5%` robo de vida; maximo `10` cargas.
- Pasiva `Oleada de sangre`: se muestra en el panel de pasivas con 2 orbes; al llegar a `2/2`, la placa se ilumina y el siguiente golpe cuerpo a cuerpo contra enemigos cura `5%` de salud maxima.
- `Mouse3` - `Aura de decadencia`: conmutador; consume `8` vigor/s; radio `6m`; pulso cada `1s`; dano directo `8 + 0.35` por nivel de Ragnar. No aplica veneno.
- `Mouse4` - `Frenesi de sangre`: sacrifica `30%` de salud maxima; durante `10s` da `+50%` velocidad de ataque, `+30%` velocidad de movimiento e `Immovable`; cooldown `30s`.
- Saltar + `Mouse4` - `Crush`: coste `45` vigor, cooldown `14s`; replica el salto/ataque de Surt y al golpear el suelo activa tambien `Suelo ardiente`.

### Hraesvelgr - arquero fisico

Arquero puro de sigilo, bestias, trampas, headshots y rafaga.

Equipo y bonus principales:

- Arco, QuickDraw, coste de tensado, velocidad de proyectil y tasa de fuego.
- `AddHraesvelgrSkill`, `HeadHunter`, `TripleBowShot` y dano fisico en rarezas altas.
- `LeatherQuiver` entra como pieza extra desde Epic si BetterArchery esta instalado.

Habilidades con set completo:

- Pasiva `Sigilo`: al agacharte/en sigilo eres invisible para monstruos; ruido `x0.5`, deteccion `x0.3`, velocidad `+15%`.
- Pasiva `Disparo certero`: cada `3` disparos con arco Hraesvelgr, la flecha cuenta como punto debil/headshot y aplica al menos `x1.5` dano. El panel de pasivas muestra `2/2` e ilumina la placa cuando la siguiente flecha esta cargada.
- `Mouse3` - `Invocar bestias`: toggle sin cooldown; invoca o guarda un bjorn/bear aliado; coste `35` vigor al invocar. Si muere estando activo, entra en cooldown de mascota muerta durante `60s`. Muestra buff visible mientras queda vivo.
- `Bloqueo + Mouse3` - trampa armada: coste `20` vigor; maximo `5` cargas, recarga `1` cada `60s`; al atrapar un enemigo hace dano perforante `35 + 0.55` por nivel de Hraesvelgr, lo inmoviliza y da `30s` para que la siguiente flecha haga `+50%` dano.
- `Mouse4` - `Rafaga rapida`: canaliza hasta `3s`, cooldown `18s`; mientras mantienes ataque y estas quieto, dispara como maximo `8` flechas exactas a `8` flechas/s; cada flecha hace `x0.5` dano normal, velocidad `90`. Si arma/flecha no aportan dano, usa alternativa perforante `12 + 0.25` por nivel de Hraesvelgr. Al llegar a 8 flechas se cancela y restaura todo el vigor.
- Ataque secundario - Dash de Freyja: coste `20` vigor, cooldown `10s`, sin dano propio.

### Hellsyng - ballestero

Ballestero cazador de brujas centrado en mascotas, cambios de forma, marcas explosivas y rafagas de ballesta.

Equipo y bonus principales:

- Ballesta, `AddHellsyngSkill`, `TripleBowShot`, `QuickDraw`, tasa de fuego y velocidad de proyectil.
- `ExplosiveArrows` no queda permanente en el set Hellsyng: se activa temporalmente con `Fuego rapido`.
- Empieza en Rare; no tiene version Magic.

Habilidades con set completo:

- Pasiva `T.N.T.`: los disparos de ballesta Hellsyng marcan al objetivo. Si un enemigo marcado muere, detona con una explosion de fuego estilo disparo explosivo; no dana aliados, jugadores friendly ni mascotas. Dano fuego `45 + 0.65` por nivel de Hellsyng.
- Pasiva `Balas de plata`: los disparos de ballesta hacen `+20%` dano contra muertos vivientes y criaturas similares: skeletons, draugr, ghosts, wraiths, vil/Bjorn muerto, espectros y variantes de no-muertos de Bestiary.
- Ataque secundario sin cambio de forma - `Mascotas Hellsyng`: invoca o guarda un lobo y un murcielago aliados. El murcielago usa la vida reforzada del lobo para que no caiga tan rapido. Cooldown `30s` al invocar o guardar. Si mueren estando activos, reaparecen pasados `30s`. Muestra buff visible mientras queda alguna mascota viva.
- `Bloqueo + Mouse4` - `Forma de hombre lobo`: dura `30s` o hasta cancelarse; cooldown `60s`, que empieza al acabar o cancelar. `Caceria` aumenta la velocidad del lobo un `40%` si hay enemigos a `50m`. Ataque normal: mordisco con cooldown `4s`, sin slash, aplica sangrado durante `10s` con dano `5 + 0.18` por nivel de Hellsyng por segundo. Ataque secundario: zarpazo con slash, derribo garantizado, cooldown `4s` y dano slash `45 + 0.55` por nivel de Hellsyng.
- `Bloqueo + Mouse3` - `Forma de murcielago`: dura `30s` o hasta cancelarse; cooldown `60s`, que empieza al acabar o cancelar. Permite volar, no permite correr con shift y usa offset visual configurable para elevar el murcielago en camara. Ataque normal: drenaje de murcielago con dano `24 + 0.35` por nivel de Hellsyng que cura el `100%` del dano hecho. Ataque secundario: `Regeneracion de murcielago`, regeneracion de salud `+100%` durante `6s`, cooldown `20s`.
- `Mouse4` - `Horda de murcielagos`: invoca una horda de murcielagos aliados sobre el objetivo durante `30s`. Escala con Hellsyng: `3/6/9/10` murcielagos, estrellas `0/1/2` y multiplicador de dano `1 + skill * 0.01`. Cooldown `60s`.
- `Mouse3` - `Fuego rapido`: durante `10s` la ballesta recarga en `0.5s`; los disparos explotan usando `50%` del dano real del impacto y rebotan hasta `2` veces entre enemigos cercanos. Si el impacto no trae dano, usa alternativa escalada con Hellsyng: fuego `18 + 0.25` y espiritu `12 + 0.20` por nivel. Cooldown `30s`.

### Nott - duelista/asesino

Duelista de clase Nott, veneno, movilidad y sigilo.

Equipo y bonus principales:

- Cuchillos, ruido reducido, `Duelist`, dano/duracion de stagger, `Opportunist`, movimiento y esquiva.

Habilidades con set completo:

- Pasiva `Sigilo`: al agacharte/en sigilo eres invisible para monstruos; ruido `x0.4`, deteccion `x0.2`, velocidad `+20%`.
- Pasiva `Momentum sombrio`: golpear enemigos da `+30%` velocidad y `+15%` dano durante `5s`; no acumula cargas, se refresca.
- Pasiva `Filo venenoso`: cada golpe contra enemigos anade veneno `8 + 0.35` por nivel de Nott.
- Pasiva `Ejecutor`: todo el dano de Nott contra enemigos que ya estan al `30%` o menos de vida hace `300%` de dano total.
- `Mouse3` - `Warp`: coste `25` vigor, cooldown `18s`, rango `18m`; teletransporta detras del enemigo apuntado, al mas cercano si no hay objetivo apuntado, o hacia delante si no hay objetivo valido. Tras Warp, `Guardia de Warp` reduce el dano recibido un `90%` durante `3s`.
- `Mouse4` - `Marca de sombra`: marca al enemigo seleccionado a `5m` durante `6s` con un efecto visual de sombra sobre el objetivo; acumula el `50%` del dano que reciba mientras dura y al terminar explota en dano de espiritu sobre el objetivo. Cooldown `60s`; si muere un enemigo a `20m` mientras esta en cooldown, se reduce `10s`.
- Ataque secundario - `Golpe de cuchillo`: requiere cuchillo, cooldown `8s`, rango `5m`; golpea al enemigo apuntado o frontal mas cercano con `50%` del dano del arma + `0.75` de dano de tajo por nivel de Nott, usa visual de slash de Norse y ralentiza `50%` durante `5s`.
- Despues de `Warp`, `Golpe de Warp` hace que el siguiente ataque contra enemigo pegue `x2.5` dano.
- Si muere un enemigo a `18m` mientras Warp esta en cooldown, se reinicia el cooldown y recuperas vigor.

### Seidr - mago elemental

Mago de eitr, control de zona e invocacion elemental.

Equipo y bonus principales:

- Bastones, eitr maximo, regeneracion de eitr, menor coste de eitr, tasa de magia.
- `DoubleMagicShot`, `AddSeidrSkill` y `ModifyElementalDamage` en rarezas altas.
- Empieza en Rare; no tiene version Magic.

Habilidades con set completo:

- `Mouse3` - `Nanocubo`: coste `50` eitr, cooldown `30s`, duracion `10s`; empuja enemigos fuera de `5m`; dentro del cubo tu dano magico se multiplica `x1.30`.
- `Mouse4` - `Escudo elemental`: toggle; coste inicial `25` eitr; sin cooldown; eres inmune al dano mientras haya eitr; consume `2%` de eitr maximo por segundo, minimo `1` eitr/s.
- Ataque secundario - `Golem de piedra`: coste `80` eitr, cooldown `60s`, duracion `60s`; invoca un golem friendly de Brokkr con stats de criatura. Obedece las ordenes globales de mascota.
- `Bloqueo + Mouse4` - `Nova de escarcha`: coste `45` eitr, cooldown `12s`, radio `8m`; dano frost `45 + 0.85` por nivel de Seidr; ralentizacion `40%` durante `5s`.

### Helveig - mago de sangre

Mago de sangre centrado en curacion, espiritu/fuego, guardaespaldas no muertos e invocaciones de bestias.

Equipo y bonus principales:

- `AddHelveigSkill`, coste de eitr reducido, salud maxima, regeneracion, dano/vida de invocaciones.
- El baston de sangre reduce coste de vida de ataque.

Habilidades con set completo:

- `Mouse3` - `Curacion sagrada`: coste `25` eitr, cooldown `10s`; cura `35 + 0.75` por nivel de Helveig al aliado apuntado en rango (`30m`) o a ti si no hay objetivo.
- Pasiva `Guardaespaldas no muerto`: al activar el set aparece `Charred_Melee_Dyrnwyn`; desaparece al perder el set. Si muere, reaparece automaticamente tras `30s`. Vida/dano escalan con Helveig de forma conservadora y obedece ordenes de mascota.
- Pasiva `Egida de sangre`: cada curacion real aplica al objetivo curado un escudo no acumulable equivalente al `15%` de la sanacion recibida durante `6s`.
- Pasiva `Devocion sanguinea`: cada curacion real otorga una carga, hasta `3`. Cada carga aumenta `+5%` el dano de invocaciones y `+10%` el dano de `Golpe sagrado`.
- `Mouse4` - `Rito de sangre`: coste `45` eitr, cooldown `20s`; canaliza `10s`, radio `30m`, pulso cada `1s`; cura a ti, jugadores, NPCs aliados, mascotas activas y domesticados `12 + 0.30` por nivel de Helveig por pulso; moverte mas de `0.65m` cancela; el cooldown empieza al terminar o romperse.
- Ataque secundario - `Golpe sagrado`: coste `25` eitr, cooldown `6s`, rango `30m`; dano fuego `55 + 0.45` y espiritu `65 + 0.70` por nivel de Helveig. Cada impacto devuelve `2` eitr.
- `Bloqueo + Mouse3` - `Invocar monstruo`: coste `55` eitr, cooldown `60s`, duracion `20s`; invoca `Ent` / `Abomination` / `ElakingMole` / `FallenValkyrie` segun nivel de Helveig. Vida/dano escalan de forma conservadora, obedece ordenes de mascota y muestra buff activo con tiempo restante.

### Moonvein - arquero magico

Arquero de eitr que mezcla arco, `SpellSword` y escalado Moonvein.

Equipo y bonus principales:

- Moonbow, `SpellSword`, `EitrLeech`, `AddMoonveinSkill`, `IncreaseEitr` y `AmmoConservation = 100`.
- `LeatherQuiver` entra como pieza extra desde Epic si BetterArchery esta instalado.
- Cada disparo con Moonbow consume eitr antes de reducciones de `ModifyAttackEitrUse`: Magic `6`, Rare `8`, Epic `10`, Legendary `12`, Mythic `14`, Ancient `16`.

Habilidades con set completo:

- Pasiva `Disparos cargados`: cada `3` disparos consecutivos lanza un hechizo aleatorio desde el arco. El panel de pasivas muestra `2/2` e ilumina la placa cuando el siguiente disparo esta cargado.
- Pasiva `Disparos arcanos`: los disparos normales con arco Moonvein anaden dano de rayo `8 + 0.20` y espiritu `8 + 0.20` por nivel de Moonvein.
- Pasiva `Recarga`: funciona igual que la de Frostbrand; ataques y habilidades Moonvein suman cargas al golpear. Dura `15s`, cada carga aumenta todo el dano un `4%` hasta `5` cargas, y a `5/5` dispara Cadena de rayos garantizada.
- Probabilidades: Proyectil acido `40%`, Proyectil de rayo `40%`, Bola de fuego `20%`.
- Proyectil acido: veneno `40 + 0.50` por nivel de Moonvein.
- Proyectil de rayo: rayo `48 + 0.65` por nivel de Moonvein.
- Bola de fuego: fuego `58 + 0.80` por nivel de Moonvein.
- `Mouse3` - `Meteoro`: coste `30` eitr, cooldown `10s`, rango `60m`; dano fuego + contundente, cada tipo `35 + 1.20` por nivel de Moonvein; empuje `80`.
- `Mouse4` - `Disparo tornado`: coste `20` eitr, cooldown `12s`; arma la siguiente flecha; al impactar invoca tornado de Njord durante `6s`; ralentizacion `40%` durante `6s`; alternativa de dano en `4m`: rayo `18 + 0.75` por nivel de Moonvein cada `0.5s`.
- `Bloqueo + Mouse3` - `Lobo espiritual`: coste `20` eitr; invoca `wolf_spirit_caller`, obedece el panel de mascotas y escala vida/dano con Moonvein. Si muere, aplica cooldown de `60s`.

### Frostbrand - spellblade

Espadachin magico de arma a dos manos. Usa eitr, fuego, escudo elemental y ataques de Surt.

Equipo y bonus principales:

- Espada a dos manos, `SpellSword`, `EitrLeech`, `ModifyAttackEitrUse`, `AddFrostbrandSkill`, dano elemental/frost.
- El arma da `IncreaseEitr = 100` para que `SpellSword` pueda atacar aunque el personaje no tenga eitr base.

Habilidades con set completo:

- `Mouse3` - `Esfera de agua`: coste `20` eitr, cooldown `20s`, duracion `8s`; usa la esfera de Njord, fuerza el punto de destino a donde apuntas respetando el rango de NorseDemigods y atrae e inmoviliza enemigos en `20m` con fuerza de gravedad `16`. Cada impacto aplica dano contundente en el radio de dano de NorseDemigods (`Aoe Damage Radius = 3` por defecto) y escala con Frostbrand usando `NorseDemigods.cfg` (`Base Damage = 8`, `Damage Per Level = 1.0` por nivel).
- Ataque secundario - `Tajo`: coste `15` eitr, cooldown `8s`; ejecuta la animacion de ataque del arma, aplica dano fuego `16 + 0.90` y tajo `16 + 0.90` por nivel de Frostbrand, y suma al contador de `Bola de fuego`.
- `Bloqueo + Mouse4` - `Escudo elemental`: coste `35` eitr, cooldown `24s`, duracion `8s`; anula fuego y mitiga todo dano `25% + 0.3%` por nivel de Frostbrand, maximo `70%`.
- `Mouse4` - `Golpe de rayo`: reemplaza `Crush`; coste `25` eitr, cooldown `18s`, rango `80m`; lanza 3 impactos de rayo estilo Thor/NorseDemigods, separados `0.25s`, con radio alternativo `3m`; dano por impacto `14 + 1.80` por nivel de Frostbrand.
- Pasiva `Bola de fuego`: cada `3` ataques iniciados con arma Frostbrand o usos de `Tajo` lanza Bola de fuego donde apuntas. El panel de pasivas muestra `2/2` e ilumina la placa cuando el siguiente ataque esta cargado; dano fuego `36 + 0.85` por nivel de Frostbrand; alternativa en radio `3.5m` si el proyectil no esta disponible.
- Pasiva `Recarga`: ataques y habilidades solo suman cargas cuando golpean a un enemigo. Dura `15s`; cada carga aumenta todo el dano un `4%` con maximo `5` cargas. A `5/5`, los ataques activan Cadena de rayos garantizada en cada golpe; salta hasta `3` enemigos cercanos en `8m`, usando parte del dano real del golpe. Los rebotes de la propia Recarga no refrescan sus cargas.

### Thor - set especial Epic

Set especial de hacha arrojadiza y tormenta.

No tiene controlador de hotkeys propio. Su identidad viene de:

- Hacha arrojadiza.
- Recall del arma.
- Dano de rayo.
- Cadena de rayos (`ChainLightning`).

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
- `Enable Ability Panel`: muestra el panel movible de habilidades activas.
- `Ability Panel Position X/Y`, `Scale`, `Opacity`: posicion, escala y opacidad del panel de habilidades.
- `Enable Passive Stack Panel`: muestra el panel movible de pasivas con orbes y estado cargado sobre la placa completa.
- `Passive Stack Panel Position X/Y`, `Scale`, `Opacity`: posicion, escala y opacidad del panel de pasivas.
- `Enable Frostbrand Abilities`
- `Enable Hraesvelgr Abilities`
- `Enable Hellsyng Abilities`: activa `T.N.T.`, `Balas de plata`, `Mascotas Hellsyng`, `Forma de hombre lobo`, `Forma de murcielago`, `Horda de murcielagos` y `Fuego rapido`.
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
- `Configure Wires Enemy HUD`

Cada seccion de habilidades permite cambiar teclas, costes, cooldowns, radios, duraciones y escalados. Los valores del README son los defaults esperados; si una config local antigua ya existe, el mod intenta migrar defaults viejos a los valores actuales con `UpgradeFloatConfig` y `UpgradeShortcutConfig`.

Si el paquete incluye `EpicLootRaritySetsHotfix.dll`, ese complemento puede usar `config/fran.mods.epiclootraritysets.hotfix.cfg` para ajustes separados del DLL principal, como opciones de forma de murcielago, `Frenesi de sangre`, `Marca de sombra` o `Ragnar Crush`.

## Desarrollo

Las configs embebidas viven en:

`src/GeneratedConfig/`

Cuando cambies JSON/CFG manualmente y quieras que viajen dentro del mod, actualiza esos archivos y recompila la DLL. Si `Generate Managed Config Files` sigue activo y los recursos embebidos de la DLL estan desactualizados, el siguiente arranque volvera a escribir la version vieja.

Build:

`tools/Build-EpicLootRaritySets.ps1`

Verificacion:

`tools/Verify-RecoveredRaritySets.ps1`
