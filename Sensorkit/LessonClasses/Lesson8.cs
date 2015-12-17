namespace Sensorkit.LessonClasses
{
    using System;
    using Windows.Devices.Gpio;
    using Windows.UI;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;

    public class Lesson8 : Lesson
    {
        private GpioPin ledPin;
        private Ellipse outputLED;
        private GpioPin switchPin;

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
            Timer.Interval = TimeSpan.FromMilliseconds(500);
            Timer.Tick += Timer_TickMini;
            Timer.Start();
        }

        private void Reed()
        {
            Timer.Interval = TimeSpan.FromMilliseconds(500);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            CheckMagnet();
        }

        private void Timer_TickMini(object sender, object e)
        {
            CheckMagnetMini();
        }
    }
}