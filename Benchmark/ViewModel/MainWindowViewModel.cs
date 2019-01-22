namespace Benchmark.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Parameters
        /// <summary>
        /// Title of the application, as displayed in the top bar of the window
        /// </summary>
        public string Title
        {
            get { return "Benchmark"; }
        }

        #endregion

        #region Constructors

        public MainWindowViewModel()
        {

        }
        #endregion

    }
}