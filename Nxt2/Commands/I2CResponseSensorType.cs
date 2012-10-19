using System.ComponentModel;
using System.Text;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LegoResponse: I2C Sensor Type
    /// </summary>
    [Description("LEGO Response: LSRead(SensorType).")]
    public class I2CResponseSensorType : LegoResponse
    {
        /// <summary>
        /// LegoResponse: I2C Sensor Type
        /// </summary>
        public I2CResponseSensorType()
            : base(20, LegoCommandCode.LSRead)
        {
        }

        /// <summary>
        /// LegoResponse: I2C Sensor Type
        /// </summary>
        /// <param name="responseData"></param>
        public I2CResponseSensorType(byte[] responseData)
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
        /// The Sensor Manufacturer
        /// </summary>
        [Description("The Sensor Manufacturer.")]
        public string Manufacturer
        {
            get
            {
                if (CommandData == null || CommandData.Length < 12)
                    return null;
                return Encoding.ASCII.GetString(base.CommandData, 4, 8).TrimEnd('\0', ' ', '?');
            }
            set
            {
            }
        }


        /// <summary>
        /// The Sensor Type
        /// </summary>
        [Description("The Sensor Type.")]
        public string SensorType
        {
            get
            {
                if (CommandData == null || CommandData.Length < 20)
                    return null;
                return Encoding.ASCII.GetString(base.CommandData, 12, 8).TrimEnd('\0', ' ', '?');
            }
            set
            {
            }
        }
    }
}