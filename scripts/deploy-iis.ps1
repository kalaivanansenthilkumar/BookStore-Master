param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("DEV","UAT","PROD")]
    [string]$Environment,
    [string]$ProjectPath = "BookStore.Web\BookStore.Web.csproj",
    [string]$Configuration = $null,
    [string]$OutputFolder = ".\artifacts",
    [string]$MsDeployUrl = "",
    [string]$MsDeployUser = "",
    [string]$MsDeployPassword = "",
    [switch]$AllowUntrusted,
    [switch]$PackageOnly,
    [string]$DbServerName= "", 
    [string]$DbName = "", 
    [string]$DbUserName = "", 
    [string]$DbPassword = "",    
    [string]$IisSiteName = "Default Web Site/MyApp", # target IIS site name
    [string]$ConnStringParamName = "BookStoreContext-Web.config Connection String" # parameter name in SetParameters.xml
)

$setParamsPath = $null
$backups = @()

try {
    if (-not $Configuration) { $Configuration = $Environment }

    $scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Definition
    $projectFullPath = Join-Path $scriptRoot $ProjectPath
    if (-not (Test-Path $projectFullPath)) { throw "Project file not found: $projectFullPath" }

    # ensure output folder
    $outputFolderFull = Join-Path $scriptRoot $OutputFolder
    if (-not (Test-Path $outputFolderFull)) { New-Item -ItemType Directory -Path $outputFolderFull | Out-Null }

    # produce package
    $packageName = "BookStoreWeb.$Environment.zip"
    $packagePath = Join-Path $outputFolderFull $packageName

    Write-Host "Packaging project: $projectFullPath (Configuration: $Configuration)"
    $msbuildExe = (Get-Command msbuild -ErrorAction SilentlyContinue).Path
    if (-not $msbuildExe) { $msbuildExe = "msbuild" }

    $msbuildArgs = "`"$projectFullPath`" /p:Configuration=$Configuration /t:Package /p:PackageLocation=`"$packagePath`" /p:AutoParameterizationWebConfigConnectionStrings=false"
    Start-Process -FilePath $msbuildExe -ArgumentList $msbuildArgs -NoNewWindow -Wait -PassThru | Out-Null
    if (-not (Test-Path $packagePath)) { throw "Package not created: $packagePath" }
    Write-Host "Package created: $packagePath"

    if ($PackageOnly) { Write-Host "Package-only; skipping deployment"; exit 0 }

    # create SetParameters.xml in artifacts folder
    $setParamsPath = Join-Path $outputFolderFull ("SetParameters.$Environment.xml")
    $connValue = if ($DbPassword) { "Data Source=$DbServerName;Initial Catalog=$DbName;User Id=$DbUserName;Password=$DbPassword;TrustServerCertificate=True" } else { "" }

    $xml = New-Object System.Xml.XmlDocument
    $declaration = $xml.CreateXmlDeclaration("1.0","utf-8",$null)
    $xml.AppendChild($declaration) | Out-Null
    $root = $xml.CreateElement("parameters")
    $xml.AppendChild($root) | Out-Null

    # IIS site parameter
    $p1 = $xml.CreateElement("setParameter")
    $p1.SetAttribute("name","IIS Web Application Name")
    $p1.SetAttribute("value",$IisSiteName)
    $root.AppendChild($p1) | Out-Null

    # Connection-string parameter (only if we have a value or placeholder)
    if ($DbPassword) {
        $p2 = $xml.CreateElement("setParameter")
        $p2.SetAttribute("name",$ConnStringParamName)
        $p2.SetAttribute("value",$connValue)
        $root.AppendChild($p2) | Out-Null
    }

    $xml.Save($setParamsPath)
    Write-Host "Generated SetParameters: $setParamsPath (will not print secret values)."

    # locate msdeploy
    $possible = @(
      "$env:ProgramFiles\IIS\Microsoft Web Deploy V3\msdeploy.exe",
      "$env:ProgramFiles(x86)\IIS\Microsoft Web Deploy V3\msdeploy.exe"
    )
    $msdeployExe = $possible | Where-Object { Test-Path $_ } | Select-Object -First 1
    if (-not $msdeployExe) { $msdeployExe = (Get-Command msdeploy.exe -ErrorAction SilentlyContinue).Path }
    if (-not $msdeployExe) { throw "msdeploy.exe not installed on agent." }

    # build msdeploy args and include setParamFile
    $dest = "auto,computerName='$MsDeployUrl'"
    if ($MsDeployUser) { $dest += ",userName='$MsDeployUser',password='$MsDeployPassword',authType='Basic'" } else { $dest += ",authtype='NTLM'" }
    $allow = $AllowUntrusted.IsPresent ? " -allowUntrusted" : ""

    $msdeployArgs = "-verb:sync -source:package=`"$packagePath`" -dest:$dest -setParamFile:`"$setParamsPath`"$allow"

    Write-Host "Running msdeploy (credentials masked)."
    $p = Start-Process -FilePath $msdeployExe -ArgumentList $msdeployArgs -NoNewWindow -Wait -PassThru
    if ($p.ExitCode -ne 0) { throw "msdeploy failed: $($p.ExitCode)" }

    Write-Host "Deployment completed."
    exit 0
}
catch {
    Write-Error $_.Exception.Message
    exit 1
}
finally {
    if ($setParamsPath -and (Test-Path $setParamsPath)) {
        Remove-Item -Path $setParamsPath -Force -ErrorAction SilentlyContinue
    }
}