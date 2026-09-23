# EpicLootRaritySets

Turns EpicLoot into a stable equipment-based class progression system for Valheim. Since version `1.0.0`, the mod is considered stable and ready for a full playthrough experience.

This mod adds custom rarity sets, boss-stage drops, special pieces, set bonuses and active abilities unlocked by completing sets. The idea is not just to find stronger items, but to build a full character around a recognizable equipment class.

If you like EpicLoot but want clearer farming goals, recognizable builds and new abilities when you complete a full set, this mod is built for that.

I am leaving my Ko-fi here in case you want to leave a tip and help me keep growing. Thank you very much for the support: https://ko-fi.com/daraodesigns

## Credits

Thanks to Alpus, Radamanto and RandyKnapp for their mods, which this project builds on or uses to improve Valheim gameplay.

## What It Adds

- Rarity sets for `Magic`, `Rare`, `Epic`, `Legendary`, `Mythic` and `Ancient`.
- Set-based classes: tank, berserker, archer, crossbow hunter, assassin, elemental mage, blood mage, magical archer and spellblade.
- Active and passive abilities unlocked by completing sets.
- Visible buffs for active sets, passives, temporary states and charges.
- Movable active ability panel with icon, name, keybind and cooldown.
- Movable static passive panel with plaques and 2 charge orbs for Moonvein, Frostbrand, Hraesvelgr and Ragnar Blood Surge.
- Set bonuses that grow by rarity and piece count.
- Natural EpicLoot drops for set pieces.
- Guaranteed boss-stage set drops when a valid roll happens.
- EpicLoot and NorseDemigods configs embedded in the DLL and synchronized at startup.
- New in-game compendium/text entry: `Epic Loot Rarity Sets`, with class descriptions, abilities, buffs and passives translated.
- Text, tooltips and compendium content in Spanish or English depending on the active game language.
- Global pet commands using `Ctrl` combinations.
- Optional BetterArchery integration for `LeatherQuiver`.
- Bestiary integration for `RDB_*` creatures in loot and bounty contracts.
- Optional WolfPack compatibility for controlling Hraesvelgr summons.
- Optional Wires Enemy HUD compatibility: forces `HealthDisplay = Current HP/Max HP` if the mod is installed.

## Dependencies

Required:

- `RandyKnapp-EpicLoot`
- `Alpus-NorseDemigods`
- `Radamanto-Bestiary`

Optional:

- `BetterArchery`: integrates `LeatherQuiver` as an enchantable archer set piece from Epic onward.
- `WolfPack`: the mod can adjust its config so Hraesvelgr summoned beasts are trainable/controllable.
- `WiresEnemyHUD`: the mod can adjust its config to show current and maximum health above enemies.

Although NorseDemigods is listed as a dependency, you do not need to play a NorseDemigods class. This mod suppresses its UI, energy, class selection and input while using some of its effects as visual/mechanical bridges.

## How It Works

The mod creates set archetypes. When you equip every non-weapon piece from the same line, a buff with the base set name is activated. The only piece that may be missing while still activating the class is the set weapon; any other missing piece prevents activation.

That buff acts as the class flag. If the class buff is active, you can use its abilities even if the set weapon is the missing piece. When an ability requires a weapon, it checks the real equipped weapon type, not whether it is exactly the set weapon.

Active class lines:

| Class | Set IDs that activate the class |
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

Full-set buffs show the class name and class skill level (`Nivel`/`Level`). Active ability cooldowns are not shown through the old buff bar; they appear in the ability panel with name, keybind and counter. Passives and temporary states use the buff area, while static charge passives for Moonvein, Frostbrand, Hraesvelgr and Ragnar Blood Surge also appear in the orb panel.

## Shared Systems

- `Ability Panel`: appears when a class buff is active. It shows active abilities only, with name, keybind and cooldown. It can be dragged while the inventory is open with `Tab`. If active pets exist, it adds a `Pets` section with `Attack` (`Ctrl + Mouse4`), `Follow` (`Ctrl + Mouse3`) and `Free` (`Ctrl + secondary attack`); the current state is highlighted.
- `Passive Panel`: shows 2-orb plaques for static charge passives (`Charged Shots`, `Fire Ball`, `Headshot`, `Blood Surge`). When both orbs are filled, the whole plaque lights up with a pulsing border and `READY` label to show that the next shot/attack is charged. It can be dragged with `Tab`.
- Pressing `Home` while the inventory is open recenters the ability and passive/orb panels so displaced panels can be recovered without moving them automatically when tabbing.
- `Ctrl + Mouse4`: orders active pets to attack the aimed enemy at practical long range as long as the target is visible under the crosshair. If the enemy dies or becomes invalid, pets return to following the player.
- `Ctrl + Mouse3`: forces active pets to follow the player in passive mode; they do not attack until `Attack` or `Free` is used again.
- `Ctrl + secondary attack`: releases pets back to normal behavior without forced following.
- Pet commands apply to persistent or controlled summons from `Hraesvelgr`, `Moonvein`, `Helveig` and `Seidr`, including the `Stone Golem`.
- Class pets use normalized mod stats: health, damage and damage taken reduction scale with the owner's class skill so they keep up with progression without relying on raw prefab values.
- Persistent pets disappear when the class buff is lost. If they die while active, they use a `30s` respawn cooldown when their class allows it.
- Friendly explosions from `T.N.T.`, `Rapid Fire` and equivalent effects avoid allied targets, friendly players and pets.
- Class damage and control abilities can interact with player targets only when PvP is active.
- Physical archer traps ignore players who do not have PvP active.
- Ability, summon and temporary-state buffs use the mod's generated icons when a specific or equivalent ability icon exists.
- The compendium, ability names, tooltips and buffs use Spanish or English according to Valheim's active language.

## Natural Drop Chances

Normal EpicLoot magic rolls can be converted into set pieces:

- Magic: `12%`
- Rare: `10%`
- Epic: `8%`
- Ancient: `5%`

Legendary and Mythic are handled through generated EpicLoot sections and their own pools.

`bosssetdrops.json` fixes each boss stage to a set rarity and converts valid equipment rolls into random set pieces from that stage. It does not inherit a random rarity from the base roll: each boss uses its assigned rarity, checks that the base prefab belongs to the allowed pool (`ForceSetDropItems`) and then randomly selects from the real set item IDs of that same rarity (`ForceSetItemIds`).

| Boss | Set rarity |
| --- | --- |
| Eikthyr | `Magic` |
| The Elder | `Rare` |
| Bonemass | `Epic` |
| Moder | `Legendary` |
| Yagluth | `Mythic` |
| The Queen | `Mythic` |
| Fader | `Ancient` |
| FrozenKing | `Ancient` |

## Managed Configs

With `Generate Managed Config Files = true`, the DLL writes/synchronizes:

- `config/EpicLoot/baseconfig/abilities.json`
- `config/EpicLoot/raritysets.json`
- `config/EpicLoot/baseconfig/legendaries.json`
- `config/EpicLoot/baseconfig/loottables.json`
- `config/EpicLoot/baseconfig/iteminfo.json`
- `config/EpicLoot/baseconfig/magiceffects.json`
- `config/EpicLoot/baseconfig/adventuredata.json`
- `config/EpicLoot/bosssetdrops.json`
- `config/NorseDemigods.cfg`

If a file already exists and differs, the mod first creates a `.bak-fran-managed-...` backup and then writes the embedded version. This means manual JSON edits are not persistent unless the embedded resources in the main DLL are updated too.

For servers, it is usually enough to upload the DLLs included in the package and restart:

- `EpicLootRaritySets.dll`: main DLL with sets, embedded configs, compendium, classes, panels, icons and base controllers.
- `EpicLootRaritySetsHotfix.dll`: included layer for abilities and adjustments that may remain separated from the main DLL when the package includes it.

## Custom EpicLoot Bonuses

- `Last Hope`: custom magic effect with its own ability. When health enters a critical state, it prevents the incoming hit if one exists, heals `100%` of maximum health and starts a `60s` cooldown.
- Custom class effects such as `AddFrostbrandSkill`, `AddHellsyngSkill`, `AddSeidrSkill`, etc. increase the effective class skill level and can push scaling beyond level `100`.
- Effects used in lower-rarity sets also have `ValuesPerRarity` configured to avoid empty bonuses, such as `FrostDamageAOE` and `Bulwark`.
- In Ragnar/Frostbrand sets, each `FrostDamageAOE` bonus is paired in the same counter with `AddFrostDamage`, because EpicLoot only triggers the frost area if the weapon hit already contains frost damage.

## In-Game Compendium

The mod adds an entry to the inventory text/compendium panel:

`Epic Loot Rarity Sets`

It includes:

- Explanation of the rarity system.
- Description of every set class.
- Abilities, keybinds, costs, cooldowns, ranges, scaling, durations and damage.
- Values read from the current config when the panel is opened.
- Active-effect sections with buffs, charges, cooldowns, passives and temporary states.
- Ability, buff and passive names localized to the active language, just like in-game text.
- The native EpicLoot `Legendary Sets` page is extended so it also compares `Magic`, `Rare`, `Epic` and `Ancient` alongside the special rarities.

## Set Classes

The following values are the default values.

### Heimdall - Shield Tank

Defensive shield tank focused on blocking and threat control.

Main equipment and bonuses:

- Shield, one-handed weapon and defensive armor.
- Block power, lower block stamina cost, block force and health.
- At higher rarities it gains effects such as `Bulwark`, `ReflectDamage`, `Undying` and `Immovable`.

Full-set abilities:

- Passive `Heimdall Guard`: taking damage grants `5%` all-damage reduction and `+10%` block power per stack for `6s`; maximum `3` stacks.
- `Mouse3` - `Lightning Storm`: cooldown `30s`; summons a fixed storm for `10s`, radius `8m`, pulse every `1s`; lightning damage `24 + 0.60` per Heimdall level; each hit redirects threat toward Heimdall and slows enemies by `30%` for `6s`.
- `Block + Mouse4` - `Water Sphere`: cost `30` stamina, cooldown `20s`, duration `8s`; uses Njord's sphere, pulls enemies within `30m` and keeps them pinned to the sphere until it ends. Each impact deals blunt damage in the NorseDemigods damage radius and scales with Heimdall from `NorseDemigods.cfg`.
- `Mouse4` - `Stone Shield`: cost `35` stamina, cooldown `24s`, duration `8s`; reduces flat damage by `20 + 0.55` per Heimdall level; reflects `25% + 0.3%` per Heimdall level of mitigated damage, maximum `75%`.
- `Block + Mouse3` - `Abyssal Harpoon`: cooldown `12s`; fires a visible aura rope at the aimed enemy and pulls it for `5s`.

### Ragnar - Axe Berserker

Ragnar berserker focused on staying alive through melee pressure.

Main equipment and bonuses:

- Axes as primary weapons.
- `LifeSteal` and reduced life attack cost.
- At Ancient, the final bonus becomes `Undying`.

Full-set abilities:

- Passive `Ragnar Fury`: each melee hit against an enemy grants a visible `6s` stacking buff; each stack gives `+3%` attack speed and `1%` life steal; maximum `10` stacks.
- Passive `Blood Surge`: shown in the passive panel with 2 orbs; at `2/2`, the plaque lights up and the next melee hit against enemies heals `25%` of maximum health.
- `Mouse3` - `Decay Aura`: toggle; consumes `8` stamina/s; radius `6m`; pulse every `1s`; direct damage `8 + 0.35` per Ragnar level. Does not apply poison.
- `Mouse4` - `Blood Frenzy`: consumes `50%` of maximum stamina; for `10s` grants `+50%` attack speed, `+30%` movement speed and `Immovable`; cooldown `30s`.
- Jump + `Mouse4` - `Ragnar Crush`: cost `45` stamina, cooldown `14s`, radius `5m`; replicates Surt's jump attack and deals fire plus blunt damage, each `52 + 1.10` per Ragnar level.

### Hraesvelgr - Physical Archer

Pure archer focused on stealth, beasts, traps, headshots and rapid volleys.

Main equipment and bonuses:

- Bow, QuickDraw, bow draw cost, projectile speed and fire rate.
- `AddHraesvelgrSkill`, `HeadHunter`, `TripleBowShot` and physical damage at high rarities.
- `LeatherQuiver` becomes an extra piece from Epic onward if BetterArchery is installed.

Full-set abilities:

- Passive `Stealth`: while crouched/in stealth, you are invisible to monsters; noise `x0.5`, detection `x0.3`, speed `+15%`.
- Passive `Headshot`: every `3` Hraesvelgr bow shots, the arrow counts as a weak-point/headshot and applies at least `x1.5` damage. The passive panel shows `2/2` and lights up the plaque when the next arrow is charged.
- `Mouse3` - `Summon Beasts`: no-cooldown toggle; summons or stores the current beast with health, damage and damage taken reduction scaled by Hraesvelgr; default creature is bjorn/bear; costs `35` stamina when summoning. If it dies while active, it enters a `60s` dead-pet cooldown. Shows a visible buff while alive.
- If the summoned beast has a saddle equipped when stored, that saddle state is saved and restored the next time it is summoned.
- `Block + Mouse4` - `Tame Beast`: tames a valid beast within `10m`, removes the current pet and makes `Summon Beasts` use that creature with Hraesvelgr scaling. Valid creatures: wolf, bjorn, asksvin, lox, moose, volture, rdb_bee, rdb_crocodile, rdb_fox, seeker, seekerbrute, hatchling, bat, ulv, rdb_lion, deathsquito and rdb_smadrek.
- `Block + Mouse3` - armed trap: cost `20` stamina; maximum `5` charges, recovers `1` every `60s`; when it catches an enemy it deals pierce damage `35 + 0.55` per Hraesvelgr level, roots the target and grants `30s` for the next arrow to deal `+50%` damage.
- `Mouse4` - `Rapid Volley`: channels up to `3s`, cooldown `18s`; while holding attack and standing still, fires up to exactly `8` arrows at `8` arrows/s; each arrow deals `x0.5` normal damage, speed `90`. If weapon/ammo provides no damage, it uses fallback pierce `12 + 0.25` per Hraesvelgr level. Reaching 8 arrows cancels the channel and restores all stamina.
- Secondary attack - `Freyja Dash`: cost `20` stamina, cooldown `10s`; enemies crossed by the dash take pierce damage `55 + 1.1` per Hraesvelgr level and are staggered. Players are affected only when PvP is active.

### Hellsyng - Crossbow Hunter

Witch-hunter crossbow class focused on pets, shapeshifting, explosive marks and crossbow bursts.

Main equipment and bonuses:

- Crossbow, `AddHellsyngSkill`, `TripleBowShot`, `QuickDraw`, fire rate and projectile speed.
- `ExplosiveArrows` is not permanent on the Hellsyng set; it is temporarily enabled by `Rapid Fire`.
- Starts at Rare; there is no Magic version.

Full-set abilities:

- Passive `T.N.T.`: Hellsyng crossbow shots mark the target. If a marked enemy dies, it detonates with a fire explosion like an explosive shot; it does not damage allies, friendly players or pets. Fire damage `45 + 0.65` per Hellsyng level.
- Passive `Silver Bullets`: crossbow shots deal `+20%` damage against undead and similar creatures: skeletons, draugr, ghosts, wraiths, dead vil/Bjorn, specters and Bestiary undead variants.
- Secondary attack without shapeshift - `Hellsyng Pets`: summons or stores an allied wolf and bat. The bat uses the wolf's reinforced health so it does not fall too quickly. Cooldown `30s` when summoning or storing. If they die while active, they respawn after `30s`. Shows a visible buff while any pet remains alive.
- `Block + Mouse4` - `Werewolf Form`: lasts `30s` or until canceled; cooldown `60s`, starting when it ends or is canceled. `Hunt` increases wolf speed by `40%` if enemies are within `50m`. Normal attack: bite with `4s` cooldown, no slash, applies bleed for `10s` with damage `5 + 0.18` per Hellsyng level per second. Secondary attack: claw slash with guaranteed knockback, `4s` cooldown and slash damage `45 + 0.55` per Hellsyng level.
- `Block + Mouse3` - `Bat Form`: lasts `30s` or until canceled; cooldown `60s`, starting when it ends or is canceled. Allows flying, disables shift sprint and uses a configurable visual offset to raise the bat in camera. Normal attack: bat drain with damage `24 + 0.35` per Hellsyng level that heals `100%` of damage dealt. Secondary attack: `Bat Regeneration`, `+100%` health regeneration for `6s`, cooldown `20s`.
- `Mouse4` - `Bat Horde`: summons an allied bat horde on the target for `30s`. Scales with Hellsyng: `3/6/9/10` bats, stars `0/1/2` and damage multiplier `1 + skill * 0.01`. Cooldown `60s`.
- `Mouse3` - `Rapid Fire`: for `10s`, crossbow reload becomes `0.5s`; shots explode using `50%` of the real impact damage and bounce up to `2` times between nearby enemies. If the impact has no damage, it uses Hellsyng-scaled fallback damage: fire `18 + 0.25` and spirit `12 + 0.20` per level. Cooldown `30s`.

### Nott - Duelist/Assassin

Nott duelist focused on poison, mobility and stealth.

Main equipment and bonuses:

- Knives, reduced noise, `Duelist`, stagger damage/duration, `Opportunist`, movement and dodge.

Full-set abilities:

- Passive `Stealth`: while crouched/in stealth, you are invisible to monsters; noise `x0.4`, detection `x0.2`, speed `+20%`.
- Passive `Shadow Momentum`: hitting enemies grants `+30%` speed and `+15%` damage for `5s`; does not stack, refreshes instead.
- Passive `Poison Edge`: every hit against enemies adds poison `8 + 0.35` per Nott level.
- Passive `Executor`: all Nott damage against enemies already at `30%` health or lower deals `300%` total damage.
- `Mouse3` - `Warp`: cost `25` stamina, cooldown `18s`, range `18m`; teleports behind the aimed enemy, to the nearest enemy if none is aimed, or forward if no valid target exists. After Warp, `Warp Guard` reduces incoming damage by `90%` for `3s`.
- `Block + Mouse4` - `Shadow Mark`: marks the selected enemy within `5m` for `6s` with a shadow visual above the target; stores `50%` of the damage it receives while active and then explodes as spirit damage on the target. Cooldown `60s`; if an enemy dies within `20m` while it is on cooldown, the cooldown is reduced by `10s`.
- `Mouse4` - `Knife Strike`: requires a knife, cooldown `8s`, range `5m`; hits the aimed enemy or nearest frontal enemy with `50%` weapon damage plus `0.75` slash damage per Nott level, uses a Norse slash visual and slows by `50%` for `5s`.
- After `Warp`, `Warp Strike` makes the next attack against an enemy deal `x2.5` damage, always stagger and slow by `50%` for `6s`.
- If an enemy dies within `18m` while Warp is on cooldown, the cooldown resets and you regain stamina.

### Seidr - Elemental Mage

Eitr mage focused on area control and elemental summoning.

Main equipment and bonuses:

- Staves, maximum eitr, eitr regeneration, reduced eitr cost, magic rate.
- `DoubleMagicShot`, `AddSeidrSkill` and `ModifyElementalDamage` at high rarities.
- Starts at Rare; there is no Magic version.

Full-set abilities:

- `Mouse3` - `Nanocube`: cost `50` eitr, cooldown `30s`, duration `10s`; pushes enemies out of `5m`; inside the cube your magic damage is multiplied by `x1.30`.
- `Mouse4` - `Elemental Shield`: toggle; initial cost `25` eitr; no cooldown; you are immune to damage while you have eitr; consumes `2%` maximum eitr per second, minimum `1` eitr/s.
- Secondary attack - `Stone Golem`: cost `80` eitr, cooldown `60s`, duration `60s`; summons a friendly Brokkr golem with health, damage and damage taken reduction scaled by Seidr. Obeys global pet commands.
- `Block + Mouse4` - `Frost Nova`: cost `45` eitr, cooldown `12s`, radius `8m`; frost damage `45 + 0.85` per Seidr level; `40%` slow for `5s`.

### Helveig - Blood Mage

Blood mage focused on healing, spirit/fire damage, undead bodyguards and beast summons.

Main equipment and bonuses:

- `AddHelveigSkill`, reduced eitr cost, maximum health, regeneration, summon damage/health.
- The blood staff reduces life attack cost.

Full-set abilities:

- `Mouse3` - `Holy Healing`: cost `25` eitr, cooldown `10s`; heals `35 + 0.75` per Helveig level to the aimed ally in range (`30m`) or yourself if there is no target.
- Passive `Undead Bodyguard`: activating the set summons `Charred_Melee_Dyrnwyn`; it disappears when the set is lost. `LeftAlt + Mouse3` stores or recalls it with a `30s` cooldown after use. If it dies, it automatically respawns after `30s`. Health, damage and damage taken reduction scale conservatively with Helveig and it obeys pet commands.
- Passive `Blood Aegis`: every heal applies a non-stacking shield to the healed target equal to `15%` of the ability's potential healing for `15s`, even if the target was already at full health, with an elemental shield visual.
- Passive `Sanguine Devotion`: every heal grants a stack for `15s`, up to `3`. Each stack increases summon damage by `+5%` and `Holy Strike` damage by `+10%`.
- `Mouse4` - `Blood Rite`: cost `45` eitr, cooldown `20s`; channels for `10s`, radius `30m`, pulse every `1s`; heals you, players, allied NPCs, active pets and tamed creatures by `12 + 0.30` per Helveig level per pulse; moving more than `0.65m` cancels; cooldown starts when the channel ends or breaks.
- Secondary attack - `Holy Strike`: cost `25` eitr, cooldown `6s`, range `30m`; fire damage `55 + 0.45` and spirit damage `65 + 0.70` per Helveig level. Each impact restores `2` eitr.
- `Block + Mouse3` - `Summon Monster`: cost `55` eitr, cooldown `60s`, duration `20s`; summons `Ent` / `Abomination` / `ElakingMole` / `FallenValkyrie` depending on Helveig level. Health, damage and damage taken reduction scale conservatively, it obeys pet commands and shows an active buff with remaining time.

### Moonvein - Magical Archer

Eitr archer mixing bow gameplay, `SpellSword` and Moonvein scaling.

Main equipment and bonuses:

- Moonbow, `SpellSword`, `EitrLeech`, `AddMoonveinSkill`, `IncreaseEitr` and `AmmoConservation = 100`.
- `LeatherQuiver` becomes an extra piece from Epic onward if BetterArchery is installed.
- Every Moonbow shot consumes eitr before `ModifyAttackEitrUse` reductions: Magic `6`, Rare `8`, Epic `10`, Legendary `12`, Mythic `14`, Ancient `16`.

Full-set abilities:

- Passive `Charged Shots`: every `3` consecutive shots fires a random spell from the bow. The passive panel shows `2/2` and lights up the plaque when the next shot is charged.
- Passive `Arcane Shots`: normal Moonvein bow shots add lightning damage `8 + 0.20` and spirit damage `8 + 0.20` per Moonvein level.
- Passive `Recharge`: works like Frostbrand's; Moonvein attacks and abilities add charges when they hit. Lasts `15s`, each charge increases all damage by `4%` up to `5` charges, and at `5/5` fires guaranteed Chain Lightning.
- Probabilities: Acid Bolt `40%`, Lightning Bolt `40%`, Fireball `20%`.
- Acid Bolt: poison `40 + 0.50` per Moonvein level.
- Lightning Bolt: lightning `48 + 0.65` per Moonvein level.
- Fireball: fire `58 + 0.80` per Moonvein level.
- `Mouse3` - `Meteor`: cost `30` eitr, cooldown `10s`, range `60m`; fire + blunt damage, each type `35 + 1.20` per Moonvein level; force `80`. Fires `1` meteor normally, `2` meteors from class skill `60`, and `3` meteors from class skill `90`. When more than one meteor fires, each meteor lands `1.5s` after the previous one; extra meteors land `1m` left/right of the main impact.
- `Mouse4` - `Tornado Shot`: cost `20` eitr, cooldown `12s`; arms the next arrow; on impact summons Njord's tornado for `6s`; `40%` slow for `6s`; fallback damage in `4m`: lightning `18 + 0.75` per Moonvein level every `0.5s`.
- `Block + Mouse3` - `Spirit Wolf`: cost `20` eitr; summons `wolf_spiritcaller`, obeys the pet panel and scales health, damage and damage taken reduction with Moonvein. If it dies, applies a `60s` cooldown.

### Frostbrand - Spellblade

Two-handed magical swordsman. Uses eitr, fire, lightning, elemental shields and Surt/Thor-style attacks.

Main equipment and bonuses:

- Two-handed sword, `SpellSword`, `EitrLeech`, `ModifyAttackEitrUse`, `AddFrostbrandSkill`, elemental/frost damage.
- The weapon grants `IncreaseEitr = 100` so `SpellSword` can attack even if the character has no base eitr.

Full-set abilities:

- `Mouse3` - `Dash`: cost `5` eitr; uses the NorseDemigods Ability Dash bridge with the Thor/lightning theme. Has `3` charges and recovers `1` charge every `15s`. On use, grants `+50%` movement speed for `3s`.
- Secondary attack - `Slash`: cost `15` eitr, cooldown `8s`; performs the weapon attack animation, applies fire damage `16 + 0.90` and slash damage `16 + 0.90` per Frostbrand level, adds to the `Fire Ball` counter, and heals the caster for `50%` of actual damage dealt.
- `Block + Mouse4` - `Elemental Shield`: cost `35` eitr, cooldown `24s`, duration `8s`; nullifies fire and mitigates all damage by `25% + 0.3%` per Frostbrand level, maximum `70%`.
- `Mouse4` - `Thor Lightning Strike`: replaces `Crush`; cost `25` eitr, cooldown `18s`, range `80m`; fires 3 Thor/NorseDemigods lightning impacts, spaced `0.25s` apart, with fallback radius `3m`; damage per impact `14 + 1.80` per Frostbrand level before resistances; roots hit enemies for `5s`.
- Passive `Fire Ball`: every `3` Frostbrand weapon attacks started or `Slash` uses launches Fireball where you aim. The passive panel shows `2/2` and lights up the plaque when the next attack is charged; fire damage `36 + 0.85` per Frostbrand level; fallback radius `3.5m` if the projectile is unavailable.
- Passive `Recharge`: attacks and abilities only add charges when they hit an enemy. Lasts `15s`; each charge increases all damage by `4%`, maximum `5` charges. At `5/5`, attacks trigger guaranteed Chain Lightning on every hit; it jumps up to `3` nearby enemies within `8m`, using part of the real hit damage. Recharge's own bounces do not refresh its charges.

### Thor - Special Epic Set

Special throwing-axe and storm set.

It does not have its own hotkey controller. Its identity comes from:

- Throwing axe.
- Weapon recall.
- Lightning damage.
- Chain Lightning (`ChainLightning`).

### Floki - Special Epic Set

Special builder set.

It does not have its own hotkey controller. Its identity comes from:

- Building hammer.
- `FreeBuild`.
- Building distance.
- Weight/carry load.
- Stamina and durable tools.

## BetterArchery LeatherQuiver

`LeatherQuiver` is treated as a `Utility` piece for EpicLoot and enters loot from Epic onward.

Sets that use it:

- `EpicHraesvelgr`
- `Hraesvelgr`
- `MythicHraesvelgr`
- `AncientHraesvelgr`
- `EpicMoonvein`
- `Moonvein`
- `MythicMoonvein`
- `AncientMoonvein`

It does not replace pieces: it is added as an extra piece from Epic onward.

- Epic: `6` pieces and `6` bonuses.
- Legendary: `7` pieces and `7` bonuses.
- Mythic: `8` pieces and `8` bonuses.
- Ancient: `9` pieces and `9` bonuses, while still keeping the final trinket.

The final extra bonus is `HeadHunter` for Hraesvelgr and `ModifyElementalDamage` for Moonvein.

## Included EpicLoot Changes

- Extended bow/crossbow effects for `Bows` and `Crossbows`.
- `ModifyAttackEitrUse` available on Magic with low values.
- `LifeSteal` allowed on Magic for Ragnar balance.
- `IncreaseEitr` allowed on one-handed and two-handed weapons.
- `FreeBuild` allowed on Epic for Floki.
- Exclusivity adjustments between `SpellSword`, `Duelist` and `EitrWeave`.
- Rarity loot pools for set pieces.
- Boss pools to push stage-based progression.
- Bestiary: the 21 `RDB_*` creatures inherit loot tiers and appear as bounty targets.
- Shop/adventure data with generated pieces.

## Main Configuration

File:

`config/fran.mods.epiclootraritysets.cfg`

Important options:

- `Generate Managed Config Files`: writes the managed configs from the DLL.
- `Enable Natural Drops`: allows normal EpicLoot rolls to convert into set pieces.
- `Enable Set Activation Buffs`: shows full-set buffs.
- `Enable Ability Panel`: shows the movable active ability panel.
- `Ability Panel Position X/Y`, `Scale`, `Opacity`: ability panel position, scale and opacity.
- `Enable Passive Stack Panel`: shows the movable passive panel with orbs and charged state across the full plaque.
- `Passive Stack Panel Position X/Y`, `Scale`, `Opacity`: passive panel position, scale and opacity.
- `Center Panels Hotkey`: hotkey used while the inventory is open to center the ability and passive/orb panels. Default: `Home`.
- `Enable Frostbrand Abilities`
- `Enable Hraesvelgr Abilities`
- `Enable Hellsyng Abilities`: enables `T.N.T.`, `Silver Bullets`, `Hellsyng Pets`, `Werewolf Form`, `Bat Form`, `Bat Horde` and `Rapid Fire`.
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

Each ability section lets you change keybinds, costs, cooldowns, radii, durations and scaling values. The README values are the expected defaults. If an older local config already exists, the mod tries to migrate old defaults to current values with `UpgradeFloatConfig` and `UpgradeShortcutConfig`.

If the package includes `EpicLootRaritySetsHotfix.dll`, that companion plugin may use `config/fran.mods.epiclootraritysets.hotfix.cfg` for settings that remain separate from the main DLL, such as bat form options, `Blood Frenzy`, `Shadow Mark` or `Ragnar Crush`.

## Development

Embedded configs live in:

`src/GeneratedConfig/`

When you manually change JSON/CFG files and want those changes to ship inside the mod, update those files and rebuild the DLL. If `Generate Managed Config Files` remains enabled and the DLL embedded resources are outdated, the next startup will write the old version again.

Build:

`tools/Build-EpicLootRaritySets.ps1`

Verification:

`tools/Verify-RecoveredRaritySets.ps1`
