using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO Response: Close
    /// </summary>
    [Description("LEGO Response: Close.")]
    public class LegoResponseClose : LegoResponse
    {
        /// <summary>
        /// LEGO Response: Close.
        /// </summary>
        public LegoResponseClose()
            : base(4, LegoCommandCode.Close) { }

        /// <summary>
        /// LEGO Response: Close.
        /// </summary>
        /// <param name="responseData"></param>
        public LegoResponseClose(byte[] responseData)
            : base(4, LegoCommandCode.Close, responseData) { }

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
                if (CommandData != null && CommandData.Length == 4)
                {
                    CommandData[3] = (byte)value;
                }
            }
        }

    }
}