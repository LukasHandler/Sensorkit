using Sensorkit.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Gpio;
using Windows.Security.ExchangeActiveSyncProvisioning;
using System.Reflection;
using Sensorkit.LessonClasses;
using Windows.UI.Xaml.Controls;

namespace Sensorkit.ViewModel
{
    public class VmMain
    {
        /// <summary>
        /// All our lessons are in here.
        /// </summary>
        public List<LessonModel> Lessons { get; set; }

        private static ResourceLoader resources = ResourceLoader.GetForCurrentView("Messages");

        /// <summary>
        /// Creating the lessons.
        /// </summary>
        public void CreateLessons()
        {
            Lessons = new List<LessonModel>();

            ResourceLoader resources = ResourceLoader.GetForCurrentView("Lessons");

            for (int i = 0; true; i++)
            {
                string text = resources.GetString("lesson" + i);

                // Be careful - If your lessons are listed like 0, 1, 2, 4 - It will stop after not finding #3.
                if (text == string.Empty)
                {
                    break;
                }

                string headLine = string.Empty;
                bool runAble = true;

                int k = 0;

                while (text[k] != '#')
                {
                    if (k == 0 && text[k] == '-')
                    {
                        runAble = false;
                    }
                    else
                    {
                        headLine += text[k];
                    }

                    k++;
                }

                string content = text.Substring(k + 1);

                string args = resources.GetString("lesson" + i + "args");
                var argsArray = args.Split('#');

                LessonModel lesson = new LessonModel() { Id = i, RunAble = runAble, Name = headLine, Content = content, Arguments = argsArray };
                Lessons.Add(lesson);
            }
        }

        /// <summary>
        /// Is a Raspberry Pi conntected?
        /// </summary>
        public bool IsRaspConnected()
        {
            GpioController gpio;

            try
            {
                gpio = GpioController.GetDefault();
            }
            catch
            {
                return false;
            }

            if (gpio == null)
            {
                return false;
            }

            var deviceInfo = new EasClientDeviceInformation();

            if (!(deviceInfo.SystemProductName.IndexOf("Raspberry", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                return false;
            }

            return true;
        }

        public string validateSystem(LessonModel lesson)
        {
            string errorMessage = string.Empty;

            if (!lesson.RunAble)
            {
                errorMessage += resources.GetString("LessonRunAble") + Environment.NewLine;
                return "Error" + Environment.NewLine + errorMessage;
            }

            if (!this.IsRaspConnected())
            {
                errorMessage += resources.GetString("LessonRasp") + Environment.NewLine;
            }

            var path = "Sensorkit.LessonClasses.Lesson";
            int currentLesson = lesson.Id;

            Type lessonType = Type.GetType(path + currentLesson);

            if (lessonType == null)
            {
                errorMessage += resources.GetString("LessonClass") + path + currentLesson + Environment.NewLine;
            }
            else
            {
                if (!lessonType.GetTypeInfo().IsSubclassOf(typeof(Lesson)))
                {
                    errorMessage += string.Format(resources.GetString("LessonChild"), path, currentLesson) + Environment.NewLine;
                }

                MethodInfo lessonMethod = lessonType.GetMethod("Start");

                if (lessonMethod == null)
                {
                    errorMessage += resources.GetString("LessonStart") + path + currentLesson + Environment.NewLine;
                }
                else
                {
                    var attributes = lessonMethod.GetParameters();

                    if (attributes.Count() == 0)
                    {
                        errorMessage += resources.GetString("LessonArg") + Environment.NewLine;
                    }

                    if (!attributes[0].ParameterType.Equals(typeof(StackPanel)))
                    {
                        errorMessage += resources.GetString("Lesson1Arg") + Environment.NewLine;
                    }
                }

                lessonMethod = lessonType.GetMethod("Stop");

                if (lessonMethod == null)
                {
                    errorMessage += resources.GetString("LessonStop") + path + currentLesson + Environment.NewLine;
                }
            }

            if (errorMessage == string.Empty)
            {
                return null;
            }
            else
            {
                return "Error" + Environment.NewLine + errorMessage;
            }
        }
    }
}

