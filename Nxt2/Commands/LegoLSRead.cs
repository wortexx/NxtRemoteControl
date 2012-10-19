using System.ComponentModel;
using System.Xml.Serialization;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO Command: LSRead.  I2C Low Speed Read
    /// </summary>
    [Description("LEGO Command: LSRead.  I2C Low Speed Read")]
    [XmlRoot("LegoLSRead")]
    public class LegoLSRead : LegoCommand
    {
        /// <summary>
        /// LEGO NXT Low Speed (I2C) Read
        /// </summary>
        public LegoLSRead()
            : base(20, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.LSRead, 0x00)
        {
            base.RequireResponse = true;
        }

        /// <summary>
        /// LEGO NXT Low Speed (I2C) Read
        /// </summary>
        /// <param name="port"></param>
        public LegoLSRead(NxtSensorPort port)
            : base(20, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.LSRead, 0x00)
        {
            base.RequireResponse = true;
            Port = port;
        }

        /// <summary>
        /// LEGO NXT Low Speed (I2C) Read
        /// </summary>
        public LegoLSRead(byte[] commandData)
            : base(20, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.LSRead, 0x00)
        {
            base.RequireResponse = true;
            this.CommandData = commandData;
        }

        /// <summary>
        /// The matching LEGO Response
        /// </summary>
        /// <param name="responseData"></param>
        /// <returns></returns>
        public override LegoResponse GetResponse(byte[] responseData)
        {
            return new LegoResponseLSRead(responseData);
        }

        /// <summary>
        /// LSRead has a fixed response size which can not be changed.
        /// Excess receive bytes are padded with zeroes.
        /// </summary>
        public override int ExpectedResponseSize
        {
            get
            {
                return 20;
            }
            set
            {
                base.ExpectedResponseSize = 20;
            }
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
        [Description("The input port (0, 1, 2, or 3).")]
        public NxtSensorPort Port
        {
            get { return NxtCommon.GetNxtSensorPort(this.CommandData[2]); }
            set { this.CommandData[2] = NxtCommon.PortNumber(value); }
        }

        #region Hide underlying data members

        /// <summary>
        /// Command Data
        /// </summary>
        public override byte[] CommandData
        {
            get { return base.CommandData; }
            set { base.CommandData = value; }
        }

        #endregion
    }
}