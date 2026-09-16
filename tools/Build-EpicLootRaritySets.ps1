$ErrorActionPreference = 'Stop'

$pluginRoot = Split-Path -Parent $PSScriptRoot
$bepInExRoot = 'C:\Users\Fran\AppData\Roaming\Thunderstore Mod Manager\DataFolder\Valheim\profiles\Valheim\BepInEx'
$valheimManagedRoot = 'D:\SteamLibrary\steamapps\common\Valheim\valheim_Data\Managed'
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
$source = Join-Path $pluginRoot 'src\EpicLootRaritySetsPlugin.cs'
$output = Join-Path $pluginRoot 'EpicLootRaritySets.dll'
$buildOutput = Join-Path $pluginRoot 'build\EpicLootRaritySets.dll'
$generatedConfigRoot = Join-Path $pluginRoot 'src\GeneratedConfig'
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
    (Join-Path $valheimManagedRoot 'assembly_valheim.dll')
    (Join-Path $valheimManagedRoot 'SoftReferenceableAssets.dll')
    (Join-Path $valheimManagedRoot 'UnityEngine.dll')
    (Join-Path $valheimManagedRoot 'UnityEngine.CoreModule.dll')
    (Join-Path $valheimManagedRoot 'UnityEngine.InputLegacyModule.dll')
    (Join-Path $valheimManagedRoot 'UnityEngine.PhysicsModule.dll')
)

$resources = @(
    'raritysets.json'
    'legendaries.json'
    'loottables.json'
    'iteminfo.json'
    'magiceffects.json'
    'adventuredata.json'
    'bosssetdrops.json'
    'NorseDemigods.cfg'
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
