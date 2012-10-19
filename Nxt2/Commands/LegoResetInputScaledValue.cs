using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// Reset Motor Position
    /// <remarks>Standard return package.</remarks>
    /// </summary>
    [Description("LEGO Command: ResetInputScaledValue.")]
    public class LegoResetInputScaledValue : LegoCommand
    {
        /// <summary>
        /// LEGO Command: ResetInputScaledValue.
        /// </summary>
        public LegoResetInputScaledValue()
            : base(3, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.ResetInputScaledValue, 0x00) { }

        /// <summary>
        /// LEGO Command: ResetInputScaledValue.
        /// </summary>
        /// <param name="inputPort"></param>
        public LegoResetInputScaledValue(NxtSensorPort inputPort)
            : base(LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.ResetInputScaledValue, 0x00)
        {
            this.InputPort = inputPort;
        }


        /// <summary>
        /// Input Port 0-3
        /// </summary>
        [Description("The input port on the NXT brick.")]
        public NxtSensorPort InputPort
        {
            get { return NxtCommon.GetNxtSensorPort(this.CommandData[2]); }
            set { this.CommandData[2] = NxtCommon.PortNumber(value); }
        }

    }
}