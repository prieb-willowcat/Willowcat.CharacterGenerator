using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Willowcat.CharacterGenerator.UI.Event;
using Willowcat.Common.UI.ViewModels;
using Willowcat.CharacterGenerator.Core;
using Willowcat.CharacterGenerator.Model;
using System.Collections.ObjectModel;
using Willowcat.CharacterGenerator.FlatFile.TextRepository;
using System.Reflection.Metadata;

namespace Willowcat.CharacterGenerator.UI.ViewModel
{
    public class CharacterDetailsViewModel : ViewModelBase
    {
        private CharacterModel _originalModel;
        private CharacterModel _characterModel;
        private readonly ICharacterSerializer _characterDetailSerializer;
        private readonly IEventAggregator _eventAggregator = null;

        private int _SelectedIndex = -1;
        private string _CurrentFilePath;
        private ObservableCollection<SelectedOption> _detailOptionCollection = new ObservableCollection<SelectedOption>();

        public string CharacterName
        {
            get => _characterModel.Name;
            set
            {
                _characterModel.Name = value;
                OnPropertyChanged();
                OnSaveCharacterDetailsExecute();
            }
        }
        public string CurrentFilePath
        {
            get => _CurrentFilePath;
            set
            {
                _CurrentFilePath = value;
                OnPropertyChanged();
            }
        }
        public bool HasUnsavedChanges => ChangeChecker.HasChanges(_originalModel, _characterModel);
        public string Notes
        {
            get => _characterModel.Notes;
            set
            {
                _characterModel.Notes = value;
                OnPropertyChanged();
            }
        }
        public int SelectedIndex
        {
            get => _SelectedIndex;
            set
            {
                _SelectedIndex = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<SelectedOption> DetailOptionCollection
        {
            get => _detailOptionCollection; 
            private set
            {
                _detailOptionCollection = value;
                OnPropertyChanged();
            }
        }
        public ICommand DeleteRowCommand { get; private set; }
        public ICommand NavigateToSelectedItemCommand { get; private set; }
        public ICommand MoveRowDownCommand { get; private set; }
        public ICommand MoveRowUpCommand { get; private set; }
        public ICommand SaveCharacterDetailsCommand { get; private set; }

        public CharacterDetailsViewModel(
            IEventAggregator eventAggregator,
            ICharacterSerializer characterDetailSerializer,
            CharacterModel characterModel = null)
        {
            _characterDetailSerializer = characterDetailSerializer ?? throw new ArgumentNullException(nameof(characterDetailSerializer));
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            _characterModel = characterModel ?? new CharacterModel();
            _originalModel = _characterModel.Clone();

            DetailOptionCollection = new ObservableCollection<SelectedOption>(_characterModel.Details);

            NavigateToSelectedItemCommand = new DelegateCommand<object>(OnNavigateToSelectedItemExecute);
            MoveRowDownCommand = new DelegateCommand(OnMoveRowDownExecute);
            MoveRowUpCommand = new DelegateCommand(OnMoveRowUpExecute);
            DeleteRowCommand = new DelegateCommand(OnDeleteRowExecute);
            SaveCharacterDetailsCommand = new DelegateCommand(OnSaveCharacterDetailsExecute);

            _eventAggregator.GetEvent<OptionSelectedEvent>().Subscribe(AddSelectedOptionEvent);
        }

        private async void AddSelectedOptionEvent(OptionSelectedEventArgs args)
        {
            await AddCharacterDetailAsync(args.SelectedOption);
        }

        public async Task AddCharacterDetailAsync(SelectedOption selectedOption)
        {
            if (selectedOption != null)
            {
                AddCharacterDetail(selectedOption, SelectedIndex);
                await SaveCharacterDetailsAsync();
            }
        }

        private void AddCharacterDetail(SelectedOption detail, int index = -1)
        {
            if (index >= 0 && index < DetailOptionCollection.Count)
            {
                Insert(index + 1, detail);
            }
            else
            {
                Add(detail);
            }
        }

        public void MoveSelectedItems(int adjust)
        {
            if (SelectedIndex >= 0)
            {
                var currentIndex = SelectedIndex;
                var newIndex = SelectedIndex + adjust;

                if (newIndex >= 0 && newIndex < DetailOptionCollection.Count)
                {
                    var item = DetailOptionCollection[currentIndex];
                    Remove(item);
                    Insert(newIndex, item);

                    SelectedIndex = newIndex;
                    OnSaveCharacterDetailsExecute();
                }
            }
        }

        private void OnDeleteRowExecute()
        {
            DetailOptionCollection.RemoveAt(SelectedIndex);
        }

        private void OnMoveRowDownExecute() => MoveSelectedItems(1);

        private void OnMoveRowUpExecute() => MoveSelectedItems(-1);

        private void OnNavigateToSelectedItemExecute(object value)
        {
            if (value is SelectedOption option)
            {
                _eventAggregator.GetEvent<ChartSelectedEvent>().Publish(new ChartSelectedEventArgs(option.ChartKey, option.Range.Start));
            }
        }

        private async void OnSaveCharacterDetailsExecute() => await SaveCharacterDetailsAsync();

        public Task SaveCharacterDetailsAsync(string filePath = null)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                CurrentFilePath = filePath;
            }
            else
            {
                filePath = CurrentFilePath;
            }

            if (!string.IsNullOrWhiteSpace(filePath))
            {
                Task.Run(() =>
                {
                    string details = _characterDetailSerializer.Serialize(_characterModel);
                    File.WriteAllText(filePath, details);
                    _originalModel = _characterModel;
                    _characterModel = _originalModel.Clone();
                    DetailOptionCollection = new ObservableCollection<SelectedOption>(_characterModel.Details);
                });
            }
            return Task.FromResult(0);
        }

        // TODO: handle keeping collections in sync differently
        private void Add(SelectedOption option)
        {
            DetailOptionCollection.Add(option);
            _characterModel.Details.Add(option);
        }
        private void Insert(int index, SelectedOption option)
        {
            DetailOptionCollection.Insert(index, option);
            _characterModel.Details.Insert(index, option);
        }
        private void Remove(SelectedOption option)
        {
            DetailOptionCollection.Remove(option);
            _characterModel.Details.Remove(option);
        }
    }
}
