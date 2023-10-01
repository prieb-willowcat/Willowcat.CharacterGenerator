namespace Willowcat.CharacterGenerator.Model.Tests
{
    [TestClass]
    public class DiceRangeTest
    {
        [TestMethod]
        public void Equals_neg1to2()
        {
            var dice1 = new DiceRange(-1, 2);
            var dice2 = new DiceRange(-1, 2);
            Assert.AreEqual(dice1, dice2);
        }

        [TestMethod]
        public void Equals_6to12()
        {
            var dice1 = new DiceRange(6, 12);
            var dice2 = new DiceRange(6, 12);
            Assert.AreEqual(dice1, dice2);
        }

        [TestMethod]
        public void Equals_6to12_12to6()
        {
            var dice1 = new DiceRange(6, 12);
            var dice2 = new DiceRange(12, 6);
            Assert.AreEqual(dice1, dice2);
        }

        [TestMethod]
        public void NotEquals_neg1to2_1to2()
        {
            var dice1 = new DiceRange(-1, 2);
            var dice2 = new DiceRange(1, 2);
            Assert.AreNotEqual(dice1, dice2);
        }

        [TestMethod]
        public void NotEquals_6to12_5to12()
        {
            var dice1 = new DiceRange(6, 12);
            var dice2 = new DiceRange(5, 12);
            Assert.AreNotEqual(dice1, dice2);
        }

        [TestMethod]
        public void NotEquals_6to12_6to10()
        {
            var dice1 = new DiceRange(6, 12);
            var dice2 = new DiceRange(6, 10);
            Assert.AreNotEqual(dice1, dice2);
        }

        [TestMethod]
        public void Parse_1to2()
        {
            var range = DiceRange.Parse("1-2");
            Assert.IsNotNull(range);
            Assert.AreEqual(1, range.Start, nameof(DiceRange.Start));
            Assert.AreEqual(2, range.End, nameof(DiceRange.End));
        }

        [TestMethod]
        public void Parse_2to1()
        {
            var range = DiceRange.Parse("2-1");
            Assert.IsNotNull(range);
            Assert.AreEqual(1, range.Start, nameof(DiceRange.Start));
            Assert.AreEqual(2, range.End, nameof(DiceRange.End));
        }

        [TestMethod]
        public void Parse_20()
        {
            var range = DiceRange.Parse("20");
            Assert.IsNotNull(range);
            Assert.AreEqual(20, range.Start, nameof(DiceRange.Start));
            Assert.AreEqual(20, range.End, nameof(DiceRange.End));
        }

        [TestMethod]
        public void Parse_5to47()
        {
            var range = DiceRange.Parse("5-47");
            Assert.IsNotNull(range);
            Assert.AreEqual(5, range.Start, nameof(DiceRange.Start));
            Assert.AreEqual(47, range.End, nameof(DiceRange.End));
        }

        [TestMethod]
        public void Parse_neg5to47()
        {
            var range = DiceRange.Parse("-5-47");
            Assert.IsNotNull(range);
            Assert.AreEqual(-5, range.Start, nameof(DiceRange.Start));
            Assert.AreEqual(47, range.End, nameof(DiceRange.End));
        }

        [TestMethod]
        public void Parse_neg5toneg47()
        {
            var range = DiceRange.Parse("-5--47");
            Assert.IsNotNull(range);
            Assert.AreEqual(-47, range.Start, nameof(DiceRange.Start));
            Assert.AreEqual(-5, range.End, nameof(DiceRange.End));
        }

        [TestMethod]
        public void Parse_neg47toneg5()
        {
            var range = DiceRange.Parse("-47--5");
            Assert.IsNotNull(range);
            Assert.AreEqual(-47, range.Start, nameof(DiceRange.Start));
            Assert.AreEqual(-5, range.End, nameof(DiceRange.End));
        }

        [TestMethod]
        public void Parse_2d()
        {
            try
            {
                var range = DiceRange.Parse("2d");
                Assert.Fail($"Expected {nameof(FormatException)} to be thrown");
            }
            catch (FormatException) { }
        }

        [TestMethod]
        public void Parse_2_to4()
        {
            try
            {
                var range = DiceRange.Parse("2 -4");
                Assert.Fail($"Expected {nameof(FormatException)} to be thrown");
            }
            catch (FormatException) { }
        }

        [TestMethod]
        public void Parse_2_to_4()
        {
            try
            {
                var range = DiceRange.Parse("2 - 4");
                Assert.Fail($"Expected {nameof(FormatException)} to be thrown");
            }
            catch (FormatException) { }
        }
    }
}