using System;

namespace Nxt2.Commands
{
    public class SetBrickName
    {
        public string Name { get; set; } 
    }

    public interface ICommand
    {
        Guid Id { get; }
    }
}