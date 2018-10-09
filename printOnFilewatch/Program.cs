using System;
using System.IO;
using Topshelf;

namespace DirPrintWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var service = HostFactory.New(x =>
            {
                x.Service<DirPrintWatcher>(s =>
                {
                    s.ConstructUsing(name => new DirPrintWatcher());
                    s.WhenStarted(dpw => dpw.Start());
                    s.WhenStopped(dpw => dpw.Stop());
                });

                x.SetDescription("Watches configured directory for new files and prints them.");
                x.SetDisplayName("Directory Print Watcher");
                x.SetServiceName("DirPrintWatcher");

                x.StartAutomatically();
                x.EnableServiceRecovery(r =>
                {
                    r.RestartService(0);
                });

                x.DependsOn("Spooler");
            });

            var runner = service.Run();

            var exitCode = (int)Convert.ChangeType(runner, runner.GetTypeCode());
            Environment.ExitCode = exitCode;            
        }       
      
    }         
        

}

