namespace Sensorkit.LessonClasses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Windows.Devices.Gpio;
    using Windows.UI.Xaml.Controls;

    public class xLesson20 : Lesson
    {
        private GpioPin adcClkPin;
        private GpioPin adcCsPin;
        private GpioPin adcDoPin;
        private TextBlock outputText;

        public void Start(StackPanel output)
        {
            Init();

            outputText = new TextBlock();
            output.Children.Add(outputText);

            Timer.Interval = TimeSpan.FromMilliseconds(100);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        protected override void OnStop()
        {
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
        }

        private int CheckResistor()
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

                switch (adcDoPin.Read())
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

                switch (adcDoPin.Read())
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

        private void Init()
        {
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

        private void Timer_Tick(object sender, object e)
        {
            OnStop();
            Init();
            var analogValue = CheckResistor();
            int ill = 210 - analogValue;
            outputText.Text = "Current illumination: " + Convert.ToString(ill);
        }
    }
}