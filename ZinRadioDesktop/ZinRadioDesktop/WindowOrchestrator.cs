using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZinRadioDesktop
{
    public static class WindowOrchestrator
    {
        /// <summary>
        /// Animates the forms X axis towards the target.
        /// </summary>
        /// <param name="targetX"></param>
        public static async Task SlideFormX(Form form, double targetX, AsyncLock? synchronisationLock = null)
        {
            // Aqquire a lock on the user specified lock object.
            IDisposable? disposableLock = null;
            if (synchronisationLock != null) { disposableLock = await synchronisationLock.LockAsync(); }

            double x = form.Location.X;

            var timer = new System.Windows.Forms.Timer();
            timer.Tick += (o, e) =>
            {
                if ((x == targetX) ||
                    (x < targetX && x >= targetX - 1) ||
                    (x > targetX && x <= targetX + 1))
                {
                    form.Left = (int)targetX;
                    timer?.Stop();
                    timer?.Dispose();

                    // Release lock if it exists.
                    disposableLock?.Dispose();

                    return;
                }

                x = Program.CosineInterpolate(x, targetX, 0.25);
                form.Left = (int)x;
            };
            timer.Interval = 8;
            timer.Start();
        }

        public static void AnimateOpacity(Form form, float targetOpacity, Action? onFinish = null, object? synchronisationLock = null)
        {
            // Aqquire a lock on the sync object if the caller has provided on.
            if (synchronisationLock != null) { Monitor.Enter(synchronisationLock); }

            bool fadingIn = form.Opacity < targetOpacity;

            var timer = new System.Windows.Forms.Timer();
            timer.Tick += (_, _) =>
            {
                if ((fadingIn && form.Opacity >= targetOpacity) ||
                    (!fadingIn && form.Opacity <= targetOpacity))
                {
                    timer.Stop();
                    timer.Dispose();

                    // Release lock as animation has finished.
                    if (synchronisationLock != null) { Monitor.Exit(synchronisationLock); }

                    // Inform caller that the animation has finished (if they are subscribed).
                    onFinish?.Invoke();
                    return;
                }

                form.Opacity += fadingIn ? 0.1 : -0.1;
            };
            timer.Interval = 8;
            timer.Start();
        }
    }
}
