using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sensorkit.LessonClasses
{
    public abstract class Lesson
    {
        protected DispatcherTimer timer;

        public Lesson()
        {
            timer = new DispatcherTimer();
        }

        public void Stop()
        {
            if (timer != null && timer.IsEnabled)
            {
                timer.Stop();
            }

            OnStop();
        }

        protected abstract void OnStop();
    }
}
