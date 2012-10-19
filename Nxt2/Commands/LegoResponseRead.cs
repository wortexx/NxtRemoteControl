using System;
using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO Response: Read.
    /// </summary>
    [Description("LEGO Response: Read.")]
    public class LegoResponseRead : LegoResponse
    {
        /// <summary>
        /// LEGO Response: Read
        /// </summary>
        public LegoResponseRead()
            : base(7, LegoCommandCode.Read) { }

        /// <summary>
        /// LEGO Response: Read
        /// </summary>
        /// <param name="responseData"></param>
        public LegoResponseRead(byte[] responseData)
            : base(responseData.Length, LegoCommandCode.Read, responseData)
        {
            this.CommandData = responseData;
            this.ExpectedResponseSize = responseData.Length;
        }

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
                if (CommandData != null && CommandData.Length >= 4)
                    return CommandData[3];
                return -1;
            }
            set
            {
                CommandData[3] = (byte)value;
            }

        }

        /// <summary>
        /// The number of bytes read.
        /// </summary>
        [Description("The number of bytes read.")]
        public int BytesRead
        {
            get
            {
                if (CommandData != null && CommandData.Length >= 6)
                    return (int)BitConverter.ToUInt16(CommandData, 4);
                return -1;
            }
            set
            {
                NxtCommon.SetUShort(CommandData, 4, value);
                ExpectedResponseSize = value + 6;
            }
        }

        /// <summary>
        /// The data read.
        /// </summary>
        [Description("The data read.")]
        public byte[] ReadData
        {
            get
            {
                if (CommandData != null && CommandData.Length == ExpectedResponseSize)
                {
                    byte[] r = new byte[ExpectedResponseSize];
                    System.Buffer.BlockCopy(this.CommandData, 6, r, 0, ExpectedResponseSize);
                    return r;
                }
                return null;
            }
            set
            {
                ExtendCommandData(6 + value.Length);
                System.Buffer.BlockCopy(value, 0, this.CommandData, 6, value.Length);
            }
        }

    }
}