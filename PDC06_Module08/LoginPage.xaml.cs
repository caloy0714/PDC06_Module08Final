using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Newtonsoft.Json;
using System.Net.Http;
using System.Threading;

using static PDC06_Module08.SearchPage;

namespace PDC06_Module08
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private const string url_login = "http://192.168.100.253/pdc6/api-login.php";
        private readonly HttpClient _client;
        public LoginPage()
        {
            InitializeComponent();
            _client = new HttpClient();
        }
  
        public async void OnLogin(object sender, EventArgs e)
        {
            string username = xUsername.Text;
            string password = xPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Username and password are required.", "OK");
                return;
            }

            try
            {
                //{ url_search}?ID ={ searchQuery}
                // Construct the login URL (replace with your actual login endpoint)
                var loginUrl = $"{url_login}/?username={username}&password={password}";

                // Make the API call
                var content = await _client.GetStringAsync(loginUrl);

                // Deserialize the response
                var responseObject = JsonConvert.DeserializeObject<ResponseObject>(content);

                if (responseObject.status)
                {
                    // Successfully logged in
                   await DisplayAlert("Success", "Login successful", "OK");
                    // Navigate to the next page or perform other actions
                    await Navigation.PushAsync(new MainPage());
                }
                else
                {
                    // Login failed
                    await DisplayAlert("Error", responseObject.message, "OK");
                }
            }
            catch (HttpRequestException ex)
            {
                await DisplayAlert("Error", $"HTTP Request Error: {ex.Message}", "OK");
            }
            catch (JsonException ex)
            {
                await DisplayAlert("Error", $"JSON Deserialization Error: {ex.Message}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }
    }
}