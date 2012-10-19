using System;
using System.ComponentModel;
using System.Text;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO Response: MessageRead.
    /// </summary>
    [Description("LEGO Response: MessageRead.")]
    public class LegoResponseMessageRead : LegoResponse
    {
        /// <summary>
        /// LEGO Response: MessageRead
        /// </summary>
        public LegoResponseMessageRead()
            : base(64, LegoCommandCode.MessageRead) { }

        /// <summary>
        /// LEGO Response: MessageRead
        /// </summary>
        /// <param name="responseData"></param>
        public LegoResponseMessageRead(byte[] responseData)
            : base(64, LegoCommandCode.MessageRead, responseData) { }

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
        /// The local communication port.
        /// </summary>
        [Description("The local communication port.")]
        public int LocalInbox
        {
            get
            {
                if (CommandData != null && CommandData.Length == ExpectedResponseSize)
                    return (int)this.CommandData[3];
                return -1;
            }
            set
            {
                CommandData[3] = (byte)value;
            }
        }

        /// <summary>
        /// The size of the message.
        /// </summary>
        [Description("The size of the message.")]
        public int MessageSize
        {
            get
            {
                if (CommandData != null && CommandData.Length >= 5)
                    return (int)this.CommandData[4];
                return -1;
            }
            set
            {
                CommandData[4] = (byte)value;
            }
        }

        /// <summary>
        /// The message data read.
        /// </summary>
        [Description("The message data read.")]
        public byte[] MessageReadData
        {
            get
            {
                if (CommandData != null && CommandData.Length == ExpectedResponseSize && MessageSize <= 59)
                {
                    byte[] r = new byte[MessageSize];
                    System.Buffer.BlockCopy(this.CommandData, 5, r, 0, MessageSize);
                    return r;
                }
                return null;
            }
            set
            {
                System.Buffer.BlockCopy(value, 0, this.CommandData, 5, Math.Min(value.Length, 59));
            }
        }

        /// <summary>
        /// The Text version of the Message
        /// </summary>
        public string Message
        {
            get
            {
                byte[] messageReadData = this.MessageReadData;
                if (messageReadData == null || messageReadData.Length == 0)
                    return null;

                return Encoding.ASCII.GetString(messageReadData).TrimEnd('\0', ' ', '?');
            }
        }

    }
}