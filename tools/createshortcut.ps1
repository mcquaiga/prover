function CreateShortcut($shortcutPath, $targetDir, $targetFileName)
{
    Write-Host "Shortcut Path = $($shortcutPath)"
    Write-Host "Target Dir = $($targetDir)"
    Write-Host "Target File = $($targetFileName)"
    
    $s=(New-Object -COM WScript.Shell).CreateShortcut($shortcutPath)
    $s.TargetPath= "$($targetDir)\$($targetFileName)"
    $s.WorkingDirectory=$targetDir
    $s.Description = 'EVC Prover'
    $s.Save()
}

    if ($args.Length -ne 3)
    {
        echo "Usage: CreateShortcut <shortcutPath> <targetDir> <targerFileName>"
    }
    else
    {
        Write-Host "$($args)"
        CreateShortcut $args[0] $args[1] $args[2]
    }

## Call from VS like so
#powershell -ExecutionPolicy RemoteSigned -noprofile -command "& '$(SolutionDir)..\tools\createshortcut.ps1' '$(TargetDir)..\EvcProver.lnk' '$(TargetDir)' 'Prover.GUI.exe'"