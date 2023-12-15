using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            xName.Text = post.name;
            xGender.Text = post.gender;
            xEmail.Text = post.email;
            xPhone.Text = post.phone;
            xAddress.Text = post.address;
            xUsername.Text = post.username;
            xPassword.Text = post.password;
            xSection.Text = post.section;
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
                    name = xName.Text,
                    gender = xGender.Text,
                    email = xEmail.Text,
                    phone = xPhone.Text,
                    address = xAddress.Text,
                    username = xUsername.Text,
                    password = xPassword.Text,
                    section = xSection.Text
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
                xName.Text = string.Empty;
                xGender.Text = string.Empty;
                xEmail.Text = string.Empty;
                xPhone.Text = string.Empty;
                xAddress.Text = string.Empty;
                xUsername.Text = string.Empty;
                xPassword.Text = string.Empty;
                xSection.Text = string.Empty;
            }
            else
            {
                try
                {
                    var searchUrl = $"{url_search}?ID={searchQuery}";

                    System.Diagnostics.Debug.WriteLine($"Search URL: {searchUrl}");

                    var content = await _Client.GetStringAsync(searchUrl);

                    var responseObject = JsonConvert.DeserializeObject<ResponseObject>(content);

                    if (responseObject.status)
                    {
                        var searchResults = JsonConvert.DeserializeObject<List<Post2>>(responseObject.data.ToString());

                        if (searchResults.Count > 0)
                        {
                            var firstResult = searchResults[0];

                            xName.Text = firstResult.name;
                            xGender.Text = firstResult.gender;
                            xEmail.Text = firstResult.email;
                            xPhone.Text = firstResult.phone;
                            xAddress.Text = firstResult.address;
                            xUsername.Text = firstResult.username;
                            xPassword.Text = firstResult.password;
                            xSection.Text = firstResult.section;
                        }
                        else
                        {
                            xName.Text = "No results found";
                            xGender.Text = string.Empty;
                            xEmail.Text = string.Empty;
                            xPhone.Text = string.Empty;
                            xAddress.Text = string.Empty;
                            xUsername.Text = string.Empty;
                            xPassword.Text = string.Empty;
                            xSection.Text = string.Empty;
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Error: {responseObject.message}");
                        xName.Text = $"Error: {responseObject.message}";
                        xGender.Text = string.Empty;
                        xEmail.Text = string.Empty;
                        xPhone.Text = string.Empty;
                        xAddress.Text = string.Empty;
                        xUsername.Text = string.Empty;
                        xPassword.Text = string.Empty;
                        xSection.Text = string.Empty;
                    }
                }

                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"An error occurred: {ex.Message}");
                    xName.Text = $"An error occurred: {ex.Message}";
                    xGender.Text = string.Empty;
                    xEmail.Text = string.Empty;
                    xPhone.Text = string.Empty;
                    xAddress.Text = string.Empty;
                    xUsername.Text = string.Empty;
                    xPassword.Text = string.Empty;
                    xSection.Text = string.Empty;
                }
            }
        }
    }
}
