using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO Response: OpenWriteLinear.
    /// </summary>
    [Description("LEGO Response: OpenWriteLinear.")]
    
    public class LegoResponseOpenWriteLinear : LegoResponse
    {
        /// <summary>
        /// LEGO Response: OpenWriteLinear
        /// </summary>
        public LegoResponseOpenWriteLinear()
            : base(4, LegoCommandCode.OpenWriteLinear) { }

        /// <summary>
        /// LEGO Response: OpenWriteLinear
        /// </summary>
        /// <param name="responseData"></param>
        public LegoResponseOpenWriteLinear(byte[] responseData)
            : base(4, LegoCommandCode.OpenWriteLinear, responseData) { }

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