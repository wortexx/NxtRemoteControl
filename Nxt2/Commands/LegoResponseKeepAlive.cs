using System;
using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO Response: KeepAlive.
    /// </summary>
    [Description("LEGO Response: KeepAlive.")]
    public class LegoResponseKeepAlive : LegoResponse
    {
        /// <summary>
        /// LEGO Response: KeepAlive.
        /// </summary>
        public LegoResponseKeepAlive()
            : base(7, LegoCommandCode.KeepAlive)
        {
        }

        /// <summary>
        /// LEGO Response: KeepAlive.
        /// </summary>
        /// <param name="responseData"></param>
        public LegoResponseKeepAlive(byte[] responseData)
            : base(7, LegoCommandCode.KeepAlive, responseData) { }

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
        /// The number of milliseconds between KeepAlive messages
        /// </summary>
        [Description("The number of milliseconds between KeepAlive messages.")]
        public long SleepTimeMilliseconds
        {
            get
            {
                if (CommandData.Length == ExpectedResponseSize)
                    return BitConverter.ToUInt32(CommandData, 3);
                return -1;
            }
            set
            {
                if (CommandData.Length >= ExpectedResponseSize)
                    NxtCommon.SetUInt32(CommandData, 3, value);
            }
        }

    }
}