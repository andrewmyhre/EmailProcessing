using System;
using System.IO;
using log4net;

namespace EmailProcessing
{
    public class EmailWatcher : IEmailWatcher
    {
        private readonly string _watchLocation;
        private readonly IEmailPackageSerialiser _packageSerialiser;
        private ILog log = LogManager.GetLogger(typeof(EmailWatcher));

        private FileSystemWatcher _fsWatcher;
        public event EventHandler<EmailToSendArgs> OnMailToSend;
        public void StartWatching()
        {
            _fsWatcher = new FileSystemWatcher(_watchLocation);
            _fsWatcher.Changed += _fsWatcher_Changed;
            _fsWatcher.Created += _fsWatcher_Created;
            _fsWatcher.EnableRaisingEvents = true;

            FlushQueue();
        }

        private void FlushQueue()
        {
            foreach(var file in Directory.GetFiles(_watchLocation, "*.xml"))
            {
                ProcessFile(file);
            }
        }

        void _fsWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Deleted) return;
            
            try
            {
                ProcessFile(e.FullPath);
            }
            catch (Exception ex)
            {
                log.Warn(ex);
            }
        }

        void _fsWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Deleted) return;
            try
            {
                ProcessFile(e.FullPath);
            }
            catch (Exception ex)
            {
                log.Warn(ex);
            }
        }

        private void ProcessFile(string path)
        {
            if (!File.Exists(path)) return;
            var message = _packageSerialiser.Deserialize(File.ReadAllText(path));
            if (OnMailToSend != null)
                OnMailToSend(this, new EmailToSendArgs(){Message=message, PackagePath = path});
        }


        public EmailWatcher(IEmailPackageSerialiser packageSerialiser)
        {
            _packageSerialiser = packageSerialiser;
            _watchLocation = EmailProcessingConfigurationManager.Section.PickupLocation;
        }

        public EmailWatcher(string watchLocation, IEmailPackageSerialiser packageSerialiser)
        {
            _watchLocation = watchLocation;
            _packageSerialiser = packageSerialiser;
        }

        public void Dispose()
        {
            _fsWatcher.Dispose();
        }
    }
}