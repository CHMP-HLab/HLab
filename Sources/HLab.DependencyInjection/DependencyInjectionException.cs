using System;

namespace HLab.DependencyInjection
{
    public class DependencyInjectionException : Exception
    {
        public DependencyInjectionException(string message) : base(message)
        {

        }
    }
}