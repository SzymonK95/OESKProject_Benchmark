using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Benchmark.ViewModel
{
    /// <summary>
    /// Implements INotifyPropertyChanged for all ViewModel
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool AlwaysTrue() { return true; }
        public bool AlwaysFalse() { return false; }
    }
}
