using System;

namespace Nxt2
{
	/// <summary>
	/// Available motor modes. This property is a bitfield that can include any combination of the flag bits.
	/// </summary>
	/// <remarks>
	/// Plural naming follows the .NET naming guideline: 
	/// Do use a plural name for enumerations with bit fields as values, also called flags enumerations.
	/// </remarks>
	[Flags]
	public enum MotorModes
	{
		/// <summary>
		/// Motors connected to the specified port(s) will rotate freely.
		/// </summary>
		Coast = 0x00,

		/// <summary>
		/// Turns on the specified motor. Enables pulse-width modulation (PWM) power to port(s) 
		/// according to the specified power value.
		/// </summary>
		On = 0x01,

		/// <summary>
		/// Break the motor after the action is completed. Applies electronic braking to port(s).
		/// </summary>
		Brake = 0x02,

		/// <summary>
		/// Turns on regulation.
		/// </summary>
		Regulated = 0x04
	}
}
