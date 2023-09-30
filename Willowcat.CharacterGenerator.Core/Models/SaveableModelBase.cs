using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Willowcat.CharacterGenerator.Core.Models
{
    public interface ISaveableModel
    {
        void AcceptChanges();
        bool HasChanges();
    }

    public class SaveableModelBase : INotifyPropertyChanged, ISaveableModel
    {
        private readonly Dictionary<string, object> _CurrentValues = new Dictionary<string, object>();
        private readonly Dictionary<string, object> _OriginalValues = new Dictionary<string, object>();

        public event PropertyChangedEventHandler PropertyChanged;

        public void AcceptChanges()
        {
            _OriginalValues.Clear();
            foreach (var property in GetType().GetProperties())
            {
                var value = property.GetValue(this);
                if (value is ISaveableModel saveable)
                {
                    saveable.AcceptChanges();
                }
            }
        }

        protected T GetProperty<T>([CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName)) return default(T);

            if (_CurrentValues.TryGetValue(propertyName, out object value))
            {
                return (T)value;
            }
            else
            {
                return default(T);
            }
        }

        public bool HasChanges()
        {
            bool hasChanges = _OriginalValues.Any();
            if (!hasChanges)
            {
                foreach (var property in GetType().GetProperties())
                {
                    var value = property.GetValue(this);
                    if (value is ISaveableModel saveable)
                    {
                        hasChanges = saveable.HasChanges();
                        if (hasChanges) break;
                    }
                }
            }
            return hasChanges;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetProperty<T>(T newValue, [CallerMemberName] string propertyName = null)
        {
            if (_OriginalValues.TryGetValue(propertyName, out object originalValue))
            {
                if ((originalValue == null && newValue == null) || (originalValue != null && originalValue.Equals(newValue)))
                {
                    _OriginalValues.Remove(propertyName);
                }
            }
            else
            {
                var currentValue = GetProperty<T>(propertyName);
                if ((currentValue == null && newValue != null) || (currentValue != null && (newValue == null || !currentValue.Equals(newValue))))
                {
                    _OriginalValues[propertyName] = currentValue;
                }
            }
            _CurrentValues[propertyName] = newValue;
            OnPropertyChanged();
        }

    }

    public class SaveableList<T> : ObservableCollection<T>, ISaveableModel
    {
        // TODO: compare list details for changes
        private bool _HasCollectionChanges = false;

        public SaveableList()
        {
        }

        public SaveableList(IEnumerable<T> value) : base(value)
        {
        }

        public void AcceptChanges()
        {
            _HasCollectionChanges = false;
            foreach (var item in this)
            {
                if (item is ISaveableModel saveable)
                {
                    saveable.AcceptChanges();
                }
            }
        }

        public bool HasChanges()
        {
            if (!_HasCollectionChanges)
            {
                bool hasChanges = false;
                foreach (var item in this)
                {
                    if (item is ISaveableModel saveable)
                    {
                        hasChanges = saveable.HasChanges();
                        if (hasChanges) break;
                    }
                }
                return hasChanges;
            }
            return _HasCollectionChanges;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            _HasCollectionChanges = true;
        }
    }
}
