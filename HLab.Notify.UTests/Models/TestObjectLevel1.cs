using HLab.DependencyInjection.Annotations;

namespace HLab.Notify.UTests
{
    internal class TestObjectLevel1
    {
        [Import] public TestNotifierLevel0 TestNotifierLevel0 { get; }
    }
}