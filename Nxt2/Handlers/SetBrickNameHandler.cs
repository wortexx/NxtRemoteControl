using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Nxt2.Commands;

namespace Nxt2.Handlers
{
    public class SetBrickNameHandler
    {
        public Task Handler(SetBrickName cmd)
        {
            Contract.Requires(cmd != null);
            Contract.Requires(cmd.Name != null, "The new brick name should be specified.");
            Contract.Requires(cmd.Name.Length < 14, "The new brick name cannot be longer than 14 characters.");

            return new Task(() =>
            {
                // Initialize command
                var data = CommandHelper.InitializeData(LegoCommandCode.SetBrickName, CommandType.SystemCommandWithResponse, 18);
                cmd.Name.ToAsciiBytes().CopyTo(data, 2);  // Byte 2-17: new name of the brick.

                TransmitAndWait(data, 3); // Return package: 0:0x02, 1:Command, 2:StatusByte
            });
        }
    }
}