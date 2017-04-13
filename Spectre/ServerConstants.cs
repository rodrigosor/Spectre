namespace Spectre
{
    internal class ServerConstants
    {
        /// <summary>
        /// A moment to wait.
        /// </summary>
        public const int ForAMoment = 100;

        /// <summary>
        /// Maximum Allowed sessions.
        /// </summary>
        public const int MaxSessionsAllowed = 10;

        /// <summary>
        /// A boolean representing forever.
        /// </summary>
        public const bool Forever = true;

        /// <summary>
        /// Holds session idle timeout (default: 1,44 minute).
        /// </summary>
        public const int SessionIdleTimeOut = 86400;

        /// <summary>
        /// Holds command ilde timeout (default: 1 minute).
        /// </summary>
        public const int CommandIdleTimeOut = 60000;
    }
}
