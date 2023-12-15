using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static PDC06_Module08.SearchPage;

namespace PDC06_Module08
{
    [XamlCompilation(XamlCompilationOptions.Compile)]


    public partial class UpdatePage : ContentPage
    {

        private const string url_search = "http://192.168.100.253/pdc6/api-searchID.php";
        private const string url_update = "http://192.168.100.253/pdc6/api-update.php";

        private HttpClient _Client = new HttpClient();
        private ObservableCollection<Post2> _posts = new ObservableCollection<Post2>();
        public UpdatePage(Post2 post)
        {
            InitializeComponent();
            xID.Text = post.ID.ToString();
                   
        }


        private async void OnUpdate(object sender, EventArgs e)
        {
            bool result = await DisplayAlert("Update Confirmation",
                $"Are you sure you want to update ID No: {xID.Text}?",
                "OK", "CANCEL");

            if (result)
            {
                await UpdatePostAsync();
            }
            else
            {
                // User clicked CANCEL, handle accordingly (if needed)
            }
        }

        private async Task UpdatePostAsync()
        {
            try
            {
                Post2 post = new Post2
                {
                    ID = int.Parse(xID.Text),
                    username = xUsername.Text,
                    password = xPassword.Text
                };

                var content = JsonConvert.SerializeObject(post);

                var response = await _Client.PostAsync(url_update, new StringContent(content, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    // Provide user feedback on successful update
                    await DisplayAlert("Success", "Post updated successfully", "OK");
                }
                else
                {
                    // Provide user feedback on unsuccessful update
                    await DisplayAlert("Error", $"Failed to update post. Status code: {response.StatusCode}", "OK");
                }
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }
        private async void OnRetrievedchanged(object sender, TextChangedEventArgs e)
        {
            string searchQuery = e.NewTextValue;

            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                // Clear the displayed information when the search query is empty or contains only whitespace.
                xUsername.Text = string.Empty;
                xPassword.Text = string.Empty;
            }
            else
            {
                try
                {
                    // Construct the search URL
                    var searchUrl = $"{url_search}?ID={searchQuery}";

                    System.Diagnostics.Debug.WriteLine($"Search URL: {searchUrl}");

                    // Use HttpClient to make the API call
                    var content = await _Client.GetStringAsync(searchUrl);

                    // Deserialize the response
                    var responseObject = JsonConvert.DeserializeObject<ResponseObject>(content);

                    if (responseObject.status)
                    {
                        // Deserialize the data part of the response into a List of Post2
                        var searchResults = JsonConvert.DeserializeObject<List<Post2>>(responseObject.data.ToString());

                        // Check if there are any results
                        if (searchResults.Count > 0)
                        {
                            // Assuming you want to display the first result
                            var firstResult = searchResults[0];

                            // Display the username and password in their respective Entry controls
                            xUsername.Text = firstResult.username;
                            xPassword.Text = firstResult.password;
                        }
                        else
                        {
                            // Handle the case when no results are found
                            xUsername.Text = "No results found";
                            xPassword.Text = string.Empty;
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Error: {responseObject.message}");
                        // Handle the case when the API returns an error.
                        // You might want to display an error message or clear the Entry controls.
                        xUsername.Text = $"Error: {responseObject.message}";
                        xPassword.Text = string.Empty;
                    }
                }
                //catch (HttpRequestException ex)
                //{
                //    System.Diagnostics.Debug.WriteLine($"HTTP Request Error: {ex.Message}");
                //    // Handle the case when there is an issue with the HTTP request.
                //    // You might want to display an error message or clear the Entry controls.
                //    xUsername.Text = $"HTTP Request Error: {ex.Message}";
                //    xPassword.Text = string.Empty;
                //}
                //catch (JsonException ex)
                //{
                //    System.Diagnostics.Debug.WriteLine($"JSON Deserialization Error: {ex.Message}");
                //    // Handle the case when there is an issue with JSON deserialization.
                //    // You might want to display an error message or clear the Entry controls.
                //    xUsername.Text = $"JSON Deserialization Error: {ex.Message}";
                //    xPassword.Text = string.Empty;
                //}
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"An error occurred: {ex.Message}");
                    // Handle other types of exceptions if needed.
                    // You might want to display a generic error message or clear the Entry controls.
                    xUsername.Text = $"An error occurred: {ex.Message}";
                    xPassword.Text = string.Empty;
                }
            }
        }




    }
}