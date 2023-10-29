using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Willowcat.CharacterGenerator.Model;
using Willowcat.CharacterGenerator.Model.Extension;
using Willowcat.CharacterGenerator.UI.ViewModel;

namespace Willowcat.CharacterGenerator.UI.View.Controls
{
    public class DiceRollerHyperlink : Hyperlink
    {
        private readonly DiceRollViewModel _viewModel;

        public DiceRollerHyperlink(Inline inline, Dice dice, string @operator, int modifier)
            : base(inline)
        {
            _viewModel = App.Provider.GetRequiredService<DiceRollViewModel>();
            Dice = dice;
            ModifierOperator = @operator;
            Modifier = modifier;
        }

        public Dice Dice
        {
            get { return (Dice)GetValue(DiceProperty); }
            set { SetValue(DiceProperty, value); }
        }
        public static readonly DependencyProperty DiceProperty =
            DependencyProperty.Register("Dice", typeof(Dice), typeof(DiceButton), new PropertyMetadata(new Dice(1,1), DiceRollerHyperlinkPropertyChanged));

        public int Modifier
        {
            get { return (int)GetValue(ModifierProperty); }
            set { SetValue(ModifierProperty, value); }
        }
        public static readonly DependencyProperty ModifierProperty =
            DependencyProperty.Register("Modifier", typeof(int), typeof(DiceButton), new PropertyMetadata(0, DiceRollerHyperlinkPropertyChanged));

        public string ModifierOperator
        {
            get { return (string)GetValue(ModifierOperatorProperty); }
            set { SetValue(ModifierOperatorProperty, value); }
        }
        public static readonly DependencyProperty ModifierOperatorProperty =
            DependencyProperty.Register("ModifierOperator", typeof(string), typeof(DiceButton), new PropertyMetadata("", DiceRollerHyperlinkPropertyChanged));

        public static void DiceRollerHyperlinkPropertyChanged(DependencyObject @object, DependencyPropertyChangedEventArgs e)
        {
            if (@object is DiceRollerHyperlink link)
            {
                string diceText = link.Dice.FormatDice(link.ModifierOperator, link.Modifier);
                link.Inlines.Clear();
                link.Inlines.Add(diceText);
            }
        }

        protected override void OnClick()
        {
            _viewModel.DiceSize = Dice.DiceSides;
            _viewModel.DiceCount = Dice.Count;
            _viewModel.RollDice();

            string diceResultsString = FormatDiceResults(_viewModel.DiceResults, _viewModel.ResultTotal); ;
            var tooltip = new ToolTip() 
            { 
                Content = diceResultsString,
                IsOpen = true
            };
            ToolTip = tooltip;
        }

        private string FormatDiceResults(IEnumerable<int> results, int total)
        {
            string diceText = string.Join(", ", results);
            string modifierText = string.Empty;
            if (Modifier != 0 && (ModifierOperator == "+" || ModifierOperator == "-"))
            {
                modifierText += $" {ModifierOperator} [{Modifier}]";
            }
            else if (Modifier != 1 && (ModifierOperator == "*" || ModifierOperator == "/"))
            {
                modifierText += $" {ModifierOperator} [{Modifier}]";
            }

            switch (ModifierOperator)
            {
                case "+": total += Modifier; break;
                case "-": total -= Modifier; break;
                case "*": total *= Modifier; break;
                case "/": total /= Modifier; break;
                default: break;
            }
            if (string.IsNullOrEmpty(modifierText) && results.Count() == 1)
            {
                return $"[{total}]";
            }
            else
            {
                return $"[{diceText}]{modifierText} = [{total}]";
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (ToolTip is ToolTip t)
            {
                t.IsOpen = false;
            }
        }
    }
}
