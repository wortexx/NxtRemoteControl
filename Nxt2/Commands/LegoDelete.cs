using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO Command: Delete
    /// </summary>
    [Description("LEGO Command: Delete.")]
    public class LegoDelete : LegoCommand
    {

        private string _fileName = string.Empty;

        /// <summary>
        /// LEGO Command: Delete
        /// </summary>
        public LegoDelete()
            : base(23, LegoCommand.NxtSystemCommand, (byte)LegoCommandCode.Delete)
        {
            ExtendCommandData(22);
            base.RequireResponse = true;
        }


        /// <summary>
        /// LEGO Command: Delete
        /// </summary>
        /// <param name="fileName"></param>
        public LegoDelete(string fileName)
            : base(23, LegoCommand.NxtSystemCommand, (byte)LegoCommandCode.Delete)
        {
            base.RequireResponse = true;
            ExtendCommandData(22);
            this.FileName = fileName;
        }

        /// <summary>
        /// The name of the file to be deleted.
        /// </summary>
        [Description("The name of the file to be deleted.")]
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
                NxtCommon.SetStringToData(this.CommandData, 2, _fileName, 20);
            }
        }

    }
}