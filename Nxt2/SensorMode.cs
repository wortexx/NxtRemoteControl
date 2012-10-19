using System.ComponentModel;

namespace Nxt2
{
    /// <summary>
    /// The translation mode of the LEGO NXT sensor.
    /// </summary>
    [Description("The translation mode of the LEGO NXT sensor.")]
    public enum LegoSensorMode
    {
        /// <summary>
        /// RawMode
        /// </summary>
        RawMode = 0x00,
        /// <summary>
        /// BooleanMode
        /// </summary>
        BooleanMode = 0x20,
        /// <summary>
        /// TransitionCountMode
        /// </summary>
        TransitionCountMode = 0x40,
        /// <summary>
        /// PeriodCounterMode
        /// </summary>
        PeriodCounterMode = 0x60,
        /// <summary>
        /// PercentFullScaleMode
        /// </summary>
        PercentFullScaleMode = 0x80,
        /// <summary>
        /// CelsiusMode
        /// </summary>
        CelsiusMode = 0xA0,
        /// <summary>
        /// FahrenheitMode
        /// </summary>
        FahrenheitMode = 0xC0,
        /// <summary>
        /// AngleStepsMode
        /// </summary>
        AngleStepsMode = 0xE0,
        /// <summary>
        /// SlopeMask
        /// </summary>
        SlopeMask = 0x1F,
        /// <summary>
        /// ModeMask
        /// </summary>
        ModeMask = 0xE0,
    }
}
