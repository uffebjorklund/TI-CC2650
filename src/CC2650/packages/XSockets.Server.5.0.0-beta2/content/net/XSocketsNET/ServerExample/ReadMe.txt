Showing howto start a XSockets.NET server

//1: Console Application
using (var container = XSockets.Plugin.Framework.Composable.GetExport<IXSocketServerContainer>())
{
    container.Start();
    Console.ReadLine();
}

//2: WebApplication, use PreApplicationStartMethod

using System.Web;
[assembly: PreApplicationStartMethod(typeof(YourWebApp.App_Start.XSocketsBootstrap), "Start")]


//Server instance
using XSockets.Core.Common.Socket;

//Create class for the server instance
public static class XSocketsBootstrap
{
    private static IXSocketServerContainer container;
    public static void Start()
    {
        container = XSockets.Plugin.Framework.Composable.GetExport<IXSocketServerContainer>();
        container.Start();        
    }        
}