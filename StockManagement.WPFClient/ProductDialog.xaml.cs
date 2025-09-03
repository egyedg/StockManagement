using System.Windows;

namespace StockManagement.WPFClient
{
    public partial class ProductDialog : Window
    {
        public Product Product { get; private set; }

        public ProductDialog()
        {
            InitializeComponent();
            Product = new Product();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(SkuTextBox.Text) ||
                !decimal.TryParse(PriceTextBox.Text, out decimal price))
            {
                MessageBox.Show("Please fill all fields with valid values.");
                return;
            }

            Product.Name = NameTextBox.Text;
            Product.Sku = SkuTextBox.Text;
            Product.Price = price;
            Product.Id = System.Guid.NewGuid().ToString();

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}