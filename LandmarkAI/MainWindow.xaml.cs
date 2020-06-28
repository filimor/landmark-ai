using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Media.Imaging;
using LandmarkAI.Classes;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace LandmarkAI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image files (*.png; *.jpg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };

            if (dialog.ShowDialog() != true) return;
            var fileName = dialog.FileName;
            SelectedImage.Source = new BitmapImage(new Uri(fileName));

            MakePredictionAsync(fileName);
        }

        private async void MakePredictionAsync(string fileName)
        {
            const string url =
                "https://wpfcourse.cognitiveservices.azure.com/customvision/v3.0/Prediction/37b8c0bd-9908-42e7-97bb-d6a813800500/classify/iterations/Iteration1/image";
            const string predictionKey = "c4b86f5473994e5bbb895f8c9a0693d5";
            const string contentType = "application/octet-stream";

            var file = File.ReadAllBytes(fileName);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Prediction-Key", predictionKey);

                using (var content = new ByteArrayContent(file))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                    var response = await client.PostAsync(url, content).ConfigureAwait(false);

                    var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    var predictions = JsonConvert.DeserializeObject<CustomVision>(responseString).Predictions;

                    PredictionsListView.ItemsSource = predictions;
                }
            }
        }
    }
}