using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// LEGO Command: SetBrickName.
    /// <remarks>Standard return package.</remarks>
    /// </summary>
    [Description("LEGO Command: SetBrickName.")]
    public class LegoSetBrickName : LegoCommand
    {
        private string _name = string.Empty;

        /// <summary>
        /// LEGO Command: SetBrickName.
        /// </summary>
        public LegoSetBrickName()
            : base(3, LegoCommand.NxtSystemCommand, (byte)LegoCommandCode.SetBrickName)
        {
            ExtendCommandData(18);
        }

        /// <summary>
        /// LEGO Command: SetBrickName.
        /// </summary>
        /// <param name="name"></param>
        public LegoSetBrickName(string name)
            : base(3, LegoCommand.NxtSystemCommand, (byte)LegoCommandCode.SetBrickName)
        {
            ExtendCommandData(18);
            this.Name = name;
        }

        /// <summary>
        /// The descriptive identifier for the NXT brick.
        /// </summary>
        [Description("The descriptive identifier for the NXT brick.")]
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                    _name = NxtCommon.DataToString(CommandData, 2, 16);

                return _name;
            }
            set
            {
                _name = value;
                NxtCommon.SetStringToData(CommandData, 2, _name, 16);
            }
        }

    }
}