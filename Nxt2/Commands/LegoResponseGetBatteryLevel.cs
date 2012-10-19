using System;
using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO NXT Response: Get Battery Level
    /// </summary>
    [Description("LEGO Response: GetBatteryLevel.")]
    public class LegoResponseGetBatteryLevel : LegoResponse
    {
        /// <summary>
        /// LEGO NXT Response: Get Battery Level
        /// </summary>
        public LegoResponseGetBatteryLevel()
            : base(5, LegoCommandCode.GetBatteryLevel)
        {
        }

        /// <summary>
        /// LEGO NXT Response: Get Battery Level
        /// </summary>
        /// <param name="responseData"></param>
        public LegoResponseGetBatteryLevel(byte[] responseData)
            : base(5, LegoCommandCode.GetBatteryLevel, responseData) { }

        #region Hide base type DataMembers


        /// <summary>
        /// Hide RequireResponse from proxy and always set it to true.
        /// </summary>
        [Description("Identifies whether to send an acknowledgement back on a command request.")]
        public override bool RequireResponse
        {
            get { return true; }
            set { base.RequireResponse = true; }
        }

        #endregion


        /// <summary>
        /// Voltage in Volts
        /// </summary>
        [Description("Indicates the voltage (in Volts).")]
        public double Voltage
        {
            get { return Millivolts / 1000.0; }
            set { Millivolts = (int)(value * 1000.0); }
        }

        /// <summary>
        /// Millivolts
        /// </summary>
        private int Millivolts
        {
            get { return BitConverter.ToUInt16(CommandData, 3); }
            set { NxtCommon.SetUShort(CommandData, 3, value); }
        }

    }
}