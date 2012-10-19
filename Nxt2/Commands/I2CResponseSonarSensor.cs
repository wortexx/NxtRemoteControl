using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LegoResponse: I2C UltraSonic Sensor 
    /// </summary>
    [Description("LEGO Response: LSRead(port, UltrasonicSensor).")]
    public class I2CResponseSonarSensor : LegoResponse
    {
        /// <summary>
        /// LegoResponse: I2C Sensor Type
        /// </summary>
        public I2CResponseSonarSensor()
            : base(20, LegoCommandCode.LSRead)
        {
        }

        /// <summary>
        /// LegoResponse: I2C UltraSonic Sensor 
        /// </summary>
        /// <param name="responseData"></param>
        public I2CResponseSonarSensor(byte[] responseData)
            : base(20, LegoCommandCode.LSRead, responseData) { }

        /// <summary>
        /// UltraSonic Sensor Variable
        /// </summary>
        [Description("Ultrasonic Sensor Variable")]
        public int UltraSonicVariable
        {
            get
            {
                if (!Success)
                    return -1;

                return CommandData[4];
            }
            set
            {
                CommandData[4] = (byte)value;
            }
        }
    }
}