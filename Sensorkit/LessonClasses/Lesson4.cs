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

    public class Lesson4 : Lesson
    {
        private int count;
        private TextBlock counter;
        private GpioPin ledPin;
        private Ellipse outputLED;
        private TextBlock outputText;
        private GpioPin shockPin;

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

            Timer.Interval = TimeSpan.FromMilliseconds(10);
            Timer.Tick += Timer_Tick;
            Timer.Start();
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
    }
}