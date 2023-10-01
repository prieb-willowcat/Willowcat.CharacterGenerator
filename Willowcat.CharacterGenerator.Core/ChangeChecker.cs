using System;
using System.Collections;
using System.Collections.Generic;

namespace Willowcat.CharacterGenerator.Core
{
    public static class ChangeChecker
    {
        public static bool HasChanges<T>(T original, T edited)
        {
            return HasChanges(original, edited, new List<object>());

        }
        public static bool HasChanges(object original, object edited, List<object> objectsChecked)
        {
            bool hasChanges = false;

            if (original is string s)
            {
                hasChanges = !string.Equals(s, edited?.ToString());
            }
            else if (original.GetType().IsValueType)
            {
                hasChanges = !original.Equals(edited);
            }
            else if (original is IList originalList)
            {
                var editedList = edited as IList;
                if (originalList == null || editedList == null)
                {
                    hasChanges = originalList != editedList;
                }
                else if (originalList.Count != editedList.Count)
                {
                    hasChanges = true;
                }
                else
                {
                    for (int i = 0; i < originalList.Count; i++)
                    {
                        hasChanges = HasChanges(originalList[i], editedList[i], objectsChecked);
                        if (hasChanges) break;
                    }
                }
            }
            else
            {
                objectsChecked.Add(original);
                var properties = original.GetType().GetProperties();
                foreach (var property in properties)
                {
                    var originalValue = property.GetValue(original);
                    var editedValue = property.GetValue(edited);
                    if (originalValue != null && editedValue != null)
                    {
                        hasChanges = HasChanges(originalValue, editedValue, objectsChecked);
                    }
                    else
                    {
                        hasChanges = originalValue != editedValue;
                    }
                    if (hasChanges) break;
                }
            }
            return hasChanges;
        }
    }
}
