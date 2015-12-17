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

    public class Lesson15 : Lesson
    {
        private GpioPin ledPin;
        private GpioPin mercuryPin;
        private Ellipse outputLED;

        public void Start(StackPanel output)
        {
            Init();

            outputLED = new Ellipse();
            outputLED.Width = 100;
            outputLED.Height = 100;
            outputLED.Fill = new SolidColorBrush(Colors.Transparent);
            outputLED.Stroke = new SolidColorBrush(Colors.Black);
            output.Children.Add(outputLED);

            Timer.Interval = TimeSpan.FromMilliseconds(10);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        protected override void OnStop()
        {
            if (mercuryPin != null)
            {
                mercuryPin.Dispose();
            }

            if (ledPin != null)
            {
                ledPin.Write(GpioPinValue.Low);
                ledPin.Dispose();
            }
        }

        private void Init()
        {
            const int MERCURY_PIN = 27;
            const int LED_PIN = 18;

            var gpio = GpioController.GetDefault();

            ledPin = gpio.OpenPin(LED_PIN);
            mercuryPin = gpio.OpenPin(MERCURY_PIN);

            mercuryPin.SetDriveMode(GpioPinDriveMode.Input);
            ledPin.SetDriveMode(GpioPinDriveMode.Output);
        }

        private void Run()
        {
            if (mercuryPin.Read() == GpioPinValue.High)
            {
                outputLED.Fill = new SolidColorBrush(Colors.Transparent);
                ledPin.Write(GpioPinValue.Low);
            }
            else
            {
                outputLED.Fill = new SolidColorBrush(Colors.Red);
                ledPin.Write(GpioPinValue.High);
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            Run();
        }
    }
}