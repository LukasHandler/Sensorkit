//----------------------------------------------------------------------------------------------
// <copyright file="xLesson18.cs" company="Lukas Handler">
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

    public class xLesson18 : Lesson
    {
        private GpioPin clockPin;
        private int counter;
        private GpioPinValue currentStatus;
        private GpioPin dataPin;
        bool flag;
        private GpioPinValue lastStatus;
        private GpioPin switchPin;
        private TextBlock text;

        public void Start(StackPanel output)
        {
            text = new TextBlock();
            output.Children.Add(text);

            Init();
            counter = 0;

            Timer.Interval = TimeSpan.FromMilliseconds(100);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        protected override void OnStop()
        {
            if (clockPin != null)
            {
                clockPin.Dispose();
            }

            if (dataPin != null)
            {
                dataPin.Dispose();
            }

            if (switchPin != null)
            {
                switchPin.Dispose();
            }
        }

        private void Init()
        {
            // SWPIN
            const int CLOCK_PIN = 22;

            // ROA
            const int DATA_PIN = 18;

            // ROB
            const int SWITCH_PIN = 27;

            var gpio = GpioController.GetDefault();

            clockPin = gpio.OpenPin(CLOCK_PIN);
            dataPin = gpio.OpenPin(DATA_PIN);
            switchPin = gpio.OpenPin(SWITCH_PIN);

            clockPin.SetDriveMode(GpioPinDriveMode.Input);
            dataPin.SetDriveMode(GpioPinDriveMode.Input);
            switchPin.SetDriveMode(GpioPinDriveMode.Input);
        }

        private void Run()
        {
            lastStatus = switchPin.Read();

            if (dataPin.Read() == GpioPinValue.High)
            {
                currentStatus = switchPin.Read();
                flag = true;
            }

            if (flag)
            {
                flag = false;
                if ((lastStatus == GpioPinValue.Low) && (currentStatus == GpioPinValue.High))
                {
                    counter++;
                }

                if ((lastStatus == GpioPinValue.High) && (currentStatus == GpioPinValue.Low))
                {
                    counter--;
                }
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            Run();
            text.Text = "Counter: " + counter;
        }
    }
}