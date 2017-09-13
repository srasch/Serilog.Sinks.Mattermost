
namespace Serilog
{
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using System.Text;
   using System.Threading.Tasks;
   using Core;
   using Events;
   using Matterhook.NET.MatterhookClient;

   public class MattermostConfig
   {
      public string WebHookUri { get; set; }
      public string Username { get; set; } = "MattermostSink";
      public string Channel { get; set; } = null;
      public Uri IconUrl { get; set; } = null;

      public Dictionary<LogEventLevel, string> ColorMap { get; set; } = new Dictionary<LogEventLevel, string>
      {
         {LogEventLevel.Verbose, "#c1c1c1"},
         {LogEventLevel.Debug, "#c1c1c1"},
         {LogEventLevel.Error, "#ff0000"},
         {LogEventLevel.Fatal, "#ff0000"},
         {LogEventLevel.Warning, "#ffff00"},
      };
   }
   public class MattermostSink : ILogEventSink
   {
      private readonly IFormatProvider _formatProvider;
      private readonly MattermostConfig _config;
      private readonly LogEventLevel _propertiesFromLevel;
      private readonly LogEventLevel _restrictedToMinimumLevel;

      public MattermostSink(IFormatProvider formatProvider, MattermostConfig config, LogEventLevel propertiesFromLevel = LogEventLevel.Error, LogEventLevel restrictedToMinimumLevel  = LogEventLevel.Error)
      {
         _formatProvider = formatProvider;
         _config = config ?? throw new ArgumentNullException(nameof(config));
         _propertiesFromLevel = propertiesFromLevel;
         _restrictedToMinimumLevel = restrictedToMinimumLevel;

         if (string.IsNullOrWhiteSpace(_config.WebHookUri))
            throw new ArgumentNullException(nameof(config) + "." + nameof(config.WebHookUri));
      }
      public void Emit(LogEvent logEvent)
      {
         if (logEvent.Level < _restrictedToMinimumLevel)
            return;

         var client = new MatterhookClient(_config.WebHookUri);

         _config.ColorMap.TryGetValue(logEvent.Level, out var levelColor);

         var message = new MattermostMessage
         {
            Username = _config.Username,
            Channel = _config.Channel,
            IconUrl = _config.IconUrl,
            Attachments = new List<MattermostAttachment>()
            {
               new MattermostAttachment()
               {
                  Text = logEvent.RenderMessage(_formatProvider),
                  Color = levelColor,
                  Fields = new List<MattermostField>()
               }
            }
         };

         if (logEvent.Properties.Any())
         {
            var builder = new StringBuilder();
            builder.AppendLine("| Key | Value |");
            builder.AppendLine("|:----|:------|");
            foreach (var property in logEvent.Properties)
            {
               builder.Append("| ");
               builder.Append(property.Key);
               builder.Append(" | ");
               builder.Append(property.Value);
               builder.AppendLine(" |");
            }

            message.Attachments.First().Fields.Add(new MattermostField
            {
               Short = true,
               Title = "Properties",
               Value = builder.ToString(),
            });
         }
         if (logEvent.Exception != null)
         {
            message.Attachments.First().Fields.Add(new MattermostField
            {
               Short = false,
               Title = "Exception",
               Value = $@"{logEvent.Exception.GetType().Name}: {logEvent.Exception.Message}
```
{logEvent.Exception.StackTrace}
```"
            });
         }
         var task = client.PostAsync(message);

         Task.WaitAll(task);
      }
   }
}
