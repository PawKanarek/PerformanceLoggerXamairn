namespace PerformanceLoggerXamairn
{
    /// <summary>
    /// Configurable Logger that supports native logging mechanisms
    /// </summary>
    public interface ILoggerProvider
    {
        /// <summary>
        /// Measure new step in performance session, automatically adds caller class, line and method
        /// </summary>
        /// <param name="reference">unique refence string that is used as hook to current performance session</param>
        /// <param name="message">Custom message</param>
        /// <param name="path">Do not set</param>
        /// <param name="member">Do not set</param>
        /// <param name="lineNumber">Do not set</param>
        void Start(string reference, string message, string path, string member, int? lineNumber);

        /// <summary>
        /// Measure new step in performance session, automatically adds caller class, line and method
        /// </summary>
        /// <param name="reference">unique refence string that is used as hook to current performance session</param>
        /// <param name="message">Custom message</param>
        /// <param name="path">Do not set</param>
        /// <param name="member">Do not set</param>
        /// <param name="lineNumber">Do not set</param>
        /// <returns>Time difference [ms] from Start event</returns>
        long Step(string reference, string message, string path, string member, int? lineNumber);

        /// <summary>
        /// Removes performance measuring session, automatically adds caller class, line and metho
        /// </summary>
        /// <param name="reference">unique refence string that is used as hook to current performance session</param>
        /// <param name="message">Custom message</param>
        /// <param name="path">Do not set</param>
        /// <param name="member">Do not set</param>
        /// <param name="lineNumber">Do not set</param>
        /// <returns>Time difference [ms] from Start event</returns>
        long Stop(string reference, string message, string path, string member, int? lineNumber);

        /// <summary>
        /// Writes custom message, automatically adds caller class, line and metho
        /// </summary>
        /// <param name="message">Custom message</param>
        /// <param name="path">Do not set</param>
        /// <param name="member">Do not set</param>
        /// <param name="lineNumber">Do not set</param>
        void WriteLine(string message, string path, string member, int? lineNumber);
    }
}