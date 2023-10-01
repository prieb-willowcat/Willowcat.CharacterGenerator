using Willowcat.CharacterGenerator.Core.Models;
using Willowcat.CharacterGenerator.Core.TextRepository;
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
                OnSaveCharacterDetailsExecute();
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
        public IList<SelectedOption> DetailOptionCollection
        {
            get => _characterModel.Details; 
            set
            {
                _characterModel.Details = value;
                OnPropertyChanged();
            }
        }

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

            MoveRowDownCommand = new DelegateCommand(OnMoveRowDownExecute);
            MoveRowUpCommand = new DelegateCommand(OnMoveRowUpExecute);
            SaveCharacterDetailsCommand = new DelegateCommand(OnSaveCharacterDetailsExecute);
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
                DetailOptionCollection.Insert(index + 1, detail);
            }
            else
            {
                DetailOptionCollection.Add(detail);
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
                    DetailOptionCollection.Remove(item);
                    DetailOptionCollection.Insert(newIndex, item);

                    SelectedIndex = newIndex;
                    OnSaveCharacterDetailsExecute();
                }
            }
        }

        private void OnMoveRowDownExecute() => MoveSelectedItems(1);

        private void OnMoveRowUpExecute() => MoveSelectedItems(-1);

        public void OnNavigateToSelectedItemExecute(SelectedOption option)
        {
            _eventAggregator.GetEvent<ChartSelectedEvent>().Publish(new ChartSelectedEventArgs(option.ChartKey, option.Range.Start));
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
                });
            }
            return Task.FromResult(0);
        }
    }
}
