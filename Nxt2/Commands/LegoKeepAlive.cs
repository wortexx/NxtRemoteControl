using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO Command: KeepAlive.
    /// </summary>
    [Description("LEGO Command: KeepAlive.")]
    public class LegoKeepAlive : LegoCommand
    {
        /// <summary>
        /// LEGO Command: KeepAlive.
        /// </summary>
        public LegoKeepAlive()
            : base(7, NxtDirectCommand, (byte)LegoCommandCode.KeepAlive)
        {
            base.RequireResponse = true;
        }
    }
}