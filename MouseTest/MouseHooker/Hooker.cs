using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace MouseHooker
{
    public interface IMouseHooker
    {
        event EventHandler<HookMouseEventArg> MouseMove;
        void Hook();
        void UnHook();
        bool Hooked();
    }

    public abstract class MouseHooker : IMouseHooker
    {
        public event EventHandler<HookMouseEventArg> MouseMove;

        protected virtual void OnMouseMove(HookMouseEventArg args)
        {
            MouseMove?.Invoke(this,args);
        }

        public abstract void Hook();
        public abstract void UnHook();
        public abstract bool Hooked();
    }
}
