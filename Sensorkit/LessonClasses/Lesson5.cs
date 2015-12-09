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
    public class Lesson5 : Lesson
    {
        private GpioPin knockPin;
        private GpioPin ledPin;
        private TextBlock outputText;
        private Ellipse outputLED;

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

            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            CheckKnock();
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
    }
}
