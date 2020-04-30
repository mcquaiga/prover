Param(
    [string]$version,
    [string]$configuration= "Release",
    [string]$buildPath = "build",
    [string]$releasesDir = "build\Releases"  
)

Write-Host "Version = $version"
Write-Host "Build path = $buildPath"
Write-Host  "Output path = $releasesDir"


function Write-Releases {
    param(
        [System.IO.FileInfo] $SquirrelExe,
        [System.IO.FileInfo] $NupkgFile
    )
    $squirrelExePath = (Resolve-Path $SquirrelExe).Path
    $nupkgFilePath = (Resolve-Path $NupkgFile).Path
    $arguments= "--releasify",$nupkgFilePath, "-releaseDir", $releasesDir
    Start-Process -FilePath $squirrelExePath `
        -ArgumentList $arguments `
        -PassThru | Wait-Process
}


if ($buildPath -eq "build"){
    $buildPath = "build\$configuration"
}


$nuspecPath = "install\EvcProver.nuspec"
$toolsDir = "tools"

#-Properties  Configuration=$configuration
$NuGetOutput = Invoke-Expression "& $toolsDir\nuget.exe pack $nuspecPath -Version $version -OutputDirectory $buildPath -BasePath $buildPath -Verbosity detailed"
#Invoke-Expression "& $toolsDir\nuget.exe pack $nuspecPath -Version $version -OutputDirectory $buildPath -BasePath $buildPath -Verbosity detailed"

#Write-Debug $NuGetOutput
Write-Host ($NuGetOutput | Out-String)

if ($LASTEXITCODE -ne 0) {
    Throw "An error occurred while restoring NuGet tools."
}

Write-Releases "install\squirrel\squirrel.exe" "$buildPath\EvcProver.$version.nupkg"
#Write-Host ($SquirrelOutput | Out-String)
