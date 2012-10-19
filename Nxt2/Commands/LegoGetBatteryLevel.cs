using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO NXT Command: Get Battery Level
    /// </summary>
    [Description("LEGO Command: GetBatteryLevel.")]
    public class LegoGetBatteryLevel : LegoCommand
    {
        /// <summary>
        /// LEGO NXT Command: Get Battery Level
        /// </summary>
        public LegoGetBatteryLevel()
            : base(5, NxtDirectCommand, (byte)LegoCommandCode.GetBatteryLevel)
        { }

        /// <summary>
        /// The matching LEGO Response
        /// </summary>
        /// <param name="responseData"></param>
        /// <returns></returns>
        public override LegoResponse GetResponse(byte[] responseData)
        {
            return new LegoResponseGetBatteryLevel(responseData);
        }
    }
}