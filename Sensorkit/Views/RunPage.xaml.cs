//----------------------------------------------------------------------------------------------
// <copyright file="RunPage.xaml.cs" company="Lukas Handler">
// Copyright (c) Lukas Handler.  All rights reserved.
// </copyright>
// <summary>
// This is the page that gets used when you want to run a lesson.
// </summary>
//-------------------------------------------------------------------------------------------------
namespace Sensorkit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Sensorkit.Model;
    using Sensorkit.ViewModel;
    using Windows.ApplicationModel.Resources;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Page where you can fill in arguments for a specific lesson, run it and get its output.
    /// </summary>
    public sealed partial class RunPage : Page
    {
        /// <summary>
        /// Our accessor for the resources.
        /// </summary>
        private static ResourceLoader resources;

        /// <summary>
        /// If this is true - we start the lesson.
        /// </summary>
        private bool justRun;

        /// <summary>
        /// The selected model.
        /// </summary>
        private LessonModel selectedModel;

        /// <summary>
        /// The ViewModel for the <c>RunPage</c>.
        /// </summary>
        private VmRun viewModelRun;

        /// <summary>
        /// Initializes a new instance of the <see cref="RunPage"/> class.
        /// </summary>
        public RunPage()
        {
            this.viewModelRun = new VmRun();
            resources = ResourceLoader.GetForCurrentView("RunView");

            txt_headLine.Text = resources.GetString("HeadLine");
            txt_input.Text = resources.GetString("Input");
            txt_output.Text = resources.GetString("Output");

            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the name of the lesson.
        /// </summary>
        /// <value>
        /// The name of the lesson.
        /// </value>
        public string LessonName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the index of the selected.
        /// </summary>
        /// <value>
        /// The selected index.
        /// </value>
        public int SelectedIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets called when it gets navigated to this page.
        /// </summary>
        /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                var value = e.Parameter as Tuple<LessonModel, bool>;
                this.selectedModel = value.Item1;
                this.SelectedIndex = this.selectedModel.Id;
                this.LessonName = this.selectedModel.Name;
                this.justRun = value.Item2;
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// Handles the Click event of the back-button. Calls the <c>MainPage</c> with the selected index.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Btn_back_Click(object sender, RoutedEventArgs e)
        {
            this.viewModelRun.StopLesson(this.SelectedIndex, this.Output);

            Frame.Navigate(typeof(MainPage), this.SelectedIndex);
        }

        /// <summary>
        /// Handles the Click event when the run button gets clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Btn_run_Click(object sender, RoutedEventArgs e)
        {
            if (StartStopButton.Symbol == Symbol.Play)
            {
                List<string> args = new List<string>();

                foreach (var item in Input.Children)
                {
                    if (item is TextBox)
                    {
                        var inputBox = item as TextBox;
                        args.Add(inputBox.Text);
                    }
                }

                try
                {
                    Output.Children.Clear();
                    this.viewModelRun.RunLesson(this.SelectedIndex, this.Output, args.ToArray());
                    StartStopButton.Symbol = Symbol.Stop;
                }
                catch (TargetInvocationException exc)
                {
                    TextBlock errorMessage = new TextBlock();
                    errorMessage.Text = exc.InnerException.Message;
                    Output.Children.Add(errorMessage);
                }
                catch (Exception exc)
                {
                    TextBlock errorMessage = new TextBlock();
                    errorMessage.Text = exc.Message;
                    Output.Children.Add(errorMessage);
                }
            }
            else
            {
                lock (new object())
                {
                    this.viewModelRun.StopLesson(this.SelectedIndex, this.Output);
                    StartStopButton.Symbol = Symbol.Play;
                }
            }
        }

        /// <summary>
        /// Called when page gets loaded. Gets the argument of the method using reflection.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this;

            this.viewModelRun.CreateInputFields(this.Input, this.selectedModel);

            if (Input.Children.Count() == 0)
            {
                TextBlock noInput = new TextBlock()
                {
                    Text = resources.GetString("NoInput"),
                    Margin = new Thickness(10)
                };
                Input.Children.Add(noInput);
            }

            if (this.justRun)
            {
                this.Btn_run_Click(null, null);
            }
        }
    }
}