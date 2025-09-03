using System.Windows;

namespace StockManagement.WPFClient
{
    public partial class CategoryDialog : Window
    {
        public Category Category { get; private set; }

        public CategoryDialog()
        {
            InitializeComponent();
            Category = new Category();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Please enter a category name.");
                return;
            }

            Category.Name = NameTextBox.Text;
            Category.Id = System.Guid.NewGuid().ToString();

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