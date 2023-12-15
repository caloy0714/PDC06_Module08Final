using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

using System.Net.Http;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using static System.Collections.Specialized.BitVector32;

namespace PDC06_Module08
{
    public class Post
    {
        public int ID { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string section { get; set; }


    }


    public partial class MainPage : ContentPage
    {
        public const string url = "http://192.168.100.253/pdc6/api_create.php";
        public const string url_retrieve = "http://192.168.100.253/pdc6/api_r2.php";

        private HttpClient _Client = new HttpClient();
        private ObservableCollection<Post> _post;

        private Picker xSectionPicker;

        public MainPage()
        {
            InitializeComponent();

            xSectionPicker = new Picker
            {
                Title = "Select Section",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.FromHex("#ecf0f1")
            };

           
            xSectionPicker.ItemsSource = new List<string> { "All Sections", "A", "B", "C" };

            
            xSectionPicker.SelectedIndexChanged += OnSectionPickerSelectedIndexChanged;

            
            mainStackLayout.Children.Insert(0, xSectionPicker);

            
            FetchAndDisplayRecords();
        }

        private async void FetchAndDisplayRecords()
        {
            var content = await _Client.GetStringAsync(url_retrieve);
            var post = JsonConvert.DeserializeObject<List<Post>>(content);

            _post = new ObservableCollection<Post>(post);
            Post_Collection.ItemsSource = _post;
        }

        private void OnSectionPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            
            FilterRecordsBySection(xSectionPicker.SelectedItem as string);
        }

        private void FilterRecordsBySection(string selectedSection)
        {
            if (selectedSection == "All Sections")
            {
                
                Post_Collection.ItemsSource = _post;
            }
            else
            {
               
                var filteredRecords = _post.Where(post => post.section == selectedSection).ToList();
                Post_Collection.ItemsSource = new ObservableCollection<Post>(filteredRecords);
            }
        }

        private async void OpenPageSearch(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SearchPage());

        }
        private async void OnAdd(object sender, EventArgs e)
        {
            Post post = new Post
            {
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

            
            var response = await _Client.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                // Successfully added record, show alert
                await DisplayAlert("Success", "Record added successfully", "OK");

                
                xName.Text = "";
                xGender.Text = "";
                xEmail.Text = "";
                xPhone.Text = "";
                xAddress.Text = "";
                xUsername.Text = "";
                xPassword.Text = "";
                xSection.Text = "";

                _post.Insert(0, post);

                var content2 = await _Client.GetStringAsync(url_retrieve);
                var post2 = JsonConvert.DeserializeObject<List<Post>>(content2);

                _post = new ObservableCollection<Post>(post2);
                Post_Collection.ItemsSource = _post;
            }
            else
            {
                // Failed to add record, show error message
                await DisplayAlert("Error", "Failed to add record", "OK");
            }
        }

        protected override async void OnAppearing()
        {
            var content = await _Client.GetStringAsync(url_retrieve);
            var post = JsonConvert.DeserializeObject<List<Post>>(content);

            _post = new ObservableCollection<Post>(post);
            Post_Collection.ItemsSource = _post; 
            base.OnAppearing();
        }

        private async void OnRefresh(object sender, System.EventArgs e)
        {
            var content = await _Client.GetStringAsync(url_retrieve);
            var post = JsonConvert.DeserializeObject<List<Post>>(content);

            _post = new ObservableCollection<Post>(post);
            Post_Collection.ItemsSource = _post;
            base.OnAppearing();
        }

        private bool isAscending = true;

        private void OnSortClicked(object sender, EventArgs e)
        {

            isAscending = !isAscending;

            
            var sortedCollection = isAscending
                ? _post.OrderBy(post => post.ID).ToList()
                : _post.OrderByDescending(post => post.ID).ToList();

            
            _post = new ObservableCollection<Post>(sortedCollection);
            Post_Collection.ItemsSource = _post;
        }

        private void OnSectionTextChanged(object sender, TextChangedEventArgs e)
        {
            
            string newText = e.NewTextValue;

            
            if (!IsValidSectionInput(newText))
            {
                
                xSection.Text = "";
            }
        }

        private bool IsValidSectionInput(string input)
        {
            
            List<string> validOptions = new List<string> { "A", "B", "C" };

            
            return validOptions.Contains(input.ToUpper()); 
        }


    }
}

