using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sensorkit.LessonClasses
{
    public class Lesson1 : Lesson
    {
        private GpioPin switchHallPin;
        private GpioPin doubleLedPin;
        private TextBlock outputText;

        private GpioPin adcCsPin;
        private GpioPin adcClkPin;
        private GpioPin adcDoPin;

        public void Start(StackPanel output, int hallIndicator)
        {
            outputText = new TextBlock();
            output.Children.Add(outputText);

            switch (hallIndicator)
            {
                case 0:
                    SwitchHallSensor();
                    break;
                case 1:
                    LinearHallSensor();
                    break;
                case 2:
                    LinearHallSensor();
                    break;
                default:
                    throw new Exception("The hallIndicator value must be between 0 and 2");
            }
        }

        private void LinearHallSensor()
        {
            LinearHallSensor_Init();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += LinearHallSensor_Timer_Tick;
            timer.Start();
        }

        private void LinearHallSensor_Init()
        {
            // Use pin numbers compatible with documentation
            const int RPI2_S_ADC_CS_PIN = 22;
            const int RPI2_S_ADC_CLK_PIN = 18;
            const int RPI2_S_ADC_DO_PIN = 27;

            var gpio = GpioController.GetDefault();

            adcCsPin = gpio.OpenPin(RPI2_S_ADC_CS_PIN);
            adcClkPin = gpio.OpenPin(RPI2_S_ADC_CLK_PIN);
            adcDoPin = gpio.OpenPin(RPI2_S_ADC_DO_PIN);

            adcCsPin.SetDriveMode(GpioPinDriveMode.Output);
            adcClkPin.SetDriveMode(GpioPinDriveMode.Output);
            adcDoPin.SetDriveMode(GpioPinDriveMode.Output);
        }

        private void LinearHallSensor_Timer_Tick(object sender, object e)
        {
            OnStop();
            LinearHallSensor_Init();
            var analogValue = LinearHallSensor_CheckMagnet();
            int mag = 210 - analogValue;
            outputText.Text = Convert.ToString(mag);
        }

        private int LinearHallSensor_CheckMagnet()
        {
            adcDoPin.SetDriveMode(GpioPinDriveMode.Output);

            int dat1 = 0;
            int dat2 = 0;

            adcCsPin.Write(GpioPinValue.Low);
            adcClkPin.Write(GpioPinValue.Low);
            adcDoPin.Write(GpioPinValue.High);
            Task.Delay(1);
            adcClkPin.Write(GpioPinValue.High);
            Task.Delay(1);

            adcClkPin.Write(GpioPinValue.Low);
            adcDoPin.Write(GpioPinValue.High);
            Task.Delay(1);
            adcClkPin.Write(GpioPinValue.High);
            Task.Delay(1);

            adcClkPin.Write(GpioPinValue.Low);
            adcDoPin.Write(GpioPinValue.Low);
            Task.Delay(1);
            adcClkPin.Write(GpioPinValue.High);
            adcDoPin.Write(GpioPinValue.High);
            Task.Delay(1);
            adcClkPin.Write(GpioPinValue.Low);
            adcDoPin.Write(GpioPinValue.High);
            Task.Delay(1);

            for (int i = 0; i < 8; i++)
            {
                adcClkPin.Write(GpioPinValue.High);
                Task.Delay(1);
                adcClkPin.Write(GpioPinValue.Low);
                Task.Delay(1);

                adcDoPin.SetDriveMode(GpioPinDriveMode.Input);

                int read = 0;

                switch(adcDoPin.Read())
                {
                    case GpioPinValue.Low:
                        read = 0;
                        break;
                    case GpioPinValue.High:
                        read = 1;
                        break;
                }

                dat1 = dat1 << 1 | read;
            }

            for (int i = 0; i < 8; i++)
            {
                int read = 0;

                switch(adcDoPin.Read())
                {
                    case GpioPinValue.Low:
                        read = 0;
                        break;
                    case GpioPinValue.High:
                        read = 1;
                        break;
                }

                dat2 = dat2 | read << i;

                adcClkPin.Write(GpioPinValue.High);
                Task.Delay(1);
                adcClkPin.Write(GpioPinValue.Low);
                Task.Delay(1);
            }

            return (dat1 == dat2) ? dat1 : 0;
        }

        private void SwitchHallSensor()
        {
            SwitchHallSensor_Init();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += SwitchHallSensor_Timer_Tick;
            timer.Start();
        }

        private void SwitchHallSensor_Init()
        {
            // Use pin numbers compatible with documentation
            const int RPI2_S_SWITCHHALL_PIN = 27;
            const int RPI2_S_DOUBLELED_PIN = 18;

            var gpio = GpioController.GetDefault();

            switchHallPin = gpio.OpenPin(RPI2_S_SWITCHHALL_PIN);
            doubleLedPin = gpio.OpenPin(RPI2_S_DOUBLELED_PIN);

            switchHallPin.SetDriveMode(GpioPinDriveMode.Input);
            doubleLedPin.SetDriveMode(GpioPinDriveMode.Output);
        }

        private void SwitchHallSensor_Timer_Tick(object sender, object e)
        {
            SwitchHallSensor_CheckMagnet();
        }

        private void SwitchHallSensor_CheckMagnet()
        {
            if (switchHallPin.Read() == GpioPinValue.Low)
            {
                Task.Delay(10);

                if (switchHallPin.Read() == GpioPinValue.Low)
                {
                    outputText.Text = "Magnet detected";
                    doubleLedPin.Write(GpioPinValue.High);
                    return;
                }
            }

           outputText.Text = "No magnet detected";
           doubleLedPin.Write(GpioPinValue.Low);

        }

        protected override void OnStop()
        {
            if (doubleLedPin != null)
            {
                doubleLedPin.Write(GpioPinValue.Low);
                doubleLedPin.Dispose();
            }

            if (adcCsPin != null)
            {
                adcCsPin.Write(GpioPinValue.Low);
                adcCsPin.Dispose();
            }

            if (adcClkPin != null)
            {
                adcClkPin.Write(GpioPinValue.Low);
                adcClkPin.Dispose();
            }

            if (adcDoPin != null)
            {
                if (adcDoPin.GetDriveMode() == GpioPinDriveMode.Output)
                {
                    adcDoPin.Write(GpioPinValue.Low);
                }

                adcDoPin.Dispose();
            }

            if (switchHallPin != null)
            {
                switchHallPin.Dispose();
            }
        }
    }
}

