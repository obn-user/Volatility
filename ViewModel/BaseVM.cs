using System.ComponentModel;

namespace Test.ViewModel

{

    public class BaseVM : INotifyPropertyChanged
    {
        public void OnPropertyChanged(string prop)
        {

            if (PropertyChanged != null)

                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }


}