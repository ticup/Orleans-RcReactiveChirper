using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebServer;

namespace WebServer
{
    class Controller
    {
        public Controller(Router router)
        {
            // Print out all the files that belong to the assembly
            // -> These are alle the files that can be returned by the .ReturnFromFile() method.
            // Add them by right-clicking on the solution -> add new/existing item.
            // Then click on the new item and go to properties (bottom right) and change "Build Action" to "Embedded Resource"!
            var assembly = Assembly.GetExecutingAssembly();
            foreach (string resourceName in assembly.GetManifestResourceNames())
            {
                Console.Out.WriteLine(resourceName);
            }


            // Setup routing
            router.Add("/", ChirperPage);
            router.Add("/chirper.min.js", ChirperJs);
        }


            /* Static pages */
            Task ChirperPage(IOwinContext context, IDictionary<string, string> parameters)
        {
            return context.ReturnFile("www.Chirper.html", "text/html");
        }

        Task ChirperJs(IOwinContext context, IDictionary<string, string> parameters)
        {
            return context.ReturnFile("www.chirper.min.js", "application/javascript");
        }
    }
}
