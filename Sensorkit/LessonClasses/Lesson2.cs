﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Sensorkit.LessonClasses
{
    public class Lesson2 : Lesson
    {
        enum LedStatus { Red, Green, Blue };
        private LedStatus ledStatus;
        private Ellipse outputLED;
        private GpioPin redpin;
        private GpioPin greenpin;
        private GpioPin bluepin;

        public void Start(StackPanel output)
        {
            outputLED = new Ellipse();
            outputLED.Width = 100;
            outputLED.Height = 100;
            outputLED.Fill = new SolidColorBrush(Colors.Transparent);
            output.Children.Add(outputLED);

            Init();

            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Init()
        {
            const int RED_LED_PIN = 22;
            const int GREEN_LED_PIN = 18;
            const int BLUE_LED_PIN = 27;

            var gpio = GpioController.GetDefault();

            greenpin = gpio.OpenPin(GREEN_LED_PIN);
            bluepin = gpio.OpenPin(BLUE_LED_PIN);
            redpin = gpio.OpenPin(RED_LED_PIN);

            redpin.SetDriveMode(GpioPinDriveMode.Output);
            greenpin.SetDriveMode(GpioPinDriveMode.Output);
            bluepin.SetDriveMode(GpioPinDriveMode.Output);
        }

        private void Timer_Tick(object sender, object e)
        {
            FlipLED();
        }

        private void FlipLED()
        {
            switch (ledStatus)
            {
                case LedStatus.Red:
                    //turn on red
                    redpin.Write(GpioPinValue.High);
                    bluepin.Write(GpioPinValue.Low);
                    greenpin.Write(GpioPinValue.Low);

                    outputLED.Fill = new SolidColorBrush(Colors.Red);
                    ledStatus = LedStatus.Green;    // go to next state
                    break;
                case LedStatus.Green:

                    //turn on green
                    redpin.Write(GpioPinValue.Low);
                    greenpin.Write(GpioPinValue.High);
                    bluepin.Write(GpioPinValue.Low);

                    outputLED.Fill = new SolidColorBrush(Colors.Green);
                    ledStatus = LedStatus.Blue;     // go to next state
                    break;
                case LedStatus.Blue:
                    //turn on blue
                    redpin.Write(GpioPinValue.Low);
                    greenpin.Write(GpioPinValue.Low);
                    bluepin.Write(GpioPinValue.High);

                    outputLED.Fill = new SolidColorBrush(Colors.Blue);
                    ledStatus = LedStatus.Red;      // go to next state
                    break;
            }
        }

        protected override void OnStop()
        {
            if (redpin != null)
            {
                redpin.Write(GpioPinValue.Low);
                redpin.Dispose();
            }

            if (bluepin != null)
            {
                bluepin.Write(GpioPinValue.Low);
                bluepin.Dispose();
            }

            if (greenpin != null)
            {
                greenpin.Write(GpioPinValue.Low);
                greenpin.Dispose();
            }
        }
    }
}

