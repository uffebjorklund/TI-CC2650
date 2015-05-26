param($installPath, $toolsPath, $package, $project)

$project.ProjectItems.Item("xsockets.license.x").Properties.Item("CopyToOutputDirectory").Value = 1

$vsVersion = ""
if($DTE.Version -eq "10.0"){$vsVersion = "2010"}
if($DTE.Version -eq "11.0"){$vsVersion = "2012"}
if($DTE.Version -eq "12.0"){$vsVersion = "2013"}
$itemTemplatesPathXSockets = [Environment]::GetFolderPath("MyDocuments") + "\Visual Studio "+$vsVersion+"\Templates\ItemTemplates\XSockets.NET 4"
$itemTemplatesPathCSharp = [Environment]::GetFolderPath("MyDocuments") + "\Visual Studio "+$vsVersion+"\Templates\ItemTemplates\Visual C#"

if(!(Test-Path ($itemTemplatesPathXSockets))){New-Item ($itemTemplatesPathXSockets) -type directory}
 
Get-ChildItem $toolspath -Filter *.zip | ForEach{
    Copy-Item -Path $_.FullName -Destination $itemTemplatesPathCSharp
    Copy-Item -Path $_.FullName -Destination $itemTemplatesPathXSockets
}

$DTE.ItemOperations.Navigate("http://xsockets.net/release-notes?" + $package.Id + "=" + $package.Version)