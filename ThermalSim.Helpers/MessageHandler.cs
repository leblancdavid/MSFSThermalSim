using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThermalSim.Helpers
{
    public class MessageHandler : NativeWindow
    {
        public event EventHandler<Message> MessageReceived;
        private const int WM_USER_SIMCONNECT = 0x0402;

        public MessageHandler()
        {
        }

        public void CreateHandle()
        {
            CreateHandle(new CreateParams());
        }

        protected override void WndProc(ref Message msg)
        {
            // filter messages here for SimConnect
            if (msg.Msg == WM_USER_SIMCONNECT && MessageReceived != null)
                try
                {
                    MessageReceived.DynamicInvoke(this, msg);
                }
                catch { } // If calling assembly generates an exception, we shouldn't allow it to break this process
            else
                base.WndProc(ref msg);
        }

        internal void Stop()
        {
            base.ReleaseHandle();
            base.DestroyHandle();
        }
    }
}
