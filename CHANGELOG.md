# Changelog

## [1.0.5] - 2026-09-18

### General
- Bumped `manifest.json` to `1.0.5`.
- Updated README documentation for the current Moonvein and Frostbrand defaults.
- Fixed EpicLoot-enchanted equipment repair checks so valid damaged magic items can be repaired at usable repair stations.
- Reduced repeated buff/UI work for passive effects: temporary passives now keep their icon fixed while their own duration expires, instead of constantly refreshing the same icon/text.
- Reduced runtime cost from pet and Water Sphere updates by throttling repeated area/target scans.

### Frostbrand
- Restored Water Sphere to the stable NorseDemigods flow: the mod now calls the native Norse Water Sphere and only adds the custom gravity/root behavior on top.
- Removed the extra custom Water Sphere visual that could create a second sphere or make the visual appear in the wrong place.
- Water Sphere gravity now runs in controlled ticks instead of scanning every frame.
- Water Sphere now forces the native Norse sphere's destination to the aimed point, while still respecting the configured target range.
- Updated Water Sphere Norse config defaults to `Base Damage = 8` and `Damage Per Level = 1`.
- Reduced Frostbrand eitr costs:
  - Water Sphere: `35 -> 20`.
  - Slash: `30 -> 15`.
  - Elemental Shield: `50 -> 35`.
  - Thor Lightning Strike: `40 -> 25`.
  - Legacy/bridged Frostbrand Dash, Holy Strike, Holy Sun and Crush costs were also migrated down.
- Rebalanced Frostbrand damage:
  - Slash base damage reduced from `24` to `16`.
  - Thor Lightning Strike increased from `8 + 1.40/level` to `14 + 1.80/level`.
- Recharge buff now updates when it gains/renews charges and then expires naturally from its duration.

### Moonvein
- Added `Disparos arcanos`: normal Moonvein bow shots add lightning and spirit damage, both scaling with Moonvein and passing through EpicLoot damage modifiers.
- Added Moonvein `Recarga`, matching Frostbrand's charge style: hits build up to `5` stacks, increase all damage, and trigger guaranteed Chain Lightning at full stacks.
- Added `Lobo espiritual` on `Bloqueo + Mouse3`: summons `wolf_spirit_caller`, scales health/damage with Moonvein, obeys the pet command panel, and applies death cooldown when killed.
- Added Spirit Wolf support to pet command/enemy relationship handling.
- Rebalanced Moonvein charged-shot projectiles:
  - Acid Bolt base damage `20 -> 40`.
  - Lightning Bolt base damage `28 -> 48`.
  - Fireball base damage `38 -> 58`.
- Rebalanced Moonvein active skills:
  - Meteor eitr cost `45 -> 30`.
  - Tornado Shot eitr cost `35 -> 20`.
  - Spirit Wolf eitr cost `35 -> 20`.
  - Meteor base damage `70 -> 35`.
  - Tornado fallback tick damage `1 + 0.39/level -> 18 + 0.75/level`.
- Moonvein passive and Spirit Wolf buffs now keep fixed icons while active instead of being refreshed continuously.

### Nott
- Added permanent passive `Ejecutor`: all Nott damage against enemies already at or below `30%` health deals `300%` total damage.
- Added `Golpe de cuchillo`: requires a knife, has `8s` cooldown, uses secondary attack input, plays a Norse slash visual, deals `50%` weapon damage plus Nott scaling, and slows the target by `50%` for `5s`.
- Added `Guardia de Warp`: after a successful Warp, Nott takes `90%` less incoming damage for `3s`.
- Added generated icons for `Ejecutor`, `Golpe de cuchillo` and `Guardia de Warp`.
- Made `Ejecutor` and `Filo venenoso` permanent non-flashing buffs.
- Improved Executor handling so threshold damage is evaluated only from the target's current health state.
- Shadow Momentum now also grants `+15%` outgoing damage while active.

### Helveig
- Changed summon cooldown flow so the monster summon cooldown starts when the summon ends, dies or is dismissed, instead of immediately on cast.
- Fixed Summon Monster and Undead Bodyguard buff refresh so their icons stay fixed instead of blinking; Undead Bodyguard also no longer shows a spurious stack number.
- Increased Holy Strike base damage by `+30` fire and `+30` spirit, and made each impact restore `2` eitr to the caster.

### Ragnar
- Fixed the Magic Ragnar set bonus effect id from `Lifesteal` to EpicLoot's valid `LifeSteal`, so the set can load/display its 3-piece bonus correctly.

### Balance And UX
- Updated the in-game compendium/readme data for Moonvein/Frostbrand values and newly added passives/skills.
- Cooldown and passive buff presentation now favors stable icons with duration removal rather than repeated icon refresh.
- Rebuilt the repo DLL and copied the matching DLL to the active Thunderstore profile for testing.
