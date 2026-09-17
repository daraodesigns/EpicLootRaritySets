$ErrorActionPreference = 'Stop'

$pluginRoot = Split-Path -Parent $PSScriptRoot
$bepInExRoot = 'C:\Users\Fran\AppData\Roaming\Thunderstore Mod Manager\DataFolder\Valheim\profiles\Valheim\BepInEx'
$valheimManagedRoot = 'D:\SteamLibrary\steamapps\common\Valheim\valheim_Data\Managed'
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
$source = Join-Path $pluginRoot 'src\EpicLootRaritySetsPlugin.cs'
$output = Join-Path $pluginRoot 'EpicLootRaritySets.dll'
$buildOutput = Join-Path $pluginRoot 'build\EpicLootRaritySets.dll'
$generatedConfigRoot = Join-Path $pluginRoot 'src\GeneratedConfig'
$skillIconRoot = Join-Path $generatedConfigRoot 'SkillIcons'
$abilityIconRoot = Join-Path $generatedConfigRoot 'AbilityIcons'
$abilityPanelIconRoot = Join-Path $generatedConfigRoot 'AbilityPanelIcons'
$cecilPath = Join-Path $bepInExRoot 'core\Mono.Cecil.dll'

$references = @(
    (Join-Path $valheimManagedRoot 'mscorlib.dll')
    (Join-Path $valheimManagedRoot 'System.dll')
    (Join-Path $valheimManagedRoot 'System.Core.dll')
    (Join-Path $bepInExRoot 'core\BepInEx.dll')
    (Join-Path $bepInExRoot 'core\0Harmony.dll')
    (Join-Path $bepInExRoot 'plugins\RandyKnapp-EpicLoot\EpicLoot.dll')
    (Join-Path $bepInExRoot 'plugins\ValheimModding-JsonDotNET\Newtonsoft.Json.dll')
    (Join-Path $valheimManagedRoot 'netstandard.dll')
    (Join-Path $valheimManagedRoot 'assembly_utils.dll')
    (Join-Path $valheimManagedRoot 'assembly_guiutils.dll')
    (Join-Path $valheimManagedRoot 'assembly_valheim.dll')
    (Join-Path $valheimManagedRoot 'SoftReferenceableAssets.dll')
    (Join-Path $valheimManagedRoot 'UnityEngine.dll')
    (Join-Path $valheimManagedRoot 'UnityEngine.CoreModule.dll')
    (Join-Path $valheimManagedRoot 'UnityEngine.AnimationModule.dll')
    (Join-Path $valheimManagedRoot 'UnityEngine.ImageConversionModule.dll')
    (Join-Path $valheimManagedRoot 'UnityEngine.InputLegacyModule.dll')
    (Join-Path $valheimManagedRoot 'UnityEngine.PhysicsModule.dll')
    (Join-Path $valheimManagedRoot 'UnityEngine.TextRenderingModule.dll')
    (Join-Path $valheimManagedRoot 'UnityEngine.UIModule.dll')
    (Join-Path $valheimManagedRoot 'UnityEngine.UI.dll')
)

$resources = @(
    'abilities.json'
    'raritysets.json'
    'legendaries.json'
    'loottables.json'
    'iteminfo.json'
    'magiceffects.json'
    'adventuredata.json'
    'bosssetdrops.json'
    'NorseDemigods.cfg'
)

$skillIconResources = @(
    'Frostbrand_spellblade.png'
    'Heimdall_tank.png'
    'Helveig_blood_mage.png'
    'Hraesvelgr_archer.png'
    'Moonvein_magic_archer.png'
    'Nott_assassin.png'
    'Ragnar_berserker.png'
    'Seidr_elemental_mage.png'
    'Hellsyng_crossbow.png'
)

$abilityIconResources = @(
    'LastHopeIcon.png'
)

foreach ($path in @($compiler, $source, $cecilPath) + $references) {
    if (-not (Test-Path -LiteralPath $path)) {
        throw "Missing build input: $path"
    }
}

$resourceArgs = @()
foreach ($resource in $resources) {
    $path = Join-Path $generatedConfigRoot $resource
    if (-not (Test-Path -LiteralPath $path)) {
        throw "Missing generated config resource: $path"
    }

    $resourceArgs += "/resource:$path,Fran.EpicLootRaritySets.GeneratedConfig.$resource"
}

foreach ($resource in $skillIconResources) {
    $path = Join-Path $skillIconRoot $resource
    if (-not (Test-Path -LiteralPath $path)) {
        throw "Missing skill icon resource: $path"
    }

    $resourceArgs += "/resource:$path,Fran.EpicLootRaritySets.SkillIcons.$resource"
}

foreach ($resource in $abilityIconResources) {
    $path = Join-Path $abilityIconRoot $resource
    if (-not (Test-Path -LiteralPath $path)) {
        throw "Missing ability icon resource: $path"
    }

    $resourceArgs += "/resource:$path,Fran.EpicLootRaritySets.AbilityIcons.$resource"
}

foreach ($folder in @('Abilities', 'Buffs', 'ClassBuffs')) {
    $folderPath = Join-Path $abilityPanelIconRoot $folder
    if (-not (Test-Path -LiteralPath $folderPath)) {
        throw "Missing ability panel icon folder: $folderPath"
    }

    foreach ($resource in Get-ChildItem -LiteralPath $folderPath -Filter '*.png' -File | Sort-Object Name) {
        $resourceArgs += "/resource:$($resource.FullName),Fran.EpicLootRaritySets.AbilityPanelIcons.$folder.$($resource.Name)"
    }
}

$referenceArgs = @($references | ForEach-Object { "/reference:$_" })
$args = @(
    '/nologo'
    '/noconfig'
    '/target:library'
    '/nostdlib+'
    '/optimize+'
    '/debug-'
    "/out:$output"
) + $referenceArgs + $resourceArgs + @($source)

& $compiler @args
if ($LASTEXITCODE -ne 0) {
    throw "csc.exe failed with exit code $LASTEXITCODE"
}

Add-Type -Path $cecilPath
$tempOutput = Join-Path $pluginRoot 'EpicLootRaritySets.cleaned.dll'
$assembly = [Mono.Cecil.AssemblyDefinition]::ReadAssembly($output)
$module = $assembly.MainModule
$mscorlib4 = @($module.AssemblyReferences | Where-Object { $_.Name -eq 'mscorlib' -and $_.Version.Major -eq 4 }) | Select-Object -First 1
$mscorlib2 = @($module.AssemblyReferences | Where-Object { $_.Name -eq 'mscorlib' -and $_.Version.Major -eq 2 }) | Select-Object -First 1
if ($mscorlib4 -and $mscorlib2) {
    foreach ($typeRef in @($module.GetTypeReferences() | Where-Object { $_.Scope -eq $mscorlib2 })) {
        $typeRef.Scope = $mscorlib4
    }

    [void]$module.AssemblyReferences.Remove($mscorlib2)
    if ([System.IO.File]::Exists($tempOutput)) {
        [System.IO.File]::Delete($tempOutput)
    }

    $assembly.Write($tempOutput)
    $assembly.Dispose()
    [System.IO.File]::Copy($tempOutput, $output, $true)
    [System.IO.File]::Delete($tempOutput)
}

New-Item -ItemType Directory -Path (Split-Path -Parent $buildOutput) -Force | Out-Null
Copy-Item -LiteralPath $output -Destination $buildOutput -Force

"Built $output"
"Copied $buildOutput"
