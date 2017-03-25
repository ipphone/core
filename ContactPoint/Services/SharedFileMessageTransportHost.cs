using System;
using System.IO;
using System.Text;
using System.Threading;
using ContactPoint.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace ContactPoint.Services
{
    class SharedFileMessageTransportHost : IDisposable
    {
        private static readonly string DirectoryName;
        private static readonly string FileName;
        private static readonly JsonSerializerSettings SerializerSettings;

        private bool _pleaseStop;

        public event Action<object> MessageReceived;

        static SharedFileMessageTransportHost()
        {
            DirectoryName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ContactPoint");
            FileName = Path.Combine(DirectoryName, "ipphone.pipe");
            SerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver(),
                TypeNameHandling = TypeNameHandling.All,
                Converters = new JsonConverter[] { new KeyValuePairConverter() }
            };
        }

        public SharedFileMessageTransportHost()
        {
            Logger.LogNotice("Starting host instance of shared file message transport");
            if (!Directory.Exists(DirectoryName))
            {
                Directory.CreateDirectory(DirectoryName);
            }

            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }

            if (!ThreadPool.QueueUserWorkItem(o =>
            {
                Logger.LogNotice("Started host instance of shared file message transport");
                var stream = File.Open(FileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Write);
                try
                {
                    var pending = false;
                    var lastLength = stream.Length;
                    var fixedLength = lastLength;
                    while (!_pleaseStop)
                    {
                        if (!stream.CanRead)
                        {
                            Logger.LogNotice("Preparing to receive message");
                            pending = true;
                        }
                        else if (stream.Length > lastLength)
                        {
                            lastLength = stream.Length;
                            pending = false;
                            Thread.Sleep(100);
                        }
                        else if (stream.Length == lastLength && lastLength > fixedLength && !pending)
                        {
                            Logger.LogNotice("Preparing to receive message");
                            pending = true;
                            Thread.Sleep(100);
                        }
                        else if (pending)
                        {
                            try
                            {
                                Logger.LogNotice("Receiving message");
                                var position = (int) stream.Position;
                                var length = (int) (stream.Length - fixedLength);
                                var buffer = new byte[length];

                                stream.Lock(position, length);
                                stream.Read(buffer, 0, length);
                                stream.Seek(0, SeekOrigin.End);
                                stream.Unlock(position, length);

                                pending = false;
                                fixedLength = stream.Length;

                                var message = Encoding.UTF8.GetString(buffer);
                                Logger.LogNotice("Message received: " + message);
                                OnMessageReceived(message);
                            }
                            catch (Exception e)
                            {
                                Logger.LogError(e);
                            }
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                    throw;
                }
                finally
                {
                    stream.Dispose();
                }
            }))
            {
                throw new InvalidOperationException("Thread pool cannot acquire one more thread");
            }

            Logger.LogNotice("Shared file message transport stopped");
        }

        public static void SendMessage(object message)
        {
            Logger.LogNotice($"Sending message of type {message.GetType()}");
            using (var writer = File.AppendText(FileName))
            {
                writer.Write(JsonConvert.SerializeObject(message, SerializerSettings));
                writer.Flush();
            }

            Logger.LogNotice("Message successfully sent");
        }

        private void OnMessageReceived(string messageString)
        {
            var message = JsonConvert.DeserializeObject(messageString, SerializerSettings);
            ThreadPool.QueueUserWorkItem(m => MessageReceived?.Invoke(m), message);
        }

        public void Dispose()
        {
            _pleaseStop = true;
        }
    }
}
