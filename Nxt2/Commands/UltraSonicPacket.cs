using System.ComponentModel;

namespace Nxt2.Commands
{
    /// <summary>
    /// UltraSonic Variables
    /// </summary>
    [Description("UltraSonic Variables")]
    public enum UltraSonicPacket
    {
        /// <summary>
        /// Factory Zero
        /// </summary>
        FactoryZero = 0x11,
        /// <summary>
        /// ContinuousMeasurementInterval
        /// </summary>
        ContinuousMeasurementInterval = 0x40,
        /// <summary>
        /// CommandState
        /// </summary>
        CommandState = 0x41,
        /// <summary>
        /// ReadMeasurement0
        /// </summary>
        ReadMeasurement1 = 0x42,
    }
}