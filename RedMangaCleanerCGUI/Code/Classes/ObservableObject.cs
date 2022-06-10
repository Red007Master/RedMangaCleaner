using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RedMangaCleanerCGUI.WPFDesign.Code.Classes
{
    class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChange([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
