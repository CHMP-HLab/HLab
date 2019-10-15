using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using HLab.Notify.PropertyChanged.Utest.Annotations;
using Xunit;
using H = HLab.Notify.PropertyChanged.NotifyHelper<HLab.Notify.PropertyChanged.UTest.CommandObject>;

namespace HLab.Notify.PropertyChanged.UTest
{
    class CommandObject
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
