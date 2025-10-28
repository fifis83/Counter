using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Counter.Models
{
    public class CounterModel : ObservableObject
    {
        private int _curValue;
        private Color _backgroundColor = Colors.White;

        public int InitValue { get; set; }
        public string Name { get; set; } = string.Empty;

        public Color BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }

        public int CurValue
        {
            get => _curValue;
            set => SetProperty(ref _curValue, value);
        }

    }
}