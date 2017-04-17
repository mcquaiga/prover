function GetGuidFromProject([string]$fileName) {
    $content = Get-Content $fileName

    $xml = [xml]$content
    $obj = $xml.Project.PropertyGroup.ProjectGuid

    return [Guid]$obj[0]
}

$slnPath = "C:\Users\adam\Dev\EvcProver\src\EvcProver-All.sln"

$solutionDirectory = [System.IO.Path]::GetDirectoryName($slnPath)

$srcPath = [System.IO.Path]::GetDirectoryName($slnPath)
$writer = new-object System.IO.StreamWriter ($slnPath, $false, [System.Text.Encoding]::UTF8)

$writer.WriteLine("Microsoft Visual Studio Solution File, Format Version 12.00")
$writer.WriteLine("# Visual Studio 15")
$writer.WriteLine("VisualStudioVersion = 15.0.26403.3")
$writer.WriteLine("MinimumVisualStudioVersion = 10.0.40219.1")

$projects = gci $srcPath -Filter *.csproj -Recurse

foreach ($project in $projects) {
   $fileName = [System.IO.Path]::GetFileNameWithoutExtension($project)

   Write-Host $project.FullName
   $guid = GetGuidFromProject $project.FullName

   $slnRelativePath = $project.FullName.Replace($solutionDirectory, "").TrimStart("\")

   # Assume the project is a C# project {FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}
   $writer.WriteLine("Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""$fileName"", ""$slnRelativePath"",""{$($guid.ToString().ToUpper())}""")
   $writer.WriteLine("EndProject")
}

$writer.Flush()
$writer.Close()
$writer.Dispose()