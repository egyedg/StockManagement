using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace StockManagement.WPFClient
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "https://localhost:7000/api";

        public MainWindow()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            StartDatePicker.SelectedDate = DateTime.Today.AddDays(-7);
            EndDatePicker.SelectedDate = DateTime.Today;
        }

        private async void LoadProducts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var products = await _httpClient.GetFromJsonAsync<List<Product>>($"{ApiBaseUrl}/reports/products");
                ProductsGrid.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}");
            }
        }

        private async void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProductDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/products", dialog.Product);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Product added successfully!");
                        LoadProducts_Click(sender, e);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding product: {ex.Message}");
                }
            }
        }

        private async void LoadCategories_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var categories = await _httpClient.GetFromJsonAsync<List<Category>>($"{ApiBaseUrl}/categories");
                CategoriesTree.ItemsSource = categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}");
            }
        }

        private async void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CategoryDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/categories", dialog.Category);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Category added successfully!");
                        LoadCategories_Click(sender, e);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding category: {ex.Message}");
                }
            }
        }

        private async void AddMovement_Click(object sender, RoutedEventArgs e)
        {
            if (ProductComboBox.SelectedItem is not Product selectedProduct ||
                !int.TryParse(QuantityTextBox.Text, out int quantity) ||
                quantity <= 0)
            {
                MessageBox.Show("Please select a product and enter a valid quantity.");
                return;
            }

            var movement = new
            {
                ProductId = selectedProduct.Id,
                Quantity = quantity,
                MovementType = (MovementTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                Reason = ReasonTextBox.Text,
                Timestamp = DateTime.UtcNow
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/stock/movement", movement);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Stock movement added successfully!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding stock movement: {ex.Message}");
            }
        }

        private async void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            if (!StartDatePicker.SelectedDate.HasValue || !EndDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select both start and end dates.");
                return;
            }

            try
            {
                var url = $"{ApiBaseUrl}/reports/stock-movements?" +
                         $"startDate={StartDatePicker.SelectedDate:yyyy-MM-dd}" +
                         $"&endDate={EndDatePicker.SelectedDate:yyyy-MM-dd}";

                var movements = await _httpClient.GetFromJsonAsync<List<StockMovement>>(url);
                ReportGrid.ItemsSource = movements;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}");
            }
        }

        private async void ProductComboBox_DropDownOpened(object sender, EventArgs e)
        {
            try
            {
                var products = await _httpClient.GetFromJsonAsync<List<Product>>($"{ApiBaseUrl}/reports/products");
                ProductComboBox.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}");
            }
        }
    }

    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public string CategoryId { get; set; }
    }

    public class Category
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ParentCategoryId { get; set; }
        public List<Category> SubCategories { get; set; }
    }

    public class StockMovement
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string MovementType { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; }
        public DateTime Timestamp { get; set; }
    }
}