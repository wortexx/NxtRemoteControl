using System.ComponentModel;
using System.Xml.Serialization;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO Response: Delete
    /// </summary>
    [Description("LEGO Response: Delete.")]
    [XmlRoot("LegoResponseDelete")]
    public class LegoResponseDelete : LegoResponse
    {
        /// <summary>
        /// LEGO Response: Delete
        /// </summary>
        public LegoResponseDelete()
            : base(23, LegoCommandCode.Delete) { }

        /// <summary>
        /// LEGO Response: Delete
        /// </summary>
        /// <param name="responseData"></param>
        public LegoResponseDelete(byte[] responseData)
            : base(23, LegoCommandCode.Delete, responseData) { }

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
                if (CommandData.Length < this.ExpectedResponseSize)
                    return string.Empty;

                return NxtCommon.DataToString(CommandData, 3, 20);
            }
            set
            {
                if (CommandData == null || CommandData.Length < this.ExpectedResponseSize)
                {
                    byte[] oldData = CommandData;
                    CommandData = new byte[this.ExpectedResponseSize];
                    if (oldData != null) oldData.CopyTo(CommandData, 0);
                }
                NxtCommon.SetStringToData(this.CommandData, 3, value, 20);
            }
        }
    }
}