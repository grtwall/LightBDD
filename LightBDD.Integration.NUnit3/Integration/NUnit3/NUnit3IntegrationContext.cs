using System;
using LightBDD.Configuration;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;
using LightBDD.Core.Notification;
using NUnit.Framework;

namespace LightBDD.Integration.NUnit3
{
    internal class NUnit3IntegrationContext : IIntegrationContext
    {
        public INameFormatter NameFormatter { get; }
        public IMetadataProvider MetadataProvider { get; }
        public Func<Exception, ExecutionStatus> ExceptionToStatusMapper { get; }
        public IFeatureProgressNotifier FeatureProgressNotifier { get; }
        public Func<object, IScenarioProgressNotifier> ScenarioProgressNotifierProvider { get; }
        public ExecutionExtensionsConfiguration ExecutionExtensions { get; }

        public NUnit3IntegrationContext(LightBddConfiguration configuration)
        {
            NameFormatter = configuration.NameFormatterConfiguration().Formatter;
            MetadataProvider = new NUnit3MetadataProvider(NameFormatter, configuration.StepTypeConfiguration(), configuration.CultureInfoProviderConfiguration().CultureInfoProvider);
            ExceptionToStatusMapper = ex => (ex is IgnoreException || ex is InconclusiveException) ? ExecutionStatus.Ignored : ExecutionStatus.Failed;
            FeatureProgressNotifier = configuration.FeatureProgressNotifierConfiguration().Notifier;
            ScenarioProgressNotifierProvider = configuration.ScenarioProgressNotifierConfiguration().NotifierProvider;
            ExecutionExtensions = configuration.ExecutionExtensionsConfiguration();
        }
    }
}