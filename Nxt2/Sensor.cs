using System;

namespace Nxt2
{
	/// <summary>
	/// Base class for common sensor features.
	/// </summary>
	public class Sensor
	{
		#region Fields

	    public Sensor()
	    {
	        Port = SensorPort.None;
	    }

	    #endregion


		#region Properties

		/// <summary>
		/// The <see cref="Nxt"/> object that is responsible for managing all communication with the NXT.
		/// </summary>
		public NxtBrick Nxt { get; set; }

	    /// <summary>
	    /// The port to which the sensor is currently connected.
	    /// </summary>
	    public SensorPort Port { get; set; }

	    /// <summary>
		/// The type of the sensor. The default is <see cref="SensorType.NoSensor"/>.
		/// </summary>
		protected virtual SensorType Type 
		{
			get
			{
				return SensorType.NoSensor;
			}
		}

		/// <summary>
		/// The mode in which the sensor operates. The default is <see cref="SensorMode.Raw"/>.
		/// </summary>
		protected virtual SensorMode Mode 
		{
			get
			{
				return SensorMode.Raw;
			}
		}

		/// <summary>
		/// The current state of the sensor, including the value of the sensor.
		/// </summary>
		public SensorState State { get; private set; }

		#endregion


		#region Events

		/// <summary>
		/// Event that is raised when the value of the sensor is read.
		/// </summary>
		public event EventHandler<SensorEventArgs> Polled;

		#endregion


		/// <summary>
		/// Connects the sensor to the specified port.
		/// </summary>
		public virtual void Init()
		{
			Nxt.SetInputMode( Port, Type, Mode );
		}


		/// <summary>
		/// Reads the current value of the sensor and raises the <see cref="Polled"/> event when the new value is avaiable.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// If the <see cref="Nxt"/> property is not initialized or the <see cref="Port"/> is not set.
		/// </exception>
		public virtual void Poll()
		{
			// Abort if the NXT is not initialized.
		    if (Nxt == null)
		        throw new InvalidOperationException("Set the Nxt property before reading the value of the sensor.");

		    if( Port == SensorPort.None )
			{
				throw new InvalidOperationException( "The sensor should be connected to a port." );
			}

			// Read the current value of the sensor.
			State = Nxt.GetInputValues( Port );

			// Raise an event to signal that the new value is available.
			if( Polled != null )
			{
				Polled( this, new SensorEventArgs( this ) );
			}		
		}

	}
}
