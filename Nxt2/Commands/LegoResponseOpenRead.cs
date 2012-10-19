using System;
using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO Response: OpenRead.
    /// </summary>
    [Description("LEGO Response: OpenRead.")]
    public class LegoResponseOpenRead : LegoResponse
    {
        /// <summary>
        /// LEGO Response: OpenRead
        /// </summary>
        public LegoResponseOpenRead()
            : base(8, LegoCommandCode.OpenRead) { }

        /// <summary>
        /// LEGO Response: OpenRead
        /// </summary>
        /// <param name="responseData"></param>
        public LegoResponseOpenRead(byte[] responseData)
            : base(8, LegoCommandCode.OpenRead, responseData) { }

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
        /// The size of the file.
        /// </summary>
        [Description("The size of the file.")]
        public long FileSize
        {
            get
            {
                if (CommandData.Length == ExpectedResponseSize)
                    return (long)BitConverter.ToUInt32(CommandData, 4);
                return -1;
            }
            set
            {
                NxtCommon.SetUInt32(CommandData, 4, value);
            }
        }

    }
}