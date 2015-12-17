namespace Sensorkit.LessonClasses
{
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

    public class Lesson9 : Lesson
    {
        private GpioPin irPin;
        private Ellipse outputLED;

        public void Start(StackPanel output)
        {
            outputLED = new Ellipse();
            outputLED.Width = 100;
            outputLED.Height = 100;
            outputLED.Stroke = new SolidColorBrush(Colors.Black);
            outputLED.Fill = new SolidColorBrush(Colors.Transparent);
            output.Children.Add(outputLED);

            Init();
            Timer.Interval = TimeSpan.FromMilliseconds(10);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        protected override void OnStop()
        {
            if (irPin != null)
            {
                irPin.Dispose();
            }
        }

        private void CheckSignal()
        {
            if (irPin.Read() == GpioPinValue.Low)
            {
                outputLED.Fill = new SolidColorBrush(Colors.Red);
            }
            else
            {
                outputLED.Fill = new SolidColorBrush(Colors.Transparent);
            }
        }

        private void Init()
        {
            const int IR_PIN = 18;

            var gpio = GpioController.GetDefault();

            irPin = gpio.OpenPin(IR_PIN);

            irPin.SetDriveMode(GpioPinDriveMode.Input);
        }

        private void Timer_Tick(object sender, object e)
        {
            CheckSignal();
        }
    }
}