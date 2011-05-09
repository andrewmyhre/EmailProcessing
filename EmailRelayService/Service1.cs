﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Amazon;
using EmailProcessing;
using log4net;
using log4net.Config;

namespace EmailRelayService
{
    public partial class Service1 : ServiceBase
    {
        private ILog _log = LogManager.GetLogger(typeof(Service1));
        IEmailPackageSerialiser packageSerializer = null;
        IEmailWatcher watcher = null;
        IEmailSender sender = null;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            XmlConfigurator.Configure();

            EmailProcessingConfigurationSection configuration = EmailProcessingConfigurationManager.Section;

            var client = AWSClientFactory.CreateAmazonSimpleEmailServiceClient(configuration.Amazon.Key,
                                                                   configuration.Amazon.Secret);

            if (!Directory.Exists(configuration.PickupLocation)) Directory.CreateDirectory(configuration.PickupLocation);
            if (!Directory.Exists(configuration.DeliveredLocation)) Directory.CreateDirectory(configuration.DeliveredLocation);
            if (!Directory.Exists(configuration.FailedLocation)) Directory.CreateDirectory(configuration.FailedLocation);

            packageSerializer = new EmailPackageSerialiser();
            watcher = new EmailWatcher(packageSerializer);
            sender = EmailSenderFactory.CreateSenderFromConfiguration();

            watcher.OnMailToSend += sender.SendMail;

            watcher.StartWatching();

            _log.Info("Email relay service watching " + configuration.PickupLocation);
        }

        protected override void OnStop()
        {
            watcher.Dispose();
        }
    }
}