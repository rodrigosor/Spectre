
namespace Spectre
{
    /// <summary>
	/// Reply reading return codes.
	/// </summary>
	public enum ReadReplyCode
    {
        /// <summary>
        /// Read completed successfully.
        /// </summary>
        Ok = 0,

        /// <summary>
        /// Read timed out.
        /// </summary>
        TimeOut = 1,

        /// <summary>
        /// Maximum allowed Length exceeded.
        /// </summary>
        LengthExceeded = 2,

        /// <summary>
        /// UnKnown error, eception raised.
        /// </summary>
        UnKnownError = 3,
    }
}
