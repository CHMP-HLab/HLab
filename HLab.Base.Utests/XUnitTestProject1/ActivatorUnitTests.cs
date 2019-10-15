using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace HLab.Base.UTests
{
    interface IParentClass
    {
        IChildObject Child { get; }
    }
    class ParentClass : IParentClass
    {
        class H : Activator<ParentClass> { }

        public ParentClass(IChildObject child)
        {
            Child = child;
            H.Activate(this);
        }

        public IChildObject Child { get; }
    }

    interface IChildObject
    {
        IParentClass Parent { get; set; }
    }

    class ChildObject: IChildObject
    {
        static ChildObject()
        {
            ChildActivator.Add<IParentClass,IChildObject>((p,c)=>c.Parent = p);
        }
        public IParentClass Parent { get; set; }
    }



    public class ActivatorUnitTests
    {
        [Fact]
        public void Test()
        {
            var parent = new ParentClass(new ChildObject());

            Assert.Same(parent,parent.Child.Parent);
        }
    }
}
