param($installPath, $toolsPath, $package, $project)

$project.ProjectItems.Item("xsockets.license.x").Properties.Item("CopyToOutputDirectory").Value = 1

$vsVersion = ""
if($DTE.Version -eq "10.0"){$vsVersion = "2010"}
if($DTE.Version -eq "11.0"){$vsVersion = "2012"}
if($DTE.Version -eq "12.0"){$vsVersion = "2013"}
if($DTE.Version -eq "14.0"){$vsVersion = "2015"}

Try
{
    $itemTemplatesPathXSockets = [Environment]::GetFolderPath("MyDocuments") + "\Visual Studio "+$vsVersion+"\Templates\ItemTemplates\XSockets.NET 5"
    #$itemTemplatesPathCSharp = [Environment]::GetFolderPath("MyDocuments") + "\Visual Studio "+$vsVersion+"\Templates\ItemTemplates\Visual C#"

    if(!(Test-Path ($itemTemplatesPathXSockets))){New-Item ($itemTemplatesPathXSockets) -type directory}
 
    Get-ChildItem $toolspath -Filter *.zip | ForEach{
        #Copy-Item -Path $_.FullName -Destination $itemTemplatesPathCSharp
        Copy-Item -Path $_.FullName -Destination $itemTemplatesPathXSockets
    }
}
Catch
{
    Write-Host "Could not copy XSockets templates to disc"
}
$DTE.ItemOperations.Navigate("http://xsockets.net/release-notes?" + $package.Id + "=" + $package.Version)