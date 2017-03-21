namespace ContactPoint.Common
{
    public delegate void ServiceStartedDelegate(object sender);
    public delegate void ServiceStoppedDelegate(object sender, string message);

    /// <summary>
    /// Base CallService service
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// Occurs when service started
        /// </summary>
        event ServiceStartedDelegate Started;

        /// <summary>
        /// Occurs when service stopped
        /// </summary>
        event ServiceStoppedDelegate Stopped;

        /// <summary>
        /// Start service
        /// </summary>
        void Start();

        /// <summary>
        /// Stop service
        /// </summary>
        void Stop();

        /// <summary>
        /// Indicates that service is started
        /// </summary>
        bool IsStarted { get; }
    }
}
