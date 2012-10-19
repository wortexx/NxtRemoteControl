using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO Command: Write.
    /// </summary>
    [Description("LEGO Command: Write.")]
    public class LegoWrite : LegoCommand
    {
        private byte[] _writeData;

        /// <summary>
        /// LEGO Command: Write
        /// </summary>
        public LegoWrite()
            : base(6, NxtSystemCommand, (byte)LegoCommandCode.Write, 0x00)
        {
            base.RequireResponse = true;
        }

        /// <summary>
        /// LEGO Command: Write
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="writeData"></param>
        public LegoWrite(int handle, byte[] writeData)
            : base(6, NxtSystemCommand, (byte)LegoCommandCode.Write, 0x00)
        {
            base.RequireResponse = true;
            this.Handle = handle;
            this.WriteData = writeData;
        }

        /// <summary>
        /// The handle to the data.
        /// </summary>
        [Description("The handle to the data.")]
        public int Handle
        {
            get { return (int)this.CommandData[2]; }
            set
            {
                this.CommandData[2] = (byte)value;
            }
        }

        /// <summary>
        /// The data to be written.
        /// </summary>
        [Description("The data to be written.")]
        public byte[] WriteData
        {
            get
            {
                if (_writeData == null && CommandData != null)
                    _writeData = new byte[CommandData.Length - 3];

                if (CommandData != null && CommandData.Length > 3)
                    System.Buffer.BlockCopy(CommandData, 3, _writeData, 0, _writeData.Length);

                return _writeData;
            }
            set
            {
                _writeData = value;
                base.ExtendCommandData(_writeData.Length + 3);
                System.Buffer.BlockCopy(_writeData, 0, this.CommandData, 3, _writeData.Length);
            }
        }


    }
}