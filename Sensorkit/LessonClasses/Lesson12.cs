using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Xaml.Controls;

namespace Sensorkit.LessonClasses
{
    public class Lesson12 : Lesson
    {
        private GpioPin btnPin;
        private GpioPin ledPin;
        private bool isOn;
        private TextBlock text;

        public void Start(StackPanel output)
        {
            text = new TextBlock();
            output.Children.Add(text);

            Init();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Init()
        {
            const int BTN_PIN = 27;
            const int LED_PIN = 18;

            var gpio = GpioController.GetDefault();

            btnPin = gpio.OpenPin(BTN_PIN);
            ledPin = gpio.OpenPin(LED_PIN);

            btnPin.SetDriveMode(GpioPinDriveMode.Input);
            ledPin.SetDriveMode(GpioPinDriveMode.Output);
        }

        private void Timer_Tick(object sender, object e)
        {
            Run();
        }

        private void Run()
        {
            if (btnPin.Read() == GpioPinValue.Low)
            {
                text.Text = "Button is pressed";

                if (isOn)
                {
                    isOn = false;
                    ledPin.Write(GpioPinValue.Low);
                }
                else
                {
                    isOn = true;
                    ledPin.Write(GpioPinValue.High);
                }

                Task.Delay(200);

                return;
            }

            text.Text = "";
        }

        protected override void OnStop()
        {
            if (ledPin != null)
            {
                ledPin.Write(GpioPinValue.Low);
                ledPin.Dispose();
            }

            if (btnPin != null)
            {
                btnPin.Dispose();
            }
        }
    }
}
