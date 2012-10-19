using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO Response: BootCommand
    /// </summary>
    [Description("LEGO Response: BootCommand")]
    public class LegoResponseBootCommand : LegoResponse
    {
        /// <summary>
        /// LEGO Response: BootCommand
        /// </summary>
        public LegoResponseBootCommand()
            : base(7, LegoCommandCode.BootCommand)
        {
            base.RequireResponse = true;
        }

        /// <summary>
        /// LEGO Response: BootCommand
        /// </summary>
        /// <param name="responseData"></param>
        public LegoResponseBootCommand(byte[] responseData)
            : base(7, LegoCommandCode.BootCommand, responseData)
        {
            base.RequireResponse = true;
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
        /// LEGO NXT Boot Acknowledgement.
        /// </summary>
        [Description("LEGO NXT Boot Acknowledgement")]
        public string Message
        {
            get
            {
                if (CommandData == null || CommandData.Length < ExpectedResponseSize)
                    return string.Empty;

                return NxtCommon.DataToString(CommandData, 3);
            }
            set
            {
                if (CommandData == null || CommandData.Length < ExpectedResponseSize)
                {
                    byte[] oldData = CommandData;
                    CommandData = new byte[ExpectedResponseSize];
                    if (oldData != null) oldData.CopyTo(CommandData, 0);
                }
                NxtCommon.SetStringToData(CommandData, 3, value, 4);
            }
        }

    }
}