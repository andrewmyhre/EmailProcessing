using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using EmailProcessing.Configuration;
using log4net;

namespace EmailProcessing
{
    public class EmailWatcher : IEmailWatcher
    {
        private static readonly EmailQueue EmailQueue = new EmailQueue();
        private readonly string _watchLocation;
        private readonly string _failedLocation;
        private readonly string _deliveredLocation;
        private readonly IEmailPackageSerialiser _packageSerialiser;
        private readonly EmailProcessingConfigurationSection _emailProcessingConfiguration;
        private ILog log = LogManager.GetLogger(typeof(EmailWatcher));
        private static readonly BackgroundWorker worker = new BackgroundWorker();
        private readonly object lockObject=new object();

        private FileSystemWatcher _fsWatcher;
        public event EventHandler<EmailToSendArgs> OnMailToSend;
        public void StartWatching()
        {
            _fsWatcher = new FileSystemWatcher(_watchLocation);
            _fsWatcher.Changed += _fsWatcher_Changed;
            _fsWatcher.Created += _fsWatcher_Changed;
            _fsWatcher.EnableRaisingEvents = true;

            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.WorkerSupportsCancellation=true;

            FlushQueue();
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (worker.CancellationPending)
            {
                worker.Dispose();
                return;
            }

            do System.Threading.Thread.Sleep(5000); while (worker.IsBusy);

            worker.RunWorkerAsync();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (EmailQueue.Count==0)
                return;

            do
            {
                var key = EmailQueue.Keys.First();
                var message = EmailQueue[key];
                string packagePath = Path.Combine(_watchLocation, key + ".xml");

                string outputLocation = _deliveredLocation;
                try
                {
                    if (OnMailToSend != null)
                    {
                        var args = new EmailToSendArgs() {Message = message};
                        OnMailToSend(this, args);

                        if (args.SendingFailed)
                        {
                            outputLocation = _failedLocation;
                        }
                    }
                }
                catch (Exception exception)
                {
                    outputLocation = _failedLocation;
                    log.Fatal("Failed to send email package " + key.ToString(), exception);
                }

                // create a directory for the package
                string deliveryPath = Path.Combine(outputLocation, key.ToString());
                Directory.CreateDirectory(deliveryPath);

                // copy attachments to delivery folder
                if (message.Attachments != null)
                {
                    foreach (string attachment in message.Attachments)
                        File.Copy(attachment, Path.Combine(deliveryPath, Path.GetFileName(attachment)), true);
                }

                // copy the email package to delivery folder
                lock (lockObject)
                {
                    if (File.Exists(Path.Combine(outputLocation, key.ToString() + ".xml")))
                    {
                        try
                        {
                            File.Delete(Path.Combine(outputLocation, key.ToString() + ".xml"));
                        }  catch (Exception deleteException)
                        {
                            // probably something using the file... abort
                            log.WarnFormat("Could not move email package to output location, probably in use. Will retry later.\nSource: {0}\nLocation:{1}\nMessage:{2}",
                                packagePath,
                                Path.Combine(outputLocation, key.ToString() + ".xml"), deleteException);
                            continue;
                        }
                    }
                    File.Move(packagePath, Path.Combine(outputLocation, key.ToString() + ".xml"));

                    EmailQueue.Remove(key);

                    log.InfoFormat("Processed email package {0}", key);
                }

            } while (EmailQueue.Count > 0 && !worker.CancellationPending);
        }


        private void FlushQueue()
        {
            foreach(var file in Directory.GetFiles(_watchLocation, "*.xml"))
            {
                ProcessFile(file);
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
            // wait for a moment so file locks have a chance to be released
            System.Threading.Thread.Sleep(300);

            if (!File.Exists(path)) return;
            var message = _packageSerialiser.Deserialize(File.ReadAllText(path));
            message.PackageLocation = path;
            Guid id = Guid.Empty;
            
            if (!Guid.TryParse(Path.GetFileNameWithoutExtension(path), out id))
            {
                log.WarnFormat("{0} does not appear to be a valid email package", path);
                if (File.Exists(Path.Combine(_failedLocation, Path.GetFileName(path))))
                {
                    try
                    {
                        File.Delete(Path.Combine(_failedLocation, Path.GetFileName(path)));
                        File.Move(path, Path.Combine(_failedLocation, Path.GetFileName(path)));
                    }
                    catch (Exception)
                    {
                    }
                    
                }
                
                return;
            }
            
            if (!EmailQueue.ContainsKey(id))
                EmailQueue.Add(id, message);

            if (!worker.IsBusy)
                worker.RunWorkerAsync();

            // TODO: queue should invoke this
            //if (OnMailToSend != null)
            //OnMailToSend(this, new EmailToSendArgs(){Message=message, PackagePath = path});
        }

        public EmailWatcher(IEmailPackageSerialiser packageSerialiser, EmailProcessingConfigurationSection emailProcessingConfiguration)
        {
            _packageSerialiser = packageSerialiser;
            _emailProcessingConfiguration = emailProcessingConfiguration;
            _watchLocation = _emailProcessingConfiguration.PickupLocation;
            _failedLocation = _emailProcessingConfiguration.FailedLocation;
            _deliveredLocation = _emailProcessingConfiguration.DeliveredLocation;
        }

        public EmailWatcher(string watchLocation, IEmailPackageSerialiser packageSerialiser)
        {
            _watchLocation = watchLocation;
            _packageSerialiser = packageSerialiser;
        }

        public void Dispose()
        {
            if (worker.IsBusy)
                worker.CancelAsync();
            worker.Dispose();

            _fsWatcher.Dispose();
        }
    }
}