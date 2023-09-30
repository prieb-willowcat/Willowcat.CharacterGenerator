using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Willowcat.CharacterGenerator.Core.Models
{
    public class OptionCollection : ICollection<OptionModel>
    {
        private readonly Dictionary<int, OptionModel> _Map = new Dictionary<int, OptionModel>();
        private readonly string _ChartKey;

        public int Count
        {
            get
            {
                int total = 0;
                IEnumerator<OptionModel> options = GetOptions();
                while(options.MoveNext())
                {
                    total++;
                }
                return total;
            }
        }

        public bool IsReadOnly => false;

        public int MaximumKey { get { return _Map.Keys.Max(); } }

        public int MinimumKey { get { return _Map.Keys.Min(); } }

        /// <summary>
        /// One-based index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public OptionModel this[int index]
        {
            get
            {
                if (!_Map.ContainsKey(index))
                {
                    throw new ArgumentOutOfRangeException($"No value found for index [{index}]");
                }
                return _Map[index];
            }
        }

        public int NumberOfPossibleRolls => _Map.Count;

        public OptionCollection(string chartKey)
        {
            _ChartKey = chartKey;
        }

        public void Add(OptionModel item)
        {
            Add(item.Range.Start, item.Range.End, item);
        }

        public void Add(int start, int end, string description)
        {
            Add(start, end, new OptionModel(_ChartKey, Guid.NewGuid(), description));
        }

        public void Add(int start, int end, OptionModel details)
        {
            for (int i = start; i <= end; i++)
            {
                _Map[i] = details;
            }
        }

        public void Clear()
        {
            _Map.Clear();
        }

        public bool Contains(OptionModel item)
        {
            return _Map.Any(kvp => kvp.Value.OptionId == item.OptionId);
        }

        public void CopyTo(OptionModel[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new NullReferenceException("array is null");
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException($"{arrayIndex} is out of range");
            }
            int i = arrayIndex;
            foreach (var item in this)
            {
                if (i >= array.Length)
                {
                    throw new ArgumentException("There are more items in the collection than space in the array");
                }
                array[i] = item;
                i++;
            }
        }

        public IEnumerator<OptionModel> GetEnumerator()
        {
            return GetOptions();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetOptions();
        }

        public bool Remove(OptionModel item)
        {
            bool Removed = false;
            for (int i = item.Range.Start; i <= item.Range.End; i++)
            {
                if (!_Map.ContainsKey(i)) continue;

                if( _Map[i].OptionId == item.OptionId)
                {
                    _Map[i] = null;
                    Removed = true;
                }
            }
            return Removed;
        }

        public IEnumerator<OptionModel> GetOptions()
        {
            OptionModel previousOption = null;
            DiceRange range = GetRange();
            int? start = null;
            int i = range.Start;
            for (; i <= range.End && _Map.Any(); i++)
            {
                OptionModel nextOption = _Map.ContainsKey(i) ? _Map[i] : null;
                if (previousOption != null && (previousOption.OptionId != nextOption?.OptionId))
                {
                    previousOption.Range = new DiceRange(start ?? (i - 1), i - 1);
                    yield return previousOption;
                    start = i;
                }
                else if (!start.HasValue && nextOption != null)
                {
                    start = i;
                }

                if (nextOption == null)
                {
                    throw new MissingItemException($"Missing item in sequence at [{i}].", i);
                }

                previousOption = _Map[i];
            }

            if (previousOption != null)
            {
                previousOption.Range = new DiceRange(start ?? range.End, range.End);
                yield return previousOption;
            }
        }

        public DiceRange GetRange()
        {
            int? min = null;
            int? max = null;
            foreach (int key in _Map.Keys)
            {
                if (!min.HasValue || !max.HasValue)
                {
                    min = key;
                    max = key;
                }
                else 
                {
                    if (min > key)
                    {
                        min = key;
                    }
                    if (max < key)
                    {
                        max = key;
                    }
                }
            }
            return new DiceRange(min ?? 0, max ?? 0);
        }

        public void SetOptions(IEnumerable<OptionModel> enumerable)
        {
            _Map.Clear();
            foreach (var option in enumerable)
            {
                Add(option);
            }
        }

    }
}
