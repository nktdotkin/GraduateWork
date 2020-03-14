using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace InventoryApp.ViewModels.Base
{
    public abstract class ViewModelsBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
