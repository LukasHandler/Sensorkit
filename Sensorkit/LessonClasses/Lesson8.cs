using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Sensorkit.LessonClasses
{
    public class Lesson8 : Lesson
    {
        private GpioPin switchPin;
        private GpioPin ledPin;
        private Ellipse outputLED;

        public void Start(StackPanel output, int switchIndicator)
        {
            outputLED = new Ellipse();
            outputLED.Width = 100;
            outputLED.Height = 100;
            outputLED.Stroke = new SolidColorBrush(Colors.Black);
            outputLED.Fill = new SolidColorBrush(Colors.Transparent);
            output.Children.Add(outputLED);

            switch (switchIndicator)
            {
                case 0:
                    Init();
                    Reed();
                    break;
                case 1:
                    Init();
                    MiniReed();
                    break;
                default:
                    throw new Exception("The switchIndicator value must be 0 or 1");
            }
        }

        private void Init()
        {
            const int SWITCH_PIN = 27;
            const int LED_PIN = 18;

            var gpio = GpioController.GetDefault();

            switchPin = gpio.OpenPin(SWITCH_PIN);
            ledPin = gpio.OpenPin(LED_PIN);

            switchPin.SetDriveMode(GpioPinDriveMode.Input);
            ledPin.SetDriveMode(GpioPinDriveMode.Output);
        }

        private void MiniReed()
        {
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_TickMini;
            timer.Start();
        }

        private void Reed()
        {
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            CheckMagnet();
        }

        private void CheckMagnet()
        {
            if (switchPin.Read() == GpioPinValue.High)
            {
                outputLED.Fill = new SolidColorBrush(Colors.Red);
                ledPin.Write(GpioPinValue.High);
            }
            else
            {
                outputLED.Fill = new SolidColorBrush(Colors.Transparent);
                ledPin.Write(GpioPinValue.Low);
            }
        }

        private void Timer_TickMini(object sender, object e)
        {
            CheckMagnetMini();
        }

        private void CheckMagnetMini()
        {
            if (switchPin.Read() == GpioPinValue.Low)
            {
                outputLED.Fill = new SolidColorBrush(Colors.Red);
                ledPin.Write(GpioPinValue.High);
            }
            else
            {
                outputLED.Fill = new SolidColorBrush(Colors.Transparent);
                ledPin.Write(GpioPinValue.Low);
            }
        }

        protected override void OnStop()
        {
            if (ledPin != null)
            {
                ledPin.Write(GpioPinValue.Low);
                ledPin.Dispose();
            }

            if (switchPin != null)
            {
                switchPin.Dispose();
            }
        }
    }
}
