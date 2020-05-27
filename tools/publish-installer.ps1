
Param(
    
    [Parameter(Mandatory)]
    [string] $version,
    
    $project = "EvcProver",
    $feed = "Tools",
    $packageName= "install-tools",
    $localPath = "install",
    $description = "EvcProver installer tools"
)
$PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent
$PSScriptRoot = Split-Path $PSScriptRoot -Parent

$localPath = "$PSScriptRoot\$localPath"
Invoke-Expression "& az artifacts universal publish --organization https://mcquaiga.visualstudio.com/ --project='$project' --scope project --feed '$feed' --name '$packageName' --version '$version' --description '$description' --path '$localPath'"