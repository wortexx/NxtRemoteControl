using System.ComponentModel;
using System.Xml.Serialization;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// Read LEGO Sonar Sensor Data
    /// </summary>
    [Description("LEGO Command: LSRead(port, UltrasonicSensor).")]
    [XmlRoot("I2CReadSonarSensor")]
    public class I2CReadSonarSensor : LegoLSWrite
    {

        /// <summary>
        /// HiTechnic Read Compass Sensor Data
        /// </summary>
        /// <param name="ultraSonicVariable"></param>
        public I2CReadSonarSensor(UltraSonicPacket ultraSonicVariable)
            : base()
        {
            base.TXData = new byte[] { NxtCommon.DefaultI2CBusAddress, (byte)ultraSonicVariable };
            base.ExpectedI2CResponseSize = 1;
            base.Port = 0;
        }

        /// <summary>
        /// HiTechnic Read Compass Sensor Data
        /// </summary>
        /// <param name="port"></param>
        /// <param name="ultraSonicVariable"></param>
        public I2CReadSonarSensor(NxtSensorPort port, UltraSonicPacket ultraSonicVariable)
            : base()
        {
            TXData = new byte[] { NxtCommon.DefaultI2CBusAddress, (byte)ultraSonicVariable };
            ExpectedI2CResponseSize = 1;
            Port = port;
        }

        /// <summary>
        /// The matching LEGO Response
        /// </summary>
        /// <param name="responseData"></param>
        /// <returns></returns>
        public override LegoResponse GetResponse(byte[] responseData)
        {
            return new I2CResponseSonarSensor(responseData);
        }


    }
}