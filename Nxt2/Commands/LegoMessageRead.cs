using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO Command: MessageRead.
    /// </summary>
    [Description("LEGO Command: MessageRead.")]
    public class LegoMessageRead : LegoCommand
    {
        private bool _remove;

        /// <summary>
        /// LEGO Command: MessageRead
        /// </summary>
        public LegoMessageRead()
            : base(64, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.MessageRead, 0x00, 0x00, 0x00)
        {
            base.RequireResponse = true;
        }

        /// <summary>
        /// LEGO Command: MessageRead
        /// </summary>
        /// <param name="remoteInbox"></param>
        /// <param name="localInbox"></param>
        /// <param name="remove"></param>
        public LegoMessageRead(int remoteInbox, int localInbox, bool remove)
            : base(64, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.MessageRead, 0x00, 0x00, 0x00)
        {
            base.RequireResponse = true;
            this.RemoteInbox = remoteInbox;
            this.LocalInbox = localInbox;
            this.Remove = remove;
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
        /// Remove Inbox 0-9
        /// </summary>
        [Description("The remote communication port (0-9).")]
        public int RemoteInbox
        {
            get { return (int)this.CommandData[2]; }
            set
            {
                if (this.CommandData == null) this.CommandData = new byte[5];
                this.CommandData[2] = (byte)value;
            }
        }

        /// <summary>
        /// Local Inbox 0-9
        /// </summary>
        [Description("The local communication port (0-9).")]
        public int LocalInbox
        {
            get { return (int)this.CommandData[3]; }
            set
            {
                if (this.CommandData == null) this.CommandData = new byte[5];
                this.CommandData[3] = (byte)value;
            }
        }

        /// <summary>
        /// Clear message from remote inbox
        /// </summary>
        [Description("Identifies whether the message has been removed.")]
        public bool Remove
        {
            get
            {
                if (this.CommandData == null || this.CommandData.Length < 5)
                    return false;

                return (this.CommandData[4] != 0);
            }
            set
            {
                _remove = value;
                if (this.CommandData == null) this.CommandData = new byte[5];
                this.CommandData[4] = (byte)((value) ? 1 : 0);
            }
        }
    }
}