namespace Willowcat.CharacterGenerator.Model.Tests
{
    [TestClass]
    public class DiceTest
    {
        [TestMethod]
        public void Equals_1d2()
        {
            var dice1 = new Dice(1, 2);
            var dice2 = new Dice(1, 2);
            Assert.AreEqual(dice1, dice2);
        }

        [TestMethod]
        public void Equals_6d12()
        {
            var dice1 = new Dice(6, 12);
            var dice2 = new Dice(6, 12);
            Assert.AreEqual(dice1, dice2);
        }

        [TestMethod]
        public void NotEquals_6d12_5d12()
        {
            var dice1 = new Dice(6, 12);
            var dice2 = new Dice(5, 12);
            Assert.AreNotEqual(dice1, dice2);
        }

        [TestMethod]
        public void NotEquals_6d12_6d10()
        {
            var dice1 = new Dice(6, 12);
            var dice2 = new Dice(6, 10);
            Assert.AreNotEqual(dice1, dice2);
        }

        [TestMethod]
        public void Parse_1d2()
        {
            var dice = Dice.Parse("1d2");
            Assert.IsNotNull(dice);
            Assert.AreEqual(1, dice.Count, nameof(Dice.Count));
            Assert.AreEqual(2, dice.DiceSides, nameof(Dice.DiceSides));
        }

        [TestMethod]
        public void Parse_d20()
        {
            var dice = Dice.Parse("d20");
            Assert.IsNotNull(dice);
            Assert.AreEqual(1, dice.Count, nameof(Dice.Count));
            Assert.AreEqual(20, dice.DiceSides, nameof(Dice.DiceSides));
        }

        [TestMethod]
        public void Parse_5d47()
        {
            var dice = Dice.Parse("5d47");
            Assert.IsNotNull(dice);
            Assert.AreEqual(5, dice.Count, nameof(Dice.Count));
            Assert.AreEqual(47, dice.DiceSides, nameof(Dice.DiceSides));
        }

        [TestMethod]
        public void Parse_2d()
        {
            try
            {
                var dice = Dice.Parse("2d");
                Assert.Fail($"Expected {nameof(FormatException)} to be thrown");
            }
            catch (FormatException) { }
        }

        [TestMethod]
        public void Parse_2_d4()
        {
            try
            {
                var dice = Dice.Parse("2 d4");
                Assert.Fail($"Expected {nameof(FormatException)} to be thrown");
            }
            catch (FormatException) { }
        }

        [TestMethod]
        public void Parse_neg2d4()
        {
            try
            {
                var dice = Dice.Parse("-2d4");
                Assert.Fail($"Expected {nameof(FormatException)} to be thrown");
            }
            catch (FormatException) { }
        }

        [TestMethod]
        public void Parse_2neg4()
        {
            try
            {
                var dice = Dice.Parse("2d-4");
                Assert.Fail($"Expected {nameof(FormatException)} to be thrown");
            }
            catch (FormatException) { }
        }

        [TestMethod]
        public void Parse_2d_4()
        {
            try
            {
                var dice = Dice.Parse("2d 4");
                Assert.Fail($"Expected {nameof(FormatException)} to be thrown");
            }
            catch (FormatException) { }
        }
    }
}