using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Willowcat.Common.UI.ViewModels;

namespace Willowcat.CharacterGenerator.UI.ViewModel
{
    public class DiceRollViewModel : ViewModelBase
    {
        private readonly Random _random;
        private int _diceCount = 1;
        private int _diceSize = 6;
        private ObservableCollection<int> _diceResults = new ObservableCollection<int>();

        public DiceRollViewModel() : this(Random.Shared)
        {
        }

        public DiceRollViewModel(Random random)
        {
            _random = random;
            OnPropertyChanged(nameof(HasResults));

            RollDiceCommand = new DelegateCommand<int?>(RollDice);
        }

        public int DiceCount
        {
            get => _diceCount;
            set
            {
                _diceCount = value;
                OnPropertyChanged();
            }
        }

        public int DiceSize
        {
            get => _diceSize;
            set
            {
                _diceSize = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<int> DiceResults
        {
            get => _diceResults; 
            private set
            {
                _diceResults = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasResults));
                OnPropertyChanged(nameof(ResultTotal));
            }
        }

        public bool HasResults => DiceResults.Any();

        public int ResultTotal => DiceResults.Sum();

        public ICommand RollDiceCommand { get; private set; }

        public void RollDice(int? diceSize = null)
        {
            if (diceSize.HasValue)
            {
                DiceSize = diceSize.Value;
            }
            int[] results = Enumerable
                .Range(1, DiceCount <= 0 ? 1 : DiceCount)
                .Select(i => _random.Next(1, DiceSize + 1))
                .ToArray();
            DiceResults = new ObservableCollection<int>(results);
        }
    }
}
