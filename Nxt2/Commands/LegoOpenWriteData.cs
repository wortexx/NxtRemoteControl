using System;
using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO Command: OpenWriteData.
    /// </summary>
    [Description("LEGO Command: OpenWriteData.")]
    public class LegoOpenWriteData : LegoCommand
    {
        private string _fileName;

        /// <summary>
        /// LEGO Command: OpenWrite
        /// </summary>
        public LegoOpenWriteData()
            : base(4, LegoCommand.NxtSystemCommand, (byte)LegoCommandCode.OpenWriteData)
        {
            base.ExtendCommandData(26);
            base.RequireResponse = true;
        }

        /// <summary>
        /// LEGO Command: OpenWrite
        /// </summary>
        /// <param name="file"></param>
        /// <param name="size"></param>
        public LegoOpenWriteData(string file, int size)
            : base(4, LegoCommand.NxtSystemCommand, (byte)LegoCommandCode.OpenWriteData)
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
}