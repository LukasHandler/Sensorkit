//----------------------------------------------------------------------------------------------
// <copyright file="VmMain.cs" company="Lukas Handler">
// Copyright (c) Lukas Handler.  All rights reserved.
// </copyright>
// <summary>
// The ViewModel for the <c>MainPage</c>.
// </summary>
//-------------------------------------------------------------------------------------------------

namespace Sensorkit.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Sensorkit.LessonClasses;
    using Sensorkit.Model;
    using Windows.ApplicationModel.Resources;
    using Windows.Devices.Gpio;
    using Windows.Security.ExchangeActiveSyncProvisioning;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// This creates the lessons from the resources and provides functionality for the <c>MainPage</c>.
    /// </summary>
    public class VmMain
    {
        /// <summary>
        /// The accessor for the resources.
        /// </summary>
        private static ResourceLoader resources = ResourceLoader.GetForCurrentView("Messages");

        /// <summary>
        /// Gets or sets the lessons.
        /// </summary>
        /// <value>
        /// The lessons.
        /// </value>
        public List<LessonModel> Lessons
        {
            get;
            set;
        }

        /// <summary>
        /// Creating the lessons.
        /// </summary>
        public void CreateLessons()
        {
            this.Lessons = new List<LessonModel>();

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

                LessonModel lesson = new LessonModel()
                {
                    Id = i,
                    RunAble = runAble,
                    Name = headLine,
                    Content = content,
                    Arguments = argsArray
                };
                this.Lessons.Add(lesson);
            }
        }

        /// <summary>
        /// Determines whether a Raspberry Pi is connected or not.
        /// </summary>
        /// <returns>True if a Raspberry Pi is connected.</returns>
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

        /// <summary>
        /// Checks if running the current lesson is valid.
        /// </summary>
        /// <param name="lesson">The lesson the user would like to run.</param>
        /// <returns>A string containing an error message or <c>String.Empty</c> if everything's fine.</returns>
        public string ValidateSystem(LessonModel lesson)
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