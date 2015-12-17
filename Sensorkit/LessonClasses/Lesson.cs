namespace Sensorkit.LessonClasses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public abstract class Lesson
    {
        public Lesson()
        {
            this.Timer = new DispatcherTimer();
        }

        protected DispatcherTimer Timer
        {
            get;
            set;
        }

        public void Stop()
        {
            if (this.Timer != null && this.Timer.IsEnabled)
            {
                this.Timer.Stop();
            }

            this.OnStop();
        }

        protected abstract void OnStop();
    }
}