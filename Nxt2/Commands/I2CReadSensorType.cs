using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO Read Sensor Type
    /// </summary>
    [Description("LEGO Command: LSRead(SensorType).")]
    public class I2CReadSensorType : LegoLSWrite
    {
        /// <summary>
        /// LEGO Read Sensor Type
        /// </summary>
        public I2CReadSensorType()
        {
            TXData = new byte[] { NxtCommon.DefaultI2CBusAddress, 0x08 };
            ExpectedI2CResponseSize = 16;
            Port = 0;
            base.RequireResponse = true;
        }

        /// <summary>
        /// LEGO Read Sensor Type
        /// </summary>
        /// <param name="port"></param>
        public I2CReadSensorType(NxtSensorPort port)
        {
            TXData = new byte[] { NxtCommon.DefaultI2CBusAddress, 0x08 };
            ExpectedI2CResponseSize = 16;
            Port = port;
            base.RequireResponse = true;
        }

        /// <summary>
        /// The matching LEGO Response
        /// </summary>
        /// <param name="responseData"></param>
        /// <returns></returns>
        public override LegoResponse GetResponse(byte[] responseData)
        {
            return new I2CResponseSensorType(responseData);
        }

    }
}