# Changelog

## [1.0.8] - 2026-09-22

### Compatibility
- Disabled BetterArchery's quiver inventory-slot handling from this DLL, so `LeatherQuiver` no longer reserves or creates the three extra quiver slots even when equipped.
- Neutralized BetterArchery's `FindEmptySlot`/`HaveEmptySlot` quiver overrides so InventorySlots expanded cells are counted correctly during normal loot pickup instead of reporting inventory full while expanded cells are free.

### UI
- Ability and passive/orb panels now refresh their localized text when the game language changes.
- Rarity set names shown on item names and set tooltips now switch to Spanish when the game language is Spanish.

### Combat
- Hraesvelgr Freyja Dash now uses this mod's PvP-safe trail hit instead of NorseDemigods internal path damage: crossed enemies take notable pierce damage scaling with Hraesvelgr skill and are staggered; players are affected only when PvP is active.
- Nott Warp Strike now always staggers the empowered-hit target and slows it by `50%` for `6s`, while keeping the empowered hit damage multiplier.
- Updated Active Effects and Epic Loot Rarity Sets compendium text in Spanish and English for Hraesvelgr Dash and Nott Warp Strike.
- EpicLoot rarity set tooltips and active set effects now use the exact bonus values from the loaded set JSON, so `FrostDamageAOE` on `MagicFrostbrand` displays and applies `10%` instead of falling back to another rarity/default value.

## [1.0.7] - 2026-09-22

### General
- Bumped `manifest.json` to `1.0.7`.
- Added a safety guard around `ZNetScene.RemoveObjects`/destroyed `ZNetView` cleanup to reduce `NullReferenceException` crashes when leaving combat scenes quickly, especially after temporary Frostbrand lightning/passive objects.
- Fixed EpicLoot magic set repairs so damaged magic rarity set pieces can appear as repairable and be repaired at valid stations.
- Relaxed EpicLoot magic-effect item-type restrictions for all guaranteed rarity-set effects, so configured set-piece bonuses are no longer rejected and replaced by fallback rolls such as `Luck`.
- Improved class active-effect localization refresh: active class buff names/tooltips now refresh after language changes instead of staying in the previous language.
- Class-name buffs now show the current class skill level as their icon number and refresh as the skill levels up.
- Updated the in-game compendium text for the latest class ability/balance changes in Spanish and English.
- Updated class ability targeting so damage/control skills can interact with players only when PvP is active.
- Fixed Hraesvelgr physical archer traps so they ignore players without PvP active.

### UI
- Reinforced passive stack/orb panels for classes with three-hit/passive charge mechanics so they survive equip/unequip transitions more reliably.
- Added panel recentering when tabbing, making displaced passive/orb panels easier to recover.
- Frostbrand Dash charge buff no longer flashes constantly; its icon text now shows seconds until the next charge only while recharging.
- Frostbrand Dash charges and active state are reflected in the ability panel icon/buff state.
- Added dedicated Frostbrand Dash and Dash speed-buff icons, separate from Hraesvelgr/Freyja Dash.

### Ragnar
- Added Blood Frenzy and Ragnar Crush details to the compendium active-effect text and EpicLoot rarity set descriptions.
- Blood Surge now heals `25%` of maximum health instead of `5%`.
- Ragnar Fury now grants `1%` lifesteal per stack instead of `0.5%`.
- Blood Frenzy no longer costs health; it now consumes `50%` of maximum stamina.
- Updated the compendium to reflect Blood Frenzy, Blood Surge, Fury and Crush changes.

### Frostbrand
- Moved Water Sphere out of Frostbrand and into Heimdall.
- Added Frostbrand Dash using the NorseDemigods Ability Dash bridge with the Thor/lightning theme instead of Freyja/Nature, while keeping the player-facing name as `Dash`.
- Frostbrand Dash now has `3` charges and recovers one charge every `15s`.
- Frostbrand Dash now grants `+50%` movement speed for `3s`.
- Slash now heals the caster for `50%` of actual damage dealt.
- Thor Lightning Strike damage/root handling was reinforced: the root is applied around the impact, movement is actively locked while rooted, and the effect lasts `5s`.
- Updated Frostbrand compendium text for Dash charges, Dash speed buff, Slash healing, Lightning Strike damage and root behavior.

### Heimdall
- Added Water Sphere to Heimdall.
- Water Sphere pull radius increased to `30m`.
- Water Sphere now keeps affected enemies pinned to the sphere until the effect ends.
- Lightning Storm now slows hit enemies by `30%` for `6s`.
- Updated Heimdall compendium text for Water Sphere and Lightning Storm slow behavior.

### Hraesvelgr
- Hraesvelgr summoned pets now remember whether they had a saddle equipped when stored, and restore that saddle state when summoned again.
- The saddle state is persisted in config with the current tamed beast prefab, so it survives reinvocation and reloads.
- Updated Hraesvelgr compendium text to mention saddle persistence.

### Moonvein
- Meteor now scales projectile count with Moonvein class skill: `1` meteor normally, `2` meteors from skill `60`, and `3` meteors from skill `90`.
- Multi-meteor casts now land each meteor `1.5s` after the previous one.
- Extra meteors land `1m` left/right of the main Meteor impact to avoid overlap.
- Spirit Wolf now shows remaining duration seconds on both the active buff icon and the ability panel while summoned.
- Arcane Shots now uses the same Moonvein charged-shots icon in the buff bar as it uses in the ability/orb panel.
- Updated Moonvein compendium text to describe Meteor count thresholds.

## [1.0.6] - 2026-09-18

### General
- Bumped `manifest.json` to `1.0.6`.
- Restored Seidr Nanocube, Elemental Shield and Frost Nova visuals to the stable `0.1.26` NorseDemigods lookup flow, then kept the current 1.0.6 costs and scaling.
- Reduced lag when swapping sets by avoiding repeated class-buff rebuilds and repeated inactive-controller cleanup every frame.
- Improved global pet commands: `Attack` now targets the aimed enemy at long visible range instead of being limited by the old short command range, and `Follow` now acts as passive follow until `Attack` or `Free` is selected again.
- Added normalized class-pet scaling for Hraesvelgr, Moonvein, Helveig and Seidr summons: health, outgoing damage and damage taken reduction now scale from each pet owner's class skill with conservative adventure-friendly values instead of relying on raw prefab stats.
- Added Hraesvelgr `Tame Beast` on `Block + Mouse4`: tames a valid beast within `10m`, removes the current Hraesvelgr pet, persists the chosen prefab and makes `Summon Beasts` spawn that creature with Hraesvelgr pet scaling.

### Helveig
- Added `Blood Aegis`: healing applies a non-stacking shield to the healed target equal to `15%` of the ability's potential healing for `15s`, even at full health.
- Blood Aegis now applies the Elemental Shield visual to the healed target and sends the buff/shield state to healed player clients.
- Added `Sanguine Devotion`: healing stacks up to `3` for `15s`; each stack grants `+5%` summon damage and `+10%` Holy Strike damage.
- Reduced Summon Monster duration from `60s` to `20s`.
- Added generated buff icons for `Blood Aegis` and `Sanguine Devotion`.

### Nott
- Moved `Knife Strike` to `Mouse4`.
- Moved `Shadow Mark` to `Block + Mouse4`, leaving plain `Mouse4` for `Knife Strike`.

### Seidr
- Fixed visual bugs.

### Moonvein
- New ability
- Rework damage

### Hraesvelgr
- New ability tame pets

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
- Added `Arcane Shots`: normal Moonvein bow shots add lightning and spirit damage, both scaling with Moonvein and passing through EpicLoot damage modifiers.
- Added Moonvein `Recharge`, matching Frostbrand's charge style: hits build up to `5` stacks, increase all damage, and trigger guaranteed Chain Lightning at full stacks.
- Added `Spirit Wolf` on `Block + Mouse3`: summons `wolf_spiritcaller`, scales health/damage with Moonvein, obeys the pet command panel, and applies death cooldown when killed.
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
- Added permanent passive `Executor`: all Nott damage against enemies already at or below `30%` health deals `300%` total damage.
- Added `Knife Strike`: requires a knife, has `8s` cooldown, uses secondary attack input, plays a Norse slash visual, deals `50%` weapon damage plus Nott scaling, and slows the target by `50%` for `5s`.
- Added `Warp Guard`: after a successful Warp, Nott takes `90%` less incoming damage for `3s`.
- Added generated icons for `Executor`, `Knife Strike` and `Warp Guard`.
- Made `Executor` and `Poison Edge` permanent non-flashing buffs.
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
