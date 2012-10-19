using System;
using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LegoLSWrite
    /// </summary>
    [Description("LEGO Command: LSWrite.")]
    public class LegoLSWrite : LegoCommand
    {
        private const int _headerSize = 5;
        private int _rxDataLength = -1;
        private NxtSensorPort _port = (NxtSensorPort)(-1);
        private byte[] _txData = null;
        private int _txDataLength = 0;

        /// <summary>
        /// Make sure CommandData is valid.
        /// </summary>
        private void ValidateCommandData()
        {
            int bufferSize = _headerSize + _txDataLength;
            if (this.CommandData.Length != bufferSize)
            {
                var priorCmd = CommandData;
                this.CommandData = new byte[bufferSize];

                if (priorCmd == null || priorCmd.Length < _headerSize)
                {
                    var temp = new LegoLSWrite();
                    priorCmd = temp.CommandData;
                }

                Buffer.BlockCopy(priorCmd, 0, CommandData, 0, priorCmd.Length);
            }
        }

        /// <summary>
        /// LegoLSWrite
        /// </summary>
        public LegoLSWrite()
            : base(3, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.LSWrite, 0x00, 0x00, 0x00)
        {
            RequireResponse = true;
        }

        /// <summary>
        /// LegoLSWrite
        /// </summary>
        /// <param name="cmd"></param>
        public LegoLSWrite(LegoCommand cmd)
            : base(3, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.LSWrite, 0x00, 0x00, 0x00)
        {
            this.CommandData = cmd.CommandData;
        }

        /// <summary>
        /// LegoLSWrite
        /// </summary>
        public LegoLSWrite(byte[] commandData)
            : base(3, NxtDirectCommand, (byte)LegoCommandCode.LSWrite, 0x00, 0x00, 0x00)
        {
            CommandData = commandData;
        }

        /// <summary>
        /// LegoLSWrite
        /// </summary>
        /// <param name="port"></param>
        /// <param name="txData"></param>
        /// <param name="rxDataLength"></param>
        public LegoLSWrite(NxtSensorPort port, byte[] txData, byte rxDataLength)
            : base(3, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.LSWrite, 0x00, 0x00, 0x00)
        {
            this.Port = port;
            this.TXData = txData;
            this.ExpectedI2CResponseSize = rxDataLength;
        }

        /// <summary>
        /// The Low Speed Response
        /// </summary>
        /// <param name="responseData"></param>
        /// <returns></returns>
        public override LegoResponse GetResponse(byte[] responseData)
        {
            if (responseData == null || responseData.Length == 3)
                return base.GetResponse(responseData);
            return new LegoResponseLSRead(responseData);
        }


        /// <summary>
        /// The Sensor Port
        /// </summary>
        [Description("The Port")]
        public NxtSensorPort Port
        {
            get
            {
                if ((int)_port == -1)
                    if (this.CommandData != null && this.CommandData.Length >= _headerSize)
                        _port = NxtCommon.GetNxtSensorPort(this.CommandData[2]);
                    else
                        return NxtSensorPort.NotConnected;

                return _port;
            }
            set
            {
                ValidateCommandData();
                _port = value;
                this.CommandData[2] = NxtCommon.PortNumber(_port);
            }
        }

        /// <summary>
        /// The transmitted data length.
        /// </summary>
        private int TXDataLength
        {
            get { return (this.CommandData == null) ? 0 : this.CommandData.Length - _headerSize; }
            set
            {
                if (value < 0 || value > 16)
                    throw new ArgumentOutOfRangeException("LSWrite(TxData) must be between 0 and 16");

                _txDataLength = value;
                ValidateCommandData();
                this.CommandData[3] = (byte)value;
            }
        }

        /// <summary>
        /// Expected Response Size
        /// Replaces RXDataLength from the LEGO Documentation
        /// </summary>
        [Description("The received data length expected from the I2C device.")]
        public int ExpectedI2CResponseSize
        {
            get
            {
                if (this.CommandData != null && this.CommandData.Length >= _headerSize)
                    _rxDataLength = this.CommandData[4];
                return Math.Max(0, _rxDataLength);
            }
            set
            {
                ValidateCommandData();
                _rxDataLength = value;
                this.CommandData[4] = (byte)_rxDataLength;
            }
        }

        /// <summary>
        /// The transmitted data
        /// </summary>
        [Description("The transmitted data.")]
        public byte[] TXData
        {
            get
            {
                if (_txData == null && TXDataLength > 0)
                {
                    _txData = new byte[TXDataLength];
                    System.Buffer.BlockCopy(this.CommandData, 5, _txData, 0, TXDataLength);
                }
                return _txData;
            }
            set
            {
                this.TXDataLength = (value == null) ? 0 : value.Length;
                _txData = value;
                if (_txData != null)
                    _txData.CopyTo(this.CommandData, 5);
            }
        }

        #region Hide underlying data members

        /// <summary>
        /// Command Data
        /// </summary>
        [Description("The LEGO Command buffer")]
        public override byte[] CommandData
        {
            get { return base.CommandData; }
            set
            {
                base.CommandData = value;

                // reset the internal variables
                _txData = null;
                _rxDataLength = -1;
                _port = (NxtSensorPort)(-1);
                _txDataLength = (value == null) ? 0 : Math.Max(0, value.Length - _headerSize);
            }
        }

        #endregion
    }
}