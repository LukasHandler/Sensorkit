using Sensorkit.LessonClasses;
using Sensorkit.Model;
using Sensorkit.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace Sensorkit
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class RunPage : Page
    {
        private static ResourceLoader resources;

        public int SelectedIndex { get; set; }

        public string LessonName { get; set; }

        private LessonModel selectedModel;

        private VmRun viewModelRun;

        private bool isDebug;

        public RunPage()
        {
            viewModelRun = new VmRun();
            resources = ResourceLoader.GetForCurrentView("RunView");

            txt_headLine.Text = resources.GetString("HeadLine");
            txt_input.Text = resources.GetString("Input");
            txt_output.Text = resources.GetString("Output");

            this.InitializeComponent();
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            viewModelRun.StopLesson(SelectedIndex, Output);

            Frame.Navigate(typeof(MainPage), SelectedIndex);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                var value = e.Parameter as Tuple<LessonModel, bool>;
                selectedModel = value.Item1;
                SelectedIndex = selectedModel.Id;
                LessonName = selectedModel.Name;
                isDebug = value.Item2;
            }
            catch(Exception)
            {
                return;
            }
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;

            viewModelRun.CreateInputFields(Input, selectedModel);

            if (Input.Children.Count() == 0)
            {
                TextBlock noInput = new TextBlock() { Text = resources.GetString("NoInput"), Margin = new Thickness(10) };
                Input.Children.Add(noInput);
            }

            if (isDebug)
            {
                btn_run_Click(null, null);
            }
        }

        private void btn_run_Click(object sender, RoutedEventArgs e)
        {
            if(StartStopButton.Symbol == Symbol.Play)
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
                    viewModelRun.RunLesson(SelectedIndex, Output, args.ToArray());
                    StartStopButton.Symbol = Symbol.Stop;
                }
                catch(TargetInvocationException exc)
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
                lock(new object())
                {
                    viewModelRun.StopLesson(SelectedIndex, Output);
                    StartStopButton.Symbol = Symbol.Play;
                }
            }
        }
    }
}
