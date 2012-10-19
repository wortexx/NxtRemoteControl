using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO NXT Response: Low Speed (I2C) Get Status
    /// </summary>
    [Description("LEGO Response: LSGetStatus.")]
    public class LegoResponseLSGetStatus : LegoResponse
    {

        /// <summary>
        /// LEGO NXT Response: Low Speed (I2C) Get Status
        /// </summary>
        public LegoResponseLSGetStatus()
            : base(4, LegoCommandCode.LSGetStatus) { }

        /// <summary>
        /// LEGO NXT Response: Low Speed (I2C) Get Status
        /// </summary>
        /// <param name="responseData"></param>
        public LegoResponseLSGetStatus(byte[] responseData)
            : base(4, LegoCommandCode.LSGetStatus, responseData) { }

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
        /// The number of bytes ready to read
        /// </summary>
        [Description("The number of bytes ready to read")]
        public int BytesReady
        {
            get
            {
                if (CommandData.Length >= 4)
                    return (int)CommandData[3];
                return -1;
            }
            set
            {
                CommandData[3] = (byte)value;
            }
        }
    }
}