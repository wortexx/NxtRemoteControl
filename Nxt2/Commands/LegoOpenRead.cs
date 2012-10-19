using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO Command: OpenRead.
    /// </summary>
    [Description("LEGO Command: OpenRead.")]
    public class LegoOpenRead : LegoCommand
    {
        private string _fileName = string.Empty;

        /// <summary>
        /// LEGO Command: OpenRead
        /// </summary>
        public LegoOpenRead()
            : base(8, LegoCommand.NxtSystemCommand, (byte)LegoCommandCode.OpenRead)
        {
            ExtendCommandData(22);
            base.RequireResponse = true;
        }

        /// <summary>
        /// LEGO Command: OpenRead
        /// </summary>
        /// <param name="fileName"></param>
        public LegoOpenRead(string fileName)
            : base(8, LegoCommand.NxtSystemCommand, (byte)LegoCommandCode.OpenRead)
        {
            ExtendCommandData(22);
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
        /// The name of the file opened for reading.
        /// </summary>
        [Description("Specifies the name of the file opened for reading.")]
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

    }
}