using System.ComponentModel;

namespace Nxt2
{
    public enum RunState
    {
        /// <summary>
        /// Idle
        /// </summary>
        [Description("Idle")]
        Idle = 0x00,
        /// <summary>
        /// Ramp Up to the specified Power
        /// </summary>
        [Description("Ramp Up to the specified Power")]
        RampUp = 0x10,
        /// <summary>
        /// Run at a Constant Power
        /// </summary>
        [Description("Run at a Constant Power")]
        Constant = 0x20,
        /// <summary>
        /// Ramp Down from the specified Power
        /// </summary>
        [Description("Ramp Down from the specified Power")]
        RampDown = 0x40,
    }
}
