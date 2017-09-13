namespace Serilog
{
   using System;
   using Configuration;
   using Events;

   public static class MattermostSinkExtension
   {
      public static LoggerConfiguration Mattermost(this LoggerSinkConfiguration loggerConfiguration, string webhookUri,
         IFormatProvider formatProvider = null)
         => loggerConfiguration.Sink(new MattermostSink(formatProvider, new MattermostConfig{WebHookUri = webhookUri}));

      public static LoggerConfiguration Mattermost(this LoggerSinkConfiguration loggerConfiguration,
         MattermostConfig config, LogEventLevel propertiesFromLevel = LogEventLevel.Error,
         LogEventLevel restrictedToMinimumLevel = LogEventLevel.Error,
         IFormatProvider formatProvider = null)
         => loggerConfiguration.Sink(new MattermostSink(formatProvider, config, propertiesFromLevel,
            restrictedToMinimumLevel));

      public static LoggerConfiguration Mattermost(this LoggerSinkConfiguration loggerConfiguration,
         Action<MattermostConfig> configActor, LogEventLevel propertiesFromLevel = LogEventLevel.Error,
         LogEventLevel restrictedToMinimumLevel = LogEventLevel.Error,
         IFormatProvider formatProvider = null)
      {
         var config = new MattermostConfig();
         configActor(config);
         return loggerConfiguration.Sink(new MattermostSink(formatProvider, config, propertiesFromLevel,
            restrictedToMinimumLevel));
      }
   }
}