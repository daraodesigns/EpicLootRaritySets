param(
    [string] $BepInExRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..\..")).Path
)

$ErrorActionPreference = "Stop"

$legendaryPath = Join-Path $BepInExRoot "config\EpicLoot\baseconfig\legendaries.json"
$raritySetsPath = Join-Path $BepInExRoot "config\EpicLoot\raritysets.json"
$magicEffectsPath = Join-Path $BepInExRoot "config\EpicLoot\baseconfig\magiceffects.json"

$magicEffectsConfig = Get-Content $magicEffectsPath -Raw | ConvertFrom-Json
$effectByType = @{}
foreach ($effect in $magicEffectsConfig.MagicItemEffects) {
    $effectByType[$effect.Type] = $effect
}

function Add-UniqueRequirementValues($effect, [string] $property, [string[]] $values) {
    if ($null -eq $effect.Requirements) {
        $effect | Add-Member -NotePropertyName Requirements -NotePropertyValue ([pscustomobject] @{})
    }

    $existingProperty = $effect.Requirements.PSObject.Properties[$property]
    if ($null -eq $existingProperty) {
        $effect.Requirements | Add-Member -NotePropertyName $property -NotePropertyValue @()
    }

    $current = @($effect.Requirements.$property)
    foreach ($value in $values) {
        if ($current -notcontains $value) {
            $current += $value
        }
    }

    $effect.Requirements.$property = @($current)
}

function Remove-RequirementValues($effect, [string] $property, [string[]] $values) {
    if ($null -eq $effect.Requirements) {
        return
    }

    $existingProperty = $effect.Requirements.PSObject.Properties[$property]
    if ($null -eq $existingProperty) {
        return
    }

    $effect.Requirements.$property = @($effect.Requirements.$property | Where-Object { $values -notcontains $_ })
}

function Remove-RequirementProperty($effect, [string] $property) {
    if ($null -eq $effect.Requirements) {
        return
    }

    $existingProperty = $effect.Requirements.PSObject.Properties[$property]
    if ($null -ne $existingProperty) {
        $effect.Requirements.PSObject.Properties.Remove($property)
    }
}

function Set-RarityValues($effect, [string] $rarity, [double] $min, [double] $max, [double] $increment) {
    if ($null -eq $effect.ValuesPerRarity) {
        $effect | Add-Member -NotePropertyName ValuesPerRarity -NotePropertyValue ([pscustomobject] @{})
    }

    $values = [pscustomobject] ([ordered] @{
            MinValue  = $min
            MaxValue  = $max
            Increment = $increment
        })

    if ($effect.ValuesPerRarity.PSObject.Properties[$rarity]) {
        $effect.ValuesPerRarity.$rarity = $values
    }
    else {
        $effect.ValuesPerRarity | Add-Member -NotePropertyName $rarity -NotePropertyValue $values
    }
}

foreach ($effectType in @("ModifyFireRate", "QuickDraw", "ExplosiveArrows", "TripleBowShot", "ModifyProjectileSpeed", "ModifyPhysicalDamage", "ModifyDamage", "ModifyDamageLowHealth", "AddFireDamage", "AddLightningDamage", "Indestructible", "Weightless")) {
    if ($effectByType.ContainsKey($effectType)) {
        Add-UniqueRequirementValues $effectByType[$effectType] "AllowedItemTypes" @("Bows", "Crossbows")
    }
}

if ($effectByType.ContainsKey("ModifyAttackEitrUse")) {
    Set-RarityValues $effectByType["ModifyAttackEitrUse"] "Magic" 1 6 1
}

if ($effectByType.ContainsKey("LifeSteal")) {
    Add-UniqueRequirementValues $effectByType["LifeSteal"] "AllowedRarities" @("Magic")
}

if ($effectByType.ContainsKey("IncreaseEitr")) {
    Add-UniqueRequirementValues $effectByType["IncreaseEitr"] "AllowedItemTypes" @("OneHandedWeapon", "TwoHandedWeapon")
}

if ($effectByType.ContainsKey("FreeBuild")) {
    Add-UniqueRequirementValues $effectByType["FreeBuild"] "AllowedRarities" @("Epic")
}

if ($effectByType.ContainsKey("SpellSword")) {
    Remove-RequirementValues $effectByType["SpellSword"] "ExclusiveEffectTypes" @("Duelist")
}

if ($effectByType.ContainsKey("Duelist")) {
    Add-UniqueRequirementValues $effectByType["Duelist"] "AllowedItemTypes" @("TwoHandedWeapon", "TwoHandedWeaponLeft")
    Remove-RequirementValues $effectByType["Duelist"] "ExclusiveEffectTypes" @("SpellSword", "EitrWeave")
}

if ($effectByType.ContainsKey("EitrWeave")) {
    Remove-RequirementValues $effectByType["EitrWeave"] "ExclusiveEffectTypes" @("Duelist")
}

function Obj($values) {
    if ($values -is [array] -and $values.Count -eq 1) {
        $values = $values[0]
    }

    return [pscustomobject] $values
}

function Set-ObjectProperty($target, [string] $property, $value) {
    if ($target.PSObject.Properties[$property]) {
        $target.$property = $value
    }
    else {
        $target | Add-Member -NotePropertyName $property -NotePropertyValue $value
    }
}

function Ensure-ClassSkillEffect([string] $className) {
    $effectType = "Add$($className)Skill"
    if ($effectByType.ContainsKey($effectType)) {
        return
    }

    $reference = if ($effectByType.ContainsKey("AddBowsSkill")) { $effectByType["AddBowsSkill"].ValuesPerRarity } else { $null }
    $token = "`$mod_epicloot_me_$($effectType.ToLowerInvariant())"
    $effect = Obj ([ordered] @{
            Type = $effectType
            DisplayText = "$($token)_display"
            Description = "$($token)_desc"
            ValuesPerRarity = $reference
            SelectionWeight = 0
            CanBeAugmented = $false
            CanBeDisenchanted = $false
            CanBeRunified = $false
            Prefixes = @("$($token)_prefix1")
            Suffixes = @("$($token)_suffix1")
        })

    $magicEffectsConfig.MagicItemEffects = @(@($magicEffectsConfig.MagicItemEffects) + $effect)
    $effectByType[$effectType] = $effect
}

foreach ($className in @("Heimdall", "Ragnar", "Hraesvelgr", "Hellsyng", "Nott", "Seidr", "Helveig", "Moonvein", "Frostbrand")) {
    Ensure-ClassSkillEffect $className
}

function Ensure-LastHopeEffect() {
    $effectType = "LastHope"
    $token = '$mod_epicloot_me_lasthope'
    if ($effectByType.ContainsKey($effectType)) {
        $effect = $effectByType[$effectType]
        Set-ObjectProperty $effect "DisplayText" "$($token)_display"
        Set-ObjectProperty $effect "Description" "$($token)_desc"
        Set-ObjectProperty $effect "SelectionWeight" 2
        Set-ObjectProperty $effect "Prefixes" @("$($token)_prefix1")
        Set-ObjectProperty $effect "Suffixes" @("$($token)_suffix1")
        Set-ObjectProperty $effect "Ability" "LastHope"
        Add-UniqueRequirementValues $effect "AllowedItemTypes" @("Utility", "Trinket")
        Add-UniqueRequirementValues $effect "AllowedRarities" @("Magic", "Rare", "Epic", "Legendary", "Mythic", "Ancient")
        Add-UniqueRequirementValues $effect "ExclusiveEffectTypes" @("Undying")
        return
    }

    $effect = Obj ([ordered] @{
            Type = $effectType
            DisplayText = "$($token)_display"
            Description = "$($token)_desc"
            Requirements = Obj ([ordered] @{
                    AllowedItemTypes = @("Utility", "Trinket")
                    AllowedRarities = @("Magic", "Rare", "Epic", "Legendary", "Mythic", "Ancient")
                    ExclusiveEffectTypes = @("Undying")
                })
            SelectionWeight = 2
            Prefixes = @("$($token)_prefix1")
            Suffixes = @("$($token)_suffix1")
            Ability = "LastHope"
        })

    $magicEffectsConfig.MagicItemEffects = @(@($magicEffectsConfig.MagicItemEffects) + $effect)
    $effectByType[$effectType] = $effect
}

Ensure-LastHopeEffect

if ($effectByType.ContainsKey("FrostDamageAOE")) {
    $frostAoeEffect = $effectByType["FrostDamageAOE"]
    if ($frostAoeEffect.PSObject.Properties["Ability"]) {
        $frostAoeEffect.PSObject.Properties.Remove("Ability")
    }
}

function ValueFor([string] $effectType, [string] $rarity) {
    if (!$effectByType.ContainsKey($effectType)) {
        return $null
    }

    $effect = $effectByType[$effectType]
    if ($null -eq $effect.ValuesPerRarity) {
        return $null
    }

    $rarityRange = $effect.ValuesPerRarity.PSObject.Properties[$rarity]
    if ($null -eq $rarityRange) {
        return $null
    }

    $range = $rarityRange.Value
    $min = [double] $range.MinValue
    $max = [double] $range.MaxValue
    $increment = if ($null -ne $range.Increment) { [double] $range.Increment } else { 1.0 }
    $positionByRarity = @{
        Magic = 0.55
        Rare = 0.60
        Epic = 0.66
        Legendary = 0.72
        Mythic = 0.78
        Ancient = 0.88
    }

    $raw = $min + (($max - $min) * [double] $positionByRarity[$rarity])
    $steps = [math]::Round(($raw - $min) / $increment)
    $value = $min + ($steps * $increment)
    if ($value -lt $min) { $value = $min }
    if ($value -gt $max) { $value = $max }

    return Obj ([ordered] @{
        MinValue = $value
        MaxValue = $value
        Increment = $range.Increment
    })
}

function ItemEffect([string] $type) {
    return Obj ([ordered] @{ Type = $type })
}

function Bonus([int] $count, [string] $type, [string] $rarity) {
    $effect = [ordered] @{ Type = $type }
    $value = ValueFor $type $rarity
    if ($null -ne $value) {
        $effect.Values = $value
    }

    return Obj ([ordered] @{
        Count = $count
        Effect = Obj $effect
    })
}

function Requirement([string] $itemType, [string[]] $skillTypes, [string[]] $itemNames = @()) {
    $requirements = [ordered] @{}
    if (![string]::IsNullOrWhiteSpace($itemType)) {
        $requirements.AllowedItemTypes = @($itemType)
    }

    if ($skillTypes -and $skillTypes.Count -gt 0) {
        $requirements.AllowedSkillTypes = @($skillTypes)
    }

    if ($itemNames -and $itemNames.Count -gt 0) {
        $requirements.AllowedItemNames = @($itemNames)
    }

    return Obj $requirements
}

function Piece([string] $slot, [string] $suffix, [string] $name, [string] $itemType, [string[]] $effects, [string[]] $skillTypes = @(), [string[]] $itemNames = @()) {
    return Obj ([ordered] @{
        Slot = $slot
        Suffix = $suffix
        Name = $name
        ItemType = $itemType
        SkillTypes = @($skillTypes)
        ItemNames = @($itemNames)
        Effects = @($effects)
    })
}

function Family([string] $id, [bool] $usesShield, [string] $theme, $pieces, $bonusTypes, $setNames) {
    if ($setNames -is [array] -and $setNames.Count -eq 1) {
        $setNames = $setNames[0]
    }

    $essentialSlots = @("Weapon")
    if ($usesShield) {
        $essentialSlots = @("Shield", "Weapon")
    }

    return Obj ([ordered] @{
        ID = $id
        UsesShield = $usesShield
        EssentialSlots = @($essentialSlots)
        Theme = $theme
        Pieces = @($pieces)
        BonusTypes = @($bonusTypes)
        SetNames = $setNames
    })
}

$rarities = @()
$rarities += Obj ([ordered] @{ Key = "Magic"; Prefix = "Magic"; PieceBase = 3; EffectCount = 2 })
$rarities += Obj ([ordered] @{ Key = "Rare"; Prefix = "Rare"; PieceBase = 4; EffectCount = 3 })
$rarities += Obj ([ordered] @{ Key = "Epic"; Prefix = "Epic"; PieceBase = 5; EffectCount = 4 })
$rarities += Obj ([ordered] @{ Key = "Legendary"; Prefix = ""; PieceBase = 6; EffectCount = 5 })
$rarities += Obj ([ordered] @{ Key = "Mythic"; Prefix = "Mythic"; PieceBase = 7; EffectCount = 6 })
$rarities += Obj ([ordered] @{ Key = "Ancient"; Prefix = "Ancient"; PieceBase = 8; EffectCount = 7 })

$utilityItemNames = @("BeltStrength", '$item_megingjord')
$demisterItemNames = @("Demister", '$item_demister')

$families = @()
$families += Family "Heimdall" $true "the Heimdall class line for shield tanking, threat control and Heimdall skill scaling" @(
        (Piece "Shield" "TowerShield" "Tower Shield" "Shield" @("ModifyBlockPower", "ModifyBlockStaminaUse", "ModifyBlockForce", "IncreaseStamina", "IncreaseHealth", "AvoidDamageTaken", "ReflectDamage")),
        (Piece "Weapon" "Oathblade" "Oathspear" "OneHandedWeapon" @("ModifyBlockStaminaUse", "ModifyPhysicalDamage", "AddFrostDamage", "OffSetAttack", "AddSpearsSkill", "Indestructible", "Weightless") @("Spears")),
        (Piece "Helmet" "Helmet" "Helmet" "Helmet" @("ModifyArmor", "IncreaseHealth", "ModifyStaminaRegen", "ModifyArmorLowHealth", "ModifyHealthRegenLowHealth", "AddPhysicalResistancePercentage", "Glowing")),
        (Piece "Chest" "Chest" "Chestguard" "Chest" @("ModifyArmor", "IncreaseHealth", "IncreaseStamina", "ModifyHealthRegen", "AvoidDamageTaken", "AddPhysicalResistancePercentage", "ReflectDamage")),
        (Piece "Legs" "Legs" "Greaves" "Legs" @("ModifyArmor", "IncreaseStamina", "ModifyDodgeStaminaUse", "AddMovementSkills", "ModifyMovementSpeedLowHealth", "AddPhysicalResistancePercentage", "ModifyStaminaRegenLowHealth")),
        (Piece "Shoulder" "Cape" "Cape" "Shoulder" @("ModifyArmor", "IncreaseHealth", "IncreaseStamina", "ModifyStaminaRegen", "AddPhysicalResistancePercentage", "Indestructible", "Warmth")),
        (Piece "Utility" "WatchSigil" "Watch Sigil" "Utility" @("ModifyArmor", "IncreaseHealth", "IncreaseStamina", "ModifyStaminaRegen", "AddPhysicalResistancePercentage", "Luck", "Glowing") @() $utilityItemNames),
        (Piece "Demister" "WardingWisp" "Warding Wisp" "Utility" @("ModifyWispRange", "IncreaseHealth", "IncreaseStamina", "ModifyStaminaRegen", "AddPhysicalResistancePercentage", "Luck", "Glowing") @() $demisterItemNames),
        (Piece "Trinket" "DawnSeal" "Dawn Seal" "Trinket" @("ModifyArmor", "IncreaseHealth", "IncreaseStamina", "ModifyStaminaRegen", "AddPhysicalResistancePercentage", "Luck", "Glowing"))
    ) @("ModifyBlockPower", "ModifyBlockStaminaUse", "ModifyBlockForce", "IncreaseHealth", "Bulwark", "ReflectDamage", "Undying", "Immovable") @{
        Magic = "Gate-Spark Ward"
        Rare = "Stonewatch Oath"
        Epic = "Sunwall Aegis"
        Legendary = "Heimdall's Watch"
        Mythic = "Gjallarhorn Guard"
        Ancient = "Bifrost Bastion"
    }
$families += Family "Ragnar" $false "the Ragnar class path for health sustain, frost pressure and Ragnar skill scaling" @(
        (Piece "Weapon" "BattleAxe" "Twin Axes" "TwoHandedWeaponLeft" @("ModifyPhysicalDamage", "Bloodlust", "LifeSteal", "OffSetAttack", "LifeStealLowHealth", "AddRagnarSkill", "AddFrostDamage", "IncreaseHealth") @("Axes")),
        (Piece "Helmet" "Helmet" "War Helm" "Helmet" @("IncreaseHealth", "ModifyStaminaRegen", "ModifyArmor", "HeadHunter", "Luck", "ModifyArmorLowHealth", "Glowing")),
        (Piece "Chest" "Chest" "War Harness" "Chest" @("ModifyArmor", "AddHealthRegen", "ModifyHealthRegen", "IncreaseHealth", "AvoidDamageTakenLowHealth", "AddFrostResistancePercentage", "ReflectDamage")),
        (Piece "Legs" "Legs" "War Strides" "Legs" @("ModifyMovementSpeed", "ModifySprintStaminaUse", "IncreaseStamina", "ModifyDodgeStaminaUse", "ModifyMovementSpeedLowHealth", "AddMovementSkills", "DoubleJump")),
        (Piece "Shoulder" "Cape" "War Cloak" "Shoulder" @("IncreaseHealth", "ModifyStaminaRegen", "IncreaseStamina", "AddFrostResistancePercentage", "RemoveSpeedPenalty", "Warmth", "Indestructible")),
        (Piece "Utility" "FuryTotem" "Fury Totem" "Utility" @("IncreaseHealth", "IncreaseStamina", "ModifyStaminaRegen", "Luck", "AddHealthRegen", "ModifyHealthRegen", "Glowing") @() $utilityItemNames),
        (Piece "Demister" "FrostWisp" "Frost Wisp" "Utility" @("ModifyWispRange", "IncreaseHealth", "IncreaseStamina", "AddFrostResistancePercentage", "ModifyStaminaRegen", "Luck", "Glowing") @() $demisterItemNames),
        (Piece "Trinket" "BloodOath" "Blood Oath" "Trinket" @("IncreaseHealth", "IncreaseStamina", "ModifyStaminaRegen", "Luck", "AddHealthRegen", "ModifyHealthRegen", "AddPhysicalResistancePercentage"))
    ) @("AddRagnarSkill", "ModifyPhysicalDamage", "LifeStealLowHealth", "Berserker", "LifeSteal", "FrostDamageAOE", "Undying") @{
        Magic = "Ember-Axe Reavers"
        Rare = "Storm-Bitten Raiders"
        Epic = "Frostfang Fury"
        Legendary = "Ragnar's Fury"
        Mythic = "Ragnarok Bloodwake"
        Ancient = "World-End Berserkers"
    }
$families += Family "Hraesvelgr" $false "the Hraesvelgr class line for range, draw stamina, projectile control and Hraesvelgr skill scaling" @(
        (Piece "Weapon" "Longbow" "Longbow" "Bow" @("AddHraesvelgrSkill", "QuickDraw", "ModifyDrawStaminaUse", "ModifyProjectileSpeed", "ModifyFireRate", "ModifyPhysicalDamage", "TripleBowShot", "Indestructible")),
        (Piece "Helmet" "Hood" "Hood" "Helmet" @("AddHraesvelgrSkill", "QuickDraw", "ModifyFireRate", "ModifyProjectileSpeed", "HeadHunter", "Luck", "ModifyPhysicalDamage")),
        (Piece "Chest" "Harness" "Harness" "Chest" @("AddHraesvelgrSkill", "QuickDraw", "ModifyDrawStaminaUse", "ModifyFireRate", "ModifyProjectileSpeed", "HeadHunter", "ModifyPhysicalDamage")),
        (Piece "Legs" "Stride" "Stride" "Legs" @("AddHraesvelgrSkill", "QuickDraw", "ModifyDrawStaminaUse", "ModifyFireRate", "ModifyProjectileSpeed", "ModifyMovementSpeed", "DoubleJump")),
        (Piece "Shoulder" "Mantle" "Mantle" "Shoulder" @("AddHraesvelgrSkill", "QuickDraw", "ModifyDrawStaminaUse", "ModifyProjectileSpeed", "ModifyFireRate", "RemoveSpeedPenalty", "Indestructible")),
        (Piece "Utility" "WindGauge" "Wind Gauge" "Utility" @("AddHraesvelgrSkill", "QuickDraw", "ModifyFireRate", "ModifyProjectileSpeed", "ModifyDrawStaminaUse", "Luck", "HeadHunter") @() $utilityItemNames),
        (Piece "Demister" "WispFeather" "Wisp Feather" "Utility" @("AddHraesvelgrSkill", "QuickDraw", "ModifyFireRate", "ModifyProjectileSpeed", "ModifyDrawStaminaUse", "Luck", "HeadHunter") @() $demisterItemNames),
        (Piece "Trinket" "SkyToken" "Sky Token" "Trinket" @("AddHraesvelgrSkill", "QuickDraw", "ModifyFireRate", "ModifyProjectileSpeed", "ModifyDrawStaminaUse", "Luck", "HeadHunter"))
    ) @("AddHraesvelgrSkill", "QuickDraw", "ModifyDrawStaminaUse", "ModifyProjectileSpeed", "ModifyFireRate", "HeadHunter", "TripleBowShot", "ModifyPhysicalDamage") @{
        Magic = "Pinewind Fletchers"
        Rare = "Skyline Hunters"
        Epic = "Gale-Eye Stalkers"
        Legendary = "Hraesvelgr's Hunt"
        Mythic = "Stormfeather Pursuit"
        Ancient = "Eagle-Wind Sovereigns"
    }
$families += Family "Hellsyng" $false "the Hellsyng class line for single heavy shots, explosive bolts, fast reloads and Hellsyng skill scaling" @(
        (Piece "Weapon" "WitchfinderArbalest" "Witchfinder Arbalest" "Bows" @("ModifyDamage", "ExplosiveArrows", "QuickDraw", "TripleBowShot", "AddHellsyngSkill", "ModifyFireRate", "ModifyProjectileSpeed") @("Crossbows")),
        (Piece "Helmet" "Widebrim" "Widebrim" "Helmet" @("HeadHunter", "ModifyDiscoveryRadius", "IncreaseStamina", "ModifyStaminaRegen", "Luck", "ModifyArmor", "Glowing")),
        (Piece "Chest" "Longcoat" "Longcoat" "Chest" @("ModifyArmor", "IncreaseStamina", "AddCarryWeight", "ModifyStaminaRegen", "Weightless", "Luck", "Glowing")),
        (Piece "Legs" "Boots" "Boots" "Legs" @("ModifyMovementSpeed", "ModifySprintStaminaUse", "ModifyDodgeStaminaUse", "IncreaseStamina", "DoubleJump", "FeatherFall", "ModifyJumpStaminaUse")),
        (Piece "Shoulder" "Mantle" "Mantle" "Shoulder" @("IncreaseStamina", "ModifyStaminaRegen", "Weightless", "Indestructible", "AddCarryWeight", "Waterproof", "Glowing")),
        (Piece "Utility" "PowderHorn" "Powder Horn" "Utility" @("AddCarryWeight", "IncreaseStamina", "ModifyStaminaRegen", "Luck", "AddMovementSkills", "Glowing", "ModifyArmor") @() $utilityItemNames),
        (Piece "Demister" "Witchlamp" "Witchlamp" "Utility" @("ModifyWispRange", "IncreaseStamina", "ModifyStaminaRegen", "AddHellsyngSkill", "ModifyProjectileSpeed", "Luck", "Glowing") @() $demisterItemNames),
        (Piece "Trinket" "SilverBolt" "Silver Bolt" "Trinket" @("AddCarryWeight", "IncreaseStamina", "ModifyStaminaRegen", "Luck", "AddMovementSkills", "Glowing", "ModifyArmor"))
    ) @("AddHellsyngSkill", "ExplosiveArrows", "TripleBowShot", "ModifyDamage", "QuickDraw", "ModifyFireRate", "ModifyProjectileSpeed") @{
        Magic = "Powder-Spark Watch"
        Rare = "Iron Bolt Vigil"
        Epic = "Hellsyng"
        Legendary = "Blackpowder Judgment"
        Mythic = "Witchfinder Verdict"
        Ancient = "Last-Rite Arbalests"
    }
$families += Family "Nott" $false "the Nott class path for stealth, stagger pressure, punishing finishers and Nott skill scaling" @(
        (Piece "Weapon" "Nightblade" "Nightblade" "OneHandedWeapon" @("Duelist", "ModifyStaggerDamage", "Opportunist", "ModifyStaggerDuration", "ModifyPhysicalDamage", "OffSetAttack", "AddNottSkill", "Executioner", "LifeStealLowHealth") @("Knives")),
        (Piece "Helmet" "Veil" "Veil" "Helmet" @("ModifyStaminaRegen", "IncreaseStamina", "HeadHunter", "ModifyDiscoveryRadius", "AddNottSkill", "OffSetAttack", "Glowing")),
        (Piece "Chest" "Leathers" "Leathers" "Chest" @("StaggerOnDamageTaken", "AddMovementSkills", "ModifyStaminaRegen", "IncreaseStamina", "AvoidDamageTakenLowHealth", "ModifyArmorLowHealth", "Glowing")),
        (Piece "Legs" "Treads" "Treads" "Legs" @("ModifyNoise", "ModifySprintStaminaUse", "ModifyMovementSpeed", "AddMovementSkills", "ModifyDodgeStaminaUse", "DoubleJump", "FeatherFall")),
        (Piece "Shoulder" "Cloak" "Cloak" "Shoulder" @("OffSetAttack", "AddMovementSkills", "ModifyStaminaRegen", "IncreaseStamina", "RemoveSpeedPenalty", "Waterproof", "Indestructible")),
        (Piece "Utility" "SmokeVial" "Smoke Vial" "Utility" @("ModifyNoise", "AddMovementSkills", "AddNottSkill", "IncreaseStamina", "OffSetAttack", "ModifyStaminaRegen", "Glowing") @() $utilityItemNames),
        (Piece "Demister" "ShadowWisp" "Shadow Wisp" "Utility" @("ModifyWispRange", "ModifyNoise", "AddMovementSkills", "IncreaseStamina", "OffSetAttack", "AddNottSkill", "Glowing") @() $demisterItemNames),
        (Piece "Trinket" "SilentCoin" "Silent Coin" "Trinket" @("ModifyNoise", "AddMovementSkills", "AddNottSkill", "IncreaseStamina", "OffSetAttack", "ModifyStaminaRegen", "Glowing"))
    ) @("ModifyNoise", "Duelist", "ModifyStaggerDamage", "Opportunist", "ModifyStaggerDuration", "OffSetAttack", "ModifyPhysicalDamage") @{
        Magic = "Duskstep Veil"
        Rare = "Blackglass Silence"
        Epic = "Umbral Knives"
        Legendary = "Nott's Silence"
        Mythic = "Night-Court Execution"
        Ancient = "Starless Covenant"
    }
$families += Family "Seidr" $false "the Seidr class line for eitr capacity, recovery, staff casting tempo and Seidr skill scaling" @(
        (Piece "Weapon" "Staff" "Star Staff" "Staff" @("ModifyAttackEitrUse", "ModifyMagicFireRate", "ModifyProjectileSpeed", "ModifyElementalDamage", "AddSeidrSkill", "DoubleMagicShot", "Indestructible") @("ElementalMagic")),
        (Piece "Helmet" "Crown" "Crown" "Helmet" @("IncreaseEitr", "ModifyEitrRegen", "IncreaseStamina", "DartingThoughts", "AddSeidrSkill", "Luck", "Glowing")),
        (Piece "Chest" "Robe" "Robe" "Chest" @("ModifyEitrRegen", "IncreaseEitr", "IncreaseStamina", "ModifyAttackEitrUse", "ModifyStaminaRegen", "Weightless", "Glowing")),
        (Piece "Legs" "Treads" "Treads" "Legs" @("ModifyMovementSpeed", "ModifyEitrRegen", "IncreaseEitr", "ModifySprintStaminaUse", "ModifyDodgeStaminaUse", "DoubleJump", "FeatherFall")),
        (Piece "Shoulder" "Mantle" "Mantle" "Shoulder" @("IncreaseEitr", "ModifyEitrRegen", "IncreaseStamina", "ModifyStaminaRegen", "Weightless", "Waterproof", "Indestructible")),
        (Piece "Utility" "EitrFocus" "Eitr Focus" "Utility" @("IncreaseEitr", "ModifyEitrRegen", "IncreaseStamina", "ModifyStaminaRegen", "Luck", "Glowing", "ModifyArmor") @() $utilityItemNames),
        (Piece "Demister" "MistWisp" "Mist Wisp" "Utility" @("ModifyWispRange", "IncreaseEitr", "ModifyEitrRegen", "ModifyAttackEitrUse", "AddSeidrSkill", "Luck", "Glowing") @() $demisterItemNames),
        (Piece "Trinket" "RuneBead" "Rune Bead" "Trinket" @("IncreaseEitr", "ModifyEitrRegen", "IncreaseStamina", "ModifyStaminaRegen", "Luck", "Glowing", "ModifyArmor"))
    ) @("IncreaseEitr", "ModifyAttackEitrUse", "ModifyEitrRegen", "ModifyMagicFireRate", "DoubleMagicShot", "AddSeidrSkill", "ModifyElementalDamage") @{
        Magic = "Rune-Spark Weave"
        Rare = "Mist-Eitr Regalia"
        Epic = "Starfall Vestments"
        Legendary = "Seidr Starfall"
        Mythic = "World-Root Arcanum"
        Ancient = "Ninefold Seidr"
    }
$families += Family "Helveig" $false "the Helveig class line for summons, healing, eitr reserves and Helveig skill scaling" @(
        (Piece "Weapon" "Bloodstaff" "Blood Staff" "Staff" @("ModifyAttackEitrUse", "AddHelveigSkill", "ModifyAttackHealthUse", "ModifySummonHealth", "ModifySummonDamage", "IncreaseEitr", "ModifyEitrRegen") @("BloodMagic")),
        (Piece "Helmet" "BoneCrown" "Bone Crown" "Helmet" @("IncreaseHealth", "ModifyHealthRegen", "ModifyEitrRegenLowHealth", "IncreaseEitr", "DartingThoughts", "Luck", "Glowing")),
        (Piece "Chest" "MarrowRobe" "Marrow Robe" "Chest" @("IncreaseHealth", "ModifyAttackEitrUse", "AddHealthRegen", "ModifyHealthRegen", "ModifyEitrRegen", "ModifyEitrRegenLowHealth", "Glowing")),
        (Piece "Legs" "GraveTreads" "Grave Treads" "Legs" @("ModifyMovementSpeed", "IncreaseHealth", "ModifyHealthRegen", "ModifyEitrRegen", "ModifySprintStaminaUse", "ModifyDodgeStaminaUse", "FeatherFall")),
        (Piece "Shoulder" "BloodMantle" "Blood Mantle" "Shoulder" @("IncreaseHealth", "ModifyHealthRegen", "AddHealthRegen", "IncreaseEitr", "ModifyEitrRegenLowHealth", "Waterproof", "Indestructible")),
        (Piece "Utility" "BoneCharm" "Bone Charm" "Utility" @("IncreaseHealth", "AddHealthRegen", "ModifyHealthRegen", "IncreaseEitr", "ModifyEitrRegen", "Luck", "DartingThoughts") @() $utilityItemNames),
        (Piece "Demister" "GraveWisp" "Grave Wisp" "Utility" @("ModifyWispRange", "IncreaseHealth", "AddHealthRegen", "ModifyHealthRegen", "ModifyEitrRegenLowHealth", "IncreaseEitr", "Glowing") @() $demisterItemNames),
        (Piece "Trinket" "MarrowSeal" "Marrow Seal" "Trinket" @("IncreaseHealth", "AddHealthRegen", "ModifyHealthRegen", "IncreaseEitr", "ModifyEitrRegen", "Luck", "Glowing"))
    ) @("AddHelveigSkill", "ModifyAttackEitrUse", "IncreaseHealth", "ModifySummonDamage", "ModifySummonHealth", "ModifyHealthRegen", "ModifyAttackHealthUse") @{
        Magic = "Bone-Spark Pact"
        Rare = "Marrowcaller Rite"
        Epic = "Draugr Choir"
        Legendary = "Helveig Covenant"
        Mythic = "Blood-Moon Conclave"
        Ancient = "Helheim Dominion"
    }
$families += Family "Moonvein" $false "the Moonvein class line for spellbow combat, eitr flow and Moonvein skill scaling" @(
        (Piece "Weapon" "Moonbow" "Moonbow" "Bow" @("SpellSword", "EitrLeech", "AddMoonveinSkill", "ModifyDrawStaminaUse", "ModifyProjectileSpeed", "ModifyFireRate", "TripleBowShot")),
        (Piece "Helmet" "Cowl" "Cowl" "Helmet" @("AddMoonveinSkill", "IncreaseEitr", "ModifyEitrRegen", "ModifyDiscoveryRadius", "HeadHunter", "Luck", "Glowing")),
        (Piece "Chest" "Hauberk" "Hauberk" "Chest" @("ModifyEitrRegen", "ModifyDrawStaminaUse", "IncreaseEitr", "ModifyStaminaRegen", "AddMovementSkills", "ModifyAttackEitrUse", "Glowing")),
        (Piece "Legs" "Striders" "Striders" "Legs" @("ModifyMovementSpeed", "ModifySprintStaminaUse", "AddMovementSkills", "ModifyEitrRegen", "IncreaseStamina", "ModifyDodgeStaminaUse", "DoubleJump")),
        (Piece "Shoulder" "Mantle" "Mantle" "Shoulder" @("IncreaseEitr", "ModifyEitrRegen", "ModifyStaminaRegen", "AddMovementSkills", "IncreaseStamina", "RemoveSpeedPenalty", "Indestructible")),
        (Piece "Utility" "MoonLens" "Moon Lens" "Utility" @("IncreaseEitr", "ModifyEitrRegen", "IncreaseStamina", "ModifyStaminaRegen", "AddMoonveinSkill", "Luck", "Glowing") @() $utilityItemNames),
        (Piece "Demister" "MoonWisp" "Moon Wisp" "Utility" @("ModifyWispRange", "IncreaseEitr", "ModifyEitrRegen", "AddMoonveinSkill", "IncreaseStamina", "Luck", "Glowing") @() $demisterItemNames),
        (Piece "Trinket" "EitrNock" "Eitr Nock" "Trinket" @("IncreaseEitr", "ModifyEitrRegen", "IncreaseStamina", "ModifyStaminaRegen", "AddMoonveinSkill", "Luck", "Glowing"))
    ) @("SpellSword", "ModifyAttackEitrUse", "EitrLeech", "AddMoonveinSkill", "IncreaseEitr", "TripleBowShot", "ModifyFireRate") @{
        Magic = "Moonlit Nock"
        Rare = "Eitrstring Path"
        Epic = "Spellbow Meridian"
        Legendary = "Moonvein Arcana"
        Mythic = "Lunar Spellshot"
        Ancient = "Celestial Bow-Rite"
    }
$families += Family "Frostbrand" $false "the Frostbrand class line for two-handed spellblade combat, elemental bursts and Frostbrand skill scaling" @(
        (Piece "Weapon" "Runeblade" "Runeblade" "TwoHandedWeapon" @("SpellSword", "ModifyAttackSpeed", "AddFrostDamage", "ModifyElementalDamage", "EitrLeech", "ModifyAttackEitrUse", "EitrWeave", "AddFrostbrandSkill", "ModifyPhysicalDamage") @("Swords")),
        (Piece "Helmet" "Crown" "Crown" "Helmet" @("IncreaseEitr", "ModifyEitrRegen", "ModifyArmor", "AddFrostResistancePercentage", "Glowing", "Luck")),
        (Piece "Chest" "Cuirass" "Cuirass" "Chest" @("ModifyEitrRegen", "IncreaseEitr", "ModifyAttackEitrUse", "ModifyArmor", "IncreaseStamina", "AddFrostResistancePercentage", "Weightless")),
        (Piece "Legs" "Greaves" "Greaves" "Legs" @("ModifyMovementSpeed", "IncreaseEitr", "ModifySprintStaminaUse", "ModifyDodgeStaminaUse", "IncreaseStamina", "AddMovementSkills", "AddFrostResistancePercentage")),
        (Piece "Shoulder" "Cloak" "Cloak" "Shoulder" @("AddFrostResistancePercentage", "IncreaseEitr", "ModifyEitrRegen", "ModifyStaminaRegen", "RemoveSpeedPenalty", "Warmth", "Indestructible")),
        (Piece "Utility" "FrostFocus" "Frost Focus" "Utility" @("IncreaseEitr", "ModifyEitrRegen", "IncreaseStamina", "AddFrostResistancePercentage", "Luck", "Glowing", "ModifyArmor") @() $utilityItemNames),
        (Piece "Demister" "FrostWisp" "Frost Wisp" "Utility" @("ModifyWispRange", "IncreaseEitr", "ModifyEitrRegen", "AddFrostResistancePercentage", "AddFrostbrandSkill", "Luck", "Glowing") @() $demisterItemNames),
        (Piece "Trinket" "FrozenSeal" "Frozen Seal" "Trinket" @("IncreaseEitr", "ModifyEitrRegen", "IncreaseStamina", "AddFrostResistancePercentage", "Luck", "Glowing", "ModifyArmor"))
    ) @("SpellSword", "IncreaseEitr", "ModifyAttackEitrUse", "EitrLeech", "AddFrostbrandSkill", "ModifyElementalDamage", "FrostDamageAOE") @{
        Magic = "Frost-Touched Blades"
        Rare = "Runeblade Accord"
        Epic = "Cold Star Blades"
        Legendary = "Frostbrand Covenant"
        Mythic = "Glacierbrand Oath"
        Ancient = "Fimbulwinter Blades"
    }

function SetIdFor([object] $family, [object] $rarity) {
    if ($rarity.Key -eq "Legendary") {
        return $family.ID
    }

    return "$($rarity.Prefix)$($family.ID)"
}

function ItemIdFor([object] $family, [object] $rarity, [object] $piece) {
    return "$($rarity.Prefix)$($family.ID)$($piece.Suffix)"
}

function ItemNameFor([object] $family, [object] $rarity, [object] $piece) {
    $setName = $family.SetNames[$rarity.Key]
    return "$setName $($piece.Name)"
}

function NewSetItem([object] $family, [object] $rarity, [object] $piece) {
    $itemId = ItemIdFor $family $rarity $piece
    $effects = @($piece.Effects | Select-Object -First $rarity.EffectCount | ForEach-Object { ItemEffect $_ })
    $article = if ($rarity.Key -match "^(Epic|Ancient)$") { "An" } else { "A" }

    return Obj ([ordered] @{
        ID = $itemId
        Name = ItemNameFor $family $rarity $piece
        Description = "$article $($rarity.Key.ToLowerInvariant()) set piece for $($family.Theme)."
        IsSetItem = $true
        Requirements = Requirement $piece.ItemType $piece.SkillTypes $piece.ItemNames
        GuaranteedEffectCount = $rarity.EffectCount
        GuaranteedMagicEffects = @($effects)
    })
}

function NewSetInfo([object] $family, [object] $rarity, [object[]] $pieces) {
    $pieceCount = $pieces.Count
    $fullSetActivationCount = [Math]::Max(2, $pieceCount - 1)
    $bonuses = @()
    for ($i = 0; $i -lt ($pieceCount - 1); $i++) {
        $bonuses += Bonus ([Math]::Min($i + 2, $fullSetActivationCount)) $family.BonusTypes[$i] $rarity.Key
    }

    return Obj ([ordered] @{
        ID = SetIdFor $family $rarity
        Name = $family.SetNames[$rarity.Key]
        LegendaryIDs = @($pieces | ForEach-Object { ItemIdFor $family $rarity $_ })
        SetBonuses = @($bonuses)
    })
}

function PiecesForRarity([object] $family, [object] $rarity) {
    $pieceCount = [int] $rarity.PieceBase
    if ($family.UsesShield) {
        $pieceCount += 1
    }

    $selected = New-Object System.Collections.Generic.List[object]
    foreach ($slot in @($family.EssentialSlots)) {
        $piece = @($family.Pieces | Where-Object { $_.Slot -eq $slot } | Select-Object -First 1)
        if ($piece.Count -eq 0) {
            throw "Family $($family.ID) is missing essential slot $slot."
        }

        $selected.Add($piece[0])
    }

    foreach ($piece in @($family.Pieces)) {
        if ($selected.Count -ge $pieceCount) {
            break
        }

        if (@($selected | Where-Object { $_.Suffix -eq $piece.Suffix }).Count -eq 0) {
            $selected.Add($piece)
        }
    }

    return @($selected.ToArray())
}

function FixedValue([double] $value, [double] $increment = 1.0) {
    return Obj ([ordered] @{
            MinValue  = $value
            MaxValue  = $value
            Increment = $increment
        })
}

function SetEffectValue([object] $effect, [double] $value, [double] $increment = 1.0) {
    $values = FixedValue $value $increment
    if ($effect.PSObject.Properties["Values"]) {
        $effect.Values = $values
    }
    else {
        $effect | Add-Member -NotePropertyName Values -NotePropertyValue $values
    }
}

function AddFixedGuaranteedEffect([object] $item, [string] $effectType, [double] $value, [double] $increment = 1.0) {
    $effects = @($item.GuaranteedMagicEffects)
    $matchingEffect = @($effects | Where-Object { $_.Type -eq $effectType } | Select-Object -First 1)
    if ($matchingEffect.Count -gt 0) {
        SetEffectValue $matchingEffect[0] $value $increment
        return
    }

    $effect = ItemEffect $effectType
    SetEffectValue $effect $value $increment
    $effects += $effect
    $item.GuaranteedMagicEffects = @($effects)
    $item.GuaranteedEffectCount = $effects.Count
}

function RemoveGuaranteedEffect([object] $item, [string] $effectType) {
    $item.GuaranteedMagicEffects = @($item.GuaranteedMagicEffects | Where-Object { $_.Type -ne $effectType })
    $item.GuaranteedEffectCount = @($item.GuaranteedMagicEffects).Count
}

function AddSetBonusIfMissing([object] $set, [int] $count, [string] $effectType, [string] $rarity) {
    $existing = @($set.SetBonuses | Where-Object { $_.Count -eq $count -and $_.Effect.Type -eq $effectType } | Select-Object -First 1)
    if ($existing.Count -gt 0) {
        return
    }

    $set.SetBonuses = @(@($set.SetBonuses) + (Bonus $count $effectType $rarity))
}

$forcedPrefabRequirements = @()

function ForcedPrefabs([string] $rarityKey, [string] $familyId, [string] $pieceSuffix, [string[]] $itemNames) {
    $script:forcedPrefabRequirements += Obj ([ordered] @{
        Rarity = $rarityKey
        Family = $familyId
        Suffix = $pieceSuffix
        ItemNames = @($itemNames)
    })
}

function SetAllowedItemNames([object] $item, [string[]] $itemNames) {
    if ($null -eq $item.Requirements) {
        $item | Add-Member -NotePropertyName Requirements -NotePropertyValue (Obj ([ordered] @{}))
    }

    foreach ($property in @("AllowedItemTypes", "AllowedSkillTypes")) {
        if ($item.Requirements.PSObject.Properties[$property]) {
            $item.Requirements.PSObject.Properties.Remove($property)
        }
    }

    if ($item.Requirements.PSObject.Properties["AllowedItemNames"]) {
        $item.Requirements.AllowedItemNames = @($itemNames)
    }
    else {
        $item.Requirements | Add-Member -NotePropertyName AllowedItemNames -NotePropertyValue @($itemNames)
    }
}

function ApplyForcedPrefabRequirements() {
    foreach ($entry in @($forcedPrefabRequirements)) {
        $prefix = if ($entry.Rarity -eq "Legendary") { "" } else { $entry.Rarity }
        $bucket = "$($entry.Rarity)Items"
        $itemId = "$prefix$($entry.Family)$($entry.Suffix)"
        $item = @($generated[$bucket] | Where-Object { $_.ID -eq $itemId } | Select-Object -First 1)
        if ($item.Count -eq 0) {
            throw "Could not find generated item $itemId for forced prefab requirements."
        }

        SetAllowedItemNames $item[0] $entry.ItemNames
    }
}

$beltStrengthPrefabNames = @("BeltStrength", '$item_megingjord')
$demisterPrefabNames = @("Demister", '$item_demister')

$trollArmor = @{
    Helmet = @("HelmetTrollLeather")
    Chest = @("ArmorTrollLeatherChest")
    Legs = @("ArmorTrollLeatherLegs")
}
$rootArmor = @{
    Helmet = @("HelmetRoot")
    Chest = @("ArmorRootChest")
    Legs = @("ArmorRootLegs")
}
$paddedArmor = @{
    Helmet = @("HelmetPadded")
    Chest = @("ArmorPaddedCuirass")
    Legs = @("ArmorPaddedGreaves")
}
$bjornArmor = @{
    Helmet = @("HelmetBerserkerHood")
    Chest = @("ArmorBerserkerChest")
    Legs = @("ArmorBerserkerLegs")
}
$vileBoneArmor = @{
    Helmet = @("HelmetBerserkerUndead")
    Chest = @("ArmorBerserkerUndeadChest")
    Legs = @("ArmorBerserkerUndeadLegs")
}
$fenringArmor = @{
    Helmet = @("HelmetFenring")
    Chest = @("ArmorFenringChest")
    Legs = @("ArmorFenringLegs")
}
$carapaceArmor = @{
    Helmet = @("HelmetCarapace")
    Chest = @("ArmorCarapaceChest")
    Legs = @("ArmorCarapaceLegs")
}
$emblaArmor = @{
    Helmet = @("HelmetMage")
    Chest = @("ArmorMageChest")
    Legs = @("ArmorMageLegs")
}
$flametalArmor = @{
    Helmet = @("HelmetFlametal")
    Chest = @("ArmorFlametalChest")
    Legs = @("ArmorFlametalLegs")
}
$askArmor = @{
    Helmet = @("HelmetAshlandsMediumHood")
    Chest = @("ArmorAshlandsMediumChest")
    Legs = @("ArmorAshlandsMediumlegs")
}
$loxArmor = @{
    Helmet = @("HelmetLox")
    Chest = @("ArmorLoxChest")
    Legs = @("ArmorLoxLegs")
}
$deepNorthHeavyArmor = @{
    Helmet = @("HelmetDNHeavy")
    Chest = @("ArmorDeepNorthHeavyChest")
    Legs = @("ArmorDeepNorthHeavylegs")
}
$deepNorthMageArmor = @{
    Helmet = @("HelmetDNMage")
    Chest = @("ArmorDeepNorthMageChest")
    Legs = @("ArmorDeepNorthMagelegs")
}
$deepNorthMediumArmor = @{
    Helmet = @("HelmetDNMediumHood")
    Chest = @("ArmorDeepNorthMediumChest")
    Legs = @("ArmorDeepNorthMediumlegs")
}
$elementalStaffs = @("StaffLightning", "StaffGreenRoots", "StaffRedTroll", "StaffFireball", "StaffIceShards", "StaffClusterbomb", "StaffOrbofAhri", "StaffThunderBlood")
$bloodStaffs = @("StaffSkeleton", "StaffShield", "StaffSpiritCaller", "StaffRedTroll", "StaffFrostOrbs")
$goldBows = @("BowGold_BloodLightning", "BowGold_FrostFire")
$goldCrossbows = @("CrossbowGold_BloodLightning", "CrossbowGold_FrostFire")
$goldKnives = @("KnifeGold_BloodLightning", "KnifeGold_FrostFire")
$goldTwoHandedSwords = @("THSwordGold_FrostFire", "THSwordGold_BloodLightning")
$ashlandsBows = @("BowAshlandsRoot", "BowAshlandsBlood", "BowAshlandsStorm")
$ashlandsCrossbows = @("CrossbowRipper", "CrossbowRipperBlood", "CrossbowRipperLightning", "CrossbowRipperNature")
$splitnerSpears = @("SpearSplitner", "SpearSplitner_Nature", "SpearSplitner_Lightning", "SpearSplitner_Blood")
$slayerSwords = @("THSwordSlayer", "THSwordSlayerNature", "THSwordSlayerLightning", "THSwordSlayerBlood")

if ($effectByType.ContainsKey("ModifyAttackHealthUse")) {
    $attackHealthUseItemNames = @(
        "FistFenrirClaw",
        "FistBjornUndeadClaw",
        "KnifeSkollAndHati",
        "AxeBerzerkrBlood"
    ) + $bloodStaffs

    Remove-RequirementProperty $effectByType["ModifyAttackHealthUse"] "ItemUsesHealthOnAttack"
    Add-UniqueRequirementValues $effectByType["ModifyAttackHealthUse"] "AllowedItemNames" $attackHealthUseItemNames
}

function ForceArmorPrefabs([string] $rarity, [string] $family, [hashtable] $armor, [string] $helmetSuffix, [string] $chestSuffix, [string] $legsSuffix) {
    ForcedPrefabs $rarity $family $helmetSuffix $armor.Helmet
    ForcedPrefabs $rarity $family $chestSuffix $armor.Chest
    ForcedPrefabs $rarity $family $legsSuffix $armor.Legs
}

ForcedPrefabs "Magic" "Heimdall" "TowerShield" @("ShieldIronTower")
ForcedPrefabs "Magic" "Heimdall" "Oathblade" @("SpearElderbark")
ForcedPrefabs "Magic" "Heimdall" "Helmet" @("HelmetIron")
ForcedPrefabs "Magic" "Heimdall" "Chest" @("ArmorIronChest")
ForcedPrefabs "Magic" "Ragnar" "BattleAxe" @("FistFenrirClaw")
ForcedPrefabs "Magic" "Ragnar" "Helmet" $trollArmor.Helmet
ForcedPrefabs "Magic" "Ragnar" "Chest" $trollArmor.Chest
ForcedPrefabs "Magic" "Hraesvelgr" "Longbow" @("BowHuntsman")
ForcedPrefabs "Magic" "Hraesvelgr" "Hood" $rootArmor.Helmet
ForcedPrefabs "Magic" "Hraesvelgr" "Harness" $rootArmor.Chest
ForcedPrefabs "Magic" "Nott" "Nightblade" @("KnifeChitin")
ForcedPrefabs "Magic" "Nott" "Veil" $trollArmor.Helmet
ForcedPrefabs "Magic" "Nott" "Leathers" $trollArmor.Chest
ForcedPrefabs "Magic" "Helveig" "Bloodstaff" @("StaffSkeleton", "StaffShield")
ForcedPrefabs "Magic" "Helveig" "BoneCrown" $emblaArmor.Helmet
ForcedPrefabs "Magic" "Helveig" "MarrowRobe" $emblaArmor.Chest
ForcedPrefabs "Magic" "Moonvein" "Moonbow" @("BowHuntsman")
ForcedPrefabs "Magic" "Moonvein" "Cowl" $rootArmor.Helmet
ForcedPrefabs "Magic" "Moonvein" "Hauberk" $rootArmor.Chest
ForcedPrefabs "Magic" "Frostbrand" "Runeblade" @("Battleaxe")
ForcedPrefabs "Magic" "Frostbrand" "Crown" @("HelmetIron")
ForcedPrefabs "Magic" "Frostbrand" "Cuirass" @("ArmorIronChest")

ForcedPrefabs "Rare" "Heimdall" "TowerShield" @("ShieldSerpentscale")
ForcedPrefabs "Rare" "Heimdall" "Oathblade" @("SpearWolfFang")
ForceArmorPrefabs "Rare" "Heimdall" $paddedArmor "Helmet" "Chest" "Legs"
ForcedPrefabs "Rare" "Ragnar" "BattleAxe" @("FistBjornUndeadClaw")
ForceArmorPrefabs "Rare" "Ragnar" $bjornArmor "Helmet" "Chest" "Legs"
ForcedPrefabs "Rare" "Hraesvelgr" "Longbow" @("BowDraugrFang")
ForceArmorPrefabs "Rare" "Hraesvelgr" $rootArmor "Hood" "Harness" "Stride"
ForcedPrefabs "Rare" "Hellsyng" "WitchfinderArbalest" @("CrossbowArbalest")
ForceArmorPrefabs "Rare" "Hellsyng" $trollArmor "Widebrim" "Longcoat" "Boots"
ForcedPrefabs "Rare" "Nott" "Nightblade" @("KnifeSilver")
ForceArmorPrefabs "Rare" "Nott" $trollArmor "Veil" "Leathers" "Treads"
ForcedPrefabs "Rare" "Seidr" "Staff" @("StaffFireball", "StaffIceShards")
ForceArmorPrefabs "Rare" "Seidr" $emblaArmor "Crown" "Robe" "Treads"
ForcedPrefabs "Rare" "Helveig" "Bloodstaff" @("StaffSkeleton", "StaffShield")
ForceArmorPrefabs "Rare" "Helveig" $emblaArmor "BoneCrown" "MarrowRobe" "GraveTreads"
ForcedPrefabs "Rare" "Moonvein" "Moonbow" @("BowDraugrFang")
ForceArmorPrefabs "Rare" "Moonvein" $rootArmor "Cowl" "Hauberk" "Striders"
ForcedPrefabs "Rare" "Frostbrand" "Runeblade" @("SwordMistwalker")
ForceArmorPrefabs "Rare" "Frostbrand" $paddedArmor "Crown" "Cuirass" "Greaves"

ForcedPrefabs "Epic" "Heimdall" "TowerShield" @("ShieldBlackmetalTower")
ForcedPrefabs "Epic" "Heimdall" "Oathblade" @("SpearCarapace")
ForceArmorPrefabs "Epic" "Heimdall" $carapaceArmor "Helmet" "Chest" "Legs"
ForcedPrefabs "Epic" "Heimdall" "Cape" @("CapeAsksvin")
ForcedPrefabs "Epic" "Ragnar" "BattleAxe" @("KnifeSkollAndHati")
ForceArmorPrefabs "Epic" "Ragnar" $bjornArmor "Helmet" "Chest" "Legs"
ForcedPrefabs "Epic" "Ragnar" "Cape" @("CapeAsksvin")
ForcedPrefabs "Epic" "Hraesvelgr" "Longbow" @("BowSpineSnap")
ForceArmorPrefabs "Epic" "Hraesvelgr" $loxArmor "Hood" "Harness" "Stride"
ForcedPrefabs "Epic" "Hraesvelgr" "Mantle" @("CapeAsksvin")
ForcedPrefabs "Epic" "Hellsyng" "WitchfinderArbalest" @("CrossbowArbalest")
ForceArmorPrefabs "Epic" "Hellsyng" $loxArmor "Widebrim" "Longcoat" "Boots"
ForcedPrefabs "Epic" "Hellsyng" "Mantle" @("CapeAsksvin")
ForcedPrefabs "Epic" "Nott" "Nightblade" @("KnifeBlackMetal")
ForceArmorPrefabs "Epic" "Nott" $fenringArmor "Veil" "Leathers" "Treads"
ForcedPrefabs "Epic" "Nott" "Cloak" @("CapeAsksvin")
ForcedPrefabs "Epic" "Seidr" "Staff" @("StaffFireball", "StaffGreenRoots", "StaffIceShards")
ForceArmorPrefabs "Epic" "Seidr" $emblaArmor "Crown" "Robe" "Treads"
ForcedPrefabs "Epic" "Seidr" "Mantle" @("CapeFeather")
ForcedPrefabs "Epic" "Helveig" "Bloodstaff" @("StaffSkeleton", "StaffShield")
ForceArmorPrefabs "Epic" "Helveig" $emblaArmor "BoneCrown" "MarrowRobe" "GraveTreads"
ForcedPrefabs "Epic" "Helveig" "BloodMantle" @("CapeFeather")
ForcedPrefabs "Epic" "Moonvein" "Moonbow" @("BowDraugrFang")
ForceArmorPrefabs "Epic" "Moonvein" $loxArmor "Cowl" "Hauberk" "Striders"
ForcedPrefabs "Epic" "Moonvein" "Mantle" @("CapeAsksvin")
ForcedPrefabs "Epic" "Frostbrand" "Runeblade" @("THSwordSlayer")
ForceArmorPrefabs "Epic" "Frostbrand" $carapaceArmor "Crown" "Cuirass" "Greaves"
ForcedPrefabs "Epic" "Frostbrand" "Cloak" @("CapeAsksvin")

ForcedPrefabs "Legendary" "Heimdall" "TowerShield" @("ShieldFlametalTower")
ForcedPrefabs "Legendary" "Heimdall" "Oathblade" $splitnerSpears
ForceArmorPrefabs "Legendary" "Heimdall" $flametalArmor "Helmet" "Chest" "Legs"
ForcedPrefabs "Legendary" "Heimdall" "Cape" @("CapeAsh")
ForcedPrefabs "Legendary" "Heimdall" "WatchSigil" $beltStrengthPrefabNames
ForcedPrefabs "Legendary" "Ragnar" "BattleAxe" @("AxeBerzerkrBlood")
ForceArmorPrefabs "Legendary" "Ragnar" $bjornArmor "Helmet" "Chest" "Legs"
ForcedPrefabs "Legendary" "Ragnar" "Cape" @("CapeDeerHide")
ForcedPrefabs "Legendary" "Ragnar" "FuryTotem" $beltStrengthPrefabNames
ForcedPrefabs "Legendary" "Hraesvelgr" "Longbow" $ashlandsBows
ForceArmorPrefabs "Legendary" "Hraesvelgr" $loxArmor "Hood" "Harness" "Stride"
ForcedPrefabs "Legendary" "Hraesvelgr" "Mantle" @("CapeDeerHide")
ForcedPrefabs "Legendary" "Hraesvelgr" "WindGauge" $beltStrengthPrefabNames
ForcedPrefabs "Legendary" "Hellsyng" "WitchfinderArbalest" $ashlandsCrossbows
ForceArmorPrefabs "Legendary" "Hellsyng" $loxArmor "Widebrim" "Longcoat" "Boots"
ForcedPrefabs "Legendary" "Hellsyng" "Mantle" @("CapeDeerHide")
ForcedPrefabs "Legendary" "Hellsyng" "PowderHorn" $beltStrengthPrefabNames
ForcedPrefabs "Legendary" "Nott" "Nightblade" $goldKnives
ForceArmorPrefabs "Legendary" "Nott" $fenringArmor "Veil" "Leathers" "Treads"
ForcedPrefabs "Legendary" "Nott" "Cloak" @("CapeDeerHide")
ForcedPrefabs "Legendary" "Nott" "SmokeVial" $beltStrengthPrefabNames
ForcedPrefabs "Legendary" "Seidr" "Staff" $elementalStaffs
ForceArmorPrefabs "Legendary" "Seidr" $emblaArmor "Crown" "Robe" "Treads"
ForcedPrefabs "Legendary" "Seidr" "Mantle" @("CapeFeather")
ForcedPrefabs "Legendary" "Seidr" "EitrFocus" $beltStrengthPrefabNames
ForcedPrefabs "Legendary" "Helveig" "Bloodstaff" $bloodStaffs
ForceArmorPrefabs "Legendary" "Helveig" $emblaArmor "BoneCrown" "MarrowRobe" "GraveTreads"
ForcedPrefabs "Legendary" "Helveig" "BloodMantle" @("CapeFeather")
ForcedPrefabs "Legendary" "Helveig" "BoneCharm" $beltStrengthPrefabNames
ForcedPrefabs "Legendary" "Moonvein" "Moonbow" $ashlandsBows
ForceArmorPrefabs "Legendary" "Moonvein" $loxArmor "Cowl" "Hauberk" "Striders"
ForcedPrefabs "Legendary" "Moonvein" "Mantle" @("CapeDeerHide")
ForcedPrefabs "Legendary" "Moonvein" "MoonLens" $beltStrengthPrefabNames
ForcedPrefabs "Legendary" "Frostbrand" "Runeblade" $slayerSwords
ForceArmorPrefabs "Legendary" "Frostbrand" $flametalArmor "Crown" "Cuirass" "Greaves"
ForcedPrefabs "Legendary" "Frostbrand" "Cloak" @("CapeDeerHide")
ForcedPrefabs "Legendary" "Frostbrand" "FrostFocus" $beltStrengthPrefabNames

ForcedPrefabs "Mythic" "Heimdall" "TowerShield" @("ShieldGoldTower")
ForcedPrefabs "Mythic" "Heimdall" "Oathblade" @("SpearGold_FrostFire")
ForceArmorPrefabs "Mythic" "Heimdall" $deepNorthHeavyArmor "Helmet" "Chest" "Legs"
ForcedPrefabs "Mythic" "Heimdall" "Cape" @("CapeAsh")
ForcedPrefabs "Mythic" "Heimdall" "WatchSigil" $beltStrengthPrefabNames
ForcedPrefabs "Mythic" "Heimdall" "WardingWisp" $demisterPrefabNames
ForcedPrefabs "Mythic" "Ragnar" "BattleAxe" @("AxeBerzerkrBlood")
ForceArmorPrefabs "Mythic" "Ragnar" $vileBoneArmor "Helmet" "Chest" "Legs"
ForcedPrefabs "Mythic" "Ragnar" "Cape" @("CapeDeepNorth")
ForcedPrefabs "Mythic" "Ragnar" "FuryTotem" $beltStrengthPrefabNames
ForcedPrefabs "Mythic" "Ragnar" "FrostWisp" $demisterPrefabNames
ForcedPrefabs "Mythic" "Hraesvelgr" "Longbow" $goldBows
ForceArmorPrefabs "Mythic" "Hraesvelgr" $loxArmor "Hood" "Harness" "Stride"
ForcedPrefabs "Mythic" "Hraesvelgr" "Mantle" @("CapeDeepNorth")
ForcedPrefabs "Mythic" "Hraesvelgr" "WindGauge" $beltStrengthPrefabNames
ForcedPrefabs "Mythic" "Hraesvelgr" "WispFeather" $demisterPrefabNames
ForcedPrefabs "Mythic" "Hellsyng" "WitchfinderArbalest" $goldCrossbows
ForceArmorPrefabs "Mythic" "Hellsyng" $vileBoneArmor "Widebrim" "Longcoat" "Boots"
ForcedPrefabs "Mythic" "Hellsyng" "Mantle" @("CapeDeepNorth")
ForcedPrefabs "Mythic" "Hellsyng" "PowderHorn" $beltStrengthPrefabNames
ForcedPrefabs "Mythic" "Hellsyng" "Witchlamp" $demisterPrefabNames
ForcedPrefabs "Mythic" "Nott" "Nightblade" $goldKnives
ForceArmorPrefabs "Mythic" "Nott" $askArmor "Veil" "Leathers" "Treads"
ForcedPrefabs "Mythic" "Nott" "Cloak" @("CapeDeepNorth")
ForcedPrefabs "Mythic" "Nott" "SmokeVial" $beltStrengthPrefabNames
ForcedPrefabs "Mythic" "Nott" "ShadowWisp" $demisterPrefabNames
ForcedPrefabs "Mythic" "Seidr" "Staff" $elementalStaffs
ForceArmorPrefabs "Mythic" "Seidr" $deepNorthMageArmor "Crown" "Robe" "Treads"
ForcedPrefabs "Mythic" "Seidr" "Mantle" @("CapeDeepNorthMage")
ForcedPrefabs "Mythic" "Seidr" "EitrFocus" $beltStrengthPrefabNames
ForcedPrefabs "Mythic" "Seidr" "MistWisp" $demisterPrefabNames
ForcedPrefabs "Mythic" "Helveig" "Bloodstaff" $bloodStaffs
ForceArmorPrefabs "Mythic" "Helveig" $deepNorthMageArmor "BoneCrown" "MarrowRobe" "GraveTreads"
ForcedPrefabs "Mythic" "Helveig" "BloodMantle" @("CapeDeepNorthMage")
ForcedPrefabs "Mythic" "Helveig" "BoneCharm" $beltStrengthPrefabNames
ForcedPrefabs "Mythic" "Helveig" "GraveWisp" $demisterPrefabNames
ForcedPrefabs "Mythic" "Moonvein" "Moonbow" $goldBows
ForceArmorPrefabs "Mythic" "Moonvein" $loxArmor "Cowl" "Hauberk" "Striders"
ForcedPrefabs "Mythic" "Moonvein" "Mantle" @("CapeDeepNorth")
ForcedPrefabs "Mythic" "Moonvein" "MoonLens" $beltStrengthPrefabNames
ForcedPrefabs "Mythic" "Moonvein" "MoonWisp" $demisterPrefabNames
ForcedPrefabs "Mythic" "Frostbrand" "Runeblade" $goldTwoHandedSwords
ForceArmorPrefabs "Mythic" "Frostbrand" $deepNorthMediumArmor "Crown" "Cuirass" "Greaves"
ForcedPrefabs "Mythic" "Frostbrand" "Cloak" @("CapeDeepNorth")
ForcedPrefabs "Mythic" "Frostbrand" "FrostFocus" $beltStrengthPrefabNames
ForcedPrefabs "Mythic" "Frostbrand" "FrostWisp" $demisterPrefabNames

ForcedPrefabs "Ancient" "Heimdall" "TowerShield" @("ShieldGoldTower")
ForcedPrefabs "Ancient" "Heimdall" "Oathblade" @("SpearGold_FrostFire")
ForceArmorPrefabs "Ancient" "Heimdall" $deepNorthHeavyArmor "Helmet" "Chest" "Legs"
ForcedPrefabs "Ancient" "Heimdall" "Cape" @("CapeAsh")
ForcedPrefabs "Ancient" "Heimdall" "WatchSigil" $beltStrengthPrefabNames
ForcedPrefabs "Ancient" "Heimdall" "WardingWisp" $demisterPrefabNames
ForcedPrefabs "Ancient" "Heimdall" "DawnSeal" @("TrinketBloodGoldStamina")
ForcedPrefabs "Ancient" "Ragnar" "BattleAxe" @("AxeBerzerkrBlood")
ForceArmorPrefabs "Ancient" "Ragnar" $vileBoneArmor "Helmet" "Chest" "Legs"
ForcedPrefabs "Ancient" "Ragnar" "Cape" @("CapeDeepNorth")
ForcedPrefabs "Ancient" "Ragnar" "FuryTotem" $beltStrengthPrefabNames
ForcedPrefabs "Ancient" "Ragnar" "FrostWisp" $demisterPrefabNames
ForcedPrefabs "Ancient" "Ragnar" "BloodOath" @("TrinketBloodGoldHealth")
ForcedPrefabs "Ancient" "Hraesvelgr" "Longbow" $goldBows
ForceArmorPrefabs "Ancient" "Hraesvelgr" $askArmor "Hood" "Harness" "Stride"
ForcedPrefabs "Ancient" "Hraesvelgr" "Mantle" @("CapeDeepNorth")
ForcedPrefabs "Ancient" "Hraesvelgr" "WindGauge" $beltStrengthPrefabNames
ForcedPrefabs "Ancient" "Hraesvelgr" "WispFeather" $demisterPrefabNames
ForcedPrefabs "Ancient" "Hraesvelgr" "SkyToken" @("TrinketBloodGoldStamina")
ForcedPrefabs "Ancient" "Hellsyng" "WitchfinderArbalest" $goldCrossbows
ForceArmorPrefabs "Ancient" "Hellsyng" $vileBoneArmor "Widebrim" "Longcoat" "Boots"
ForcedPrefabs "Ancient" "Hellsyng" "Mantle" @("CapeDeepNorth")
ForcedPrefabs "Ancient" "Hellsyng" "PowderHorn" $beltStrengthPrefabNames
ForcedPrefabs "Ancient" "Hellsyng" "Witchlamp" $demisterPrefabNames
ForcedPrefabs "Ancient" "Hellsyng" "SilverBolt" @("TrinketBloodGoldStamina")
ForcedPrefabs "Ancient" "Nott" "Nightblade" $goldKnives
ForceArmorPrefabs "Ancient" "Nott" $askArmor "Veil" "Leathers" "Treads"
ForcedPrefabs "Ancient" "Nott" "Cloak" @("CapeDeepNorth")
ForcedPrefabs "Ancient" "Nott" "SmokeVial" $beltStrengthPrefabNames
ForcedPrefabs "Ancient" "Nott" "ShadowWisp" $demisterPrefabNames
ForcedPrefabs "Ancient" "Nott" "SilentCoin" @("TrinketBloodGoldStamina")
ForcedPrefabs "Ancient" "Seidr" "Staff" $elementalStaffs
ForceArmorPrefabs "Ancient" "Seidr" $deepNorthMageArmor "Crown" "Robe" "Treads"
ForcedPrefabs "Ancient" "Seidr" "Mantle" @("CapeDeepNorthMage")
ForcedPrefabs "Ancient" "Seidr" "EitrFocus" $beltStrengthPrefabNames
ForcedPrefabs "Ancient" "Seidr" "MistWisp" $demisterPrefabNames
ForcedPrefabs "Ancient" "Seidr" "RuneBead" @("TrinketBloodGoldHealth")
ForcedPrefabs "Ancient" "Helveig" "Bloodstaff" $bloodStaffs
ForceArmorPrefabs "Ancient" "Helveig" $deepNorthMageArmor "BoneCrown" "MarrowRobe" "GraveTreads"
ForcedPrefabs "Ancient" "Helveig" "BloodMantle" @("CapeDeepNorthMage")
ForcedPrefabs "Ancient" "Helveig" "BoneCharm" $beltStrengthPrefabNames
ForcedPrefabs "Ancient" "Helveig" "GraveWisp" $demisterPrefabNames
ForcedPrefabs "Ancient" "Helveig" "MarrowSeal" @("TrinketBloodGoldHealth")
ForcedPrefabs "Ancient" "Moonvein" "Moonbow" $goldBows
ForceArmorPrefabs "Ancient" "Moonvein" $askArmor "Cowl" "Hauberk" "Striders"
ForcedPrefabs "Ancient" "Moonvein" "Mantle" @("CapeDeepNorth")
ForcedPrefabs "Ancient" "Moonvein" "MoonLens" $beltStrengthPrefabNames
ForcedPrefabs "Ancient" "Moonvein" "MoonWisp" $demisterPrefabNames
ForcedPrefabs "Ancient" "Moonvein" "EitrNock" @("TrinketBloodGoldHealth")
ForcedPrefabs "Ancient" "Frostbrand" "Runeblade" $goldTwoHandedSwords
ForceArmorPrefabs "Ancient" "Frostbrand" $deepNorthHeavyArmor "Crown" "Cuirass" "Greaves"
ForcedPrefabs "Ancient" "Frostbrand" "Cloak" @("CapeDeepNorth")
ForcedPrefabs "Ancient" "Frostbrand" "FrostFocus" $beltStrengthPrefabNames
ForcedPrefabs "Ancient" "Frostbrand" "FrostWisp" $demisterPrefabNames
ForcedPrefabs "Ancient" "Frostbrand" "FrozenSeal" @("TrinketBloodGoldHealth")

function ApplyMoonveinAmmoConservationOverrides() {
    foreach ($bucket in @("MagicItems", "RareItems", "EpicItems", "LegendaryItems", "MythicItems", "AncientItems")) {
        foreach ($item in @($generated[$bucket] | Where-Object { $_.ID -like "*MoonveinMoonbow" })) {
            AddFixedGuaranteedEffect $item "AmmoConservation" 100
        }
    }
}

function ApplyNottStealthBonusOverrides() {
    $noiseBySetId = [ordered] @{
        MagicNott   = 60
        RareNott    = 70
        EpicNott    = 80
        Nott        = 90
        MythicNott  = 95
        AncientNott = 100
    }

    foreach ($bucket in @("MagicSets", "RareSets", "EpicSets", "LegendarySets", "MythicSets", "AncientSets")) {
        foreach ($set in @($generated[$bucket] | Where-Object { $noiseBySetId.Contains($_.ID) })) {
            $noiseBonus = @($set.SetBonuses | Where-Object { $_.Effect.Type -eq "ModifyNoise" } | Select-Object -First 1)
            if ($noiseBonus.Count -eq 0) {
                throw "Could not find ModifyNoise bonus on $($set.ID)."
            }

            SetEffectValue $noiseBonus[0].Effect $noiseBySetId[$set.ID]
        }
    }
}

function ApplyFrostbrandRunebladeEitrOverride() {
    $found = 0
    foreach ($bucket in @("MagicItems", "RareItems", "EpicItems", "LegendaryItems", "MythicItems", "AncientItems")) {
        foreach ($item in @($generated[$bucket] | Where-Object { $_.ID -like "*FrostbrandRuneblade" })) {
            RemoveGuaranteedEffect $item "ModifyAttackSpeed"
            AddFixedGuaranteedEffect $item "IncreaseEitr" 100
            $found++
        }
    }

    if ($found -eq 0) {
        throw "Could not find Frostbrand runeblade items."
    }
}

function ApplyMagicRagnarSurvivalOverride() {
    $item = @($generated.MagicItems | Where-Object { $_.ID -eq "MagicRagnarBattleAxe" } | Select-Object -First 1)
    if ($item.Count -eq 0) {
        throw "Could not find MagicRagnarBattleAxe."
    }

    AddFixedGuaranteedEffect $item[0] "IncreaseHealth" 50
}

function ApplyRagnarLifeStealOverrides() {
    $lifeStealByBucket = [ordered] @{
        MagicItems     = 5
        RareItems      = 6
        EpicItems      = 8
        LegendaryItems = 10
        MythicItems    = 12
        AncientItems   = 14
    }

    foreach ($bucket in @($lifeStealByBucket.Keys)) {
        foreach ($item in @($generated[$bucket] | Where-Object { $_.ID -like "*RagnarBattleAxe" })) {
            AddFixedGuaranteedEffect $item "LifeSteal" $lifeStealByBucket[$bucket] 0.5
        }
    }
}

function ApplyRagnarAttackHealthUseOverrides() {
    foreach ($bucket in @("MagicItems", "RareItems", "EpicItems", "LegendaryItems", "MythicItems", "AncientItems")) {
        foreach ($item in @($generated[$bucket] | Where-Object { $_.ID -like "*RagnarBattleAxe" })) {
            AddFixedGuaranteedEffect $item "ModifyAttackHealthUse" 50
        }
    }
}

function ApplyHelveigBloodstaffHealthUseOverrides() {
    foreach ($bucket in @("MagicItems", "RareItems", "EpicItems", "LegendaryItems", "MythicItems", "AncientItems")) {
        foreach ($item in @($generated[$bucket] | Where-Object { $_.ID -like "*HelveigBloodstaff" })) {
            AddFixedGuaranteedEffect $item "ModifyAttackHealthUse" 50
        }
    }
}

function ApplyFrostDamageAoePairing() {
    $setBuckets = [ordered] @{
        MagicSets     = "Magic"
        RareSets      = "Rare"
        EpicSets      = "Epic"
        LegendarySets = "Legendary"
        MythicSets    = "Mythic"
        AncientSets   = "Ancient"
    }

    foreach ($bucket in @($setBuckets.Keys)) {
        $rarity = $setBuckets[$bucket]
        foreach ($set in @($generated[$bucket] | Where-Object { $_.ID -match "^(Magic|Rare|Epic|Mythic|Ancient)?(Ragnar|Frostbrand)$" })) {
            foreach ($frostAoeBonus in @($set.SetBonuses | Where-Object { $_.Effect.Type -eq "FrostDamageAOE" })) {
                AddSetBonusIfMissing $set $frostAoeBonus.Count "AddFrostDamage" $rarity
            }
        }
    }
}

$generated = @{
    MagicItems = @()
    MagicSets = @()
    RareItems = @()
    RareSets = @()
    EpicItems = @()
    EpicSets = @()
    LegendaryItems = @()
    LegendarySets = @()
    MythicItems = @()
    MythicSets = @()
    AncientItems = @()
    AncientSets = @()
}

foreach ($rarity in $rarities) {
    foreach ($family in $families) {
        if ($rarity.Key -eq "Magic" -and $family.ID -in @("Hellsyng", "Seidr")) {
            continue
        }

        $pieces = PiecesForRarity $family $rarity
        $items = @($pieces | ForEach-Object { NewSetItem $family $rarity $_ })
        $set = NewSetInfo $family $rarity $pieces

        $generated["$($rarity.Key)Items"] += $items
        $generated["$($rarity.Key)Sets"] += $set
    }
}

ApplyForcedPrefabRequirements
ApplyNottStealthBonusOverrides
ApplyMoonveinAmmoConservationOverrides
ApplyFrostbrandRunebladeEitrOverride
ApplyMagicRagnarSurvivalOverride
ApplyRagnarLifeStealOverrides
ApplyRagnarAttackHealthUseOverrides
ApplyHelveigBloodstaffHealthUseOverrides
ApplyFrostDamageAoePairing

$thorPieces = @(
    (Piece "Weapon" "ReturningAxe" "Returning Axe" "OneHandedWeapon" @("Throwable", "RecallWeapon", "AddLightningDamage", "ChainLightning") @("Axes") @("AxeJotunBane", "AxeBlackMetal", "AxeIron")),
    (Piece "Helmet" "StormHelm" "Storm Helm" "Helmet" @("AddLightningResistancePercentage", "IncreaseStamina", "ModifyArmor", "Luck") @() @("HelmetCarapace", "HelmetPadded", "HelmetDrake")),
    (Piece "Chest" "ThunderHarness" "Thunder Harness" "Chest" @("ModifyArmor", "IncreaseHealth", "AddLightningResistancePercentage", "ModifyStaminaRegen") @() @("ArmorCarapaceChest", "ArmorPaddedCuirass", "ArmorIronChest")),
    (Piece "Legs" "Stormstride" "Stormstride" "Legs" @("ModifyMovementSpeed", "ModifySprintStaminaUse", "AddLightningResistancePercentage", "ModifyDodgeStaminaUse") @() @("ArmorCarapaceLegs", "ArmorPaddedGreaves", "ArmorIronLegs")),
    (Piece "Shoulder" "StormMantle" "Storm Mantle" "Shoulder" @("AddLightningResistancePercentage", "IncreaseStamina", "RemoveSpeedPenalty", "Indestructible") @() @("CapeLox", "CapeFeather", "CapeWolf"))
)

$thor = Family "Thor" $false "an axe-throwing storm set built around recall and lightning damage" $thorPieces @("AddAxesSkill", "AddLightningDamage", "ChainLightning", "ModifyPhysicalDamage") @{
    Epic = "Thunder-Return Pact"
}

$epicRarity = $rarities | Where-Object { $_.Key -eq "Epic" }
$generated.EpicItems += @($thorPieces | ForEach-Object { NewSetItem $thor $epicRarity $_ })
$generated.EpicSets += NewSetInfo $thor $epicRarity $thorPieces

$flokiPieces = @(
    (Piece "Weapon" "ShipwrightHammer" "Shipwright Hammer" "Tool" @("FreeBuild", "ModifyBuildDistance", "Indestructible", "Weightless") @() @("Hammer", '$item_hammer')),
    (Piece "Chest" "TarredHarness" "Tarred Harness" "Chest" @("AddCrafterSkills", "AddCarryWeight", "IncreaseStamina", "ModifyStaminaRegen") @() @("ArmorPaddedCuirass", "ArmorCarapaceChest", "ArmorIronChest")),
    (Piece "Legs" "Dockstride" "Dockstride" "Legs" @("ModifyMovementSpeed", "ModifySprintStaminaUse", "ModifyJumpStaminaUse", "IncreaseStamina") @() @("ArmorPaddedGreaves", "ArmorCarapaceLegs", "ArmorIronLegs")),
    (Piece "Shoulder" "SailclothMantle" "Sailcloth Mantle" "Shoulder" @("AddCarryWeight", "AddCrafterSkills", "Weightless", "Indestructible") @() @("CapeLox", "CapeFeather", "CapeWolf")),
    (Piece "Utility" "BuilderBelt" "Builder Belt" "Utility" @("AddCarryWeight", "ModifyBuildDistance", "IncreaseStamina", "ModifyStaminaRegen") @() $utilityItemNames)
)

$floki = Family "Floki" $false "a shipwright construction set built around free building, build reach, stamina and durable tools" $flokiPieces @("ModifyBuildDistance", "AddCarryWeight", "IncreaseStamina", "FreeBuild") @{
    Epic = "Floki's Shipyard"
}

$generated.EpicItems += @($flokiPieces | ForEach-Object { NewSetItem $floki $epicRarity $_ })
$generated.EpicSets += NewSetInfo $floki $epicRarity $flokiPieces

$raritySetsConfig = Obj ([ordered] @{
    MagicItems = @($generated.MagicItems)
    MagicSets = @($generated.MagicSets)
    RareItems = @($generated.RareItems)
    RareSets = @($generated.RareSets)
    EpicItems = @($generated.EpicItems)
    EpicSets = @($generated.EpicSets)
    AncientItems = @($generated.AncientItems)
    AncientSets = @($generated.AncientSets)
})

$legendaryConfig = Get-Content $legendaryPath -Raw | ConvertFrom-Json
$familyPattern = "^(Mythic)?(Heimdall|Ragnar|Hraesvelgr|Hellsyng|Nott|Seidr|Helveig|Moonvein|Frostbrand)"
$legendarySetIds = @($families | ForEach-Object { $_.ID })
$mythicSetIds = @($families | ForEach-Object { "Mythic$($_.ID)" })

$legendaryConfig.LegendaryItems = @($legendaryConfig.LegendaryItems | Where-Object { $_.ID -notmatch $familyPattern }) + @($generated.LegendaryItems)
$legendaryConfig.LegendarySets = @($legendaryConfig.LegendarySets | Where-Object { $legendarySetIds -notcontains $_.ID }) + @($generated.LegendarySets)
$legendaryConfig.MythicItems = @($legendaryConfig.MythicItems | Where-Object { $_.ID -notmatch $familyPattern }) + @($generated.MythicItems)
$legendaryConfig.MythicSets = @($legendaryConfig.MythicSets | Where-Object { $mythicSetIds -notcontains $_.ID }) + @($generated.MythicSets)

$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
[System.IO.File]::WriteAllText($raritySetsPath, (($raritySetsConfig | ConvertTo-Json -Depth 40) + [Environment]::NewLine), $utf8NoBom)
[System.IO.File]::WriteAllText($legendaryPath, (($legendaryConfig | ConvertTo-Json -Depth 40) + [Environment]::NewLine), $utf8NoBom)
[System.IO.File]::WriteAllText($magicEffectsPath, (($magicEffectsConfig | ConvertTo-Json -Depth 40) + [Environment]::NewLine), $utf8NoBom)

Write-Host "Generated raritysets.json and refreshed generated sets in legendaries.json."
Write-Host ("Magic: {0} items / {1} sets" -f $generated.MagicItems.Count, $generated.MagicSets.Count)
Write-Host ("Rare: {0} items / {1} sets" -f $generated.RareItems.Count, $generated.RareSets.Count)
Write-Host ("Epic: {0} items / {1} sets" -f $generated.EpicItems.Count, $generated.EpicSets.Count)
Write-Host ("Legendary: {0} items / {1} sets" -f $generated.LegendaryItems.Count, $generated.LegendarySets.Count)
Write-Host ("Mythic: {0} items / {1} sets" -f $generated.MythicItems.Count, $generated.MythicSets.Count)
Write-Host ("Ancient: {0} items / {1} sets" -f $generated.AncientItems.Count, $generated.AncientSets.Count)
