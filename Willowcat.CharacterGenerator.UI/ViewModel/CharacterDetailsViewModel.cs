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

namespace Willowcat.CharacterGenerator.UI.ViewModel
{
    public class CharacterDetailsViewModel : ViewModelBase
    {
        private readonly CharacterModel _CharacterModel;
        private readonly ICharacterSerializer _CharacterDetailSerializer;
        private readonly IEventAggregator _EventAggregator = null;

        private int _SelectedIndex = -1;
        private string _CurrentFilePath;

        public string CharacterName
        {
            get => _CharacterModel.Name;
            set
            {
                _CharacterModel.Name = value;
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
        public bool HasUnsavedChanges => _CharacterModel.HasChanges();
        public string Notes
        {
            get => _CharacterModel.Notes;
            set
            {
                _CharacterModel.Notes = value;
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
            get => _CharacterModel.Details; 
            set
            {
                _CharacterModel.Details = value;
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
            _CharacterDetailSerializer = characterDetailSerializer ?? throw new ArgumentNullException(nameof(characterDetailSerializer));
            _EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            _CharacterModel = characterModel ?? new CharacterModel();

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
            _EventAggregator.GetEvent<ChartSelectedEvent>().Publish(new ChartSelectedEventArgs(option.ChartKey, option.Range.Start));
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
                    string details = _CharacterDetailSerializer.Serialize(_CharacterModel);
                    File.WriteAllText(filePath, details);
                    _CharacterModel.AcceptChanges();
                });
            }
            return Task.FromResult(0);
        }
    }
}
