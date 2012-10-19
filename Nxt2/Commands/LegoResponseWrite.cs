using System;
using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO Response: Write.
    /// </summary>
    [Description("LEGO Response: Write.")]
    public class LegoResponseWrite : LegoResponse
    {
        /// <summary>
        /// LEGO Response: Write.
        /// </summary>
        public LegoResponseWrite()
            : base(6, LegoCommandCode.Write) { }

        /// <summary>
        /// LEGO Response: Write.
        /// </summary>
        /// <param name="responseData"></param>
        public LegoResponseWrite(byte[] responseData)
            : base(6, LegoCommandCode.Write, responseData) { }

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
        /// The handle to the data.
        /// </summary>
        [Description("The handle to the data.")]
        public int Handle
        {
            get
            {
                if (CommandData != null && CommandData.Length == ExpectedResponseSize)
                    return CommandData[3];
                return -1;
            }
            set
            {
                CommandData[3] = (byte)value;
            }
        }

        /// <summary>
        /// The number of bytes written.
        /// </summary>
        [Description("The number of bytes written.")]
        public int BytesWritten
        {
            get
            {
                if (CommandData != null && CommandData.Length == ExpectedResponseSize)
                    return (int)BitConverter.ToUInt16(CommandData, 4);
                return -1;
            }
            set { NxtCommon.SetUShort(CommandData, 4, value); }
        }

    }
}