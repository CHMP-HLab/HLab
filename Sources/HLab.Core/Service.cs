using System;
using System.Collections.Generic;
using System.Text;
using HLab.Core.Annotations;

namespace HLab.Core
{
    public abstract class Service : IService
    {
        public ServiceState ServiceState { get; protected set; } = ServiceState.Available;
    }
}
