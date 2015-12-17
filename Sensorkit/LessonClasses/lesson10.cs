namespace Sensorkit.LessonClasses
{
    using System;
    using System.Threading.Tasks;
    using Windows.Devices.Gpio;
    using Windows.UI.Xaml.Controls;

    public class Lesson10 : Lesson
    {
        private GpioPin clkPin;
        private GpioPin csPin;
        private GpioPin dioPin;
        private TextBlock outputText;

        public void Start(StackPanel output)
        {
            outputText = new TextBlock();
            output.Children.Add(outputText);

            Init();
            Timer.Interval = TimeSpan.FromMilliseconds(500);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        protected override void OnStop()
        {
            if (csPin != null)
            {
                csPin.Write(GpioPinValue.Low);
                csPin.Dispose();
            }

            if (clkPin != null)
            {
                clkPin.Write(GpioPinValue.Low);
                clkPin.Dispose();
            }

            if (dioPin != null)
            {
                if (dioPin.GetDriveMode() == GpioPinDriveMode.Output)
                {
                    dioPin.Write(GpioPinValue.Low);
                }

                dioPin.Dispose();
            }
        }

        private int CheckTemp()
        {
            dioPin.SetDriveMode(GpioPinDriveMode.Output);

            int dat1 = 0;
            int dat2 = 0;

            csPin.Write(GpioPinValue.Low);
            clkPin.Write(GpioPinValue.Low);
            dioPin.Write(GpioPinValue.High);
            Task.Delay(1);
            clkPin.Write(GpioPinValue.High);
            Task.Delay(1);

            clkPin.Write(GpioPinValue.Low);
            dioPin.Write(GpioPinValue.High);
            Task.Delay(1);
            clkPin.Write(GpioPinValue.High);
            Task.Delay(1);

            clkPin.Write(GpioPinValue.Low);
            dioPin.Write(GpioPinValue.Low);
            Task.Delay(1);
            clkPin.Write(GpioPinValue.High);
            dioPin.Write(GpioPinValue.High);
            Task.Delay(1);
            clkPin.Write(GpioPinValue.Low);
            dioPin.Write(GpioPinValue.High);
            Task.Delay(1);

            for (int i = 0; i < 8; i++)
            {
                clkPin.Write(GpioPinValue.High);
                Task.Delay(1);
                clkPin.Write(GpioPinValue.Low);
                Task.Delay(1);

                dioPin.SetDriveMode(GpioPinDriveMode.Input);

                int read = 0;

                switch (dioPin.Read())
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

                switch (dioPin.Read())
                {
                case GpioPinValue.Low:
                    read = 0;
                    break;
                case GpioPinValue.High:
                    read = 1;
                    break;
                }

                dat2 = dat2 | read << i;

                clkPin.Write(GpioPinValue.High);
                Task.Delay(1);
                clkPin.Write(GpioPinValue.Low);
                Task.Delay(1);
            }

            csPin.Write(GpioPinValue.High);

            return (dat1 == dat2) ? dat1 : 0;
        }

        private void Init()
        {
            const int ADC_CS_PIN = 22;
            const int ADC_CLK_PIN = 18;
            const int ADC_DIO_PIN = 27;

            var gpio = GpioController.GetDefault();

            csPin = gpio.OpenPin(ADC_CS_PIN);
            clkPin = gpio.OpenPin(ADC_CLK_PIN);
            dioPin = gpio.OpenPin(ADC_DIO_PIN);

            csPin.SetDriveMode(GpioPinDriveMode.Output);
            clkPin.SetDriveMode(GpioPinDriveMode.Output);
            dioPin.SetDriveMode(GpioPinDriveMode.Output);
        }

        private void Timer_Tick(object sender, object e)
        {
            OnStop();
            Init();
            var analogValue = CheckTemp();
            var temp = analogValue;

            outputText.Text = Convert.ToString(temp);
        }
    }
}