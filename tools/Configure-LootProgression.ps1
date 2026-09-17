$ErrorActionPreference = 'Stop'

$bepInExRoot = Resolve-Path (Join-Path $PSScriptRoot '..\..\..')
$lootTablesPath = Join-Path $bepInExRoot 'config\EpicLoot\baseconfig\loottables.json'
$itemInfoPath = Join-Path $bepInExRoot 'config\EpicLoot\baseconfig\iteminfo.json'
$adventureDataPath = Join-Path $bepInExRoot 'config\EpicLoot\baseconfig\adventuredata.json'
$legendaryPath = Join-Path $bepInExRoot 'config\EpicLoot\baseconfig\legendaries.json'
$raritySetsPath = Join-Path $bepInExRoot 'config\EpicLoot\raritysets.json'
$pluginConfigPath = Join-Path $bepInExRoot 'config\fran.mods.epiclootraritysets.cfg'
$bossSetDropsPath = Join-Path $bepInExRoot 'config\EpicLoot\bosssetdrops.json'

function New-LootEntry {
    param(
        [Parameter(Mandatory = $true)][string]$Item,
        [double]$Weight = 1.0,
        $Rarity = $null
    )

    [pscustomobject][ordered]@{
        Item = $Item
        Weight = $Weight
        Rarity = $Rarity
    }
}

function New-WeightedItem {
    param(
        [Parameter(Mandatory = $true)][string]$Item,
        [double]$Weight = 1.0
    )

    [pscustomobject][ordered]@{
        Item = $Item
        Weight = $Weight
    }
}

function New-StoreItem {
    param(
        [Parameter(Mandatory = $true)][string]$Item,
        [int]$CoinsCost,
        [int]$ForestTokenCost = 0,
        [int]$IronBountyTokenCost = 0,
        [int]$GoldBountyTokenCost = 0
    )

    [pscustomobject][ordered]@{
        Item = $Item
        CoinsCost = $CoinsCost
        ForestTokenCost = $ForestTokenCost
        IronBountyTokenCost = $IronBountyTokenCost
        GoldBountyTokenCost = $GoldBountyTokenCost
    }
}

function Set-ItemSet {
    param(
        [Parameter(Mandatory = $true)]$Config,
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)]$Items
    )

    $loot = @(
        foreach ($item in $Items) {
            New-LootEntry -Item $item.Item -Weight $item.Weight -Rarity $null
        }
    )

    $existing = @($Config.ItemSets | Where-Object { $_.Name -eq $Name }) | Select-Object -First 1
    if ($existing) {
        $existing.Loot = $loot
        return
    }

    $set = [pscustomobject][ordered]@{
        Name = $Name
        Loot = $loot
    }

    $Config.ItemSets = @($Config.ItemSets) + $set
}

function Add-ItemsToItemSet {
    param(
        [Parameter(Mandatory = $true)]$Config,
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)]$Items,
        [double[]]$Rarity = @(0.0, 5.0, 33.0, 42.0, 15.0, 5.0)
    )

    $existing = @($Config.ItemSets | Where-Object { $_.Name -eq $Name }) | Select-Object -First 1
    if (-not $existing) {
        throw "Could not find item set $Name"
    }

    $knownItems = New-Object 'System.Collections.Generic.HashSet[string]' ([System.StringComparer]::OrdinalIgnoreCase)
    foreach ($entry in @($existing.Loot)) {
        [void]$knownItems.Add($entry.Item)
    }

    foreach ($item in $Items) {
        if ($knownItems.Contains($item.Item)) {
            continue
        }

        $existing.Loot = @($existing.Loot) + (New-LootEntry -Item $item.Item -Weight $item.Weight -Rarity (Copy-Rarity $Rarity))
        [void]$knownItems.Add($item.Item)
    }
}

function Copy-LootEntry {
    param(
        [Parameter(Mandatory = $true)]$Entry,
        [hashtable]$ItemMap = @{},
        [switch]$NoAncient
    )

    $item = if ($ItemMap.ContainsKey($Entry.Item)) { $ItemMap[$Entry.Item] } else { $Entry.Item }
    $rarity = Copy-Rarity $Entry.Rarity
    if ($NoAncient -and $null -ne $rarity -and @($rarity).Count -ge 6) {
        $rarity[5] = 0.0
    }

    New-LootEntry -Item $item -Weight $Entry.Weight -Rarity $rarity
}

function Set-NoAncientTier7ItemSets {
    param([Parameter(Mandatory = $true)]$Config)

    $map = @{
        Tier7Weapons = 'FranTier7WeaponsNoAncient'
        Tier7Armor = 'FranTier7ArmorNoAncient'
        Tier7Shields = 'FranTier7ShieldsNoAncient'
        Tier7Trinkets = 'FranTier7TrinketsNoAncient'
    }

    foreach ($sourceName in @('Tier7Weapons', 'Tier7Armor', 'Tier7Shields', 'Tier7Trinkets')) {
        $source = @($Config.ItemSets | Where-Object { $_.Name -eq $sourceName }) | Select-Object -First 1
        if (-not $source) {
            throw "Could not find item set $sourceName"
        }

        $targetName = $map[$sourceName]
        $loot = @($source.Loot | ForEach-Object { Copy-LootEntry -Entry $_ -NoAncient })
        $existing = @($Config.ItemSets | Where-Object { $_.Name -eq $targetName }) | Select-Object -First 1

        if ($existing) {
            $existing.Loot = $loot
        }
        else {
            $Config.ItemSets = @($Config.ItemSets) + ([pscustomobject][ordered]@{
                Name = $targetName
                Loot = $loot
            })
        }
    }

    $everything = @($Config.ItemSets | Where-Object { $_.Name -eq 'Tier7Everything' }) | Select-Object -First 1
    if (-not $everything) {
        throw 'Could not find item set Tier7Everything'
    }

    $everythingLoot = @($everything.Loot | ForEach-Object { Copy-LootEntry -Entry $_ -ItemMap $map -NoAncient })
    $existingEverything = @($Config.ItemSets | Where-Object { $_.Name -eq 'FranTier7EverythingNoAncient' }) | Select-Object -First 1

    if ($existingEverything) {
        $existingEverything.Loot = $everythingLoot
    }
    else {
        $Config.ItemSets = @($Config.ItemSets) + ([pscustomobject][ordered]@{
            Name = 'FranTier7EverythingNoAncient'
            Loot = $everythingLoot
        })
    }
}

function Copy-Rarity {
    param($Rarity)

    if ($null -eq $Rarity) {
        return $null
    }

    return @($Rarity | ForEach-Object { [double]$_ })
}

function Get-RarityLock {
    param([Parameter(Mandatory = $true)][string]$Rarity)

    switch ($Rarity) {
        'Magic' { return @(100.0, 0.0, 0.0, 0.0, 0.0, 0.0) }
        'Rare' { return @(0.0, 100.0, 0.0, 0.0, 0.0, 0.0) }
        'Epic' { return @(0.0, 0.0, 100.0, 0.0, 0.0, 0.0) }
        'Legendary' { return @(0.0, 0.0, 0.0, 100.0, 0.0, 0.0) }
        'Mythic' { return @(0.0, 0.0, 0.0, 0.0, 100.0, 0.0) }
        'Ancient' { return @(0.0, 0.0, 0.0, 0.0, 0.0, 100.0) }
        default { throw "Unknown rarity $Rarity" }
    }
}

function Get-GeneratedSetItems {
    param(
        [Parameter(Mandatory = $true)][string]$Rarity,
        [Parameter(Mandatory = $true)]$RaritySetsConfig,
        [Parameter(Mandatory = $true)]$LegendaryConfig
    )

    if ($Rarity -in @('Legendary', 'Mythic')) {
        $bucketName = "$($Rarity)Items"
        $items = @($LegendaryConfig.$bucketName)
        $familyPattern = if ($Rarity -eq 'Mythic') {
            '^(Mythic)(Heimdall|Ragnar|Hraesvelgr|Hellsyng|Nott|Seidr|Helveig|Moonvein|Frostbrand)'
        }
        else {
            '^(Heimdall|Ragnar|Hraesvelgr|Hellsyng|Nott|Seidr|Helveig|Moonvein|Frostbrand)'
        }

        return @($items | Where-Object { $_.IsSetItem -eq $true -and $_.ID -match $familyPattern })
    }

    $bucketName = "$($Rarity)Items"
    if (-not $RaritySetsConfig.PSObject.Properties[$bucketName]) {
        return @()
    }

    return @($RaritySetsConfig.$bucketName | Where-Object { $_.IsSetItem -eq $true })
}

function Get-RaritySetBaseItems {
    param(
        [Parameter(Mandatory = $true)][string]$Rarity,
        [Parameter(Mandatory = $true)]$RaritySetsConfig,
        [Parameter(Mandatory = $true)]$LegendaryConfig
    )

    $weights = @{}
    foreach ($item in @(Get-GeneratedSetItems $Rarity $RaritySetsConfig $LegendaryConfig)) {
        if ($null -eq $item.Requirements -or -not $item.Requirements.PSObject.Properties['AllowedItemNames']) {
            continue
        }

        $baseItems = @(
            $item.Requirements.AllowedItemNames |
                Where-Object { -not [string]::IsNullOrWhiteSpace($_) -and -not $_.StartsWith('$') } |
                Sort-Object -Unique
        )

        if ($baseItems.Count -eq 0) {
            continue
        }

        $share = 1.0 / [double]$baseItems.Count
        foreach ($baseItem in $baseItems) {
            if (-not $weights.ContainsKey($baseItem)) {
                $weights[$baseItem] = 0.0
            }

            $weights[$baseItem] += $share
        }
    }

    return @(
        $weights.GetEnumerator() |
            Sort-Object Name |
            ForEach-Object { New-WeightedItem $_.Key ([math]::Round([double]$_.Value, 3)) }
    )
}

function Set-RaritySetEquipmentPools {
    param(
        [Parameter(Mandatory = $true)]$Config,
        [Parameter(Mandatory = $true)]$RaritySetsConfig,
        [Parameter(Mandatory = $true)]$LegendaryConfig
    )

    foreach ($rarity in @('Magic', 'Rare', 'Epic', 'Legendary', 'Mythic', 'Ancient')) {
        $baseItems = @(Get-RaritySetBaseItems $rarity $RaritySetsConfig $LegendaryConfig)
        if ($baseItems.Count -eq 0) {
            throw "No base items found for $rarity set equipment."
        }

        Set-ItemSet $Config "Fran$($rarity)SetEquipment" $baseItems
    }
}

function Add-BossPool {
    param(
        [Parameter(Mandatory = $true)]$Config,
        [Parameter(Mandatory = $true)][string]$Object,
        [Parameter(Mandatory = $true)][string]$Pool,
        [double]$Weight,
        [double[]]$Rarity = $null
    )

    $table = @(
        $Config.LootTables |
            Where-Object { $_.Object -eq $Object -and $null -ne $_.LeveledLoot -and @($_.LeveledLoot).Count -gt 1 }
    ) | Select-Object -First 1

    if (-not $table) {
        throw "Could not find primary loot table for $Object"
    }

    foreach ($level in @($table.LeveledLoot)) {
        $raritySource = @($level.Loot | Where-Object { $null -ne $_.Rarity }) | Select-Object -First 1
        $rarityToUse = if ($null -ne $Rarity) { Copy-Rarity $Rarity } elseif ($raritySource) { Copy-Rarity $raritySource.Rarity } else { $null }
        $existing = @($level.Loot | Where-Object { $_.Item -eq $Pool }) | Select-Object -First 1

        if ($existing) {
            $existing.Weight = $Weight
            $existing.Rarity = $rarityToUse
        }
        else {
            $level.Loot = @($level.Loot) + (New-LootEntry -Item $Pool -Weight $Weight -Rarity $rarityToUse)
        }
    }
}

function New-BossSetProgressionGuarantee {
    param(
        [Parameter(Mandatory = $true)][string]$Object,
        [Parameter(Mandatory = $true)][string]$Pool,
        [Parameter(Mandatory = $true)][double[]]$Rarity,
        [int]$GuaranteedDrops = 1
    )

    $levels = @(
        for ($level = 1; $level -le 6; $level++) {
            [pscustomobject][ordered]@{
                Level = $level
                Drops = @(, @([double]$GuaranteedDrops, 100.0))
                Loot = @(
                    New-LootEntry -Item $Pool -Weight 1.0 -Rarity $Rarity
                )
            }
        }
    )

    [pscustomobject][ordered]@{
        Object = $Object
        RefObject = $null
        Drops = $null
        Loot = @()
        LeveledLoot = $levels
    }
}

function Set-BossSetProgressionGuarantee {
    param(
        [Parameter(Mandatory = $true)]$Config,
        [Parameter(Mandatory = $true)][string]$Object,
        [Parameter(Mandatory = $true)][string]$Pool,
        [Parameter(Mandatory = $true)][double[]]$Rarity,
        [int]$GuaranteedDrops = 1
    )

    $existing = @(
        $Config.LootTables |
            Where-Object {
                $_.Object -eq $Object -and
                $null -ne $_.LeveledLoot -and
                (@($_.LeveledLoot) | Where-Object {
                    @($_.Loot | Where-Object { $_.Item -eq $Pool }).Count -gt 0
                }).Count -gt 0
            }
    ) | Select-Object -First 1

    $replacement = New-BossSetProgressionGuarantee $Object $Pool $Rarity $GuaranteedDrops
    if ($existing) {
        $existing.RefObject = $replacement.RefObject
        $existing.Drops = $replacement.Drops
        $existing.Loot = $replacement.Loot
        $existing.LeveledLoot = $replacement.LeveledLoot
    }
    else {
        $Config.LootTables = @($Config.LootTables) + $replacement
    }
}

function Set-BossSetProgressionGuarantees {
    param([Parameter(Mandatory = $true)]$Config)

    $rules = @(
        [pscustomobject]@{ Object = 'Eikthyr'; Pool = 'FranMagicSetEquipment'; Rarity = Get-RarityLock 'Magic'; Drops = 1 }
        [pscustomobject]@{ Object = 'gd_king'; Pool = 'FranRareSetEquipment'; Rarity = Get-RarityLock 'Rare'; Drops = 2 }
        [pscustomobject]@{ Object = 'Bonemass'; Pool = 'FranEpicSetEquipment'; Rarity = Get-RarityLock 'Epic'; Drops = 2 }
        [pscustomobject]@{ Object = 'Dragon'; Pool = 'FranLegendarySetEquipment'; Rarity = Get-RarityLock 'Legendary'; Drops = 3 }
        [pscustomobject]@{ Object = 'GoblinKing'; Pool = 'FranMythicSetEquipment'; Rarity = Get-RarityLock 'Mythic'; Drops = 3 }
        [pscustomobject]@{ Object = 'SeekerQueen'; Pool = 'FranMythicSetEquipment'; Rarity = Get-RarityLock 'Mythic'; Drops = 4 }
        [pscustomobject]@{ Object = 'Fader'; Pool = 'FranAncientSetEquipment'; Rarity = Get-RarityLock 'Ancient'; Drops = 4 }
        [pscustomobject]@{ Object = 'FrozenKing_p3'; Pool = 'FranAncientSetEquipment'; Rarity = Get-RarityLock 'Ancient'; Drops = 5 }
    )

    foreach ($rule in $rules) {
        Set-BossSetProgressionGuarantee $Config $rule.Object $rule.Pool $rule.Rarity $rule.Drops
    }
}

function New-BossRunestoneGuarantee {
    param(
        [Parameter(Mandatory = $true)][string]$Object,
        [Parameter(Mandatory = $true)][string]$Runestone,
        [int]$GuaranteedDrops = 1
    )

    $levels = @(
        for ($level = 1; $level -le 6; $level++) {
            [pscustomobject][ordered]@{
                Level = $level
                Drops = @(, @([double]$GuaranteedDrops, 100.0))
                Loot = @(
                    New-LootEntry -Item $Runestone -Weight 1.0 -Rarity $null
                )
            }
        }
    )

    [pscustomobject][ordered]@{
        Object = $Object
        RefObject = $null
        Drops = $null
        Loot = @()
        LeveledLoot = $levels
    }
}

function Set-BossRunestoneGuarantee {
    param(
        [Parameter(Mandatory = $true)]$Config,
        [Parameter(Mandatory = $true)][string]$Object,
        [Parameter(Mandatory = $true)][string]$Runestone,
        [int]$GuaranteedDrops = 1
    )

    $existing = @(
        $Config.LootTables |
            Where-Object {
                $_.Object -eq $Object -and
                $null -ne $_.LeveledLoot -and
                (@($_.LeveledLoot) | Where-Object {
                    @($_.Loot | Where-Object { $_.Item -eq $Runestone }).Count -gt 0
                }).Count -gt 0
            }
    ) | Select-Object -First 1

    $replacement = New-BossRunestoneGuarantee $Object $Runestone $GuaranteedDrops
    if ($existing) {
        $existing.RefObject = $replacement.RefObject
        $existing.Drops = $replacement.Drops
        $existing.Loot = $replacement.Loot
        $existing.LeveledLoot = $replacement.LeveledLoot
    }
    else {
        $Config.LootTables = @($Config.LootTables) + $replacement
    }
}

function Set-BossRunestoneGuarantees {
    param([Parameter(Mandatory = $true)]$Config)

    $rules = @(
        [pscustomobject]@{ Object = 'Eikthyr'; Runestone = 'RunestoneMagic'; Drops = 1 }
        [pscustomobject]@{ Object = 'gd_king'; Runestone = 'RunestoneRare'; Drops = 1 }
        [pscustomobject]@{ Object = 'Bonemass'; Runestone = 'RunestoneEpic'; Drops = 1 }
        [pscustomobject]@{ Object = 'Dragon'; Runestone = 'RunestoneLegendary'; Drops = 1 }
        [pscustomobject]@{ Object = 'GoblinKing'; Runestone = 'RunestoneMythic'; Drops = 1 }
        [pscustomobject]@{ Object = 'SeekerQueen'; Runestone = 'RunestoneMythic'; Drops = 1 }
        [pscustomobject]@{ Object = 'Fader'; Runestone = 'RunestoneAncient'; Drops = 1 }
        [pscustomobject]@{ Object = 'FrozenKing_p3'; Runestone = 'RunestoneAncient'; Drops = 1 }
    )

    foreach ($rule in $rules) {
        Set-BossRunestoneGuarantee $Config $rule.Object $rule.Runestone $rule.Drops
    }
}

function Limit-AncientLootToFinalBosses {
    param([Parameter(Mandatory = $true)]$Config)

    $allowedObjects = @('Fader', 'FrozenKing_p3')
    $noAncientItemSets = @{
        Tier7Everything = 'FranTier7EverythingNoAncient'
        Tier7Weapons = 'FranTier7WeaponsNoAncient'
        Tier7Armor = 'FranTier7ArmorNoAncient'
        Tier7Shields = 'FranTier7ShieldsNoAncient'
        Tier7Trinkets = 'FranTier7TrinketsNoAncient'
    }

    foreach ($table in @($Config.LootTables)) {
        $allowAncient = $allowedObjects -contains $table.Object
        $lootGroups = @()

        if ($null -ne $table.Loot) {
            $lootGroups += , @($table.Loot)
        }

        foreach ($level in @($table.LeveledLoot)) {
            if ($null -ne $level.Loot) {
                $lootGroups += , @($level.Loot)
            }
        }

        foreach ($lootGroup in $lootGroups) {
            foreach ($entry in @($lootGroup)) {
                if (-not $allowAncient -and $noAncientItemSets.ContainsKey($entry.Item)) {
                    $entry.Item = $noAncientItemSets[$entry.Item]
                }

                if (-not $allowAncient -and $null -ne $entry.Rarity -and @($entry.Rarity).Count -ge 6) {
                    $entry.Rarity[5] = 0.0
                }

                if (-not $allowAncient -and $null -ne $entry.RarityItems -and ($entry.RarityItems.PSObject.Properties.Name -contains 'Ancient')) {
                    $entry.RarityItems.PSObject.Properties.Remove('Ancient')
                }
            }
        }
    }
}

function Set-BossRarityWeights {
    param([Parameter(Mandatory = $true)]$Config)

    $raritiesByBoss = @{
        Fader = @(0.0, 0.0, 0.0, 0.0, 50.0, 50.0)
        FrozenKing_p3 = @(0.0, 0.0, 0.0, 0.0, 0.0, 100.0)
    }

    foreach ($table in @($Config.LootTables)) {
        if (-not $raritiesByBoss.ContainsKey($table.Object)) {
            continue
        }

        $rarity = $raritiesByBoss[$table.Object]
        foreach ($entry in @($table.Loot)) {
            if ($null -ne $entry.Rarity) {
                $entry.Rarity = Copy-Rarity $rarity
            }
        }

        foreach ($level in @($table.LeveledLoot)) {
            foreach ($entry in @($level.Loot)) {
                if ($null -ne $entry.Rarity) {
                    $entry.Rarity = Copy-Rarity $rarity
                }
            }
        }
    }
}

function Set-StaffUnlockProgression {
    param([Parameter(Mandatory = $true)]$Config)

    $staffs = @($Config.ItemInfo | Where-Object { $_.Type -eq 'Staffs' }) | Select-Object -First 1
    if (-not $staffs) {
        throw 'Could not find Staffs entry in iteminfo.json'
    }

    $staffs.ItemFallback = 'StaffFireball'
    $staffs.ItemsByBoss.none = @()
    $staffs.ItemsByBoss.defeated_eikthyr = @('StaffSkeleton', 'StaffFireball', 'StaffIceShards', 'StaffShield')
    $staffs.ItemsByBoss.defeated_gdking = @('StaffSkeleton', 'StaffFireball', 'StaffIceShards', 'StaffLightning', 'StaffShield')
    $staffs.ItemsByBoss.defeated_bonemass = @('StaffSkeleton', 'StaffFireball', 'StaffIceShards', 'StaffLightning', 'StaffShield', 'StaffClusterbomb')
    $staffs.ItemsByBoss.defeated_dragon = @('StaffSkeleton', 'StaffFireball', 'StaffIceShards', 'StaffLightning', 'StaffShield', 'StaffClusterbomb', 'StaffGreenRoots', 'StaffRedTroll')
    $staffs.ItemsByBoss.defeated_goblinking = @('StaffSkeleton', 'StaffFireball', 'StaffIceShards', 'StaffLightning', 'StaffShield', 'StaffClusterbomb', 'StaffGreenRoots', 'StaffRedTroll')
    $staffs.ItemsByBoss.defeated_queen = @('StaffSkeleton', 'StaffFireball', 'StaffIceShards', 'StaffLightning', 'StaffShield', 'StaffClusterbomb', 'StaffGreenRoots', 'StaffRedTroll')
    $staffs.ItemsByBoss.defeated_fader = @('StaffLightning', 'StaffGreenRoots', 'StaffRedTroll', 'StaffSkeleton', 'StaffFireball', 'StaffIceShards', 'StaffShield', 'StaffClusterbomb', 'StaffOrbofAhri', 'StaffFrostOrbs', 'StaffThunderBlood', 'StaffSpiritCaller')
    $staffs.ItemsByBoss.defeated_frozenking_p3 = @('StaffLightning', 'StaffGreenRoots', 'StaffRedTroll', 'StaffSkeleton', 'StaffFireball', 'StaffIceShards', 'StaffShield', 'StaffClusterbomb', 'StaffOrbofAhri', 'StaffFrostOrbs', 'StaffThunderBlood', 'StaffSpiritCaller')
}

function Set-ItemsByBossEntry {
    param(
        [Parameter(Mandatory = $true)]$ItemInfo,
        [Parameter(Mandatory = $true)][string]$BossKey,
        [string[]]$Items = @()
    )

    if ($ItemInfo.ItemsByBoss.PSObject.Properties.Name -contains $BossKey) {
        $ItemInfo.ItemsByBoss.$BossKey = @($Items)
        return
    }

    $ItemInfo.ItemsByBoss | Add-Member -NotePropertyName $BossKey -NotePropertyValue @($Items)
}

function Add-ItemsByBossEntry {
    param(
        [Parameter(Mandatory = $true)]$ItemInfo,
        [Parameter(Mandatory = $true)][string]$BossKey,
        [string[]]$Items = @()
    )

    $current = @()
    if ($ItemInfo.ItemsByBoss.PSObject.Properties.Name -contains $BossKey) {
        $current = @($ItemInfo.ItemsByBoss.$BossKey)
    }

    foreach ($item in $Items) {
        if ($current -notcontains $item) {
            $current += $item
        }
    }

    Set-ItemsByBossEntry $ItemInfo $BossKey $current
}

function Add-LoxArmorUnlockProgression {
    param([Parameter(Mandatory = $true)]$Config)

    $chest = @($Config.ItemInfo | Where-Object { $_.Type -eq 'ChestArmor' }) | Select-Object -First 1
    $legs = @($Config.ItemInfo | Where-Object { $_.Type -eq 'LegsArmor' }) | Select-Object -First 1
    $head = @($Config.ItemInfo | Where-Object { $_.Type -eq 'HeadArmor' }) | Select-Object -First 1
    if (-not $chest -or -not $legs -or -not $head) {
        throw 'Could not find armor entries in iteminfo.json'
    }

    foreach ($bossKey in @('defeated_fader', 'defeated_frozenking_p3')) {
        Add-ItemsByBossEntry $chest $bossKey @('ArmorLoxChest')
        Add-ItemsByBossEntry $legs $bossKey @('ArmorLoxLegs')
        Add-ItemsByBossEntry $head $bossKey @('HelmetLox')
    }
}

function Set-UtilityUnlockProgression {
    param([Parameter(Mandatory = $true)]$Config)

    $utility = @($Config.ItemInfo | Where-Object { $_.Type -eq 'Utility' }) | Select-Object -First 1
    if (-not $utility) {
        throw 'Could not find Utility entry in iteminfo.json'
    }

    $utility.ItemFallback = 'BeltStrength'
    Set-ItemsByBossEntry $utility 'none' @()
    Set-ItemsByBossEntry $utility 'defeated_eikthyr' @('LeatherBelt')
    Set-ItemsByBossEntry $utility 'defeated_gdking' @('GoldRubyRing', 'LeatherBelt', 'BeltStrength')
    Set-ItemsByBossEntry $utility 'defeated_bonemass' @('GoldRubyRing', 'LeatherBelt', 'BeltStrength')
    Set-ItemsByBossEntry $utility 'defeated_dragon' @('SilverRing', 'BeltStrength')
    Set-ItemsByBossEntry $utility 'defeated_goblinking' @('SilverRing', 'BeltStrength')
    Set-ItemsByBossEntry $utility 'defeated_queen' @('BeltStrength', 'Demister')
    Set-ItemsByBossEntry $utility 'defeated_fader' @('BeltStrength', 'Demister')
    Set-ItemsByBossEntry $utility 'defeated_frozenking_p3' @('BeltStrength', 'Demister')
}

function Write-JsonFile {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)]$Value
    )

    $json = $Value | ConvertTo-Json -Depth 100
    [System.IO.File]::WriteAllText($Path, $json + [Environment]::NewLine, [System.Text.UTF8Encoding]::new($false))
}

function Get-ForceSetDropItems {
    param(
        [Parameter(Mandatory = $true)]$Config,
        [Parameter(Mandatory = $true)][string[]]$Pools
    )

    $items = New-Object 'System.Collections.Generic.HashSet[string]' ([System.StringComparer]::OrdinalIgnoreCase)
    foreach ($pool in $Pools) {
        [void]$items.Add($pool)
        $itemSet = @($Config.ItemSets | Where-Object { $_.Name -eq $pool }) | Select-Object -First 1
        if (-not $itemSet) {
            throw "Could not find item set $pool"
        }

        foreach ($entry in @($itemSet.Loot)) {
            [void]$items.Add($entry.Item)
        }
    }

    return @($items | Sort-Object)
}

function Get-ForceSetItemIds {
    param(
        [Parameter(Mandatory = $true)][string]$Rarity,
        [Parameter(Mandatory = $true)]$RaritySetsConfig,
        [Parameter(Mandatory = $true)]$LegendaryConfig
    )

    return @(
        Get-GeneratedSetItems $Rarity $RaritySetsConfig $LegendaryConfig |
            Where-Object { $_.IsSetItem -eq $true } |
            ForEach-Object { $_.ID } |
            Sort-Object -Unique
    )
}

function Write-BossSetDropRules {
    param(
        [Parameter(Mandatory = $true)]$Config,
        [Parameter(Mandatory = $true)]$RaritySetsConfig,
        [Parameter(Mandatory = $true)]$LegendaryConfig
    )

    $bossRules = @(
        [pscustomobject]@{ Object = 'Eikthyr'; Rarity = 'Magic'; Pool = 'FranMagicSetEquipment' }
        [pscustomobject]@{ Object = 'gd_king'; Rarity = 'Rare'; Pool = 'FranRareSetEquipment' }
        [pscustomobject]@{ Object = 'Bonemass'; Rarity = 'Epic'; Pool = 'FranEpicSetEquipment' }
        [pscustomobject]@{ Object = 'Dragon'; Rarity = 'Legendary'; Pool = 'FranLegendarySetEquipment' }
        [pscustomobject]@{ Object = 'GoblinKing'; Rarity = 'Mythic'; Pool = 'FranMythicSetEquipment' }
        [pscustomobject]@{ Object = 'SeekerQueen'; Rarity = 'Mythic'; Pool = 'FranMythicSetEquipment' }
        [pscustomobject]@{ Object = 'Fader'; Rarity = 'Ancient'; Pool = 'FranAncientSetEquipment' }
        [pscustomobject]@{ Object = 'FrozenKing_p3'; Rarity = 'Ancient'; Pool = 'FranAncientSetEquipment' }
    )

    $rules = [pscustomobject][ordered]@{
        Enabled = $true
        Bosses = @(
            foreach ($rule in $bossRules) {
                [pscustomobject][ordered]@{
                    Object = $rule.Object
                    Enabled = $true
                    Rarity = $rule.Rarity
                    GuaranteedSetDrops = 1
                    ExtraSetDropChance = 1.0
                    ForceSetDropItems = Get-ForceSetDropItems $Config @($rule.Pool)
                    ForceSetItemIds = Get-ForceSetItemIds $rule.Rarity $RaritySetsConfig $LegendaryConfig
                }
            }
        )
    }

    Write-JsonFile $bossSetDropsPath $rules
}

function New-LootTableReference {
    param(
        [Parameter(Mandatory = $true)][string]$Object,
        [Parameter(Mandatory = $true)][string]$RefObject
    )

    [pscustomobject][ordered]@{
        Object = $Object
        RefObject = $RefObject
        Drops = $null
        Loot = @()
        LeveledLoot = @()
    }
}

function Set-BestiaryLootIntegration {
    param([Parameter(Mandatory = $true)]$Config)

    $entries = @(
        [pscustomobject]@{ Object = 'RDB_Goblin_Treasure'; RefObject = 'Tier2Mob' }
        [pscustomobject]@{ Object = 'RDB_Goblin_Treasure_blue'; RefObject = 'Tier5Mob' }
        [pscustomobject]@{ Object = 'RDB_Goblin_Treasure_red'; RefObject = 'Tier7Mob' }
        [pscustomobject]@{ Object = 'RDB_rabbit'; RefObject = 'Tier0Mob' }
        [pscustomobject]@{ Object = 'RDB_Bee'; RefObject = 'Tier0Mob' }
        [pscustomobject]@{ Object = 'RDB_fox'; RefObject = 'Tier0Mob' }
        [pscustomobject]@{ Object = 'RDB_Ent'; RefObject = 'Tier3EliteMob' }
        [pscustomobject]@{ Object = 'RDB_minotaur'; RefObject = 'Tier3EliteMob' }
        [pscustomobject]@{ Object = 'RDB_Genganger'; RefObject = 'Tier2Mob' }
        [pscustomobject]@{ Object = 'RDB_crocodile'; RefObject = 'Tier2Mob' }
        [pscustomobject]@{ Object = 'RDB_Wendigo'; RefObject = 'Tier4Mob' }
        [pscustomobject]@{ Object = 'RDB_lizardwarrior'; RefObject = 'Tier4Mob' }
        [pscustomobject]@{ Object = 'RDB_giant_rat'; RefObject = 'Tier3Mob' }
        [pscustomobject]@{ Object = 'RDB_Werewolf'; RefObject = 'Tier5Mob' }
        [pscustomobject]@{ Object = 'RDB_Weregoat'; RefObject = 'Tier4Mob' }
        [pscustomobject]@{ Object = 'RDB_graywolf'; RefObject = 'Tier5Mob' }
        [pscustomobject]@{ Object = 'RDB_lion'; RefObject = 'Tier5Mob' }
        [pscustomobject]@{ Object = 'RDB_Smadrek'; RefObject = 'Tier7Mob' }
        [pscustomobject]@{ Object = 'RDB_white_shark'; RefObject = 'Tier5Mob' }
        [pscustomobject]@{ Object = 'RDB_turtle'; RefObject = 'Tier4Mob' }
        [pscustomobject]@{ Object = 'RDB_Whale'; RefObject = 'Tier5EliteMob' }
    )

    foreach ($entry in $entries) {
        $existing = @($Config.LootTables | Where-Object { $_.Object -eq $entry.Object }) | Select-Object -First 1
        $replacement = New-LootTableReference $entry.Object $entry.RefObject
        if ($existing) {
            $existing.RefObject = $replacement.RefObject
            $existing.Drops = $replacement.Drops
            $existing.Loot = $replacement.Loot
            $existing.LeveledLoot = $replacement.LeveledLoot
        }
        else {
            $Config.LootTables = @($Config.LootTables) + $replacement
        }
    }
}

function New-BountyAdd {
    param(
        [Parameter(Mandatory = $true)][string]$ID,
        [Parameter(Mandatory = $true)][int]$Count
    )

    [pscustomobject][ordered]@{
        ID = $ID
        Count = $Count
    }
}

function Add-BountyTarget {
    param(
        [Parameter(Mandatory = $true)]$Config,
        [Parameter(Mandatory = $true)][string]$Biome,
        [Parameter(Mandatory = $true)][string]$TargetID,
        [int]$RewardGold = 0,
        [int]$RewardIron = 0,
        [int]$RewardCoins = 0,
        $Adds = @()
    )

    $addList = @(
        foreach ($add in @($Adds)) {
            New-BountyAdd $add.ID $add.Count
        }
    )

    $replacement = [pscustomobject][ordered]@{
        Biome = $Biome
        TargetID = $TargetID
        RewardGold = $RewardGold
        RewardIron = $RewardIron
        RewardCoins = $RewardCoins
        Adds = $addList
    }

    $existing = @($Config.Bounties.Targets | Where-Object { $_.Biome -eq $Biome -and $_.TargetID -eq $TargetID }) | Select-Object -First 1
    if ($existing) {
        $existing.RewardGold = $replacement.RewardGold
        $existing.RewardIron = $replacement.RewardIron
        $existing.RewardCoins = $replacement.RewardCoins
        $existing.Adds = $replacement.Adds
    }
    else {
        $Config.Bounties.Targets = @($Config.Bounties.Targets) + $replacement
    }
}

function Set-BestiaryBountyIntegration {
    param([Parameter(Mandatory = $true)]$Config)

    if ($null -eq $Config.Bounties -or -not $Config.Bounties.PSObject.Properties['Targets']) {
        throw 'Could not find Bounties.Targets in adventuredata.json'
    }

    Add-BountyTarget $Config 'Meadows' 'RDB_rabbit' 0 1 25
    Add-BountyTarget $Config 'Meadows' 'RDB_Bee' 0 1 30
    Add-BountyTarget $Config 'Meadows' 'RDB_fox' 0 2 35
    Add-BountyTarget $Config 'BlackForest' 'RDB_crocodile' 0 2 85
    Add-BountyTarget $Config 'BlackForest' 'RDB_Genganger' 0 2 90 @(
        New-BountyAdd 'Skeleton' 2
    )
    Add-BountyTarget $Config 'BlackForest' 'RDB_Ent' 1 0 95 @(
        New-BountyAdd 'Greydwarf' 3
    )
    Add-BountyTarget $Config 'BlackForest' 'RDB_minotaur' 1 0 100 @(
        New-BountyAdd 'RDB_Genganger' 1
    )
    Add-BountyTarget $Config 'BlackForest' 'RDB_Goblin_Treasure' 1 2 125
    Add-BountyTarget $Config 'Swamp' 'RDB_giant_rat' 0 3 150 @(
        New-BountyAdd 'RDB_giant_rat' 2
    )
    Add-BountyTarget $Config 'Swamp' 'RDB_lizardwarrior' 0 4 165
    Add-BountyTarget $Config 'Swamp' 'RDB_Wendigo' 1 4 175 @(
        New-BountyAdd 'Draugr' 2
    )
    Add-BountyTarget $Config 'Mountain' 'RDB_Weregoat' 0 4 230
    Add-BountyTarget $Config 'Mountain' 'RDB_Werewolf' 2 5 270 @(
        New-BountyAdd 'Wolf' 2
    )
    Add-BountyTarget $Config 'Mountain' 'RDB_Goblin_Treasure_blue' 2 4 290
    Add-BountyTarget $Config 'Plains' 'RDB_graywolf' 0 5 300 @(
        New-BountyAdd 'RDB_graywolf' 2
    )
    Add-BountyTarget $Config 'Plains' 'RDB_lion' 3 4 350 @(
        New-BountyAdd 'RDB_graywolf' 2
    )
    Add-BountyTarget $Config 'Ocean' 'RDB_white_shark' 1 0 340
    Add-BountyTarget $Config 'Ocean' 'RDB_turtle' 1 0 320
    Add-BountyTarget $Config 'Ocean' 'RDB_Whale' 2 3 430 @(
        New-BountyAdd 'RDB_white_shark' 1
    )
    Add-BountyTarget $Config 'Mistlands' 'RDB_Smadrek' 4 7 520 @(
        New-BountyAdd 'SeekerBrood' 5
    )
    Add-BountyTarget $Config 'Mistlands' 'RDB_Goblin_Treasure_red' 5 5 575
}

function New-SetStoreItem {
    param(
        [Parameter(Mandatory = $true)][string]$Item,
        [Parameter(Mandatory = $true)][string]$Rarity
    )

    $premiumPiece = $Item -match '(TowerShield|Oathblade|BattleAxe|Longbow|WitchfinderArbalest|Nightblade|Staff|Bloodstaff|Moonbow|Runeblade|ReturningAxe|ShipwrightHammer)$'

    switch ($Rarity) {
        'Magic' {
            $coins = if ($premiumPiece) { 900 } else { 600 }
            return New-StoreItem $Item $coins
        }
        'Rare' {
            $coins = if ($premiumPiece) { 1800 } else { 1200 }
            $forestTokens = if ($premiumPiece) { 2 } else { 1 }
            return New-StoreItem $Item $coins $forestTokens
        }
        'Epic' {
            $coins = if ($premiumPiece) { 4500 } else { 3000 }
            $ironTokens = if ($premiumPiece) { 2 } else { 1 }
            return New-StoreItem $Item $coins 0 $ironTokens
        }
        default {
            throw "Unsupported store rarity $Rarity"
        }
    }
}

function Set-HaldorRaritySetStock {
    param(
        [Parameter(Mandatory = $true)]$Config,
        [Parameter(Mandatory = $true)]$RaritySetsConfig,
        [Parameter(Mandatory = $true)]$LegendaryConfig
    )

    if ($null -eq $Config.SecretStash) {
        throw 'Could not find SecretStash in adventuredata.json'
    }

    if (-not $Config.SecretStash.PSObject.Properties['OtherItems']) {
        $Config.SecretStash | Add-Member -NotePropertyName OtherItems -NotePropertyValue @()
    }

    $setItemIds = New-Object 'System.Collections.Generic.HashSet[string]' ([System.StringComparer]::OrdinalIgnoreCase)
    $setStock = @()
    foreach ($rarity in @('Magic', 'Rare', 'Epic')) {
        foreach ($item in @(Get-GeneratedSetItems $rarity $RaritySetsConfig $LegendaryConfig | Sort-Object ID)) {
            [void]$setItemIds.Add($item.ID)
            $setStock += New-SetStoreItem $item.ID $rarity
        }
    }

    $managedSetItemPattern = '^(Magic|Rare|Epic)(Heimdall|Ragnar|Hraesvelgr|Hellsyng|Nott|Seidr|Helveig|Moonvein|Frostbrand|Thor|Floki)'
    $preservedOtherItems = @(
        $Config.SecretStash.OtherItems |
            Where-Object { -not $setItemIds.Contains($_.Item) -and $_.Item -notmatch $managedSetItemPattern }
    )

    $Config.SecretStash.OtherItems = @($preservedOtherItems + $setStock)
}

$raritySetsConfig = Get-Content $raritySetsPath -Raw | ConvertFrom-Json
$legendaryConfig = Get-Content $legendaryPath -Raw | ConvertFrom-Json
$lootConfig = Get-Content $lootTablesPath -Raw | ConvertFrom-Json

Set-ItemSet $lootConfig 'FranEarlyCasterWeapons' @(
    New-WeightedItem 'StaffSkeleton' 1.2
    New-WeightedItem 'StaffFireball' 1.0
    New-WeightedItem 'StaffShield' 0.6
)

Set-ItemSet $lootConfig 'FranMidCasterWeapons' @(
    New-WeightedItem 'StaffIceShards' 1.0
    New-WeightedItem 'FW_StaffFireball' 0.8
    New-WeightedItem 'SP_StaffFireball' 0.8
    New-WeightedItem 'StaffClusterbomb' 0.5
)

Set-ItemSet $lootConfig 'FranLateCasterWeapons' @(
    New-WeightedItem 'StaffLightning' 1.0
    New-WeightedItem 'StaffGreenRoots' 0.9
    New-WeightedItem 'StaffRedTroll' 0.9
    New-WeightedItem 'SP_StaffLightning' 0.8
    New-WeightedItem 'FW_StaffLightning' 0.8
    New-WeightedItem 'StaffThunderBlood' 0.7
    New-WeightedItem 'StaffSpiritCaller' 0.7
)

Set-RaritySetEquipmentPools $lootConfig $raritySetsConfig $legendaryConfig

Add-ItemsToItemSet $lootConfig 'ModUtility' @(
    New-WeightedItem 'BeltStrength' 1.0
)

Add-ItemsToItemSet $lootConfig 'Tier2Trinkets' @(
    New-WeightedItem 'BeltStrength' 0.7
) -Rarity @(80.0, 14.0, 4.0, 2.0, 0.0)

Add-ItemsToItemSet $lootConfig 'Tier3Trinkets' @(
    New-WeightedItem 'BeltStrength' 0.8
) -Rarity @(38.0, 50.0, 8.0, 3.0, 1.0)

Add-ItemsToItemSet $lootConfig 'Tier4Trinkets' @(
    New-WeightedItem 'BeltStrength' 0.9
) -Rarity @(5.0, 35.0, 50.0, 17.0, 3.0)

Add-ItemsToItemSet $lootConfig 'Tier5Trinkets' @(
    New-WeightedItem 'BeltStrength' 0.9
) -Rarity @(0.0, 15.0, 60.0, 20.0, 5.0)

Add-ItemsToItemSet $lootConfig 'Tier6Trinkets' @(
    New-WeightedItem 'BeltStrength' 0.9
) -Rarity @(0.0, 10.0, 50.0, 30.0, 10.0)

Add-ItemsToItemSet $lootConfig 'Tier7Weapons' @(
    New-WeightedItem 'StaffSkeleton' 1.0
    New-WeightedItem 'StaffFireball' 1.0
    New-WeightedItem 'StaffIceShards' 1.0
    New-WeightedItem 'StaffShield' 1.0
    New-WeightedItem 'FW_StaffFireball' 0.8
    New-WeightedItem 'SP_StaffFireball' 0.8
    New-WeightedItem 'StaffThunderBlood' 1.0
    New-WeightedItem 'StaffSpiritCaller' 1.0
    New-WeightedItem 'StaffOrbofAhri' 1.0
    New-WeightedItem 'StaffFrostOrbs' 1.0
    New-WeightedItem 'SwordGold' 1.0
    New-WeightedItem 'SwordGold_BloodLightning' 1.0
    New-WeightedItem 'SwordGold_FrostFire' 1.0
    New-WeightedItem 'THSwordGold' 1.0
    New-WeightedItem 'THSwordGold_BloodLightning' 1.0
    New-WeightedItem 'THSwordGold_FrostFire' 1.0
    New-WeightedItem 'AxeGold' 1.0
    New-WeightedItem 'AxeGold_BloodLightning' 1.0
    New-WeightedItem 'AxeGold_FrostFire' 1.0
    New-WeightedItem 'BattleaxeGold' 1.0
    New-WeightedItem 'BattleaxeGold_BloodLightning' 1.0
    New-WeightedItem 'BattleaxeGold_FrostFire' 1.0
    New-WeightedItem 'KnifeGold' 1.0
    New-WeightedItem 'KnifeGold_BloodLightning' 1.0
    New-WeightedItem 'KnifeGold_FrostFire' 1.0
    New-WeightedItem 'KnifeVoid' 1.0
    New-WeightedItem 'FistGold' 1.0
    New-WeightedItem 'FistGold_BloodLightning' 1.0
    New-WeightedItem 'FistGold_FrostFire' 1.0
    New-WeightedItem 'MaceGold' 1.0
    New-WeightedItem 'MaceGold_BloodLightning' 1.0
    New-WeightedItem 'MaceGold_FrostFire' 1.0
    New-WeightedItem 'SledgeGold' 1.0
    New-WeightedItem 'SledgeGold_BloodLightning' 1.0
    New-WeightedItem 'SledgeGold_FrostFire' 1.0
    New-WeightedItem 'AtgeirGold' 1.0
    New-WeightedItem 'AtgeirGold_BloodLightning' 1.0
    New-WeightedItem 'AtgeirGold_FrostFire' 1.0
    New-WeightedItem 'SpearGold' 1.0
    New-WeightedItem 'SpearGold_BloodLightning' 1.0
    New-WeightedItem 'SpearGold_FrostFire' 1.0
    New-WeightedItem 'BowGold' 1.0
    New-WeightedItem 'BowGold_BloodLightning' 1.0
    New-WeightedItem 'BowGold_FrostFire' 1.0
    New-WeightedItem 'CrossbowGold' 1.0
    New-WeightedItem 'CrossbowGold_BloodLightning' 1.0
    New-WeightedItem 'CrossbowGold_FrostFire' 1.0
    New-WeightedItem 'DvergerArbalest_shootDeepNorth' 0.8
)

Add-ItemsToItemSet $lootConfig 'Tier7Armor' @(
    New-WeightedItem 'HelmetDNMage' 1.0
    New-WeightedItem 'HelmetDNHeavy' 1.0
    New-WeightedItem 'HelmetDNMediumHood' 1.0
    New-WeightedItem 'HelmetCrownofValheim' 1.0
    New-WeightedItem 'HelmetLox' 1.0
    New-WeightedItem 'ArmorLoxChest' 1.0
    New-WeightedItem 'ArmorLoxLegs' 1.0
    New-WeightedItem 'ArmorDeepNorthMageChest' 1.0
    New-WeightedItem 'ArmorDeepNorthHeavyChest' 1.0
    New-WeightedItem 'ArmorDeepNorthMediumChest' 1.0
    New-WeightedItem 'ArmorDeepNorthMagelegs' 1.0
    New-WeightedItem 'ArmorDeepNorthHeavylegs' 1.0
    New-WeightedItem 'ArmorDeepNorthMediumlegs' 1.0
    New-WeightedItem 'CapeAsh' 1.0
    New-WeightedItem 'CapeDeepNorth' 1.0
    New-WeightedItem 'CapeDeepNorthMage' 1.0
)

Add-ItemsToItemSet $lootConfig 'Tier7Shields' @(
    New-WeightedItem 'ShieldGold' 1.0
    New-WeightedItem 'ShieldGoldTower' 1.0
    New-WeightedItem 'ShieldGoldBuckler' 1.0
    New-WeightedItem 'ShieldRoots' 1.0
)

Add-ItemsToItemSet $lootConfig 'Tier7Trinkets' @(
    New-WeightedItem 'BeltStrength' 0.9
    New-WeightedItem 'TrinketBloodGoldHealth' 1.0
    New-WeightedItem 'TrinketBloodGoldStamina' 1.0
    New-WeightedItem 'Demister' 0.8
)

Set-NoAncientTier7ItemSets $lootConfig

Add-BossPool $lootConfig 'Eikthyr' 'FranEarlyCasterWeapons' 0.35
Add-BossPool $lootConfig 'gd_king' 'FranEarlyCasterWeapons' 0.20
Add-BossPool $lootConfig 'gd_king' 'FranMidCasterWeapons' 0.20
Add-BossPool $lootConfig 'Bonemass' 'FranMidCasterWeapons' 0.35
Add-BossPool $lootConfig 'Dragon' 'FranMidCasterWeapons' 0.20
Add-BossPool $lootConfig 'Dragon' 'FranLateCasterWeapons' 0.15
Add-BossPool $lootConfig 'GoblinKing' 'FranLateCasterWeapons' 0.35
Add-BossPool $lootConfig 'SeekerQueen' 'FranLateCasterWeapons' 0.25
Add-BossPool $lootConfig 'Fader' 'FranLateCasterWeapons' 0.20

Set-BossRarityWeights $lootConfig
Set-BossSetProgressionGuarantees $lootConfig
Set-BossRunestoneGuarantees $lootConfig
Limit-AncientLootToFinalBosses $lootConfig
Set-BestiaryLootIntegration $lootConfig
Write-JsonFile $lootTablesPath $lootConfig

$itemInfoConfig = Get-Content $itemInfoPath -Raw | ConvertFrom-Json
Set-StaffUnlockProgression $itemInfoConfig
Set-UtilityUnlockProgression $itemInfoConfig
Add-LoxArmorUnlockProgression $itemInfoConfig
Write-JsonFile $itemInfoPath $itemInfoConfig

$adventureDataConfig = Get-Content $adventureDataPath -Raw | ConvertFrom-Json
Set-HaldorRaritySetStock $adventureDataConfig $raritySetsConfig $legendaryConfig
Set-BestiaryBountyIntegration $adventureDataConfig
Write-JsonFile $adventureDataPath $adventureDataConfig

if (Test-Path $pluginConfigPath) {
    $cfg = Get-Content $pluginConfigPath -Raw
    $cfg = $cfg -replace '# Default value: 1', '# Default value: 0.05'
    $cfg = $cfg -replace 'Ancient Set Drop Chance = [0-9.]+', 'Ancient Set Drop Chance = 0.05'
    if ($cfg -notmatch 'Read Boss Set Drop Rules') {
        $cfg = $cfg -replace '(Read Rarity Sets Json = true\r?\n)', "`$1`r`n## Read per-boss set conversion rules from EpicLoot/bosssetdrops.json.`r`n# Setting type: Boolean`r`n# Default value: true`r`nRead Boss Set Drop Rules = true`r`n"
    }
    [System.IO.File]::WriteAllText($pluginConfigPath, $cfg, [System.Text.UTF8Encoding]::new($false))
}

Write-BossSetDropRules $lootConfig $raritySetsConfig $legendaryConfig

Write-Output 'Loot progression configured.'
