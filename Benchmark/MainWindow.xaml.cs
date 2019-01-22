using Benchmark.ViewModel;
using Benchmark.Views;
using System.Windows;

namespace Benchmark
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
            Content = new TestsView();
        }
    }
}
