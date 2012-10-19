using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO Response: OpenWriteData.
    /// </summary>
    [Description("LEGO Response: OpenWriteData.")]
    public class LegoResponseOpenWriteData : LegoResponse
    {
        /// <summary>
        /// LEGO Command: OpenWrite
        /// </summary>
        public LegoResponseOpenWriteData()
            : base(4, LegoCommandCode.OpenWriteData) { }

        /// <summary>
        /// LEGO Command: OpenWrite
        /// </summary>
        /// <param name="responseData"></param>
        public LegoResponseOpenWriteData(byte[] responseData)
            : base(4, LegoCommandCode.OpenWriteData, responseData) { }


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
                if (CommandData != null && CommandData.Length == this.ExpectedResponseSize)
                    return CommandData[3];
                return -1;
            }
            set
            {
                CommandData[3] = (byte)value;
            }
        }

    }
}