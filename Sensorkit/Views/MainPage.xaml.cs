namespace Sensorkit
{
    using Sensorkit.Model;
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media.Imaging;
    using Windows.UI.Text;
    using Sensorkit.ViewModel;
    using Windows.UI.Popups;
    using System.Reflection;
    using Windows.UI.Xaml.Navigation;
    using LessonClasses;
    using System.Linq;
    using Windows.ApplicationModel.Resources;
    using Windows.UI.Xaml.Media;
    using Windows.UI;

    /// <summary>
    /// Page where you can click through the tutorial or start an exercise when the device it's running on is the raspberry pi b+.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private VmMain modelViewMain;

        // Indicates which index gets selected when the site gets called.
        private int index;

        // Start settings - Useful if you don't want to connect your raspberry pi to an output device. Given "lesson" id will start automatically.
        private bool justRun = false;
        private int lesson = 0;

        /// <summary>
        /// Some initial configuration is in here. Also getting the Lessons.
        /// </summary>
        public MainPage()
        {
            // Sets the start index to lesson 0. This will be overwritten later in the "OnWindowLoaded" Method when navigated back from RunPage.
            index = 0;

            modelViewMain = new VmMain();
            modelViewMain.CreateLessons();

            DataContext = modelViewMain;

            this.InitializeComponent();
        }

        /// <summary>
        /// Fill content page generic with labels and pictures from the ressource file.
        /// </summary>
        private void lv_navigation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            grid_content.Children.Clear();
            grid_content.RowDefinitions.Clear();

            var listview = sender as ListView;
            var selectedLesson = ((LessonModel)listview.SelectedItem);

            int currentRow = 0;

            // Empty Error Text
            grid_content.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            TextBlock errorText = new TextBlock();
            errorText.Margin = new Thickness(5);
            errorText.FontSize = 20;
            errorText.SetValue(Grid.RowProperty, currentRow);
            errorText.Visibility = Visibility.Collapsed;
            grid_content.Children.Add(errorText);
            currentRow++;

            // Headline
            grid_content.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
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
                grid_content.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
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
                // Text
                else
                {
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
        /// Close navigation splitview when width is smaller than 641. 
        /// </summary>
        private void lv_navigation_ItemClick(object sender, ItemClickEventArgs e)
        {
            double width = Window.Current.Bounds.Width;

            if (width < 641)
            {
                tb_navigationToggle.IsChecked = false;
            }
        }

        /// <summary>
        /// Select lesson index when window gets loaded.
        /// </summary>
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (justRun)
            {
                var selectedItem = modelViewMain.Lessons.FirstOrDefault(l => l.Id == lesson);

                if (selectedItem != null)
                {
                    lv_navigation.SelectedItem = selectedItem;
                }

                index = lesson;
                btn_play_Click(null, null);
            }
            else
            {
                var selectedItem = modelViewMain.Lessons.FirstOrDefault(l => l.Id == index);

                if (selectedItem != null)
                {
                    lv_navigation.SelectedItem = selectedItem;
                }
            }
        }

        /// <summary>
        /// Checking if you can play the current lesson when clicking on the playbutton. Does the called lesson have a code behind? A Start Method? Are you running on a Raspberry Pi B+.
        /// </summary>
        private async void btn_play_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = modelViewMain.validateSystem((LessonModel)lv_navigation.SelectedItem);

            if (errorMessage == null)
            {
                Tuple<LessonModel, bool> value = new Tuple<LessonModel, bool>(modelViewMain.Lessons[((LessonModel)lv_navigation.SelectedItem).Id], justRun);
                Frame.Navigate(typeof(RunPage), value);
            }
            else
            {
                if (modelViewMain.IsRaspConnected())
                {
                    scrollViewer.ChangeView(0, 0, null);
                    TextBlock errorText = ((TextBlock)grid_content.Children[0]);
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
        /// Setting the current index when navigated back from the RunPage.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                index = (int)e.Parameter;
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}