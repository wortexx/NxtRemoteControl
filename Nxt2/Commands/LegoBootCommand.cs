using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO Command: USB Command to Reset the LEGO Brick.
    /// </summary>
    [Description("LEGO Command: USB Command to Reset the LEGO Brick.")]
    public sealed class LegoBootCommand : LegoCommand
    {
        /// <summary>
        /// USB Command to Reset the LEGO Brick.
        /// </summary>
        public LegoBootCommand()
            : base(7, NxtSystemCommand, (byte)LegoCommandCode.BootCommand)
        {
            ExtendCommandData(21);
            NxtCommon.SetStringToData(CommandData, 2, "Let's dance: SAMBA", 19);
        }
    }
}