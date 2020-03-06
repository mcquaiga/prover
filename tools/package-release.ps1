param($version)

$buildPath = "build"
$nuspecPath = "EvcProver.nuspec"
$toolsDir = "tools"


& "$toolsDir\nuget.exe" pack $nuspecPath -Version $version -Properties Configuration=Release -OutputDirectory $buildPath -BasePath $buildPath -Verbosity detailed