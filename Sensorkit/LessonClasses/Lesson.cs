//----------------------------------------------------------------------------------------------
// <copyright file="Lesson.cs" company="Lukas Handler">
// Copyright (c) Lukas Handler.  All rights reserved.
// </copyright>
// <summary>
// This is our abstract class which every Lesson has to inherit from.
// </summary>
//-------------------------------------------------------------------------------------------------

namespace Sensorkit.LessonClasses
{
    using Windows.UI.Xaml;

    /// <summary>
    /// This class contains the dispatcher timer and main properties and methods lessons need.
    /// </summary>
    public abstract class Lesson
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Lesson"/> class.
        /// </summary>
        public Lesson()
        {
            this.Timer = new DispatcherTimer();
        }

        /// <summary>
        /// Gets or sets the timer.
        /// </summary>
        /// <value>
        /// The timer to make UI Changes.
        /// </value>
        protected DispatcherTimer Timer
        {
            get;
            set;
        }

        /// <summary>
        /// Stops this lesson.
        /// </summary>
        public void Stop()
        {
            if (this.Timer != null && this.Timer.IsEnabled)
            {
                this.Timer.Stop();
            }

            this.OnStop();
        }

        /// <summary>
        /// Gets called when to stop the lesson - close and dispose pins.
        /// </summary>
        protected abstract void OnStop();
    }
}