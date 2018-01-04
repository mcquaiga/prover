param (
    [Parameter(Mandatory=$true)] 
        [string]$url,    
    [Parameter(Mandatory=$true)] 
        [string]$outputDir
)

function DownloadFile([string]$url, [string]$dest){    
    Invoke-WebRequest $url -OutFile $dest
    return
}

function GetReleaseAssetUrl([string]$url, [string] $name){
    $releaseInfo = Invoke-WebRequest $url | ConvertFrom-Json
    $release = $releaseInfo.assets | Where-Object {$_.name -eq 'Releases.zip'}
    $release.browser_download_url
    return
}

Add-Type -AssemblyName System.IO.Compression.FileSystem
function Unzip
{
    param([string]$zipfile, [string]$outpath)

    [System.IO.Compression.ZipFile]::ExtractToDirectory($zipfile, $outpath)
} 

#$url = "https://api.github.com/repos/mcquaiga/EvcProver/releases/latest"
#$outputDir = "..\build"
$zipFile = "$outputDir\releases.zip"

$assetUrl = GetReleaseAssetUrl $url 'Releases.zip'
DownloadFile $assetUrl $zipFile
Expand-Archive $zipFile -DestinationPath $outputDir -Force


