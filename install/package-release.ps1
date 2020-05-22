Param(
    
[ValidateSet('pack','release', 'all')]
    [string]$command = "all",
    
    [string]$version = "",

    [string]$configuration= "Release",
    
    [string]$buildPath = "",

    [string]$outputPath = "",

    [string]$releasesDir = "build\Releases",

    [string]$packagePath
)

$PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent
$nugetSpecPath = "$PSScriptRoot\EvcProver.nuspec"

if ([string]::IsNullOrEmpty($buildPath)){
    #$buildPath = "$PSScriptRoot\..\build\$configuration"
    $buildPath = ".\build\$configuration"
}

if ([string]::IsNullOrEmpty($outputPath)){
    $outputPath = $buildPath
}  

if ([string]::IsNullOrEmpty($packagePath))
{
    $packagePath = "$outputPath\EvcProver.$version.nupkg"
}

Write-Host "Command         = $command"
Write-Host "Version         = $version"
Write-Host "Nuspec          = $nugetSpecPath"
Write-Host "Build path      = $buildPath"
Write-Host "Output path     = $outputPath"
Write-Host "Releases path   = $releasesDir"
Write-Host "Package path    = $packagePath"
Write-Host "Script root = $PSScriptRoot"

function Start-Command ($commandTitle, $commandPath, $commandArguments)
{
    $pinfo = New-Object System.Diagnostics.ProcessStartInfo
    $pinfo.FileName = $commandPath
    $pinfo.RedirectStandardError = $true
    $pinfo.RedirectStandardOutput = $true
    $pinfo.UseShellExecute = $false
    $pinfo.Arguments = $commandArguments
    $p = New-Object System.Diagnostics.Process
    $p.StartInfo = $pinfo
    $p.Start() | Out-Null
    $p.WaitForExit()
    [pscustomobject]@{
        commandTitle = $commandTitle
        returnCode = $p.ExitCode
        #stdout = $p.StandardOutput.ReadToEnd()
        #stderr = $p.StandardError.ReadToEnd()
        ExitCode = $p.ExitCode
    }
}

function Build-NuGetPackage {
    param (
        [string] $nugetSpecPath,
        [string] $version,
        [string] $buildDir,
        [string] $packageOutputDir
    )

    $NuGetOutput =  Invoke-Expression "& nuget.exe pack $nugetSpecPath -Version `"$version`" -OutputDirectory $packageOutputDir -BasePath $buildDir -Verbosity detailed"
    Write-Host ($NuGetOutput | Out-String)
}

#Invoke-Expression "& $toolsDir\nuget.exe pack $nuspecPath -Version $version -OutputDirectory $buildPath -BasePath $buildPath -Verbosity detailed"

function Write-Releases {
    param(
        [System.IO.FileInfo] $SquirrelExe,
        [System.IO.FileInfo] $NupkgFile
    )
    $squirrelExePath = (Resolve-Path $SquirrelExe).Path
    $nupkgFilePath = (Resolve-Path $NupkgFile).Path

    $arguments= "--releasify",$nupkgFilePath, "-releaseDir", $releasesDir, "--no-msi"
    
    Write-Host $arguments
    #Start-Command -commandTitle "Releasify" -commandPath $squirrelExePath -commandArguments $arguments
    Start-Process -FilePath $squirrelExePath `
        -ArgumentList $arguments `
        -PassThru | Wait-Process

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Squirrel threw an error occurred."
        Throw "An error occurred."
       
    }
    else{
        Write-Host "Squirrel successfully releasified!"
    }
   
}

if ($command -eq "pack" -or $command -eq "all")
{
    if ($version -eq ""){
        throw "Version number cannot be empty."
    }

    Build-NuGetPackage $nugetSpecPath $version $buildPath $outputPath
}

if ($command -eq "release" -or $command -eq "all"){   
    Write-Releases "$PSScriptRoot\squirrel\squirrel.exe" "$packagePath"
}

if ($LASTEXITCODE -ne 0) {
    Throw "An error occurred. $LASTEXITCODE"
}




#Write-Host ($SquirrelOutput | Out-String)
