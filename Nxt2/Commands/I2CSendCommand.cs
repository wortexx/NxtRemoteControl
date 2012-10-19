using System.ComponentModel;
using System.Xml.Serialization;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// Read LEGO Sonar Sensor Data
    /// </summary>
    [Description("LEGO Command: LSWrite(address, cmd).")]
    [XmlRoot("I2CSendCommand")]
    public class I2CSendCommand : LegoLSWrite
    {
        /// <summary>
        /// Send an I2C command to the specified address
        /// </summary>
        public I2CSendCommand()
            : base()
        {
            base.TXData = new byte[] { NxtCommon.DefaultI2CBusAddress, 0x41 };
            base.ExpectedI2CResponseSize = 0;
            base.Port = 0;
            RequireResponse = true;
        }

        /// <summary>
        /// Send an I2C command to the specified address
        /// </summary>
        /// <param name="port"></param>
        /// <param name="address"></param>
        public I2CSendCommand(NxtSensorPort port, int address)
            : base()
        {
            TXData = new byte[] { NxtCommon.DefaultI2CBusAddress, (byte)address };
            ExpectedI2CResponseSize = 0;
            Port = port;
            RequireResponse = true;
        }

        /// <summary>
        /// Send an I2C command to the specified address
        /// </summary>
        /// <param name="port"></param>
        /// <param name="address"></param>
        /// <param name="i2CBusAddress"></param>
        public I2CSendCommand(NxtSensorPort port, int address, int i2CBusAddress)
            : base()
        {
            TXData = new byte[] { (byte)i2CBusAddress, (byte)address };
            ExpectedI2CResponseSize = 0;
            Port = port;
            RequireResponse = true;
        }

        /// <summary>
        /// Command Address
        /// </summary>
        public int Address
        {
            get
            {
                if (TXData == null || TXData.Length < 2)
                    return 0;
                return TXData[1];
            }
            set
            {
                if (TXData == null)
                {
                    TXData = new byte[2];
                    TXData[0] = NxtCommon.DefaultI2CBusAddress;
                }
                TXData[1] = (byte)value;
            }
        }

        /// <summary>
        /// The I2C Bus Address which identifies the sensor on the I2C Bus
        /// <remarks>This is usually 0x02 (NxtCommon.DefaultI2CBusAddress) for most devices</remarks>
        /// </summary>
        public int I2CBusAddress
        {
            get
            {
                if (TXData == null || TXData.Length < 2)
                    return NxtCommon.DefaultI2CBusAddress;
                return TXData[0];
            }
            set
            {
                if (TXData == null)
                    TXData = new byte[2];
                TXData[0] = (byte)value;
            }
        }

    }
}