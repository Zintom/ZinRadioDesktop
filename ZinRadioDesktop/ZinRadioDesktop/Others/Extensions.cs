using System.ComponentModel;
using System.Windows.Forms;

namespace ZinRadioDesktop
{
    public static class Extensions
    {
        /// <summary>
        /// Invokes the given <paramref name="action"/> on the UI thread of the <paramref name="control"/> unless
        /// we are already executing on the UI thread of the control.
        /// </summary>
        public static void SafeInvokeIfRequired(this ISynchronizeInvoke control, MethodInvoker action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action, null);
            }
            else
            {
                action.Invoke();
            }
        }
    }
}
