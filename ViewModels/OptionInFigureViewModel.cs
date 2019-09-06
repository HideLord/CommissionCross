using Cross.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Cross.ViewModels
{
    public class OptionInFigureViewModel : INotifyPropertyChanged
    {
        private Figure square;
        public Figure Square
        {
            get => square;
            set
            {
                square = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Square"));
            }
        }
        private string optionOne;
        public string OptionOne
        {
            get => optionOne;
            set
            {
                optionOne = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OptionOne"));
            }
        }
        private string optionTwo;
        public string OptionTwo
        {
            get => optionTwo;
            set
            {
                optionTwo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OptionTwo"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
