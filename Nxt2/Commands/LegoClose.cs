using System.ComponentModel;
using System.Xml.Serialization;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO Command: Close a file handle.
    /// </summary>
    [Description("LEGO Command: Close a file handle.")]
    [XmlRoot("LegoClose")]
    public class LegoClose : LegoCommand
    {

        /// <summary>
        /// Close a file handle.
        /// </summary>
        public LegoClose()
            : base(4, NxtSystemCommand, (byte)LegoCommandCode.Close, 0x00)
        { base.RequireResponse = true; }

        /// <summary>
        /// Close a file handle.
        /// </summary>
        /// <param name="handle"></param>
        public LegoClose(int handle)
            : base(4, NxtSystemCommand, (byte)LegoCommandCode.Close, 0x00)
        {
            base.RequireResponse = true;
            Handle = handle;
        }

        /// <summary>
        /// The handle to the file.
        /// </summary>
        [Description("The handle to the file.")]
        public int Handle
        {
            get { return CommandData[2]; }
            set
            {
                CommandData[2] = (byte)value;
            }
        }
    }
}