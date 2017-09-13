
namespace Serilog.Sinks.Mattermost.Client
{
   using System;

   static class Program
   {
      static void Main(string[] args)
      {
         var log = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Mattermost(cfg => cfg.WebHookUri = "<WEBHOOKURI>")
              .CreateLogger();

         var position = new { Latitude = 25, Longitude = 134 };
         var elapsedMs = 34;

         log.Verbose("verbose");
         log.Warning("warning");
         log.Fatal("fatal");
         log.Debug("debug");
         log.Information("Processed {@Position} in {Elapsed:000} ms.", position, elapsedMs);
         try { Foo(); } catch (Exception ex) { log.Error(ex, "found an exception..."); }
         Console.ReadKey();
      }

      private static void Foo()
      {
         throw new NotImplementedException();
      }
   }
}
