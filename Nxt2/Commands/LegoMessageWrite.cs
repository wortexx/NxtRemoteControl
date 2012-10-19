using System;
using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO Command: MessageWrite.  Send a message to the NXT that the NXT can read with a message block.
    /// <remarks>Standard return package.</remarks>
    /// </summary>
    [Description("LEGO Command: MessageWrite.  Send a message to the NXT that the NXT can read with a message block.")]
    public class LegoMessageWrite : LegoCommand
    {
        private byte[] _messageData = null;

        /// <summary>
        /// Send a message to the NXT that the NXT can read with a message block.
        /// </summary>
        public LegoMessageWrite()
            : base(3, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.MessageWrite, 0x00, 0x00)
        {
        }

        /// <summary>
        /// Send a message to the NXT that the NXT can read with a message block.
        /// </summary>
        /// <param name="inbox"></param>
        /// <param name="messageData"></param>
        public LegoMessageWrite(int inbox, byte[] messageData)
            : base(3, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.MessageWrite, (byte)inbox, Math.Min((byte)messageData.Length, (byte)59))
        {
            this.Inbox = inbox;
            this.MessageData = messageData;
        }

        /// <summary>
        /// Send a message to the NXT that the NXT can read with a message block.
        /// </summary>
        /// <param name="inbox"></param>
        /// <param name="message"></param>
        public LegoMessageWrite(int inbox, string message)
            : base(3, LegoCommand.NxtDirectCommand, (byte)LegoCommandCode.MessageWrite, 0x00, 0x00)
        {
            this.Inbox = inbox;
            this.MessageDataString = message;
        }


        /// <summary>
        /// LEGO NXT Inbox where message should be delivered
        /// </summary>
        [Description("LEGO NXT Inbox where message should be delivered")]
        public int Inbox
        {
            get { return (int)this.CommandData[2]; }
            set { this.CommandData[2] = (byte)value; }
        }

        /// <summary>
        /// The size of the message to be written (0-60).
        /// </summary>
        [Description("The size of the message to be written (1-58).")]
        public int MessageSize
        {
            get { return (int)this.CommandData[3]; }
            set
            {
                if (value < 1 || value > 58)
                    throw new ArgumentOutOfRangeException("MessageSize must be between 1 and 58 bytes");

                this.CommandData[3] = (byte)value;
            }
        }

        /// <summary>
        /// The Message Data
        /// </summary>
        public byte[] MessageData
        {
            get
            {
                if (MessageSize < 1 || MessageSize > 58)
                    return null;
                if (_messageData == null || _messageData.Length != MessageSize)
                {
                    _messageData = new byte[MessageSize];
                    System.Buffer.BlockCopy(CommandData, 4, _messageData, 0, MessageSize);
                }
                return _messageData;
            }
            set
            {
                int length = (value == null) ? 0 : value.Length;

                if (length == 0)
                    throw new ArgumentOutOfRangeException("MessageData must be at least 1 byte.");

                if (length > 58)
                    throw new ArgumentOutOfRangeException("MessageData must be no larger than 58 bytes.");

                _messageData = value;
                base.ExtendCommandData(length + 4);
                MessageSize = length;
                System.Buffer.BlockCopy(value, 0, this.CommandData, 4, length);
            }
        }

        /// <summary>
        /// Expose the message data as a string.
        /// </summary>
        [Description("The message data.")]
        public string MessageDataString
        {
            get
            {
                if (MessageData == null || _messageData.Length < 2)
                    return string.Empty;

                return NxtCommon.DataToString(CommandData, 4);
            }

            set
            {
                MessageData = NxtCommon.StringToData(value, value.Length + 1);
            }
        }

    }
}