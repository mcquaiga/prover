
Param(
    
    [Parameter(Mandatory)]
    [string] $version,
    
    $project = "EvcProver",
    $feed = "Tools",
    $packageName= "install-tools",
    $localPath = ".\install\",
    $description = "EvcProver installer tools"
)

Invoke-Expression "& az artifacts universal publish --organization https://mcquaiga.visualstudio.com/ --project='$project' --scope project --feed '$feed' --name '$packageName' --version '$version' --description '$description' --path '$localPath'"