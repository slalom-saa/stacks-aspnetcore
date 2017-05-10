$paths = nuget locals all -list
foreach($path in $paths) {
    $path = $path.Substring($path.IndexOf(' ')).Trim()

    if (Test-Path $path) {

        Push-Location $path

        foreach($item in Get-ChildItem -Filter "*slalom*" -Recurse) {
            Remove-Item $item.FullName -Recurse -Force
            Write-Host "Removing $item"
        }

        Pop-Location
    
    }
}

Push-Location $PSScriptRoot

foreach($item in Get-ChildItem -Path ..\ -Filter 'project.json' -Recurse) {
    Push-Location $item.Directory

    dotnet restore

    Pop-Location
}

Pop-Location