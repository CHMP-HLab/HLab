namespace HLab.Notify.PropertyChanged;

public class Boxed<T>
{
    public T Value;
}

public abstract class NotifierTest<T> : NotifierBase
    where T : NotifierTest<T>
{
    protected class H : H<T> { }

    static readonly IEventHandlerService _eventHandlerService = new EventHandlerService();
    protected NotifierTest()
    {
        H<NotifierTest<T>>.Initialize(this);
        H.Initialize((T)this);
    }

}