using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Counter.Models;

namespace Counter
{
    class MainPageViewModel : ObservableObject
    {
        private string path = Path.Combine(FileSystem.AppDataDirectory, "Counters.xml");
        public string _initNumber = "";
        public string Name { get; set; } = "";
        public string InitNumber
        {
            get => _initNumber;
            set
            {
                if (value == "delete")
                {
                    _initNumber = "";
                    OnPropertyChanged(nameof(InitNumber));
                    return;
                }

                if (!(new Regex(@"^\d+$", RegexOptions.IgnoreCase).Match(value).Success))
                {
                    if (value.Length == 0) return;
                    _initNumber = value.Remove(value.Length - 1);
                    OnPropertyChanged(nameof(InitNumber));
                    return;
                }

                _initNumber = value;
                OnPropertyChanged(nameof(InitNumber));
            }
        }

        public ObservableCollection<CounterModel> AllCounters { get; }
        public ICommand NewCounterCommand { get; }
        public ICommand DeleteCounterCommand { get; }
        public ICommand SaveCommand { get; }

        public MainPageViewModel()
        {

            AllCounters = Load();
            DeleteCounterCommand = new AsyncRelayCommand<string>(DeleteCounterAsync);
            NewCounterCommand = new AsyncRelayCommand(NewCounterAsync);
            SaveCommand = new AsyncRelayCommand(Save);
        }

        private async Task NewCounterAsync()
        {
            if (Name=="" || !int.TryParse(_initNumber, out var number)) return;
            var model = new CounterModel
            {
                InitValue = number,
                CurValue = number,
                Name = Name,
                BackgroundColor = Colors.White,
            };
            InitNumber = "delete";
            Name = "";
            AllCounters.Add(model);
            Save();
            OnPropertyChanged(nameof(AllCounters));
            OnPropertyChanged(nameof(Name));
        }
        private async Task DeleteCounterAsync(string name)
        {
            AllCounters.Remove(AllCounters.Where(n => n.Name == name).First());
            Save();
            OnPropertyChanged(nameof(AllCounters));

        }
        private ObservableCollection<CounterModel> Load()
        {
            if (!File.Exists(path)) return new();

            string contents = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(contents)) return new();

            XElement root;
            try
            {
                root = XElement.Parse(contents);
            }
            catch
            {
                return new();
            }

            var list = new ObservableCollection<CounterModel>();

            foreach (var itemElem in root.Elements())
            {
                var name = itemElem.Name.LocalName ?? string.Empty;

                int initValue = 0;
                int curValue = 0;
                Color backgroundColor = Colors.White;

                var initEl = itemElem.Element("InitValue");
                if (initEl != null) int.TryParse(initEl.Value, out initValue);

                var curEl = itemElem.Element("CurValue");
                if (curEl != null) int.TryParse(curEl.Value, out curValue);

                var BgEl = itemElem.Element("BackgroundColor");
                if (BgEl != null) Color.TryParse(BgEl.Value, out backgroundColor);

                if (curEl == null)
                    curValue = initValue;

                var model = new CounterModel
                {
                    Name = name,
                    InitValue = initValue,
                    CurValue = curValue,
                    BackgroundColor = backgroundColor,
                };

                list.Add(model);
            }

            return list;
        }
        public async Task Save()
        {
            XElement countersXML = new XElement("counters");
            foreach (var item in AllCounters)
            {
                countersXML.Add(new XElement(item.Name,
                    new XElement("BackgroundColor",item.BackgroundColor.ToHex()),
                    new XElement("InitValue",item.InitValue),
                    new XElement("CurValue",item.CurValue)));
            }
            File.WriteAllText(path, countersXML.ToString());
        }

    }
}
