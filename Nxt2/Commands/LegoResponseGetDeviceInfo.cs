using System;
using System.ComponentModel;
using System.Text;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO NXT Response: Get Device Info
    /// </summary>
    [Description("LEGO Response: GetDeviceInfo.")]
    public class LegoResponseGetDeviceInfo : LegoResponse
    {
        /// <summary>
        /// LEGO NXT Response: Get Device Info
        /// </summary>
        public LegoResponseGetDeviceInfo()
            : base(33, LegoCommandCode.GetDeviceInfo) { }

        /// <summary>
        /// LEGO NXT Response: Get Device Info
        /// </summary>
        /// <param name="responseData"></param>
        public LegoResponseGetDeviceInfo(byte[] responseData)
            : base(33, LegoCommandCode.GetDeviceInfo, responseData) { }

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
        /// The descriptive name of the NXT brick
        /// </summary>
        [Description("The descriptive name of the NXT brick.")]
        public string BrickName
        {
            get
            {
                if (CommandData == null || CommandData.Length < 33)
                    return string.Empty;

                return NxtCommon.DataToString(CommandData, 3, 15);
            }
            set
            {

                string newValue = value ?? string.Empty;
                if (newValue.Length > 14)
                    newValue = newValue.Substring(0, 14);

                if (CommandData == null || CommandData.Length < 33)
                {
                    byte[] oldData = this.CommandData;
                    this.CommandData = new byte[33];
                    if (oldData != null) oldData.CopyTo(this.CommandData, 0);
                }
                NxtCommon.StringToData(newValue, newValue.Length + 1).CopyTo(this.CommandData, 3);
            }
        }

        /// <summary>
        /// The Bluetooth address
        /// </summary>
        [Description("The Bluetooth address.")]
        public string BluetoothAddress
        {
            get
            {
                if (CommandData == null || CommandData.Length < 33)
                    return string.Empty;

                var sb = new StringBuilder();
                for (var ix = 18; ix < 25; ix++)
                    sb.Append(CommandData[ix] + ".");
                sb.Length--;
                return sb.ToString();

            }
            set
            {
                string[] values = value.Split('.');
                if (values.Length != 7)
                    throw new InvalidOperationException("Bluetooth address is not valid.");

                int ix = 18;
                foreach (string number in values)
                {
                    byte v;
                    if (byte.TryParse(number, out v))
                    {
                        this.CommandData[ix] = v;
                    }
                    ix++;
                }
            }
        }

        /// <summary>
        /// The Bluetooth signal strength
        /// </summary>
        [Description("The Bluetooth signal strength.")]
        public long BluetoothSignalStrength
        {
            get
            {
                if (CommandData.Length >= 33)
                    return BitConverter.ToUInt32(CommandData, 25);
                return -1;
            }
            set
            {
                if (CommandData.Length >= 33)
                    NxtCommon.SetUInt32(CommandData, 25, value);
            }
        }

        /// <summary>
        /// The amount of memory available
        /// </summary>
        [Description("The amount of memory available.")]
        public long FreeMemory
        {
            get
            {
                if (CommandData.Length >= 33)
                    return BitConverter.ToUInt32(CommandData, 29);
                return -1;
            }
            set
            {
                if (CommandData.Length >= 33)
                    NxtCommon.SetUInt32(CommandData, 29, value);
            }
        }

    }
}