using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO Command: Read.
    /// </summary>
    [Description("LEGO Command: Read.")]
    public class LegoRead : LegoCommand
    {
        private int _bytesToRead;

        /// <summary>
        /// LEGO Command: Read
        /// </summary>
        public LegoRead()
            : base(5, LegoCommand.NxtSystemCommand, (byte)LegoCommandCode.Read, 0, 0, 0)
        {
            base.RequireResponse = true;
        }

        /// <summary>
        /// LEGO Command: Read
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="bytesToRead"></param>
        public LegoRead(int handle, int bytesToRead)
            : base(5, LegoCommand.NxtSystemCommand, (byte)LegoCommandCode.Read, 0, 0, 0)
        {
            base.RequireResponse = true;
            this.Handle = handle;
            this.BytesToRead = bytesToRead;
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
        /// The handle to the data.
        /// </summary>
        [Description("The handle to the data.")]
        public int Handle
        {
            get
            {
                if (CommandData != null && CommandData.Length >= 3)
                    return CommandData[2];
                return -1;
            }
            set
            {
                CommandData[2] = (byte)value;
            }
        }

        /// <summary>
        /// The number of bytes to read.
        /// </summary>
        [Description("The number of bytes to read.")]
        public int BytesToRead
        {
            get
            {
                _bytesToRead = NxtCommon.GetUShort(this.CommandData, 3);
                return (int)_bytesToRead;
            }
            set
            {
                _bytesToRead = value;
                ExpectedResponseSize = _bytesToRead + 6;
                if (CommandData == null) CommandData = new byte[5];
                NxtCommon.SetUShort(CommandData, 3, _bytesToRead);
            }
        }
    }
}