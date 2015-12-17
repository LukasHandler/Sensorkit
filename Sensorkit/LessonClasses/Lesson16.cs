namespace Sensorkit.LessonClasses
{
    using System;
    using Windows.Devices.Gpio;
    using Windows.UI;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;

    public class Lesson16 : Lesson
    {
        private GpioPin ledPin;
        private GpioPin magicCup;
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
            if (magicCup != null)
            {
                magicCup.Dispose();
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

            magicCup = gpio.OpenPin(MERCURY_PIN);
            ledPin = gpio.OpenPin(LED_PIN);

            magicCup.SetDriveMode(GpioPinDriveMode.Input);
            ledPin.SetDriveMode(GpioPinDriveMode.Output);
        }

        private void Run()
        {
            if (magicCup.Read() == GpioPinValue.High)
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

        private void Timer_Tick(object sender, object e)
        {
            Run();
        }
    }
}