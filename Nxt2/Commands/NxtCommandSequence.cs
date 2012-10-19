using System.Collections.Generic;
using System.ComponentModel;

namespace Nxt2.Commands
{
    /// <summary>
    /// A sequence of LEGO Commands
    /// </summary>
    [Description("Identifies an ordered sequence of LEGO Commands.")]
    public class NxtCommandSequence
    {
        /// <summary>
        /// The Commands which make up this sequence.
        /// </summary>
        [Description("The Commands which make up this sequence.")]
        public List<LegoCommand> Commands = new List<LegoCommand>();

        /// <summary>
        /// Continue processing commands after an error 
        /// </summary>
        [Description("Continue processing commands after an error.")]
        public bool ContinueOnError;

        /// <summary>
        /// Polling Freqency (ms)
        /// </summary>
        [Description("Indicates the Polling Frequency in milliseconds (-1 = disabled).")]
        public int PollingFrequencyMs;

        /// <summary>
        /// Average Polling Freqency Milliseconds (read-only)
        /// </summary>
        [Description("Average Polling Freqency Milliseconds (read-only)")]
        [Browsable(false)]
        public double AveragePollingFrequencyMs;

        /// <summary>
        /// The original value of the PollingFrequencyMs prior to any adjustment.
        /// </summary>
        [Description("The original value of the PollingFrequencyMs.")]
        [Browsable(false)]
        public int OriginalPollingFrequencyMs;

        #region Constructors

        /// <summary>
        /// A sequence of LEGO Commands
        /// </summary>
        public NxtCommandSequence() { }

        /// <summary>
        /// Generate a Sequence of Initialization Commands
        /// </summary>
        /// <param name="cmds"></param>
        public NxtCommandSequence(params LegoCommand[] cmds)
        {
            if (cmds != null && cmds.Length > 0)
                Commands = new List<LegoCommand>(cmds);
            else
                Commands = null;
        }

        /// <summary>
        /// Generate a Sequence of Polling Commands
        /// </summary>
        /// <param name="pollingFrequencyMs"></param>
        /// <param name="cmds"></param>
        public NxtCommandSequence(int pollingFrequencyMs, params LegoCommand[] cmds)
        {
            OriginalPollingFrequencyMs = PollingFrequencyMs = pollingFrequencyMs;
            if (cmds != null && cmds.Length > 0)
                Commands = new List<LegoCommand>(cmds);
            else
                Commands = null;
        }

        #endregion
    }
}