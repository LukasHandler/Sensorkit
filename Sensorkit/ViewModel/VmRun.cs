//----------------------------------------------------------------------------------------------
// <copyright file="VmRun.cs" company="Lukas Handler">
// Copyright (c) Lukas Handler.  All rights reserved.
// </copyright>
// <summary>
// Represents our ViewModel for the <c>RunPage</c>.
// </summary>
//-------------------------------------------------------------------------------------------------

namespace Sensorkit.ViewModel
{
    using System;
    using System.Linq;
    using System.Reflection;
    using LessonClasses;
    using Model;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// This class starts the methods, creates the arguments and validates them using reflection.
    /// </summary>
    public class VmRun
    {
        /// <summary>
        /// Saves the current Lesson - so when we want to stop it - we know which lesson to stop.
        /// </summary>
        private Lesson currentLesson;

        /// <summary>
        /// Creates the input fields.
        /// </summary>
        /// <param name="inputGrid">The input grid which is a container for our UI Elements.</param>
        /// <param name="selectedModel">The selected model.</param>
        public void CreateInputFields(Grid inputGrid, LessonModel selectedModel)
        {
            int lessonId = selectedModel.Id;

            var lessonMethod = this.GetMethodInfo(lessonId, "Start");

            var parameter = lessonMethod.GetParameters().Skip(1);

            if (parameter.Count() != 0)
            {
                inputGrid.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = GridLength.Auto
                });
                inputGrid.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(100)
                });
                inputGrid.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = GridLength.Auto
                });
                inputGrid.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
            }

            int i = 0;

            foreach (var item in parameter)
            {
                inputGrid.RowDefinitions.Add(new RowDefinition());

                // Variable Name
                TextBlock labelName = new TextBlock();
                labelName.Width = double.NaN;
                labelName.Text = item.Name;
                labelName.Margin = new Windows.UI.Xaml.Thickness(10);
                labelName.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
                labelName.SetValue(Grid.RowProperty, i);
                labelName.SetValue(Grid.ColumnProperty, 0);

                // Input
                TextBox input = new TextBox();
                input.Margin = new Windows.UI.Xaml.Thickness(5);
                input.Height = 20;
                input.SetValue(Grid.RowProperty, i);
                input.SetValue(Grid.ColumnProperty, 1);

                // Type
                TextBlock labelType = new TextBlock();
                labelType.Width = double.NaN;
                labelType.Text = "Type: " + item.ParameterType.Name;
                labelType.Margin = new Windows.UI.Xaml.Thickness(10);
                labelType.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
                labelType.SetValue(Grid.RowProperty, i);
                labelType.SetValue(Grid.ColumnProperty, 2);

                inputGrid.Children.Add(labelName);
                inputGrid.Children.Add(input);
                inputGrid.Children.Add(labelType);

                if (i < selectedModel.Arguments.Count())
                {
                    // LabelArguments
                    TextBlock labelArgument = new TextBlock();
                    labelArgument.Text = selectedModel.Arguments[i];
                    labelArgument.Margin = new Windows.UI.Xaml.Thickness(10);
                    labelArgument.VerticalAlignment = VerticalAlignment.Center;
                    labelArgument.TextWrapping = TextWrapping.Wrap;
                    labelArgument.SetValue(Grid.RowProperty, i);
                    labelArgument.SetValue(Grid.ColumnProperty, 3);

                    inputGrid.Children.Add(labelArgument);
                }

                i++;
            }
        }

        /// <summary>
        /// Runs the lesson.
        /// </summary>
        /// <param name="lessonId">The lesson identifier.</param>
        /// <param name="outputGrid">The output grid.</param>
        /// <param name="inputStrings">The input strings.</param>
        public void RunLesson(int lessonId, StackPanel outputGrid, string[] inputStrings)
        {
            var parameters = this.CreatePrameters(lessonId, outputGrid, inputStrings);
            var lessonMethod = this.GetMethodInfo(lessonId, "Start");
            var path = "Sensorkit.LessonClasses.Lesson";
            Type lessonType = Type.GetType(path + lessonId);

            // Set currentLesson so we can use the created instance when stopping.
            this.currentLesson = (Lesson)Activator.CreateInstance(lessonType);

            // Start the lesson.
            lessonMethod.Invoke(this.currentLesson, parameters);
        }

        /// <summary>
        /// Stops the lesson.
        /// </summary>
        /// <param name="lessonId">The lesson identifier.</param>
        /// <param name="output">The output.</param>
        public void StopLesson(int lessonId, StackPanel output)
        {
            if (this.currentLesson != null)
            {
                var lessonMethod = this.GetMethodInfo(lessonId, "Stop");
                lessonMethod.Invoke(this.currentLesson, new object[0]);
                output.Children.Clear();

                this.currentLesson = null;
            }
        }

        /// <summary>
        /// Creates and validates the parameters.
        /// </summary>
        /// <param name="lessonId">The lesson identifier.</param>
        /// <param name="output">The <c>stackpanel</c> in which we insert our UI elements.</param>
        /// <param name="inputStrings">The input strings.</param>
        /// <returns>An array of the arguments for the Start method.</returns>
        /// <exception cref="Exception">Throws an exception if the input is not valid.</exception>
        private object[] CreatePrameters(int lessonId, StackPanel output, string[] inputStrings)
        {
            var lessonMethod = this.GetMethodInfo(lessonId, "Start");

            var parameter = lessonMethod.GetParameters().Skip(1);

            object[] parameters = new object[inputStrings.Length + 1];
            parameters[0] = output;
            object para = null;

            int i = 0;

            foreach (var item in parameter)
            {
                Type paraType = item.ParameterType;

                try
                {
                    para = Convert.ChangeType(inputStrings[i], paraType);
                }
                catch (Exception)
                {
                    throw new Exception(string.Format("Error: Validating input\r\n{0}. Input named \"{1}\" with value \"{2}\" couldn't be converted to {3}", i + 1, item.Name, inputStrings[i], paraType.Name.ToLower()));
                }

                // First parameter is always the grid - which doesn't require input
                parameters[i + 1] = para;
                i++;
            }

            return parameters;
        }

        /// <summary>
        /// Gets the MethodInfo to a specific lesson.
        /// </summary>
        /// <param name="lessonId">The lesson identifier.</param>
        /// <param name="name">The name of the lesson.</param>
        /// <returns>Returns the MethodInfo for the specific lesson.</returns>
        private MethodInfo GetMethodInfo(int lessonId, string name)
        {
            var path = "Sensorkit.LessonClasses.Lesson";
            Type lessonType = Type.GetType(path + lessonId);

            MethodInfo lessonMethod = lessonType.GetMethod(name);

            return lessonMethod;
        }
    }
}