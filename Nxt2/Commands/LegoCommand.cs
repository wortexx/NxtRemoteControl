using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// The base type for all LEGO Commands
    /// </summary>
    [Description("Any command sequence which can be sent to the LEGO NXT Brick.")]
    public class LegoCommand : ICloneable
    {
        #region Static Members
        /// <summary>
        /// Indicates a LEGO NXT Direct Command
        /// </summary>
        [Description("Indicates a LEGO NXT Direct Command")]
        public const byte NxtDirectCommand = 0x00;

        /// <summary>
        /// Indicates a LEGO NXT System Command
        /// </summary>
        [Description("Indicates a LEGO NXT System Command")]
        public const byte NxtSystemCommand = 0x01;
        #endregion

        #region Private Members

        private bool? _requireResponse = null;
        /// <summary>
        /// The Expected size of a required LegoResponse
        /// </summary>
        protected internal int internalExpectedResponseSize = 0;
        private int _tryCount = 1;

        /// <summary>
        /// The command buffer 
        /// </summary>
        protected internal byte[] internalCommandData = null;


        /// <summary>
        /// Extend the size of the CommandData.
        /// <remarks>Requires the 2 header bytes to be declared in the constructor.</remarks>
        /// </summary>
        /// <param name="newSize"></param>
        protected void ExtendCommandData(int newSize)
        {
            if (this.CommandData != null && this.CommandData.Length == newSize)
                return;

            byte[] header = this.CommandData;
            if (header == null || header.Length < 2)
                throw new InvalidOperationException("The LegoCommand must declare the first two header bytes in the constructor.");
            this.CommandData = new byte[newSize];
            System.Buffer.BlockCopy(header, 0, this.CommandData, 0, header.Length);
        }

        #endregion

        /// <summary>
        /// The expected response buffer size
        /// </summary>
        [Description("The expected size of the LEGO Response buffer.")]
        public virtual int ExpectedResponseSize
        {
            get { return (RequireResponse) ? internalExpectedResponseSize : 0; }
            set { internalExpectedResponseSize = value; }
        }

        /// <summary>
        /// The LEGO Command data buffer
        /// </summary>
        [Description("The LEGO Command buffer")]
        public virtual byte[] CommandData
        {
            get
            {
                if (internalCommandData == null)
                    return null;

                // Set the RequireResponse flag in the data packet
                this.internalCommandData[0] = (byte)(this.internalCommandData[0] & 0x7F | ((RequireResponse) ? 0x00 : 0x80));

                return internalCommandData;
            }
            set
            {
                byte[] priorData = internalCommandData;
                if (value == null || value.Length < 2)
                    throw new ArgumentOutOfRangeException("LegoCommand.CommandData must be at least 2 bytes");

                internalCommandData = value;
                if (internalCommandData != null && internalCommandData.Length >= 1)
                    _requireResponse = (internalCommandData[0] < 0x80);
                if (priorData != null
                    && priorData.Length >= 2
                    && (internalCommandData[0] != priorData[0] || internalCommandData[1] != priorData[1]))
                {
                    internalCommandData[0] = priorData[0];
                    internalCommandData[1] = priorData[1];
                }
            }
        }

        /// <summary>
        /// Determines whether a response will be sent from the LEGO NXT Brick.
        /// When no respone is required, the TryCount will always be 1.
        /// </summary>
        [Description("Determines whether a response will be sent from the LEGO NXT Brick. \n"
            + "When no respone is required, the TryCount will always be 1.")]
        public virtual bool RequireResponse
        {
            get
            {
                if (_requireResponse != null)
                    return (bool)_requireResponse;

                return (internalExpectedResponseSize > 0);
            }

            set
            {
                _requireResponse = value;
                if (!value)
                    _tryCount = 1;
            }
        }

        /// <summary>
        /// Specifies how many times the command will be attempted (1-20).
        /// When TryCount is more than 1, a response will always be required.
        /// </summary>
        [Description("Specifies how many times the command will be attempted (1-20). \n"
            + "When TryCount is more than 1, a response will always be required.")]
        public virtual int TryCount
        {
            get
            {
                return _tryCount;
            }

            set
            {
                if (value > 1)
                    _requireResponse = true;
                _tryCount = Math.Max(1, Math.Min(20, value));
            }
        }

        /// <summary>
        /// The time a command was sent or response was received
        /// </summary>
        [Description("Indicates the time a command was sent or response was received.")]
        public DateTime TimeStamp;


        #region Constructors

        /// <summary>
        /// The base type for all LEGO Commands
        /// </summary>
        public LegoCommand()
        {
            this.CommandData = new byte[2];
            this.ExpectedResponseSize = 0;
        }

        /// <summary>
        /// The base type for all LEGO Commands
        /// </summary>
        /// <param name="expectedResponseSize"></param>
        /// <param name="commandData"></param>
        public LegoCommand(int expectedResponseSize, params byte[] commandData)
        {
            this.CommandData = commandData;
            this.ExpectedResponseSize = expectedResponseSize;
        }

        #endregion

        #region Helper Properties

        /// <summary>
        /// The LEGO Command Code
        /// </summary>
        public virtual LegoCommandCode LegoCommandCode
        {
            get { return (LegoCommandCode)this.internalCommandData[1]; }
            set { this.internalCommandData[1] = (byte)value; }
        }

        /// <summary>
        /// The type of LEGO Command (System, Direct, Response)
        /// </summary>
        public virtual LegoCommandType CommandType
        {
            get
            {
                if (this.internalCommandData == null)
                    return LegoCommandType.Response;

                return (LegoCommandType)(this.internalCommandData[0] & 0x03);
            }
            set
            {
                if (this.internalCommandData != null)
                {
                    this.internalCommandData[0] = (byte)((int)value & 0x7F | ((RequireResponse) ? 0x00 : 0x80));
                }
            }
        }

        #endregion

        #region Virtual Methods

        /// <summary>
        /// Construct a LEGO NXT Response packet for the specified request
        /// </summary>
        /// <param name="responseData"></param>
        /// <returns>A Generic Lego Response</returns>
        /// <remarks>Override this method to return a custom response</remarks>
        public virtual LegoResponse GetResponse(byte[] responseData)
        {
            var expectedResponseSize = (RequireResponse) ? ExpectedResponseSize : 3;
            var response = new LegoResponse(expectedResponseSize, LegoCommandCode, responseData);
            return response;
        }
        #endregion

        #region IDssSerializable

        /// <summary>
        /// Clone Lego Command
        /// </summary>
        public virtual object Clone()
        {
            LegoCommand target = new LegoCommand();

            target.ExpectedResponseSize = this.ExpectedResponseSize;

            // copy System.Byte[] CommandData
            if (this.CommandData != null)
            {
                target.CommandData = new System.Byte[this.CommandData.GetLength(0)];
                System.Buffer.BlockCopy(this.CommandData, 0, target.CommandData, 0, this.CommandData.GetLength(0));
            }
            target.RequireResponse = this.RequireResponse;
            target.TryCount = this.TryCount;
            target.TimeStamp = this.TimeStamp;
            return target;

        }
        
        #endregion
    }

    #region Lego Command Collections

    #endregion

    #region LEGO NXT API Commands

    #region LegoLSWrite

    #endregion

    #region LegoLSRead

    /// <summary>
    /// LEGO Response: LSRead.  I2C Low Speed Read.
    /// </summary>
    [Description("LEGO Response: LSRead.  I2C Low Speed Read.")]
    public class LegoResponseLSRead : LegoResponse
    {
        /// <summary>
        /// LEGO NXT Response: Low Speed (I2C) Read
        /// </summary>
        public LegoResponseLSRead()
            : base(20, LegoCommandCode.LSRead)
        {
        }

        /// <summary>
        /// LEGO NXT Response: Low Speed (I2C) Read
        /// </summary>
        /// <param name="responseData"></param>
        public LegoResponseLSRead(byte[] responseData)
            : base(20, LegoCommandCode.LSRead, responseData) { }

        #region Hide base type DataMembers

        /// <summary>
        /// Hide RequireResponse from proxy and always set it to true.
        /// </summary>
        [Description("Identifies whether to send an acknowledgement back on a command request.")]
        public override bool RequireResponse
        {
            get { return true; }
            set { base.RequireResponse = true; }
        }

        #endregion

        /// <summary>
        /// The number of bytes read
        /// </summary>
        [Description("The number of bytes read.")]
        public int BytesRead
        {
            get
            {
                if (CommandData != null && CommandData.Length >= 4)
                    return CommandData[3];
                return -1;
            }
            set
            {
                CommandData[3] = (byte)value;
            }
        }

        /// <summary>
        /// The received data
        /// </summary>
        [Description("The received data.")]
        public byte[] RXData
        {
            get
            {
                if (BytesRead < 1 || (CommandData.Length < (4 + BytesRead)))
                    return new byte[0];

                byte[] rxdata = new byte[BytesRead];
                Buffer.BlockCopy(this.CommandData, 4, rxdata, 0, BytesRead);
                return rxdata;
            }
            set
            {
                BytesRead = Math.Min(16, value.Length);
                Buffer.BlockCopy(value, 0, this.CommandData, 4, BytesRead);
            }
        }
    }


    #endregion

    #region LegoSetInputMode
    /// <summary>
    /// LEGO Command: SetInputMode.
    /// <remarks>Standard return package.</remarks>
    /// </summary>
    [Description("LEGO Command: SetInputMode.")]
    
    public class LegoSetInputMode : LegoCommand
    {
        /// <summary>
        /// Range 0-3
        /// </summary>
        private NxtSensorPort _inputPort;
        private LegoSensorType _sensorType;
        private LegoSensorMode _sensorMode;

        /// <summary>
        /// LEGO NXT Command: Set Input Mode
        /// </summary>
        public LegoSetInputMode()
            : base(3, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.SetInputMode, 0x00, 0x00, 0x00)
        {
        }

        /// <summary>
        /// LEGO NXT Command: Set Input Mode
        /// </summary>
        /// <param name="sensorPort"></param>
        /// <param name="sensorType"></param>
        /// <param name="sensorMode"></param>
        public LegoSetInputMode(NxtSensorPort sensorPort, LegoSensorType sensorType, LegoSensorMode sensorMode)
            : base(3, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.SetInputMode, 0x00, 0x00, 0x00)
        {
            InputPort = sensorPort;
            SensorType = sensorType;
            SensorMode = sensorMode;
        }

        /// <summary>
        /// The LEGO NXT Sensor Port
        /// </summary>
        [Description("The input port on the NXT brick.")]
        public NxtSensorPort InputPort
        {
            get { return _inputPort; }
            set
            {
                _inputPort = value;
                this.CommandData[2] = NxtCommon.PortNumber(_inputPort);
            }
        }

        /// <summary>
        /// Sensor Type
        /// </summary>
        public LegoSensorType SensorType
        {
            get { return _sensorType; }
            set
            {
                _sensorType = value;
                this.CommandData[3] = (byte)_sensorType;
            }
        }

        /// <summary>
        /// Sensor Type
        /// </summary>
        [Description("The translation mode of the LEGO NXT sensor.")]
        public LegoSensorMode SensorMode
        {
            get { return _sensorMode; }
            set
            {
                _sensorMode = value;
                this.CommandData[4] = (byte)_sensorMode;
            }
        }
    }

    #endregion

    /// <summary>
    /// LEGO NXT Response: Get Battery Level
    /// </summary>
    [Description("LEGO Response: GetBatteryLevel.")]
    [XmlRootAttribute("LegoResponseGetBatteryLevel")]
    public class LegoResponseGetBatteryLevel : LegoResponse
    {
        /// <summary>
        /// LEGO NXT Response: Get Battery Level
        /// </summary>
        public LegoResponseGetBatteryLevel()
            : base(5, LegoCommandCode.GetBatteryLevel)
        {
        }

        /// <summary>
        /// LEGO NXT Response: Get Battery Level
        /// </summary>
        /// <param name="responseData"></param>
        public LegoResponseGetBatteryLevel(byte[] responseData)
            : base(5, LegoCommandCode.GetBatteryLevel, responseData) { }

        #region Hide base type DataMembers


        /// <summary>
        /// Hide RequireResponse from proxy and always set it to true.
        /// </summary>
        [Description("Identifies whether to send an acknowledgement back on a command request.")]
        public override bool RequireResponse
        {
            get { return true; }
            set { base.RequireResponse = true; }
        }

        #endregion


        /// <summary>
        /// Voltage in Volts
        /// </summary>
        [Description("Indicates the voltage (in Volts).")]
        public double Voltage
        {
            get { return (double)Millivolts / 1000.0; }
            set { Millivolts = (int)(value * 1000.0); }
        }

        /// <summary>
        /// Millivolts
        /// </summary>
        private int Millivolts
        {
            get { return (int)BitConverter.ToUInt16(this.CommandData, 3); }
            set { NxtCommon.SetUShort(this.CommandData, 3, value); }
        }

    }

    /// <summary>
    /// LEGO NXT Command: Get Device Info
    /// </summary>
    [Description("LEGO Command: GetDeviceInfo.")]
    [XmlRootAttribute("LegoGetDeviceInfo")]
    public class LegoGetDeviceInfo : LegoCommand
    {
        /// <summary>
        /// LEGO NXT Command: Get Device Info
        /// </summary>
        public LegoGetDeviceInfo()
            : base(33, LegoCommand.NxtSystemCommand, (byte)LegoCommandCode.GetDeviceInfo)
        {
        }

        /// <summary>
        /// The matching LEGO Response
        /// </summary>
        /// <param name="responseData"></param>
        /// <returns></returns>
        public override LegoResponse GetResponse(byte[] responseData)
        {
            return new LegoResponseGetDeviceInfo(responseData);
        }
    }

    /// <summary>
    /// LEGO NXT Response: Get Device Info
    /// </summary>
    [Description("LEGO Response: GetDeviceInfo.")]
    [XmlRootAttribute("LegoResponseGetDeviceInfo")]
    public class LegoResponseGetDeviceInfo : LegoResponse
    {
        /// <summary>
        /// LEGO NXT Response: Get Device Info
        /// </summary>
        public LegoResponseGetDeviceInfo()
            : base(33, LegoCommandCode.GetDeviceInfo) { }

        /// <summary>
        /// LEGO NXT Response: Get Device Info
        /// </summary>
        /// <param name="responseData"></param>
        public LegoResponseGetDeviceInfo(byte[] responseData)
            : base(33, LegoCommandCode.GetDeviceInfo, responseData) { }

        #region Hide base type DataMembers


        /// <summary>
        /// Hide RequireResponse from proxy and always set it to true.
        /// </summary>
        [Description("Identifies whether to send an acknowledgement back on a command request.")]
        public override bool RequireResponse
        {
            get { return true; }
            set { base.RequireResponse = true; }
        }

        #endregion


        /// <summary>
        /// The descriptive name of the NXT brick
        /// </summary>
        [Description("The descriptive name of the NXT brick.")]
        public string BrickName
        {
            get
            {
                if (this.CommandData == null || this.CommandData.Length < 33)
                    return string.Empty;

                return NxtCommon.DataToString(this.CommandData, 3, 15);
            }
            set
            {

                string newValue = value ?? string.Empty;
                if (newValue.Length > 14)
                    newValue = newValue.Substring(0, 14);

                if (this.CommandData == null || this.CommandData.Length < 33)
                {
                    byte[] oldData = this.CommandData;
                    this.CommandData = new byte[33];
                    if (oldData != null) oldData.CopyTo(this.CommandData, 0);
                }
                NxtCommon.StringToData(newValue, newValue.Length + 1).CopyTo(this.CommandData, 3);
            }
        }

        /// <summary>
        /// The Bluetooth address
        /// </summary>
        [Description("The Bluetooth address.")]
        public string BluetoothAddress
        {
            get
            {
                if (this.CommandData == null || this.CommandData.Length < 33)
                    return string.Empty;

                StringBuilder sb = new StringBuilder();
                for (int ix = 18; ix < 25; ix++)
                    sb.Append(this.CommandData[ix].ToString() + ".");
                sb.Length--;
                return sb.ToString();

            }
            set
            {
                string[] values = value.Split('.');
                if (values.Length != 7)
                    throw new InvalidOperationException("Bluetooth address is not valid.");

                int ix = 18;
                foreach (string number in values)
                {
                    byte v;
                    if (byte.TryParse(number, out v))
                    {
                        this.CommandData[ix] = v;
                    }
                    ix++;
                }
            }
        }

        /// <summary>
        /// The Bluetooth signal strength
        /// </summary>
        [Description("The Bluetooth signal strength.")]
        public long BluetoothSignalStrength
        {
            get
            {
                if (this.CommandData.Length >= 33)
                    return (long)BitConverter.ToUInt32(this.CommandData, 25);
                return -1;
            }
            set
            {
                if (this.CommandData.Length >= 33)
                    NxtCommon.SetUInt32(this.CommandData, 25, value);
            }
        }

        /// <summary>
        /// The amount of memory available
        /// </summary>
        [Description("The amount of memory available.")]
        public long FreeMemory
        {
            get
            {
                if (this.CommandData.Length >= 33)
                    return (long)BitConverter.ToUInt32(this.CommandData, 29);
                return -1;
            }
            set
            {
                if (this.CommandData.Length >= 33)
                    NxtCommon.SetUInt32(this.CommandData, 29, value);
            }
        }

    }


    /// <summary>
    /// LEGO NXT Command: Get Firmware Version
    /// </summary>
    [Description("LEGO Command: GetFirmwareVersion.")]
    [XmlRootAttribute("LegoGetFirmwareVersion")]
    public class LegoGetFirmwareVersion : LegoCommand
    {
        /// <summary>
        /// LEGO NXT Command: Get Firmware Version
        /// </summary>
        public LegoGetFirmwareVersion()
            : base(7, LegoCommand.NxtSystemCommand, (byte)LegoCommandCode.GetFirmwareVersion)
        {
        }

        /// <summary>
        /// The matching LEGO Response
        /// </summary>
        /// <param name="responseData"></param>
        /// <returns></returns>
        public override LegoResponse GetResponse(byte[] responseData)
        {
            return new LegoResponseGetFirmwareVersion(responseData);
        }
    }

    /// <summary>
    /// LEGO NXT Response: Get Firmware Version
    /// </summary>
    [Description("LEGO Response: GetFirmwareVersion.")]
    [XmlRootAttribute("LegoResponseGetFirmwareVersion")]
    public class LegoResponseGetFirmwareVersion : LegoResponse
    {
        /// <summary>
        /// LEGO NXT Response: Get Firmware Version
        /// </summary>
        public LegoResponseGetFirmwareVersion()
            : base(7, LegoCommandCode.GetFirmwareVersion)
        {
        }

        /// <summary>
        /// LEGO NXT Response: Get Firmware Version
        /// </summary>
        /// <param name="responseData"></param>
        public LegoResponseGetFirmwareVersion(byte[] responseData)
            : base(7, LegoCommandCode.GetFirmwareVersion, responseData)
        {
        }

        #region Hide base type DataMembers

        /// <summary>
        /// Hide RequireResponse from proxy and always set it to true.
        /// </summary>
        [Description("Identifies whether to send an acknowledgement back on a command request.")]
        public override bool RequireResponse
        {
            get { return true; }
            set { base.RequireResponse = true; }
        }

        #endregion


        /// <summary>
        /// The minor protocol version number
        /// </summary>
        [Description("The minor protocol version number.")]
        public int MinorProtocolVersion
        {
            get
            {
                if (CommandData.Length >= 4)
                    return CommandData[3];
                return -1;
            }
            set
            {
                if (CommandData.Length >= 4)
                    CommandData[3] = (byte)value;
            }
        }

        /// <summary>
        /// The major protocol version number
        /// </summary>
        [Description("The major protocol version number.")]
        public int MajorProtocolVersion
        {
            get
            {
                if (CommandData.Length >= 5)
                    return CommandData[4];
                return -1;
            }
            set
            {
                if (CommandData.Length >= 5)
                    CommandData[4] = (byte)value;
            }
        }

        /// <summary>
        /// The minor firmware version number
        /// </summary>
        [Description("The minor firmware version number.")]
        public int MinorFirmwareVersion
        {
            get
            {
                if (CommandData.Length >= 6)
                    return CommandData[5];
                return -1;
            }
            set
            {
                if (CommandData.Length >= 6)
                    CommandData[5] = (byte)value;
            }
        }

        /// <summary>
        /// The major firmware version number
        /// </summary>
        [Description("The major firmware version number.")]
        public int MajorFirmwareVersion
        {
            get
            {
                if (CommandData.Length >= 7)
                    return CommandData[6];
                return -1;
            }
            set
            {
                if (CommandData.Length >= 7)
                    CommandData[6] = (byte)value;
            }
        }
    }


    #region LegoSetOutputState
    /// <summary>
    /// LEGO NXT Command: Set Output State
    /// <remarks>Standard return package.</remarks>
    /// </summary>
    [Description("LEGO Command: SetOutputState.")]
    [XmlRootAttribute("LegoSetOutputState")]
    public class LegoSetOutputState : LegoCommand
    {
        #region Constructors

        /// <summary>
        /// LEGO NXT Command: Set Output State
        /// </summary>
        public LegoSetOutputState()
            : base(3, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.SetOutputState, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00) { }

        /// <summary>
        /// LEGO NXT Command: Set Output State
        /// </summary>
        /// <param name="motorPort"></param>
        /// <param name="powerSetPoint"></param>
        /// <param name="mode"></param>
        /// <param name="regulationMode"></param>
        /// <param name="turnRatio"></param>
        /// <param name="runState"></param>
        /// <param name="rotationLimit"></param>
        public LegoSetOutputState(NxtMotorPort motorPort,
                                  int powerSetPoint,
                                  LegoOutputMode mode,
                                  LegoRegulationMode regulationMode,
                                  int turnRatio,
                                  RunState runState,
                                  long rotationLimit)
            : base(3, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.SetOutputState, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00)
        {
            this.MotorPort = motorPort;
            this.PowerSetPoint = powerSetPoint;
            this.Mode = mode;
            this.RegulationMode = regulationMode;
            this.TurnRatio = turnRatio;
            this.RunState = runState;
            this.EncoderLimit = rotationLimit;
        }

        #endregion

        /// <summary>
        /// The Motor Port
        /// </summary>
        [Description("The output port on the NXT brick (or NxtMotorPort.AnyMotorPort for all three.)")]
        
        public NxtMotorPort MotorPort
        {
            get
            {

                if (CommandData.Length == 12)
                    return NxtCommon.GetNxtMotorPort(CommandData[2]);
                return NxtMotorPort.NotConnected;
            }
            set
            {
                this.CommandData[2] = NxtCommon.PortNumber(value);
            }
        }

        /// <summary>
        /// Power Setpoint (range -100 to +100)
        /// </summary>
        [Description("The motor power setting (range -100 to +100).")]
        
        public int PowerSetPoint
        {
            get
            {
                if (CommandData.Length == 12)
                    return (int)NxtCommon.GetSByte(this.CommandData, 3);
                return -1;
            }
            set
            {
                NxtCommon.SetSByte(this.CommandData, 3, value);
            }
        }

        /// <summary>
        /// A limit on the number of motor rotations (360 per rotation).
        /// </summary>
        [Description("A limit on the number of motor rotations (360 per rotation).")]
        
        public long EncoderLimit
        {
            get
            {
                if (CommandData.Length == 12)
                    return (long)(long)BitConverter.ToUInt32(this.CommandData, 8);
                return -1;
            }
            set
            {
                if (this.CommandData == null) this.CommandData = new byte[12];
                NxtCommon.SetUInt32(this.CommandData, 8, value);
            }
        }

        /// <summary>
        /// Mode
        /// </summary>
        [Description("The NXT output mode.")]
        public LegoOutputMode Mode
        {
            get
            {
                if (CommandData.Length == 12)
                    return (LegoOutputMode)CommandData[4];
                return LegoOutputMode.Brake;
            }
            set
            {
                CommandData[4] = (byte)value;
            }
        }

        /// <summary>
        /// RunState
        /// </summary>
        [Description("The Motor Run State")]
        public RunState RunState
        {
            get
            {
                if (CommandData.Length == 12)
                    return (RunState)CommandData[7];
                return RunState.Idle;
            }
            set
            {
                this.CommandData[7] = (byte)value;
            }
        }


        /// <summary>
        /// Lego Regulation Mode
        /// </summary>
        [Description("The NXT regulation mode.")]
        public LegoRegulationMode RegulationMode
        {
            get
            {
                if (CommandData.Length == 12)
                    return (LegoRegulationMode)CommandData[5];
                return LegoRegulationMode.Idle;
            }
            set
            {
                this.CommandData[5] = (byte)value;
            }
        }

        /// <summary>
        /// The Motor Turn Ratio
        /// <remarks>(-100 - 100)</remarks>
        /// </summary>
        [Description("The Motor Turn Ratio")]
        public int TurnRatio
        {
            get
            {
                if (CommandData.Length == 12)
                    return (int)NxtCommon.GetSByte(this.CommandData, 6);
                return -1;
            }
            set
            {
                NxtCommon.SetSByte(this.CommandData, 6, value);
            }
        }

        /// <summary>
        /// String representation of SetOutputState with parameters.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{7} SetOutputState(port={0},power={1},rotations={2},mode={3},runstate={4},regulation={5},turnratio={6})",
                this.MotorPort,
                this.PowerSetPoint,
                this.EncoderLimit,
                this.Mode,
                this.RunState,
                this.RegulationMode,
                this.TurnRatio,
                this.TimeStamp.ToString("HH:mm:ss.fffffff"));
        }
    }

    #endregion


    #region LegoGetButtonState
    /// <summary>
    /// LEGO NXT Command: Get Button State
    /// NOTE: 0x01, 0x94, 0x01, 0x00, 0x04, 0x00, 0x20, 0x00, 0x04, 0x00
    /// </summary>
    [Description("LEGO Command: GetButtonState.")]
    [XmlRootAttribute("LegoGetButtonState")]
    public class LegoGetButtonState : LegoCommand
    {
        /// <summary>
        /// LEGO NXT Command: Get Button State
        /// </summary>
        public LegoGetButtonState()
            : base(13, LegoCommand.NxtSystemCommand, (byte)LegoCommandCode.ReadIOMap, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00)
        {
            RequireResponse = true;

            // Set the Module
            NxtCommon.SetUInt32(this.CommandData, 2, 0x00040001);

            // Set the offset
            NxtCommon.SetUShort(this.CommandData, 6, 0x0020);

            // Set the number of bytes to read
            NxtCommon.SetUShort(this.CommandData, 8, 0x0004);
        }

        /// <summary>
        /// The matching LEGO NXT Response
        /// </summary>
        /// <param name="responseData"></param>
        /// <returns></returns>
        public override LegoResponse GetResponse(byte[] responseData)
        {
            return new LegoResponseGetButtonState(responseData);
        }

        /// <summary>
        /// Hide RequireResponse from proxy and always set it to true.
        /// </summary>
        [Description("Identifies whether to send an acknowledgement back on a command request.")]
        public override bool RequireResponse
        {
            get { return true; }
            set { base.RequireResponse = true; }
        }

    }

    /// <summary>
    /// LEGO NXT Response: Get Button State
    /// NOTE: Because this return package does not return the index of the button queried,
    /// something special will have to be done.
    /// </summary>
    [Description("LEGO Response: GetButtonState.")]
    [XmlRootAttribute("LegoResponseGetButtonState")]
    public class LegoResponseGetButtonState : LegoResponse
    {
        /// <summary>
        /// LEGO NXT Response: Get Button State
        /// </summary>
        public LegoResponseGetButtonState()
            : base(13, LegoCommandCode.ReadIOMap) { }

        /// <summary>
        /// LEGO NXT Response: Get Button State
        /// </summary>
        /// <param name="responseData"></param>
        public LegoResponseGetButtonState(byte[] responseData)
            : base(13, LegoCommandCode.ReadIOMap, responseData)
        {
        }

        #region Hide base type DataMembers


        /// <summary>
        /// Hide RequireResponse from proxy and always set it to true.
        /// </summary>
        [Description("Identifies whether to send an acknowledgement back on a command request.")]
        public override bool RequireResponse
        {
            get { return true; }
            set { base.RequireResponse = true; }
        }

        #endregion

        /// <summary>
        /// Determine if the LEGO response packet was a request for Button state.
        /// </summary>
        public override bool Success
        {
            get
            {
                if (!base.Success)
                    return false;

                if (CommandData[2] != 0x00 // success
                    || CommandData[3] != 0x01 // offset byte 1
                    || CommandData[4] != 0x00 // offset byte 2
                    || CommandData[5] != 0x04 // offset byte 3
                    || CommandData[6] != 0x00 // offset byte 4
                    || CommandData[7] != 0x04 // data length byte 1
                    || CommandData[8] != 0x00 // data length byte 2
                   )
                    return false;

                return true;
            }
        }
        /// <summary>
        /// Determine if the LEGO response packet was a request for Button state.
        /// </summary>
        /// <param name="legoResponse"></param>
        /// <returns></returns>
        public static bool IsValidButtonStateResponse(LegoResponse legoResponse)
        {
            if ((legoResponse == null)
                || (legoResponse.CommandData == null)
                || legoResponse.CommandData.Length != 13
                || legoResponse.CommandType != LegoCommandType.Response // Lego Return Code
                || legoResponse.LegoCommandCode != LegoCommandCode.ReadIOMap
                || legoResponse.ErrorCode != LegoErrorCode.Success // success
                || legoResponse.CommandData[3] != 0x01 // offset byte 1
                || legoResponse.CommandData[4] != 0x00 // offset byte 2
                || legoResponse.CommandData[5] != 0x04 // offset byte 3
                || legoResponse.CommandData[6] != 0x00 // offset byte 4
                || legoResponse.CommandData[7] != 0x04 // data length byte 1
                || legoResponse.CommandData[8] != 0x00 // data length byte 2
                )
                return false;

            return true;
        }

        /// <summary>
        /// LEGO NXT Response: Get Button State
        /// </summary>
        /// <param name="right"></param>
        /// <param name="left"></param>
        /// <param name="enter"></param>
        /// <param name="cancel"></param>
        public LegoResponseGetButtonState(bool right, bool left, bool enter, bool cancel)
            : base(13, LegoCommandCode.ReadIOMap)
        {
            PressedRight = right;
            PressedLeft = left;
            PressedEnter = enter;
            PressedCancel = cancel;
        }


        /// <summary>
        /// The number of bytes read from IO Mapped data
        /// </summary>
        private int BytesRead
        {
            get { return NxtCommon.GetUShort(this.CommandData, 7); }
        }

        /// <summary>
        /// The IO Mapped Data which is returned
        /// </summary>
        private byte[] MappedData
        {
            get
            {
                int bytesRead = this.BytesRead;
                if (CommandData == null || bytesRead == 0 || CommandData.Length < (9 + bytesRead))
                    return null;

                byte[] mappedData = new byte[bytesRead];
                for (int ix = 0; ix < bytesRead; ix++)
                    mappedData[ix] = CommandData[ix + 9];

                return mappedData;
            }
        }


        /// <summary>
        /// Right Button is pressed
        /// </summary>
        [Description("Indicates that the right button was pressed.")]
        
        public bool PressedRight
        {
            get
            {
                if (CommandData == null || CommandData.Length < 13)
                    return false;

                return (CommandData[10] & 0x80) == 0x80;
            }
            set
            {
                CommandData[10] = (byte)(value ? 0x80 : 0x00);
            }
        }

        /// <summary>
        /// Left button is pressed.
        /// </summary>
        [Description("Indicates the left button was pressed.")]
        
        public bool PressedLeft
        {
            get
            {
                if (CommandData == null || CommandData.Length < 13)
                    return false;

                return (CommandData[11] & 0x80) == 0x80;
            }
            set
            {
                CommandData[11] = (byte)(value ? 0x80 : 0x00);
            }
        }

        /// <summary>
        /// Enter button is pressed
        /// </summary>
        [Description("Indicates that the Enter button was pressed.")]
        
        public bool PressedEnter
        {
            get
            {
                if (CommandData == null || CommandData.Length < 13)
                    return false;

                return (CommandData[12] & 0x80) == 0x80;
            }
            set
            {
                CommandData[12] = (byte)(value ? 0x80 : 0x00);
            }
        }


        /// <summary>
        /// Cancel Button is pressed
        /// </summary>
        [Description("Indicates that the Cancel button was pressed.")]
        public bool PressedCancel
        {
            get
            {
                if (CommandData == null || CommandData.Length < 13)
                    return false;

                return (CommandData[9] & 0x80) == 0x80;
            }
            set
            {
                CommandData[9] = (byte)(value ? 0x80 : 0x00);
            }
        }

    }

    #endregion


    #region LegoGetInputValues

    /// <summary>
    /// LEGO NXT Command: GetInputValues
    /// </summary>
    [Description("LEGO Command: GetInputValues.")]
    [XmlRootAttribute("LegoGetInputValues")]
    public class LegoGetInputValues : LegoCommand
    {
        /// <summary>
        /// Range 0-3
        /// </summary>
        private NxtSensorPort _inputPort;

        /// <summary>
        /// LEGO NXT Command: GetInputValues
        /// </summary>
        public LegoGetInputValues()
            : base(16, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.GetInputValues, 0x00)
        {
            base.RequireResponse = true;
        }

        /// <summary>
        /// LEGO NXT Command: GetInputValues
        /// </summary>
        /// <param name="inputPort"></param>
        public LegoGetInputValues(NxtSensorPort inputPort)
            : base(16, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.GetInputValues, 0x00)
        {
            base.RequireResponse = true;
            InputPort = inputPort;
        }

        /// <summary>
        /// LEGO NXT Command: GetInputValues
        /// </summary>
        /// <param name="responseData"></param>
        /// <returns></returns>
        public override LegoResponse GetResponse(byte[] responseData)
        {
            return new LegoResponseGetInputValues(responseData);
        }

        /// <summary>
        /// Hide RequireResponse from proxy and always set it to true.
        /// </summary>
        [Description("Identifies whether to send an acknowledgement back on a command request.")]
        public override bool RequireResponse
        {
            get { return true; }
            set { base.RequireResponse = true; }
        }

        /// <summary>
        /// 0,1,2,3
        /// </summary>
        [Description("The input port on the NXT brick.")]
        public NxtSensorPort InputPort
        {
            get { return _inputPort; }
            set
            {
                _inputPort = value;
                this.CommandData[2] = NxtCommon.PortNumber(_inputPort);
            }
        }
    }

    /// <summary>
    /// LEGO NXT Command Response: GetInputValues
    /// </summary>
    [Description("LEGO Response: GetInputValues.")]
    [XmlRootAttribute("LegoResponseGetInputValues")]
    public class LegoResponseGetInputValues : LegoResponse
    {
        /// <summary>
        /// LEGO NXT Command Response: GetInputValues
        /// </summary>
        public LegoResponseGetInputValues()
            : base(16, LegoCommandCode.GetInputValues)
        {
        }

        /// <summary>
        /// LEGO NXT Command Response: GetInputValues
        /// </summary>
        /// <param name="responseData"></param>
        public LegoResponseGetInputValues(byte[] responseData)
            : base(16, LegoCommandCode.GetInputValues, responseData) { }

        #region Hide base type DataMembers


        /// <summary>
        /// Hide RequireResponse from proxy and always set it to true.
        /// </summary>
        [Description("Identifies whether to send an acknowledgement back on a command request.")]
        public override bool RequireResponse
        {
            get { return true; }
            set { base.RequireResponse = true; }
        }

        #endregion


        /// <summary>
        /// The input port on the NXT brick.
        /// </summary>
        [Description("The input port on the NXT brick")]
        public NxtSensorPort InputPort
        {
            get
            {
                if (CommandData.Length >= 4)
                    return NxtCommon.GetNxtSensorPort(CommandData[3]);

                return NxtSensorPort.NotConnected;
            }
            set
            {
                if (CommandData.Length >= 4)
                    CommandData[3] = NxtCommon.PortNumber(value);
            }
        }

        /// <summary>
        /// Success
        /// </summary>
        public override bool Success
        {
            get
            {
                if (!base.Success)
                    return false;

                // Check the Valid field
                if (CommandData.Length >= 5)
                    return (CommandData[4] == 0) ? false : true;

                return false;
            }
        }

        /// <summary>
        /// Is the specified sensor Calibrated?
        /// </summary>
        [Description("Is the specified sensor Calibrated?")]
        public bool Calibrated
        {
            get
            {
                if (CommandData.Length >= 6)
                    return (CommandData[5] == 0) ? false : true;
                return false;
            }
            set
            {
                if (CommandData.Length >= 6)
                    CommandData[5] = (byte)((value) ? 1 : 0);
            }
        }

        /// <summary>
        /// The LEGO NXT Sensor Type as defined by LEGO
        /// </summary>
        [Description("The LEGO NXT Sensor Type as defined by LEGO")]
        public LegoSensorType SensorType
        {
            get
            {
                if (CommandData.Length >= 7)
                    return (LegoSensorType)CommandData[6];

                return LegoSensorType.NoSensor;
            }
            set
            {
                if (CommandData.Length >= 7)
                    CommandData[6] = (byte)value;
            }
        }

        /// <summary>
        /// The Sensor Mode
        /// </summary>
        [Description("The Sensor Mode")]
        public LegoSensorMode SensorMode
        {
            get
            {
                if (CommandData.Length >= 8)
                    return (LegoSensorMode)CommandData[7];
                return LegoSensorMode.RawMode;
            }
            set
            {
                if (CommandData.Length >= 8)
                    CommandData[7] = (byte)value;
            }
        }

        /// <summary>
        /// The raw reading from the sensor.
        /// </summary>
        [Description("The raw reading from the sensor.")]
        public int RawValue
        {
            get { return (int)BitConverter.ToUInt16(CommandData, 8); }
            set { NxtCommon.SetUShort(CommandData, 8, value); }
        }

        /// <summary>
        /// The normalized reading from the sensor
        /// </summary>
        [Description("The normalized reading from the sensor.")]
        public int NormalizedValue
        {
            get { return (int)BitConverter.ToUInt16(CommandData, 10); }
            set { NxtCommon.SetUShort(CommandData, 10, value); }
        }

        /// <summary>
        /// The scaled reading from the sensor
        /// </summary>
        [Description("The scaled reading from the sensor.")]
        public int ScaledValue
        {
            get { return (int)BitConverter.ToInt16(CommandData, 12); }
            set { NxtCommon.SetShort(CommandData, 12, value); }
        }


        /// <summary>
        /// The calibrated reading from the sensor
        /// </summary>
        [Description("The calibrated reading from the sensor.")]
        public int CalibratedValue
        {
            get { return (int)BitConverter.ToInt16(CommandData, 14); }
            set { NxtCommon.SetShort(CommandData, 14, value); }
        }
    }

    #endregion

    #region LegoGetOutputState

    /// <summary>
    /// LEGO NXT Command: GetOutputState
    /// </summary>
    [Description("LEGO Command: GetOutputState.")]
    [XmlRootAttribute("LegoGetOutputState")]
    public class LegoGetOutputState : LegoCommand
    {
        /// <summary>
        /// LEGO NXT Command: GetOutputState
        /// </summary>
        public LegoGetOutputState()
            : base(25, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.GetOutputState, 0x00)
        {
            base.RequireResponse = true;
        }

        /// <summary>
        /// LEGO NXT Command: GetOutputState
        /// </summary>
        /// <param name="outputPort"></param>
        public LegoGetOutputState(NxtMotorPort outputPort)
            : base(25, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.GetOutputState, 0x00)
        {
            base.RequireResponse = true;
            OutputPort = outputPort;
        }

        /// <summary>
        /// The matching LEGO Response
        /// </summary>
        /// <param name="responseData"></param>
        /// <returns></returns>
        public override LegoResponse GetResponse(byte[] responseData)
        {
            return new LegoResponseGetOutputState(responseData);
        }

        /// <summary>
        /// Hide RequireResponse from proxy and always set it to true.
        /// </summary>
        [Description("Identifies whether to send an acknowledgement back on a command request.")]
        public override bool RequireResponse
        {
            get { return true; }
            set { base.RequireResponse = true; }
        }

        /// <summary>
        /// 0, 1, 2
        /// </summary>
        [Description("The NXT output port (0,1, or 2).")]
        public NxtMotorPort OutputPort
        {
            get
            {
                return NxtCommon.GetNxtMotorPort(this.CommandData[2]);
            }
            set
            {
                this.CommandData[2] = NxtCommon.PortNumber(value);
            }
        }
    }

    /// <summary>
    /// LEGO NXT Response: Get Output State
    /// </summary>
    [Description("LEGO Response: GetOutputState.")]
    [XmlRootAttribute("LegoResponseGetOutputState")]
    public class LegoResponseGetOutputState : LegoResponse
    {
        /// <summary>
        /// LEGO NXT Response: Get Output State
        /// </summary>
        public LegoResponseGetOutputState()
            : base(25, LegoCommandCode.GetOutputState) { }

        /// <summary>
        /// LEGO NXT Response: Get Output State
        /// </summary>
        /// <param name="responseData"></param>
        public LegoResponseGetOutputState(byte[] responseData)
            : base(25, LegoCommandCode.GetOutputState, responseData) { }

        #region Hide base type DataMembers


        /// <summary>
        /// Hide RequireResponse from proxy and always set it to true.
        /// </summary>
        [Description("Identifies whether to send an acknowledgement back on a command request.")]
        public override bool RequireResponse
        {
            get { return true; }
            set { base.RequireResponse = true; }
        }

        #endregion


        /// <summary>
        /// The NXT Motor port
        /// </summary>
        [Description("The NXT Motor port")]
        public NxtMotorPort MotorPort
        {
            get
            {
                if (CommandData.Length >= 4)
                    return NxtCommon.GetNxtMotorPort(CommandData[3]);
                return NxtMotorPort.NotConnected;
            }
            set
            {
                CommandData[3] = NxtCommon.PortNumber(value);
            }
        }

        /// <summary>
        /// The motor power setting (range -100 to +100)
        /// </summary>
        [Description("The motor power setting (range -100 to +100).")]
        public int PowerSetPoint
        {
            get
            {
                if (CommandData.Length >= 5)
                    return (int)(sbyte)CommandData[4];
                return -1;
            }
            set
            {
                CommandData[4] = (byte)value;
            }
        }

        /// <summary>
        /// The NXT output mode
        /// </summary>
        [Description("The NXT output mode.")]
        public LegoOutputMode Mode
        {
            get
            {
                if (CommandData.Length >= 6)
                    return (LegoOutputMode)CommandData[5];
                return LegoOutputMode.Brake;
            }
            set
            {
                CommandData[5] = (byte)value;
            }
        }

        /// <summary>
        /// The NXT regulation mode
        /// </summary>
        [Description("The NXT regulation mode.")]
        public LegoRegulationMode RegulationMode
        {
            get
            {
                if (CommandData.Length >= 7)
                    return (LegoRegulationMode)CommandData[6];
                return LegoRegulationMode.Idle;
            }
            set
            {
                CommandData[6] = (byte)value;
            }
        }

        /// <summary>
        /// The Motor Turn Ratio
        /// <remarks>(-100 - 100)</remarks>
        /// </summary>
        [Description("Motor Turn Ratio")]
        public int TurnRatio
        {
            get
            {
                if (CommandData.Length >= 8)
                    return CommandData[7];
                return -1;
            }
            set
            {
                CommandData[7] = (byte)value;
            }
        }

        /// <summary>
        /// The Motor running state
        /// </summary>
        [Description("The Motor running state")]
        public RunState RunState
        {
            get
            {
                if (CommandData.Length >= 9)
                    return (RunState)CommandData[8];
                return RunState.Idle;
            }
            set
            {
                CommandData[8] = (byte)value;
            }
        }

        /// <summary>
        /// A limit on the number of motor rotations (360 per rotation).
        /// </summary>
        [Description("The Motor Encoder Limit (360 per rotation).")]
        public long EncoderLimit
        {
            get { return (long)BitConverter.ToUInt32(CommandData, 9); }
            set
            {
                NxtCommon.SetUInt32(CommandData, 9, value);
            }
        }

        /// <summary>
        /// The Motor Encoder Count (360 per rotation).
        /// </summary>
        [Description("The Motor Encoder Count (360 per rotation).")]
        public int EncoderCount
        {
            get { return BitConverter.ToInt32(CommandData, 13); }
            set { NxtCommon.SetUInt32(CommandData, 13, value); }
        }

        /// <summary>
        /// The Motor Block Tachometer Count (360 per rotation).
        /// </summary>
        [Description("The Motor Block Tachometer Count (360 per rotation).")]
        public int BlockTachoCount
        {
            get { return BitConverter.ToInt32(CommandData, 17); }
            set { NxtCommon.SetUInt32(CommandData, 17, value); }
        }

        /// <summary>
        /// The Resettable Encoder Count (360 per rotation).
        /// </summary>
        [Description("The Resettable Encoder Count (360 per rotation).")]
        public long ResettableCount
        {
            get { return (long)BitConverter.ToInt32(CommandData, 21); }
            set { NxtCommon.SetUInt32(CommandData, 21, value); }
        }

        /// <summary>
        /// String representation of GetOutputState with parameters.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{10} GetOutputState(port={0},power={1},rotations={2},mode={3},runstate={4},regulation={5},turnratio={6},encoder={7},block={8},resettable={9})",
                this.MotorPort,
                this.PowerSetPoint,
                this.EncoderLimit,
                this.Mode,
                this.RunState,
                this.RegulationMode,
                this.TurnRatio,
                this.EncoderCount,
                this.BlockTachoCount,
                this.ResettableCount,
                this.TimeStamp.ToString("HH:mm:ss.fffffff"));
        }
    }


    #endregion

    #region LegoPlayTone

    /// <summary>
    /// Play a tone on the NXT
    /// </summary>
    /// <remarks>Standard return package.</remarks>    
    [Description("LEGO Command: PlayTone.")]
    [XmlRootAttribute("LegoPlayTone")]
    public class LegoPlayTone : LegoCommand
    {
        /// <summary>
        /// Play a tone on the NXT
        /// </summary>
        public LegoPlayTone()
            : base(3, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.PlayTone, 0x00, 0x00, 0x00, 0x00) { }

        /// <summary>
        /// Play a tone on the NXT
        /// </summary>
        /// <param name="frequency"></param>
        /// <param name="duration"></param>
        public LegoPlayTone(int frequency, int duration)
            : base(3, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.PlayTone, 0x00, 0x00, 0x00, 0x00)
        {
            this.Frequency = frequency;
            this.Duration = duration;
        }


        /// <summary>
        /// 200 - 14000 Hz
        /// </summary>
        [Description("The frequency of the note.")]
        
        public int Frequency
        {
            get
            {
                if (this.CommandData == null || this.CommandData.Length < 6)
                    return 0;
                return NxtCommon.GetUShort(this.CommandData, 2);
            }
            set
            {
                ushort frequency = (ushort)Math.Min(Math.Max(200, value), 14000);
                if (this.CommandData == null) this.CommandData = new byte[6];
                NxtCommon.SetUShort(this.CommandData, 2, frequency);
            }
        }

        /// <summary>
        /// Duration to play tome in ms
        /// </summary>
        [Description("The duration to play the note (in ms).")]
        
        public int Duration
        {
            get
            {
                if (CommandData == null || CommandData.Length < 6)
                    return 0;
                return NxtCommon.GetUShort(this.CommandData, 4);
            }

            set
            {
                ushort duration = (ushort)Math.Max(1, Math.Min(30000, value));
                if (this.CommandData == null) this.CommandData = new byte[6];
                NxtCommon.SetUShort(this.CommandData, 4, duration);
            }
        }
    }

    #endregion

    #region LegoLSGetStatus

    /// <summary>
    /// LEGO NXT Command: Low Speed (I2C) Get Status
    /// </summary>
    [Description("LEGO Command: LSGetStatus.")]
    [XmlRootAttribute("LegoLSGetStatus")]
    public class LegoLSGetStatus : LegoCommand
    {
        /// <summary>
        /// LEGO NXT Command: Low Speed (I2C) Get Status
        /// </summary>
        public LegoLSGetStatus()
            : base(4, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.LSGetStatus, 0x00)
        {
            base.RequireResponse = true;
        }

        /// <summary>
        /// LEGO NXT Command: Low Speed (I2C) Get Status
        /// </summary>
        /// <param name="port"></param>
        public LegoLSGetStatus(NxtSensorPort port)
            : base(4, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.LSGetStatus, 0x00)
        {
            base.RequireResponse = true;
            Port = port;
        }

        /// <summary>
        /// The Matching LEGO Response
        /// </summary>
        /// <param name="responseData"></param>
        /// <returns></returns>
        public override LegoResponse GetResponse(byte[] responseData)
        {
            return new LegoResponseLSGetStatus(responseData);
        }

        /// <summary>
        /// Hide RequireResponse from proxy and always set it to true.
        /// </summary>
        [Description("Identifies whether to send an acknowledgement back on a command request.")]
        public override bool RequireResponse
        {
            get { return true; }
            set { base.RequireResponse = true; }
        }

        /// <summary>
        /// The Sensor Port to Query Status
        /// </summary>
        [Description("The Sensor Port to Query Status")]
        public NxtSensorPort Port
        {
            get { return NxtCommon.GetNxtSensorPort(this.CommandData[2]); }
            set { this.CommandData[2] = NxtCommon.PortNumber(value); }
        }
    }

    #endregion

  

    #region LegoClose

    #endregion

    #region LegoDelete

    #endregion

    #region LegoStartProgram
    /// <summary>
    /// LEGO Command: Starts a program on the NXT.
    /// <remarks>Standard return package.</remarks>
    /// </summary>
    [Description("LEGO Command: Starts a program on the NXT.")]
    [XmlRootAttribute("LegoStartProgram")]
    public class LegoStartProgram : LegoCommand
    {
        private string _fileName = string.Empty;

        /// <summary>
        /// Starts a program on the NXT.
        /// </summary>
        public LegoStartProgram()
            : base(3, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.StartProgram)
        {
            base.ExtendCommandData(22);
        }

        /// <summary>
        /// Starts a program on the NXT.
        /// </summary>
        /// <param name="fileName"></param>
        public LegoStartProgram(string fileName)
            : base(3, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.StartProgram)
        {
            base.ExtendCommandData(22);
            this.FileName = fileName;
        }

        /// <summary>
        /// The name of the file to be started.
        /// </summary>
        [Description("The name of the file to be started.")]
        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(_fileName))
                    _fileName = NxtCommon.DataToString(this.CommandData, 2, 20);
                return _fileName;
            }
            set
            {
                _fileName = value;
                NxtCommon.SetStringToData(this.CommandData, 2, _fileName, 20);
            }
        }
    }

    #endregion

    #region LegoPlaySoundFile
    /// <summary>
    /// LEGO Command: Play a sound file
    /// <remarks>Standard return package.</remarks>
    /// </summary>
    [Description("LEGO Command: Play a sound file.")]
    [XmlRootAttribute("LegoPlaySoundFile")]
    public class LegoPlaySoundFile : LegoCommand
    {
        private string _fileName = string.Empty;
        private bool? _loop = null;

        /// <summary>
        /// Play a sound file
        /// </summary>
        public LegoPlaySoundFile()
            : base(3, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.PlaySoundFile)
        {
            ExtendCommandData(23);
        }

        /// <summary>
        /// Play a sound file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="loop"></param>
        public LegoPlaySoundFile(string fileName, bool loop)
            : base(3, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.PlaySoundFile)
        {
            ExtendCommandData(23);
            this.FileName = fileName;
        }

        /// <summary>
        /// The name of the sound file to be played.
        /// </summary>
        [Description("The name of the sound file to be played.")]
        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(_fileName))
                    _fileName = NxtCommon.DataToString(this.CommandData, 3, 20);
                return _fileName;
            }
            set
            {
                _fileName = value;
                NxtCommon.SetStringToData(this.CommandData, 3, _fileName, 20);
            }
        }

        /// <summary>
        /// Repeat the sound file
        /// </summary>
        [Description("Repeat the sound file")]
        public bool Loop
        {
            get
            {
                if (_loop == null)
                    _loop = (this.CommandData != null && this.CommandData.Length >= 3 && this.CommandData[2] != 0);
                return (bool)_loop;
            }
            set
            {
                _loop = value;
                this.CommandData[2] = (byte)((value) ? 1 : 0);
            }
        }
    }


    #endregion

    #region LegoStopSoundPlayback

    /// <summary>
    /// Stop sound playback on the NXT.
    /// <remarks>Standard return package.</remarks>
    /// </summary>
    [Description("LEGO Command: StopSoundPlayback \nStop sound playback on the NXT.")]
    [XmlRootAttribute("LegoStopSoundPlayback")]
    public class LegoStopSoundPlayback : LegoCommand
    {
        /// <summary>
        /// Stop sound playback on the NXT.
        /// </summary>
        public LegoStopSoundPlayback()
            : base(3, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.StopSoundPlayback) { }

    }

    #endregion

    #region LegoStopProgram

    /// <summary>
    /// LEGO Command: Stop a program on the NXT.
    /// <remarks>Standard return package.</remarks>
    /// </summary>
    [Description("LEGO Command: Stop a program on the NXT.")]
    [XmlRootAttribute("LegoStopProgram")]
    public class LegoStopProgram : LegoCommand
    {
        /// <summary>
        /// LEGO Command: Stop a program on the NXT.
        /// </summary>
        public LegoStopProgram()
            : base(3, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.StopProgram)
        {
            // Default return a status
            base.RequireResponse = true;
        }

    }

    #endregion

    #region LegoFindFirst

    /// <summary>
    /// LEGO Command: FindFirst
    /// </summary>
    [Description("LEGO Command: FindFirst.")]
    [XmlRootAttribute("LegoFindFirst")]
    public class LegoFindFirst : LegoCommand
    {
        private string _fileName = string.Empty;

        /// <summary>
        /// LEGO Command: FindFirst
        /// </summary>
        public LegoFindFirst()
            : base(28, LegoCommand.NxtSystemCommand, (byte)LegoCommandCode.FindFirst)
        {
            base.ExtendCommandData(22);
            base.RequireResponse = true;
        }

        /// <summary>
        /// LEGO Command: FindFirst
        /// </summary>
        /// <param name="fileName"></param>
        public LegoFindFirst(string fileName)
            : base(28, LegoCommand.NxtSystemCommand, (byte)LegoCommandCode.FindFirst, 0)
        {
            base.ExtendCommandData(22);
            base.RequireResponse = true;
            this.FileName = fileName;
        }

        /// <summary>
        /// Hide RequireResponse from proxy and always set it to true.
        /// </summary>
        [Description("Identifies whether to send an acknowledgement back on a command request.")]
        public override bool RequireResponse
        {
            get { return true; }
            set { base.RequireResponse = true; }
        }

        /// <summary>
        /// The name of the file.
        /// </summary>
        [Description("The name of the file.")]
        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(_fileName))
                    _fileName = NxtCommon.DataToString(this.CommandData, 2);
                return _fileName;
            }
            set
            {
                _fileName = value;
                NxtCommon.SetStringToData(this.CommandData, 2, _fileName, 20);
            }
        }

    }

    /// <summary>
    /// LEGO Response: Find First.
    /// </summary>
    [Description("LEGO Response: Find First.")]
    [XmlRootAttribute("LegoResponseFindFirst")]
    public class LegoResponseFindFirst : LegoResponse
    {
        /// <summary>
        /// LEGO Response: Find First.
        /// </summary>
        public LegoResponseFindFirst()
            : base(28, LegoCommandCode.FindFirst)
        {
        }

        /// <summary>
        /// LEGO Response: Find First.
        /// </summary>
        /// <param name="responseData"></param>
        public LegoResponseFindFirst(byte[] responseData)
            : base(28, LegoCommandCode.FindFirst, responseData) { }

        #region Hide base type DataMembers


        /// <summary>
        /// Hide RequireResponse from proxy and always set it to true.
        /// </summary>
        [Description("Identifies whether to send an acknowledgement back on a command request.")]
        public override bool RequireResponse
        {
            get { return true; }
            set { base.RequireResponse = true; }
        }

        #endregion

        /// <summary>
        /// The handle to the data.
        /// </summary>
        [Description("The handle to the data.")]
        public int Handle
        {
            get
            {
                if (CommandData != null && CommandData.Length == this.ExpectedResponseSize)
                    return CommandData[3];
                return -1;
            }
            set
            {
                if (CommandData != null && CommandData.Length == this.ExpectedResponseSize)
                    CommandData[3] = (byte)value;
            }

        }

        /// <summary>
        /// The name of the file.
        /// </summary>
        [Description("The name of the file.")]
        public string FileName
        {
            get
            {
                if (CommandData == null || CommandData.Length != this.ExpectedResponseSize)
                    return string.Empty;

                return NxtCommon.DataToString(CommandData, 4, 20);
            }
            set
            {
                string newFilename = value;
                if (value.Length > 19)
                    newFilename = value.Substring(0, 19);

                if (CommandData == null || CommandData.Length < this.ExpectedResponseSize)
                {
                    byte[] oldData = CommandData;
                    CommandData = new byte[this.ExpectedResponseSize];
                    if (oldData != null) oldData.CopyTo(CommandData, 0);
                }
                NxtCommon.SetStringToData(this.CommandData, 4, value, 20);
            }
        }

        /// <summary>
        /// The size of the file.
        /// </summary>
        [Description("The size of the file.")]
        public long FileSize
        {
            get
            {
                if (CommandData.Length == this.ExpectedResponseSize)
                    return (long)BitConverter.ToUInt32(CommandData, 24);
                return -1;
            }
            set
            {
                if (CommandData.Length == this.ExpectedResponseSize)
                    NxtCommon.SetUInt32(CommandData, 24, value);
            }
        }

    }
    #endregion

    #region LegoFindNext

    /// <summary>
    /// LEGO Command: FindNext
    /// </summary>
    [Description("LEGO Command: FindNext.")]
    [XmlRootAttribute("LegoFindNext")]
    public class LegoFindNext : LegoCommand
    {
        /// <summary>
        /// LEGO Command: FindNext
        /// </summary>
        public LegoFindNext()
            : base(28, LegoCommand.NxtSystemCommand, (byte)LegoCommandCode.FindNext, 0x00)
        {
            base.RequireResponse = true;
        }

        /// <summary>
        /// LEGO Command: FindNext
        /// </summary>
        /// <param name="handle"></param>
        public LegoFindNext(int handle)
            : base(28, LegoCommand.NxtSystemCommand, (byte)LegoCommandCode.FindNext, 0)
        {
            base.RequireResponse = true;
            this.Handle = handle;
        }

        /// <summary>
        /// Hide RequireResponse from proxy and always set it to true.
        /// </summary>
        [Description("Identifies whether to send an acknowledgement back on a command request.")]
        public override bool RequireResponse
        {
            get { return true; }
            set { base.RequireResponse = true; }
        }

        /// <summary>
        /// The handle from LegoResponseFirst or LegoResponseNxt.
        /// </summary>
        [Description("The handle from LegoResponseFindFirst or LegoResponseFindNxt.")]
        public int Handle
        {
            get
            {
                return (int)this.CommandData[2];
            }
            set
            {
                this.CommandData[2] = (byte)value;
            }
        }

    }

    /// <summary>
    /// LEGO Response: Find Next.
    /// </summary>
    [Description("LEGO Response: Find Next.")]
    [XmlRootAttribute("LegoResponseFindNext")]
    public class LegoResponseFindNext : LegoResponse
    {
        /// <summary>
        /// LEGO Response: Find Next.
        /// </summary>
        public LegoResponseFindNext()
            : base(28, LegoCommandCode.FindNext)
        {
        }

        /// <summary>
        /// LEGO Response: Find Next.
        /// </summary>
        /// <param name="responseData"></param>
        public LegoResponseFindNext(byte[] responseData)
            : base(28, LegoCommandCode.FindNext, responseData) { }

        #region Hide base type DataMembers


        /// <summary>
        /// Hide RequireResponse from proxy and always set it to true.
        /// </summary>
        [Description("Identifies whether to send an acknowledgement back on a command request.")]
        public override bool RequireResponse
        {
            get { return true; }
            set { base.RequireResponse = true; }
        }

        #endregion

        /// <summary>
        /// The handle to the data.
        /// </summary>
        [Description("The handle to the data.")]
        public int Handle
        {
            get
            {
                if (CommandData != null && CommandData.Length == this.ExpectedResponseSize)
                    return CommandData[3];
                return -1;
            }
            set
            {
                if (CommandData != null && CommandData.Length == this.ExpectedResponseSize)
                    CommandData[3] = (byte)value;
            }

        }

        /// <summary>
        /// The name of the file.
        /// </summary>
        [Description("The name of the file.")]
        public string FileName
        {
            get
            {
                if (CommandData == null || CommandData.Length != this.ExpectedResponseSize)
                    return string.Empty;

                return NxtCommon.DataToString(CommandData, 4, 20);
            }
            set
            {
                string newFilename = value;
                if (value.Length > 19)
                    newFilename = value.Substring(0, 19);

                if (CommandData == null || CommandData.Length < this.ExpectedResponseSize)
                {
                    byte[] oldData = CommandData;
                    CommandData = new byte[this.ExpectedResponseSize];
                    if (oldData != null) oldData.CopyTo(CommandData, 0);
                }
                NxtCommon.SetStringToData(this.CommandData, 4, value, 20);
            }
        }

        /// <summary>
        /// The size of the file.
        /// </summary>
        [Description("The size of the file.")]
        public long FileSize
        {
            get
            {
                if (CommandData.Length == this.ExpectedResponseSize)
                    return (long)BitConverter.ToUInt32(CommandData, 24);
                return -1;
            }
            set
            {
                if (CommandData.Length == this.ExpectedResponseSize)
                    NxtCommon.SetUInt32(CommandData, 24, value);
            }
        }

    }
    #endregion

    #region LegoGetCurrentProgramName
    /// <summary>
    /// LEGO Command: GetCurrentProgramName
    /// </summary>
    [Description("LEGO Command: GetCurrentProgramName.")]
    [XmlRootAttribute("LegoGetCurrentProgramName")]
    public class LegoGetCurrentProgramName : LegoCommand
    {
        /// <summary>
        /// LEGO Command: GetCurrentProgramName
        /// </summary>
        public LegoGetCurrentProgramName()
            : base(23, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.GetCurrentProgramName)
        {
            base.RequireResponse = true;
        }


        /// <summary>
        /// Hide RequireResponse from proxy and always set it to true.
        /// </summary>
        [Description("Identifies whether to send an acknowledgement back on a command request.")]
        public override bool RequireResponse
        {
            get { return true; }
            set { base.RequireResponse = true; }
        }
    }

    /// <summary>
    /// LEGO Response: GetCurrentProgramName
    /// </summary>
    [Description("LEGO Response: Get Current Program Name.")]
    [XmlRootAttribute("LegoResponseGetCurrentProgramName")]
    public class LegoResponseGetCurrentProgramName : LegoResponse
    {
        /// <summary>
        /// LEGO Response: GetCurrentProgramName
        /// </summary>
        public LegoResponseGetCurrentProgramName()
            : base(23, LegoCommandCode.GetCurrentProgramName) { }

        /// <summary>
        /// LEGO Response: GetCurrentProgramName
        /// </summary>
        /// <param name="responseData"></param>
        public LegoResponseGetCurrentProgramName(byte[] responseData)
            : base(23, LegoCommandCode.GetCurrentProgramName, responseData) { }


        #region Hide base type DataMembers


        /// <summary>
        /// Hide RequireResponse from proxy and always set it to true.
        /// </summary>
        [Description("Identifies whether to send an acknowledgement back on a command request.")]
        public override bool RequireResponse
        {
            get { return true; }
            set { base.RequireResponse = true; }
        }

        #endregion

        /// <summary>
        /// The name of the file.
        /// </summary>
        [Description("The name of the file.")]
        public string FileName
        {
            get
            {
                if (CommandData == null || CommandData.Length < this.ExpectedResponseSize)
                    return string.Empty;

                return NxtCommon.DataToString(CommandData, 3, 20);
            }
            set
            {
                if (CommandData == null || CommandData.Length != this.ExpectedResponseSize)
                {
                    byte[] oldData = CommandData;
                    CommandData = new byte[this.ExpectedResponseSize];
                    if (oldData != null) oldData.CopyTo(CommandData, 0);
                }
                NxtCommon.SetStringToData(this.CommandData, 3, value, 20);
            }
        }

    }

    #endregion

    #region LegoOpenWrite
    /// <summary>
    /// LEGO Command: OpenWrite
    /// </summary>
    [Description("LEGO Command: OpenWrite.")]
    [XmlRootAttribute("LegoOpenWrite")]
    public class LegoOpenWrite : LegoCommand
    {
        private string _fileName;

        /// <summary>
        /// LEGO Command: OpenWrite
        /// </summary>
        public LegoOpenWrite()
            : base(4, LegoCommand.NxtSystemCommand, (byte)LegoCommandCode.OpenWrite)
        {
            base.ExtendCommandData(26);
            base.RequireResponse = true;
        }

        /// <summary>
        /// LEGO Command: OpenWrite
        /// </summary>
        /// <param name="file"></param>
        /// <param name="size"></param>
        public LegoOpenWrite(string file, int size)
            : base(4, LegoCommand.NxtSystemCommand, (byte)LegoCommandCode.OpenWrite)
        {
            base.ExtendCommandData(26);
            base.RequireResponse = true;
            this.FileName = file;
            this.FileSize = size;
        }

        /// <summary>
        /// Hide RequireResponse from proxy and always set it to true.
        /// </summary>
        [Description("Identifies whether to send an acknowledgement back on a command request.")]
        public override bool RequireResponse
        {
            get { return true; }
            set { base.RequireResponse = true; }
        }

        /// <summary>
        /// The name of the file to be opened for writing.
        /// </summary>
        [Description("The name of the file to be opened for writing.")]
        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(_fileName))
                    _fileName = NxtCommon.DataToString(this.CommandData, 2, 20);
                return _fileName;
            }
            set
            {
                _fileName = value;
                if (this.CommandData == null) this.CommandData = new byte[26];
                NxtCommon.SetStringToData(this.CommandData, 2, _fileName, 20);
            }
        }

        /// <summary>
        /// The size of the file.
        /// </summary>
        [Description("The size of the file.")]
        public int FileSize
        {
            get
            {
                return (int)System.BitConverter.ToUInt32(this.CommandData, 22);
            }
            set
            {
                if (this.CommandData == null) this.CommandData = new byte[26];
                uint fileSize = (UInt32)value;
                NxtCommon.SetUInt32(this.CommandData, 22, fileSize);
            }
        }
    }

    /// <summary>
    /// LEGO Response: OpenWrite.
    /// </summary>
    [Description("LEGO Response: OpenWrite.")]
    [XmlRootAttribute("LegoResponseOpenWrite")]
    public class LegoResponseOpenWrite : LegoResponse
    {
        /// <summary>
        /// LEGO Command: OpenWrite
        /// </summary>
        public LegoResponseOpenWrite()
            : base(4, LegoCommandCode.OpenWrite) { }

        /// <summary>
        /// LEGO Command: OpenWrite
        /// </summary>
        /// <param name="responseData"></param>
        public LegoResponseOpenWrite(byte[] responseData)
            : base(4, LegoCommandCode.OpenWrite, responseData) { }


        #region Hide base type DataMembers

        /// <summary>
        /// Hide RequireResponse from proxy and always set it to true.
        /// </summary>
        [Description("Identifies whether to send an acknowledgement back on a command request.")]
        public override bool RequireResponse
        {
            get { return true; }
            set { base.RequireResponse = true; }
        }

        #endregion


        /// <summary>
        /// The handle to the data.
        /// </summary>
        [Description("The handle to the data.")]
        public int Handle
        {
            get
            {
                if (CommandData != null && CommandData.Length == this.ExpectedResponseSize)
                    return CommandData[3];
                return -1;
            }
            set
            {
                CommandData[3] = (byte)value;
            }
        }

    }
    #endregion

    #region LegoOpenWriteLinear
    /// <summary>
    /// LEGO Command: OpenWriteLinear.
    /// </summary>
    [Description("LEGO Command: OpenWriteLinear.")]
    [XmlRootAttribute("LegoOpenWriteLinear")]
    public class LegoOpenWriteLinear : LegoCommand
    {
        private string _fileName;
        private UInt32 _fileSize;

        /// <summary>
        /// LEGO Command: OpenWriteLinear
        /// </summary>
        public LegoOpenWriteLinear()
            : base(4, LegoCommand.NxtSystemCommand, (byte)LegoCommandCode.OpenWriteLinear)
        {
            base.ExtendCommandData(26);
            base.RequireResponse = true;
        }

        /// <summary>
        /// LEGO Command: OpenWriteLinear
        /// </summary>
        /// <param name="file"></param>
        /// <param name="size"></param>
        public LegoOpenWriteLinear(string file, int size)
            : base(4, LegoCommand.NxtSystemCommand, (byte)LegoCommandCode.OpenWriteLinear)
        {
            base.ExtendCommandData(26);
            base.RequireResponse = true;
            this.FileName = file;
            this.FileSize = size;
        }

        /// <summary>
        /// Hide RequireResponse from proxy and always set it to true.
        /// </summary>
        [Description("Identifies whether to send an acknowledgement back on a command request.")]
        public override bool RequireResponse
        {
            get { return true; }
            set { base.RequireResponse = true; }
        }

        /// <summary>
        /// The name of the file.
        /// </summary>
        [Description("The name of the file.")]
        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(_fileName))
                    _fileName = NxtCommon.DataToString(this.CommandData, 2, 20);
                return _fileName;
            }
            set
            {
                _fileName = value;
                if (this.CommandData == null) this.CommandData = new byte[26];
                NxtCommon.SetStringToData(this.CommandData, 2, _fileName, 20);
            }
        }

        /// <summary>
        /// The size of the file.
        /// </summary>
        [Description("The size of the file.")]
        public int FileSize
        {
            get
            {
                return (int)System.BitConverter.ToUInt32(this.CommandData, 22);
            }
            set
            {
                _fileSize = (UInt32)value;
                if (this.CommandData == null) this.CommandData = new byte[26];
                NxtCommon.SetUInt32(this.CommandData, 22, _fileSize);
            }
        }


    }

    #endregion

    
   
     


    #endregion

}