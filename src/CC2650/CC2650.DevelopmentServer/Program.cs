using System;
using XSockets.Core.Common.Socket;
using XSockets.Plugin.Framework;

namespace CC2650.DevelopmentServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //Start up a XSockets server
            using (var container = Composable.GetExport<IXSocketServerContainer>())
            {
                container.Start();
                
                Console.ReadLine();
            }
        }
    }
}


