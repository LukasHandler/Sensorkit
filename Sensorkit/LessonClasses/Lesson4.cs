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
    public class Lesson4 : Lesson
    {
        private GpioPin shockPin;
        private GpioPin ledPin;
        private int count;
        private TextBlock outputText;
        private TextBlock counter;
        private Ellipse outputLED;

        public void Start(StackPanel output)
        {
            counter = new TextBlock();
            output.Children.Add(counter);

            outputText = new TextBlock();
            output.Children.Add(outputText);

            outputLED = new Ellipse();
            outputLED.Width = 100;
            outputLED.Height = 100;
            outputLED.Stroke = new SolidColorBrush(Colors.Black);
            outputLED.Fill = new SolidColorBrush(Colors.White);
            output.Children.Add(outputLED);

            Init();

            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Init()
        {
            const int SHOCK_PIN = 27;
            const int LED_PIN = 18;

            var gpio = GpioController.GetDefault();

            shockPin = gpio.OpenPin(SHOCK_PIN);
            ledPin = gpio.OpenPin(LED_PIN);

            shockPin.SetDriveMode(GpioPinDriveMode.Input);
            ledPin.SetDriveMode(GpioPinDriveMode.Output);
        }

        private void Timer_Tick(object sender, object e)
        {
            CheckShake();
        }

        private void CheckShake()
        {
            counter.Text = "Counter: " + count;

            if (shockPin.Read() == GpioPinValue.Low)
            {
                Task.Delay(10);

                if (shockPin.Read() == GpioPinValue.Low)
                {
                    count++;
                    outputText.Text = "Detected Shaking";
                    outputLED.Fill = new SolidColorBrush(Colors.Red);
                    ledPin.Write(GpioPinValue.High);
                    return;
                }
            }

            outputText.Text = string.Empty;
            ledPin.Write(GpioPinValue.Low);

            outputLED.Fill = new SolidColorBrush(Colors.White);
        }

        protected override void OnStop()
        {
            if (shockPin != null)
            {
                shockPin.Dispose();
            }

            if (ledPin != null)
            {
                ledPin.Write(GpioPinValue.Low);
                ledPin.Dispose();
            }
        }
    }
}
