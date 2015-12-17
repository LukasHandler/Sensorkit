//----------------------------------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Lukas Handler">
// Copyright (c) Lukas Handler.  All rights reserved.
// </copyright>
// <summary>
// This is the start page of the application.
// </summary>
//-------------------------------------------------------------------------------------------------
namespace Sensorkit
{
    using System;
    using System.Linq;
    using Model;
    using ViewModel;
    using Windows.UI.Popups;
    using Windows.UI.Text;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media.Imaging;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Page where you can click through the tutorial or start an exercise when the device it's running on is the raspberry pi b+.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Indicates which index gets selected when the site gets called.
        /// </summary>
        private int index;

        /// <summary>
        /// Useful if you don't want to connect your raspberry pi to an output device.
        /// </summary>
        private bool justRun = false;

        /// <summary>
        ///  Given "lesson" id will start automatically if justRun = true.
        /// </summary>
        private int lesson = 0;

        /// <summary>
        /// This is the ViewModel for the MainPage.
        /// </summary>
        private VmMain modelViewMain;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            // Sets the start index to lesson 0. This will be overwritten later in the "OnWindowLoaded" Method when navigated back from RunPage.
            this.index = 0;

            this.modelViewMain = new VmMain();
            this.modelViewMain.CreateLessons();

            this.DataContext = this.modelViewMain;

            this.InitializeComponent();
        }

        /// <summary>
        /// Setting the current index when navigated back from the RunPage.
        /// </summary>
        /// <param name="e">Arguments that contains our lesson we started earlier.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                this.index = (int)e.Parameter;
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// Checking if you can play the current lesson when clicking the play-button. Does the called lesson have a code behind? A Start Method? Are you running on a Raspberry Pi B+.
        /// </summary>
        /// <param name="sender">The sender of this Event.</param>
        /// <param name="e">Arguments of the event.</param>
        private async void Btn_play_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = this.modelViewMain.ValidateSystem((LessonModel)lv_navigation.SelectedItem);

            if (errorMessage == null)
            {
                Tuple<LessonModel, bool> value = new Tuple<LessonModel, bool>(this.modelViewMain.Lessons[((LessonModel)lv_navigation.SelectedItem).Id], this.justRun);
                Frame.Navigate(typeof(RunPage), value);
            }
            else
            {
                if (this.modelViewMain.IsRaspConnected())
                {
                    scrollViewer.ChangeView(0, 0, null);
                    TextBlock errorText = (TextBlock)grid_content.Children[0];
                    errorText.Text = errorMessage;
                    errorText.Visibility = Visibility.Visible;
                    errorText.TextWrapping = TextWrapping.Wrap;
                }
                else
                {
                    var dialog = new MessageDialog(errorMessage);
                    await dialog.ShowAsync();
                    return;
                }
            }
        }

        /// <summary>
        /// Close navigation <c>splitview</c> when width is smaller than 641.
        /// </summary>
        /// <param name="sender">The sender of this Event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void Lv_navigation_ItemClick(object sender, ItemClickEventArgs e)
        {
            double width = Window.Current.Bounds.Width;

            if (width < 641)
            {
                tb_navigationToggle.IsChecked = false;
            }
        }

        /// <summary>
        /// Fill content page generic with labels and pictures from the resource file.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void Lv_navigation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            grid_content.Children.Clear();
            grid_content.RowDefinitions.Clear();

            var listview = sender as ListView;
            var selectedLesson = (LessonModel)listview.SelectedItem;

            int currentRow = 0;

            // Empty Error Text
            grid_content.RowDefinitions.Add(new RowDefinition()
            {
                Height = GridLength.Auto
            });
            TextBlock errorText = new TextBlock();
            errorText.Margin = new Thickness(5);
            errorText.FontSize = 20;
            errorText.SetValue(Grid.RowProperty, currentRow);
            errorText.Visibility = Visibility.Collapsed;
            grid_content.Children.Add(errorText);
            currentRow++;

            // Headline
            grid_content.RowDefinitions.Add(new RowDefinition()
            {
                Height = GridLength.Auto
            });
            TextBlock header = new TextBlock();
            header.Margin = new Thickness(5);
            header.SetValue(Grid.RowProperty, currentRow);
            header.Text = selectedLesson.Name;
            header.FontSize = 30;
            header.HorizontalAlignment = HorizontalAlignment.Center;
            header.TextWrapping = TextWrapping.Wrap;
            grid_content.Children.Add(header);

            var parts = selectedLesson.Content.Split('#');

            foreach (var item in parts)
            {
                grid_content.RowDefinitions.Add(new RowDefinition()
                {
                    Height = GridLength.Auto
                });
                currentRow++;

                // Image
                if (item.StartsWith("img"))
                {
                    var imageName = item.Split(':')[1];

                    Image img = new Image();
                    img.HorizontalAlignment = HorizontalAlignment.Center;
                    img.SetValue(Grid.RowProperty, currentRow);
                    img.MaxHeight = 800;
                    img.MaxWidth = 800;
                    img.Margin = new Thickness(5);

                    string imagePath = "../Assets/" + imageName;

                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.UriSource = new Uri(this.BaseUri, imagePath);

                    img.Source = bitmapImage;

                    grid_content.Children.Add(img);
                }
                else
                {
                    // Text
                    TextBlock text = new TextBlock();
                    text.Margin = new Thickness(10, 5, 5, 5);
                    text.SetValue(Grid.RowProperty, currentRow);

                    if (item.StartsWith("_"))
                    {
                        text.FontWeight = FontWeights.Bold;
                        text.Text = item.Substring(1);
                    }
                    else
                    {
                        text.Text = item;
                    }

                    text.TextWrapping = TextWrapping.Wrap;
                    grid_content.Children.Add(text);
                }
            }
        }

        /// <summary>
        /// Gets the current Item from the index or starts a lesson when justRun is true.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (this.justRun)
            {
                var selectedItem = this.modelViewMain.Lessons.FirstOrDefault(l => l.Id == this.lesson);

                if (selectedItem != null)
                {
                    lv_navigation.SelectedItem = selectedItem;
                }

                this.index = this.lesson;
                this.Btn_play_Click(null, null);
            }
            else
            {
                var selectedItem = this.modelViewMain.Lessons.FirstOrDefault(l => l.Id == this.index);

                if (selectedItem != null)
                {
                    lv_navigation.SelectedItem = selectedItem;
                }
            }
        }
    }
}