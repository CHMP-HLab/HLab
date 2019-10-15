using System;
using System.Collections.Generic;
using System.Text;
using HLab.DependencyInjection.Annotations;

namespace HLab.DependencyInjection.UTests
{
    class RecursiveInjectedSingletonChild
    {
        [Import]public ISingleton Singleton { get; private set; } 
    }
    class RecursiveInjectedSingletonParent
    {
        [Import]public ISingleton Singleton { get; private set; } 
        [Import]public RecursiveInjectedSingletonChild Child { get; private set; } 
    }
}
