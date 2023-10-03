
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Willowcat.CharacterGenerator.Core.Tests.Extension;
using Willowcat.CharacterGenerator.FlatFile.Repository;
using Willowcat.CharacterGenerator.FlatFile.TextRepository;
using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.Core.Tests
{
    [TestClass]
    public class FlatFileSerializerTests
    {
        private readonly ChartFlatFileSerializer _FlatFileSerializer = new ChartFlatFileSerializer();
        private readonly string _Source = "Xanathar";

        public void AssertEquals(FlatFileChartModel expected, FlatFileChartModel actual)
        {
            Assert.AreEqual(expected.Key, actual.Key, nameof(actual.Key));
            Assert.AreEqual(expected.ChartName, actual.ChartName, nameof(actual.ChartName));
            Assert.AreEqual(expected.Notes, actual.Notes, nameof(actual.Notes));
            Assert.AreEqual(expected.Dice.Count, actual.Dice.Count, "Dice.Count");
            Assert.AreEqual(expected.Dice.DiceSides, actual.Dice.DiceSides, "Dice Sides");
            Assert.AreEqual(expected.Options.Count, actual.Options.Count, "Options.Count");

            var expectedMin = expected.Options.Min(x => x.Range.Start);
            var expectedMax = expected.Options.Max(x => x.Range.End);
            var actualMin = actual.Options.Min(x => x.Range.Start);
            var actualMax = actual.Options.Max(x => x.Range.End);
            Assert.AreEqual(expectedMin, actualMin, "options start");
            Assert.AreEqual(expectedMax, actualMax, "options end");

            for (int i = 0; i < expected.Options.Count; i++)
            {
                Assert.AreEqual(expected.Options[i].Description, actual.Options[i].Description, $"Options[{i}].Description");
            }
        }

        [TestMethod]
        public void Deserialize_SimpleChart()
        {
            string input = "# X01" + Environment.NewLine +
                "Parents" + Environment.NewLine +
                "1d6" + Environment.NewLine +
                "1\tOption 1" + Environment.NewLine +
                "2\tOption 2" + Environment.NewLine +
                "3\tOption 3" + Environment.NewLine +
                "4\tOption 4" + Environment.NewLine +
                "5\tOption 5" + Environment.NewLine +
                "6\tOption 6";
            FlatFileChartModel expected = new FlatFileChartModel()
            {
                ChartName = "Parents",
                Dice = new Dice(1, 6),
                Key = "X01"
            };
            expected.Options.Add(1, 1, "Option 1");
            expected.Options.Add(2, 2, "Option 2");
            expected.Options.Add(3, 3, "Option 3");
            expected.Options.Add(4, 4, "Option 4");
            expected.Options.Add(5, 5, "Option 5");
            expected.Options.Add(6, 6, "Option 6");

            List<FlatFileChartModel> output = _FlatFileSerializer.Deserialize(_Source, input);

            Assert.AreEqual(1, output.Count, "list count");
            
            AssertEquals(expected, output[0]);
        }

        [TestMethod]
        public void Deserialize_RangeChart_Negative()
        {
            string input = "# X01" + Environment.NewLine +
                "Parents" + Environment.NewLine +
                "1d6" + Environment.NewLine +
                "-2\tReroll" + Environment.NewLine +
                "-1-4\tOption 1" + Environment.NewLine +
                "5-6\tOption 2";
            FlatFileChartModel expected = new FlatFileChartModel()
            {
                ChartName = "Parents",
                Dice = new Dice(1, 6),
                Key = "X01"
            };
            expected.Options.Add(-2, -2, "Reroll");
            expected.Options.Add(-1, 4, "Option 1");
            expected.Options.Add(5, 6, "Option 2");

            List<FlatFileChartModel> output = _FlatFileSerializer.Deserialize(_Source, input);

            Assert.AreEqual(1, output.Count, "list count");

            AssertEquals(expected, output[0]);
        }

        [TestMethod]
        public void Deserialize_RangeChart()
        {
            string input = "# X01" + Environment.NewLine +
                "Parents" + Environment.NewLine +
                "1d6" + Environment.NewLine +
                "1-4\tOption 1" + Environment.NewLine +
                "5-6\tOption 2";
            FlatFileChartModel expected = new FlatFileChartModel()
            {
                ChartName = "Parents",
                Dice = new Dice(1, 6),
                Key = "X01"
            };
            expected.Options.Add(1, 4, "Option 1");
            expected.Options.Add(5, 6, "Option 2");

            List<FlatFileChartModel> output = _FlatFileSerializer.Deserialize(_Source, input);

            Assert.AreEqual(1, output.Count, "list count");

            AssertEquals(expected, output[0]);
        }

        [TestMethod]
        public void Deserialize_TwoChart()
        {
            string input = "# X01" + Environment.NewLine +
                "Parents" + Environment.NewLine +
                "1d6" + Environment.NewLine +
                "1-4\tOption 1" + Environment.NewLine +
                "5-6\tOption 2" + Environment.NewLine + Environment.NewLine +
                "# X02" + Environment.NewLine +
                "Birthplace" + Environment.NewLine +
                "1d8" + Environment.NewLine +
                "1-4\tThe Best 1" + Environment.NewLine +
                "5-8\tThe Worst 2";
            FlatFileChartModel expected1 = new FlatFileChartModel()
            {
                ChartName = "Parents",
                Dice = new Dice(1, 6),
                Key = "X01"
            };
            expected1.Options.Add(1, 4, "Option 1");
            expected1.Options.Add(5, 6, "Option 2");
            FlatFileChartModel expected2 = new FlatFileChartModel()
            {
                ChartName = "Birthplace",
                Dice = new Dice(1, 8),
                Key = "X02"
            };
            expected2.Options.Add(1, 4, "The Best 1");
            expected2.Options.Add(5, 8, "The Worst 2");

            List<FlatFileChartModel> output = _FlatFileSerializer.Deserialize(_Source, input);

            Assert.AreEqual(2, output.Count, "list count");

            AssertEquals(expected1, output[0]);
            AssertEquals(expected2, output[1]);
        }

        [TestMethod]
        public void Deserialize_RangeChart_Notes()
        {
            string input = "# X01" + Environment.NewLine +
                "Parents" + Environment.NewLine +
                "Notes: This is a note" + Environment.NewLine + 
                "1d6" + Environment.NewLine +
                "1-4\tOption 1" + Environment.NewLine +
                "5-6\tOption 2";
            FlatFileChartModel expected = new FlatFileChartModel()
            {
                ChartName = "Parents",
                Dice = new Dice(1, 6),
                Notes = "This is a note",
                Key = "X01",
            };
            expected.Options.Add(1, 4, "Option 1");
            expected.Options.Add(5, 6, "Option 2");

            List<FlatFileChartModel> output = _FlatFileSerializer.Deserialize(_Source, input);

            Assert.AreEqual(1, output.Count, "list count");

            AssertEquals(expected, output[0]);
        }

        [TestMethod]
        public void Deserialize_OutOfDiceRangeChart()
        {
            string input = "# X01" + Environment.NewLine +
                "Parents" + Environment.NewLine +
                "1d6" + Environment.NewLine +
                "1-4\tOption 1" + Environment.NewLine +
                "5-7\tOption 2";
            FlatFileChartModel expected = new FlatFileChartModel()
            {
                ChartName = "Parents",
                Dice = new Dice(1, 6),
                Key = "X01"
            };
            expected.Options.Add(1, 4, "Option 1");
            expected.Options.Add(5, 7, "Option 2");

            List<FlatFileChartModel> output = _FlatFileSerializer.Deserialize(_Source, input);

            Assert.AreEqual(1, output.Count, "chart count");
            AssertEquals(expected, output[0]);
        }
    }
}
