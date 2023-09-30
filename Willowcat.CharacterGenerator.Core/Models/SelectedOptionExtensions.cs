using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Willowcat.CharacterGenerator.Core.Models
{
    public static class SelectedOptionExtensions
    {
        public static string GetSelectionText(this IEnumerable<SelectedOption> options)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var option in options)
            {
                if (builder.Length > 0)
                {
                    builder.Append(" - ");
                }
                builder.Append(option.Description);
            }
            return builder.ToString();
        }
    }
}
