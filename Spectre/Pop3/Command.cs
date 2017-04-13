namespace Spectre.Pop3
{
    public class Command
    {
        /// <summary>
        /// The command itself.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// The command arguments.
        /// </summary>
        public string Arguments { get; private set; }

        /// <summary>
        /// Creates an instance of <see cref="Command"/>.
        /// </summary>
        /// <param name="commandLine">A command line as it was readed from socket.</param>
        public Command(string commandLine)
        {
            //---- Parse command --------------------------------------------------//
            var commandParts = commandLine.TrimStart().Split(new char[] { ' ' });
            Text = commandParts[0].ToUpper().Trim();
            Arguments = GetArgsText(commandLine, Text);
            //---------------------------------------------------------------------//
        }

        /// <summary>
        /// Get command arguments.
        /// </summary>
        /// <param name="commandLine">A command line as it was readed from socket.</param>
        /// <param name="text">The command itself.</param>
        /// <returns>The command arguments</returns>
        private string GetArgsText(string commandLine, string text)
        {
            var buff = commandLine.Trim();

            if (buff.Length >= text.Length)
            {
                buff = buff.Substring(text.Length);
            }

            buff = buff.Trim();

            return buff;
        }
    }
}
