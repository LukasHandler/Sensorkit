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

namespace Sensorkit.LessonClasses
{
    public class xLesson17 : Lesson
    {
        public void Start(StackPanel output)
        {
            Init();



            Timer.Interval = TimeSpan.FromMilliseconds(10);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        private void Init()
        {

        }

        private void Timer_Tick(object sender, object e)
        {
            Run();
        }

        private void Run()
        {

        }

        protected override void OnStop()
        {

        }
    }
}
