using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO NXT Command: Get Firmware Version
    /// </summary>
    [Description("LEGO Command: GetFirmwareVersion.")]
    public class LegoGetFirmwareVersion : LegoCommand
    {
        /// <summary>
        /// LEGO NXT Command: Get Firmware Version
        /// </summary>
        public LegoGetFirmwareVersion()
            : base(7, LegoCommand.NxtSystemCommand, (byte)LegoCommandCode.GetFirmwareVersion)
        {
        }

        /// <summary>
        /// The matching LEGO Response
        /// </summary>
        /// <param name="responseData"></param>
        /// <returns></returns>
        public override LegoResponse GetResponse(byte[] responseData)
        {
            return new LegoResponseGetFirmwareVersion(responseData);
        }
    }
}