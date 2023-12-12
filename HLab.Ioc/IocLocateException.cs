using System;

namespace HLab.Ioc;

public class IocLocateException : Exception
{
    public IocLocateException(string message) : base(message)
    {

    }
    public IocLocateException(string message, Exception inner) : base(message,inner)
    {

    }
}