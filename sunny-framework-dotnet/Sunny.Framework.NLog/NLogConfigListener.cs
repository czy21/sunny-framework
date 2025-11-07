using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Config;
using LogLevel = NLog.LogLevel;

namespace Sunny.Framework.NLog;

public class NLogConfigListener(IOptionsMonitor<LoggerFilterOptions> logLevelOptions) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logLevelOptions.OnChange(_ => { ReloadRules(); });
        ReloadRules();
        return Task.CompletedTask;
    }

    private void ReloadRules()
    {
        var config = LogManager.Configuration;
        if (config == null) return;
        var configTargetNames = config.ConfiguredNamedTargets.Select(t => t.Name);
        var configDefaultRules = config.LoggingRules.Where(t => configTargetNames.Contains(t.RuleName)).ToList();
        config.LoggingRules.Clear();

        foreach (var r in logLevelOptions.CurrentValue.Rules.Where(t => !string.IsNullOrEmpty(t.CategoryName)))
        {
            var mLogLevel = (int?)r.LogLevel;
            var nLogLevel = LogLevel.AllLevels.FirstOrDefault(t => t.Ordinal.Equals(mLogLevel));

            foreach (var dr in configDefaultRules)
            {
                var nLogRule = new LoggingRule(r.CategoryName, nLogLevel, null)
                {
                    FinalMinLevel = nLogLevel
                };
                nLogRule.Targets.Clear();
                dr.Targets.ToList().ForEach(t => nLogRule.Targets.Add(t));
                dr.Filters.ToList().ForEach(t => nLogRule.Filters.Add(t));
                config.LoggingRules.Add(nLogRule);
            }
        }

        configDefaultRules.ToList().ForEach(t => config.LoggingRules.Add(t));
        LogManager.ReconfigExistingLoggers();
    }
}