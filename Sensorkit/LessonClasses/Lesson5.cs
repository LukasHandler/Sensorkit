﻿namespace Sensorkit.LessonClasses
{
    using System;
    using Windows.Devices.Gpio;
    using Windows.UI;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;

    public class Lesson5 : Lesson
    {
        private GpioPin knockPin;
        private GpioPin ledPin;
        private Ellipse outputLED;
        private TextBlock outputText;

        public void Start(StackPanel output)
        {
            outputLED = new Ellipse();
            outputLED.Width = 100;
            outputLED.Height = 100;
            outputLED.Stroke = new SolidColorBrush(Colors.Black);
            outputLED.Fill = new SolidColorBrush(Colors.White);
            output.Children.Add(outputLED);

            outputText = new TextBlock();

            Init();

            Timer.Interval = TimeSpan.FromMilliseconds(10);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        protected override void OnStop()
        {
            if (knockPin != null)
            {
                knockPin.Dispose();
            }

            if (ledPin != null)
            {
                ledPin.Write(GpioPinValue.Low);
                ledPin.Dispose();
            }
        }

        private void CheckKnock()
        {
            if (knockPin.Read() == GpioPinValue.Low)
            {
                outputText.Text = "Detected knocking";
                ledPin.Write(GpioPinValue.High);
                outputLED.Fill = new SolidColorBrush(Colors.Red);

            }
            else
            {
                ledPin.Write(GpioPinValue.Low);
                outputLED.Fill = new SolidColorBrush(Colors.White);
            }
        }

        private void Init()
        {
            const int KNOCK_PIN = 27;
            const int LED_PIN = 18;

            var gpio = GpioController.GetDefault();

            knockPin = gpio.OpenPin(KNOCK_PIN);
            ledPin = gpio.OpenPin(LED_PIN);

            knockPin.SetDriveMode(GpioPinDriveMode.Input);
            ledPin.SetDriveMode(GpioPinDriveMode.Output);
        }

        private void Timer_Tick(object sender, object e)
        {
            CheckKnock();
        }
    }
}