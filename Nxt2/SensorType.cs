using System.ComponentModel;

namespace Nxt2
{
    [Description("Identifies the type of LEGO NXT Sensor.")]
    public enum LegoSensorType
    {
        /// <summary>
        /// No sensor attached
        /// </summary>
        NoSensor = 0x00,
        /// <summary>
        /// NXT or RCX touch sensor
        /// </summary>
        Switch = 0x01,
        /// <summary>
        /// RCX temperature sensor
        /// </summary>
        Temperature = 0x02,
        /// <summary>
        /// RCX light sensor
        /// </summary>
        Reflection = 0x03,
        /// <summary>
        /// RCX encoder
        /// </summary>
        Angle = 0x04,
        /// <summary>
        /// NXT light sensor (with spotlight)
        /// </summary>
        LightActive = 0x05,
        /// <summary>
        /// NXT light sensor (without spotlight)
        /// </summary>
        LightInactive = 0x06,
        /// <summary>
        /// NXT sound sensor (dB scaling)
        /// </summary>
        SoundDb = 0x07,
        /// <summary>
        /// NXT sound sensor (dBA scaling)
        /// </summary>
        SoundDba = 0x08,
        /// <summary>
        /// Custom
        /// </summary>
        Custom = 0x09,
        /// <summary>
        /// I2C Sensor
        /// </summary>
        I2C = 0x0A,
        /// <summary>
        /// I2C 9Volt
        /// </summary>
        I2C_9V = 0x0B, //previously LowSpeed9V
        /// <summary>
        /// I2C High speed?  Unused
        /// </summary>
        HighSpeed = 0x0C,
        /// <summary>
        /// NXT Color sensor in Color mode
        /// </summary>
        ColorFull = 0x0D,
        /// <summary>
        /// NXT Color sensor in light sensor mode with red light
        /// </summary>
        ColorRed = 0x0E,
        /// <summary>
        /// NXT Color sensor in light sensor mode with green light
        /// </summary>
        ColorGreen = 0x0F,
        /// <summary>
        /// NXT Color sensor in light sensor mode with blue light
        /// </summary>
        ColorBlue = 0x10,
        /// <summary>
        /// NXT Color sensor in light sensor mode with no light
        /// </summary>
        ColorNone = 0x11,
        /// <summary>
        /// Total number of types
        /// </summary>
        NumberOfSensorTypes = 0x12,
    }
}
