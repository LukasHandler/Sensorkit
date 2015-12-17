//----------------------------------------------------------------------------------------------
// <copyright file="Lesson3.cs" company="Lukas Handler">
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

    public class Lesson3 : Lesson
    {
        private LedStatus ledStatus;
        private Ellipse outputLED;
        private GpioPin redPin;
        private GpioPin yellowPin;

        enum LedStatus
        {
            Red, Green, Mixed, None
        }

        public void Start(StackPanel output)
        {
            outputLED = new Ellipse();
            outputLED.Width = 100;
            outputLED.Height = 100;
            outputLED.Fill = new SolidColorBrush(Colors.Transparent);
            output.Children.Add(outputLED);

            Init();

            Timer.Interval = TimeSpan.FromMilliseconds(500);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        protected override void OnStop()
        {
            if (redPin != null)
            {
                redPin.Write(GpioPinValue.Low);
                redPin.Dispose();
            }

            if (yellowPin != null)
            {
                yellowPin.Write(GpioPinValue.Low);
                yellowPin.Dispose();
            }
        }

        private void FlipLED()
        {
            switch (ledStatus)
            {
            case LedStatus.None:
            {
                redPin.Write(GpioPinValue.High);
                ledStatus = LedStatus.Red;
                outputLED.Fill = new SolidColorBrush(Colors.Red);
                break;
            }

            case LedStatus.Red:
            {
                yellowPin.Write(GpioPinValue.High);
                ledStatus = LedStatus.Mixed;
                outputLED.Fill = new SolidColorBrush(Colors.Orange);
                break;
            }

            case LedStatus.Mixed:
            {
                redPin.Write(GpioPinValue.Low);
                yellowPin.Write(GpioPinValue.High);
                ledStatus = LedStatus.Green;
                outputLED.Fill = new SolidColorBrush(Colors.Green);
                break;
            }

            case LedStatus.Green:
            {
                yellowPin.Write(GpioPinValue.Low);
                redPin.Write(GpioPinValue.High);
                ledStatus = LedStatus.Red;
                outputLED.Fill = new SolidColorBrush(Colors.Red);
                break;
            }
            }
        }

        private void Init()
        {
            const int RED_LED_PIN = 27;
            const int YELLOW_LED_PIN = 18;

            var gpio = GpioController.GetDefault();

            redPin = gpio.OpenPin(RED_LED_PIN);
            yellowPin = gpio.OpenPin(YELLOW_LED_PIN);

            redPin.SetDriveMode(GpioPinDriveMode.Output);
            yellowPin.SetDriveMode(GpioPinDriveMode.Output);

            ledStatus = LedStatus.None;
        }

        private void Timer_Tick(object sender, object e)
        {
            FlipLED();
        }
    }
}