using Willowcat.CharacterGenerator.Application.Interface;
using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.OnlineGenerators
{

    public class RandomNameChart : ChartModel
    {
        public RandomNameChart(INameGenerator nameGenerator, string key, string name)
        {
            NameGenerator = nameGenerator;
            Key = key;
            ChartName = name;
            Source = "Names";
            AutogenerateOptions = true;
        }

        public INameGenerator NameGenerator { get; private set; }
    }
}
