using System.Windows.Input;
using Xunit;

namespace HLab.Notify.PropertyChanged.UTest
{
    using H = H<CommandObject>;

    internal class CommandObject : NotifierBase
    {
        public CommandObject()
        {
            H.Initialize(this);
        }

        public bool Result = false;

        public ICommand Command { get; } = H.Command(c => c
            .Action(e => e.Result = true)
        );
    }

    public class UnitTestCommand
    {
        [Fact]
        public void TestAction()
        {
            var command = new CommandObject();
            command.Command.Execute(null);

            Assert.True(command.Result);
        }

    }
}
