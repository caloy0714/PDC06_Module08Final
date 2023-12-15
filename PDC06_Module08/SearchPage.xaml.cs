using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;

using System.Net.Http;

using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.ComponentModel;
using System.Globalization;

namespace PDC06_Module08
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class Post2
    {
        public int ID { get; set; }
        public string username { get; set; }

        public string password { get; set; }

    }



    public partial class SearchPage : ContentPage
    {
        private const string url_search = "http://192.168.100.253/pdc6/api-search.php";
        private const string url_delete = "http://192.168.100.253/pdc6/api-delete.php";
        private HttpClient _Client = new HttpClient();
        private ObservableCollection<Post2> _posts;
        public SearchPage()
        {
            InitializeComponent();
        }
        public async void OnMore(object sender, EventArgs e)
        {
          try
            {
                var mi = (MenuItem)sender;

                // Check if CommandParameter is an integer (System.Int32)
                if (mi.CommandParameter is int postId)
                {
                    bool result = await DisplayAlert("Update Context Action",
                        $"Are you sure you want to update ID No: {postId}?",
                        "OK", "CANCEL");

                    if (result)
                    {

                        Post2 post = new Post2 { ID = postId };
                        await Navigation.PushAsync(new UpdatePage(post));

                       // Handle any additional actions after deletion if needed
                    }
                    else
                    {
                        // User clicked CANCEL, handle accordingly (if needed)
                    }
                }
                else
                {
                    // Log the unexpected type
                    var unexpectedType = mi.CommandParameter?.GetType().ToString();
                    Console.WriteLine($"Unexpected type in OnDelete: {unexpectedType}");

                    // Handle the case where the data context is not of the expected type
                }
            }
            catch (Exception ex)
            {
                // Log or display the exception details
                Console.WriteLine($"Exception in OnDelete: {ex.Message}");
            }

        }

        public async void OnDelete(object sender, EventArgs e)
        {
            try
            {
                var mi = (MenuItem)sender;

                // Check if CommandParameter is an integer (System.Int32)
                if (mi.CommandParameter is int postId)
                {
                    bool result = await DisplayAlert("Delete Context Action",
                        $"Are you sure you want to delete ID No: {postId}?",
                        "OK", "CANCEL");

                    if (result)
                    {
                        Post2 post = new Post2 { ID = postId };

                        var content = JsonConvert.SerializeObject(post);
                        await _Client.PostAsync(url_delete, new StringContent(content, Encoding.UTF8, "application/json"));

                        await DisplayAlert("Success", $"Post with ID No: {postId} deleted successfully.", "OK");
                         // Handle any additional actions after deletion if needed
                    }
                    else
                    {
                        // User clicked CANCEL, handle accordingly (if needed)
                    }
                }
                else
                {
                    // Log the unexpected type
                    var unexpectedType = mi.CommandParameter?.GetType().ToString();
                    Console.WriteLine($"Unexpected type in OnDelete: {unexpectedType}");
                    // Handle the case where the data context is not of the expected type
                }
            }
            catch (Exception ex)
            {
                // Log or display the exception details
                Console.WriteLine($"Exception in OnDelete: {ex.Message}");
            }
        }

        public class ResponseObject
        {
            public bool status { get; set;}
            public JToken data { get; set; }
            public string message { get; set;}
        }
        private async void OnSearchTextchanged (object sender, TextChangedEventArgs e)
        {
            string searchQuery = e.NewTextValue;
            if (string.IsNullOrWhiteSpace(searchQuery)) 
            { 
            
            }
            else
            {
                try
                {

                    var searchUrl = $"{url_search}?username={searchQuery}";

                    System.Diagnostics.Debug.WriteLine($"Search URL: {searchUrl}");

                    var content = await _Client.GetStringAsync(searchUrl);

                    var responseObject = JsonConvert.DeserializeObject<ResponseObject>(content);

                    if(responseObject.status)
                    {
                        var searchResult = JsonConvert.DeserializeObject<List<Post2>>(responseObject.data.ToString());

                        _posts = new ObservableCollection<Post2>(searchResult);
                        Post_list2.ItemsSource = _posts;
                     }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Error: {responseObject.message}");
                    }
                }
                catch (Exception ex)
                {

                    System.Diagnostics.Debug.WriteLine($"An error occured: {ex.Message}");
                }
            }

        }
    }
}