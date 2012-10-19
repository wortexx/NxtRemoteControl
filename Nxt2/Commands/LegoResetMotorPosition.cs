using System.ComponentModel;
using System.Xml.Serialization;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// Reset Motor Position
    /// <remarks>Standard return package.</remarks>
    /// </summary>
    [Description("LEGO Command: ResetMotorPosition.")]
    [XmlRoot("LegoResetMotorPosition")]
    public class LegoResetMotorPosition : LegoCommand
    {
        /// <summary>
        /// Reset Motor Position
        /// </summary>
        public LegoResetMotorPosition()
            : base(3, NxtDirectCommand, (byte)LegoCommandCode.ResetMotorPosition, 0x00, 0x00) { }

        /// <summary>
        /// Reset Motor Position
        /// </summary>
        /// <param name="outputPort"></param>
        /// <param name="relative"></param>
        public LegoResetMotorPosition(NxtMotorPort outputPort, bool relative)
            : base(3, NxtDirectCommand, (byte)LegoCommandCode.ResetMotorPosition, 0x00, 0x00)
        {
            OutputPort = outputPort;
            Relative = relative;
        }

        /// <summary>
        /// Output Port 0-2
        /// </summary>
        [Description("The NXT Motor Port")]
        
        public NxtMotorPort OutputPort
        {
            get
            {
                if (this.CommandData == null)
                    return NxtMotorPort.NotConnected;
                return NxtCommon.GetNxtMotorPort(this.CommandData[2]);
            }
            set
            {
                if (this.CommandData == null)
                    this.CommandData = new byte[4];

                this.CommandData[2] = NxtCommon.PortNumber(value);
            }
        }

        /// <summary>
        /// Position relative to last movement or absolute?
        /// </summary>
        [Description("Identifies whether the position is relative to the last movement.")]
        public bool Relative
        {
            get
            {
                if (this.CommandData == null || this.CommandData.Length < 4)
                    return false;
                return (this.CommandData[3] != 0);
            }
            set
            {
                if (this.CommandData == null) this.CommandData = new byte[4];
                this.CommandData[3] = (byte)((value) ? 1 : 0);
            }
        }
    }
}