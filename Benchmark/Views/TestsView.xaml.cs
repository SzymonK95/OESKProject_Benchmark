using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using Benchmark.ViewModel;

namespace Benchmark.Views
{
    /// <summary>
    /// Interaction logic for TestsView.xaml
    /// </summary>
    public partial class TestsView : Page
    {
        public TestsView()
        {
            InitializeComponent();
            this.DataContext = new TestsViewModel();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
