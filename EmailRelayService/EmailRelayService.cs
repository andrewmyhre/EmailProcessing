using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Amazon;
using EmailProcessing;
using EmailProcessing.Configuration;
using log4net;
using log4net.Config;

namespace EmailRelayService
{
    public partial class EmailRelayService : ServiceBase
    {
        private ILog _log = LogManager.GetLogger(typeof(EmailRelayService));
        IEmailPackageSerialiser packageSerializer = null;
        IEmailWatcher watcher = null;
        IEmailSender sender = null;

        public EmailRelayService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            try
            {
                EmailProcessingConfigurationSection configuration = EmailProcessingConfigurationManager.GetConfiguration();

                if (configuration == null)
                {
                    _log.Warn("Could not load configuration");
                } else
                {
                    _log.Info("Configuration loaded");
                    _log.InfoFormat("amazon key: {0}", configuration.Amazon.Key);
                    _log.InfoFormat("amazon key: {0}", configuration.Amazon.Secret);
                }

                var client = AWSClientFactory.CreateAmazonSimpleEmailServiceClient(configuration.Amazon.Key,
                                                                                   configuration.Amazon.Secret);

                if (!Directory.Exists(configuration.PickupLocation))
                    Directory.CreateDirectory(configuration.PickupLocation);
                if (!Directory.Exists(configuration.DeliveredLocation))
                    Directory.CreateDirectory(configuration.DeliveredLocation);
                if (!Directory.Exists(configuration.FailedLocation))
                    Directory.CreateDirectory(configuration.FailedLocation);

                packageSerializer = new EmailPackageSerialiser();
                watcher = new EmailWatcher(packageSerializer, configuration);
                sender = EmailSenderFactory.CreateSenderFromConfiguration(configuration);

                watcher.OnMailToSend += sender.SendMail;

                watcher.StartWatching();

                _log.Info("Email relay service watching " + configuration.PickupLocation);
                var appenders = _log.Logger.Repository.GetAppenders();

                foreach (var appender in appenders)
                {
                    EventLog.WriteEntry("Appender: " + appender.Name);
                }
            } catch(Exception ex)
            {
                _log.Fatal("Failed to start service", ex);
                throw;
            }

            _log.Info("Email relay service started");
        }

        protected override void OnStop()
        {
            watcher.Dispose();
        }
    }
}
