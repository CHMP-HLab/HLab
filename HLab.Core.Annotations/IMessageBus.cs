using System;

namespace HLab.Core.Annotations;

public interface IMessagesService
{
    void Publish<T>(T payload);
    void Subscribe<T>(Action<T> action);
    void Unsubscribe<T>(Action<T> action);
}