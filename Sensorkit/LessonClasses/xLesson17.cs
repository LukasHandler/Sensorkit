//----------------------------------------------------------------------------------------------
// <copyright file="xLesson17.cs" company="Lukas Handler">
// Copyright (c) Lukas Handler.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Sensorkit.LessonClasses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Windows.Devices.Gpio;
    using Windows.UI;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;

    public class xLesson17 : Lesson
    {
        public void Start(StackPanel output)
        {
            Init();

            Timer.Interval = TimeSpan.FromMilliseconds(10);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        protected override void OnStop()
        {
        }

        private void Init()
        {
        }

        private void Run()
        {
        }

        private void Timer_Tick(object sender, object e)
        {
            Run();
        }
    }
}