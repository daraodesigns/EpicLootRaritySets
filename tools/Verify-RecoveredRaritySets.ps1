$ErrorActionPreference = 'Stop'

$pluginRoot = Split-Path -Parent $PSScriptRoot
$generatedConfigRoot = Join-Path $pluginRoot 'src\GeneratedConfig'

$dllPath = Join-Path $pluginRoot 'EpicLootRaritySets.dll'
$sourcePath = Join-Path $pluginRoot 'src\EpicLootRaritySetsPlugin.cs'
$raritySetsPath = Join-Path $generatedConfigRoot 'raritysets.json'
$legendariesPath = Join-Path $generatedConfigRoot 'legendaries.json'
$lootTablesPath = Join-Path $generatedConfigRoot 'loottables.json'
$adventureDataPath = Join-Path $generatedConfigRoot 'adventuredata.json'
$bossSetDropsPath = Join-Path $generatedConfigRoot 'bosssetdrops.json'
$generateScriptPath = Join-Path $pluginRoot 'tools\Generate-RarityProgressionSets.ps1'
$lootScriptPath = Join-Path $pluginRoot 'tools\Configure-LootProgression.ps1'

foreach ($path in @($dllPath, $sourcePath, $generateScriptPath, $lootScriptPath, $raritySetsPath, $legendariesPath, $lootTablesPath, $adventureDataPath, $bossSetDropsPath)) {
    if (-not (Test-Path -LiteralPath $path)) {
        throw "Missing required file: $path"
    }
}

foreach ($path in @($generateScriptPath, $lootScriptPath)) {
    $tokens = $null
    $errors = $null
    $null = [System.Management.Automation.Language.Parser]::ParseFile($path, [ref]$tokens, [ref]$errors)
    if ($errors.Count -gt 0) {
        throw "PowerShell syntax check failed for ${path}: $($errors[0].Message)"
    }
}

foreach ($path in @($raritySetsPath, $legendariesPath, $lootTablesPath, $adventureDataPath, $bossSetDropsPath)) {
    $null = Get-Content -LiteralPath $path -Raw | ConvertFrom-Json
    "OK JSON: $path"
}

$raritySets = Get-Content -LiteralPath $raritySetsPath -Raw | ConvertFrom-Json
$legendaries = Get-Content -LiteralPath $legendariesPath -Raw | ConvertFrom-Json
$lootTables = Get-Content -LiteralPath $lootTablesPath -Raw | ConvertFrom-Json
$adventureData = Get-Content -LiteralPath $adventureDataPath -Raw | ConvertFrom-Json
$bossSetDrops = Get-Content -LiteralPath $bossSetDropsPath -Raw | ConvertFrom-Json

$counts = [pscustomobject]@{
    MagicItems = @($raritySets.MagicItems).Count
    MagicSets = @($raritySets.MagicSets).Count
    RareItems = @($raritySets.RareItems).Count
    RareSets = @($raritySets.RareSets).Count
    EpicItems = @($raritySets.EpicItems).Count
    EpicSets = @($raritySets.EpicSets).Count
    LegendaryItems = @($legendaries.LegendaryItems).Count
    LegendarySets = @($legendaries.LegendarySets).Count
    MythicItems = @($legendaries.MythicItems).Count
    MythicSets = @($legendaries.MythicSets).Count
    AncientItems = @($raritySets.AncientItems).Count
    AncientSets = @($raritySets.AncientSets).Count
}

$allConfigText = @(
    Get-Content -LiteralPath $raritySetsPath -Raw
    Get-Content -LiteralPath $legendariesPath -Raw
    Get-Content -LiteralPath $adventureDataPath -Raw
) -join "`n"

if ($allConfigText -match 'MagicSolomonKane|MagicSeidr') {
    throw 'Found stale MagicSolomonKane/MagicSeidr entries.'
}

$moonveinBows = @(
    $raritySets.MagicItems | Where-Object ID -eq 'MagicMoonveinMoonbow'
    $raritySets.RareItems | Where-Object ID -eq 'RareMoonveinMoonbow'
    $raritySets.EpicItems | Where-Object ID -eq 'EpicMoonveinMoonbow'
    $legendaries.LegendaryItems | Where-Object ID -eq 'MoonveinMoonbow'
    $legendaries.MythicItems | Where-Object ID -eq 'MythicMoonveinMoonbow'
    $raritySets.AncientItems | Where-Object ID -eq 'AncientMoonveinMoonbow'
)

foreach ($bow in $moonveinBows) {
    $ammo = @($bow.GuaranteedMagicEffects | Where-Object { $_.Type -eq 'AmmoConservation' }) | Select-Object -First 1
    if (-not $ammo -or -not $ammo.Values -or $ammo.Values.MinValue -ne 100 -or $ammo.Values.MaxValue -ne 100) {
        throw "Moonvein bow missing AmmoConservation 100: $($bow.ID)"
    }
}

$shopCounts = [pscustomobject]@{
    Magic = @($adventureData.SecretStash.OtherItems | Where-Object { $_.Item -like 'Magic*' }).Count
    Rare = @($adventureData.SecretStash.OtherItems | Where-Object { $_.Item -like 'Rare*' }).Count
    Epic = @($adventureData.SecretStash.OtherItems | Where-Object { $_.Item -like 'Epic*' }).Count
}

$source = Get-Content -LiteralPath $sourcePath -Raw
foreach ($needle in @('PluginVersion = "0.1.39"', 'radamanto.Bestiary', 'GeneratedConfigSynchronizer', 'MoonveinBowEitrUse', 'NottAbilityController', 'SolomonKaneAbilityController', 'Bat Form', 'vampireformIV', 'TrySafeWarp', 'NorseNjordTornadoBridge', 'NorseWaterSphereBridge', 'HelveigAbilityController', 'HraesvelgrRapidVolleyEquipmentEffectValuesForPatch', 'WhirlwindMinDuration', 'MaterialManPropertyContainerUpdateBlockPatch', 'itemDrop.m_itemData.m_dropPrefab = template.gameObject')) {
    if ($source -notlike "*$needle*") {
        throw "Source check failed: $needle"
    }
}

$bestiaryPrefabs = @(
    'RDB_Goblin_Treasure'
    'RDB_Goblin_Treasure_blue'
    'RDB_Goblin_Treasure_red'
    'RDB_rabbit'
    'RDB_Bee'
    'RDB_fox'
    'RDB_Ent'
    'RDB_minotaur'
    'RDB_Genganger'
    'RDB_crocodile'
    'RDB_Wendigo'
    'RDB_lizardwarrior'
    'RDB_giant_rat'
    'RDB_Werewolf'
    'RDB_Weregoat'
    'RDB_graywolf'
    'RDB_lion'
    'RDB_Smadrek'
    'RDB_white_shark'
    'RDB_turtle'
    'RDB_Whale'
)

foreach ($prefab in $bestiaryPrefabs) {
    $lootEntries = @($lootTables.LootTables | Where-Object Object -eq $prefab)
    if ($lootEntries.Count -ne 1 -or [string]::IsNullOrWhiteSpace($lootEntries[0].RefObject)) {
        throw "Bestiary loot table missing or invalid: $prefab"
    }

    $bountyEntries = @($adventureData.Bounties.Targets | Where-Object TargetID -eq $prefab)
    if ($bountyEntries.Count -ne 1) {
        throw "Bestiary bounty target missing or duplicated: $prefab"
    }
}

foreach ($bossRule in @($bossSetDrops.Bosses)) {
    if ($bossRule.GuaranteedSetDrops -lt 1) {
        throw "Boss rule is not guaranteed: $($bossRule.Object)"
    }
}

'Recovered EpicLootRaritySets checks passed.'
$counts
$shopCounts
"Boss rules: $(@($bossSetDrops.Bosses).Count)"
"Bestiary integrated prefabs: $($bestiaryPrefabs.Count)"
