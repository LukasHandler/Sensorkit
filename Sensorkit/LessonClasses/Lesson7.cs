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

namespace Sensorkit.LessonClasses
{
    public class Lesson7 : Lesson
    {
        private GpioPin laserPin;

        private GpioPinValue currentValue;
        private Ellipse outputLED;

        public void Start(StackPanel output)
        {
            outputLED = new Ellipse();
            outputLED.Width = 100;
            outputLED.Height = 100;
            outputLED.Stroke = new SolidColorBrush(Colors.Black);
            outputLED.Fill = new SolidColorBrush(Colors.White);
            output.Children.Add(outputLED);

            Init();

            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Init()
        {
            const int LASER_PIN = 18;

            var gpio = GpioController.GetDefault();

            laserPin = gpio.OpenPin(LASER_PIN);

            laserPin.SetDriveMode(GpioPinDriveMode.Output);

            currentValue = GpioPinValue.Low;
        }

        private void Timer_Tick(object sender, object e)
        {
            Run();
        }

        private void Run()
        {
            switch(currentValue)
            {
                case GpioPinValue.High:
                    laserPin.Write(GpioPinValue.Low);
                    currentValue = GpioPinValue.Low;
                    outputLED.Fill = new SolidColorBrush(Colors.Transparent);
                    break;
                case GpioPinValue.Low:
                    laserPin.Write(GpioPinValue.High);
                    currentValue = GpioPinValue.High;
                    outputLED.Fill = new SolidColorBrush(Colors.Red);
                    break;
            }
        }

        protected override void OnStop()
        {
            if (laserPin != null)
            {
                laserPin.Write(GpioPinValue.Low);
                laserPin.Dispose();
            }
        }
    }
}
