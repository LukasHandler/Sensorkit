using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Xaml.Controls;

namespace Sensorkit.LessonClasses
{
    public class Lesson11 : Lesson
    {
        private GpioPin buzzerPin;
        private bool isPlaying;

        public void Start(StackPanel output)
        {
            ActiveBuzzerInit();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += ActiveBuzzerTimerTick;
            timer.Start();
        }

        private void ActiveBuzzerInit()
        {
            const int BUZZER_PIN = 18;

            var gpio = GpioController.GetDefault();

            buzzerPin = gpio.OpenPin(BUZZER_PIN);

            buzzerPin.SetDriveMode(GpioPinDriveMode.Output);
        }

        private void ActiveBuzzerTimerTick(object sender, object e)
        {
            ActiveBuzzerRun();
        }

        private void ActiveBuzzerRun()
        {
            if (isPlaying)
            {
                buzzerPin.Write(GpioPinValue.Low);
                isPlaying = false;

            }
            else
            {
                buzzerPin.Write(GpioPinValue.High);
                isPlaying = true;
            }
        }

        protected override void OnStop()
        {
            if (buzzerPin != null)
            {
                buzzerPin.Write(GpioPinValue.Low);
                buzzerPin.Dispose();
            }
        }
    }
}
