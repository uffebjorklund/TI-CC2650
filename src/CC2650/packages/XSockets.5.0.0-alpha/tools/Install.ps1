param($installPath, $toolsPath, $package, $project)
#install.ps1 v: 2.0
$defaultProject = Get-Project

$projext = '.csproj'

if($defaultProject.Type -eq 'VB.NET'){
    $projext = '.vbproj'
}

$defaultNamespace = (Get-Project $defaultProject.Name).Properties.Item("DefaultNamespace").Value
$path = $defaultProject.FullName.Replace($defaultProject.Name + $projext,'').Replace('\\','\')

###################################
#Adding plugin info to config     #
###################################
$config = $path + "Web.config"

$webproject = $true

if((Test-Path $config) -eq $false){
    $config = $path + "App.config"
    $webproject = $false
}

#Write-Host $config

if((Test-Path $config) -eq $true){   

    Write-Host "Getting content of $config."
    $xml = [xml](get-content($config))

    Write-Host "Adding appsettings in $config"

    if($xml.configuration['appSettings'] -eq $null){
        Write-Host 'Add appsettings'
        $a = $xml.CreateElement('appSettings')
        $xml.configuration.AppendChild($a)
    }

    if(($xml.configuration['appSettings'].add | Where-Object {$_.key -eq 'XSockets.PluginCatalog'}) -eq $null){
        $pluginCatalog = $xml.CreateElement('add')
        $pluginCatalog.setAttribute('key','XSockets.PluginCatalog')
        #XSockets\XSocketServerPlugins\
        $pluginCatalog.setAttribute('value','')
        $xml.configuration["appSettings"].AppendChild($pluginCatalog)
    }

    #if(($xml.configuration['appSettings'].add | Where-Object {$_.key -eq 'XSockets.PluginFilter'}) -eq $null){
    #    $pluginFilter = $xml.CreateElement('add')
    #    $pluginFilter.setAttribute('key','XSockets.PluginFilter')
    #    if($webproject -eq $false){
    #        $pluginFilter.setAttribute('value','*.dll,*.exe')
    #    }
    #    else{
    #        $pluginFilter.setAttribute('value','*.dll')        
    #    }
    #    $xml.configuration["appSettings"].AppendChild($pluginFilter)
    #}

    Write-Host "Saving $config."
    $xml.Save($config)
}

#Add bootstrapper if webproject
if($webproject -eq $true){
    #Scaffold XSockets.Bootstrapper
    
    ###################################
    #Add fallback if MVC
    ###################################
    #Install-Package XSockets.Fallback -ProjectName $defaultProject.Name
    
    ###################################
    #Add JsAPI if WEB
    ###################################
    #Install-Package XSockets.JsApi -ProjectName $defaultProject.Name
    
    Write-Host ""
    Write-Host "########################################################################################################################" -ForegroundColor Green
    Write-Host "New to XSockets.NET? Visit http://xsockets.net/docs/4/xsocketsnet-4-documentation " -ForegroundColor Blue    
    Write-Host "########################################################################################################################" -ForegroundColor Green
}

#Write example code if not webproject
if($webproject -eq $false){
Write-Host ""
Write-Host "//How to start a server (example)"
Write-Host "//using XSockets.Core.Common.Socket;"
Write-Host "using (var container = XSockets.Plugin.Framework.Composable.GetExport<IXSocketServerContainer>())" 
Write-Host "{"
Write-Host "    container.Start();"
Write-Host "    Console.ReadLine();"
Write-Host "}"
}