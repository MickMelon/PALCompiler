Get-ChildItem "Programs\*.txt" -Recurse | 
Foreach-Object {
    Write-Host "**********************"
    Write-Host "Away to do " + $_.FullName

    $content = Get-Content $_.FullName

    $content | ..\out\netcoreapp3.1\Assessment.exe $_.FullName

    Write-Host "Completed " + $_.FullName
    Write-Host "**********************"
}