using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Willowcat.CharacterGenerator.Core.Models;

namespace Willowcat.CharacterGenerator.Core.Tests
{
    [TestClass]
    public class OptionCollectionTests
    {
        //#region Initialize_1_to_6
        //[TestMethod]
        //public void Initialize_1_to_6()
        //{
        //    int start = 1;
        //    int end = 6;
        //    int count = 6;
        //    OptionCollection collection = new OptionCollection(start, end);

        //    Assert.AreEqual(start, collection.ValueRange.Start, "Min range");
        //    Assert.AreEqual(end, collection.ValueRange.End, "Max range");
        //    Assert.AreEqual(count, collection.ValueCount, "Count");
        //    for (int i = start; i <= end; i++)
        //    {
        //        Assert.IsNull(collection[i], $"[{i}] is not null");
        //    }
        //}
        //#endregion Initialize_1_to_6

        //#region Initialize_2_to_12
        //[TestMethod]
        //public void Initialize_2_to_12()
        //{
        //    int start = 2;
        //    int end = 12;
        //    int count = 11;
        //    OptionCollection collection = new OptionCollection(start, end);

        //    Assert.AreEqual(start, collection.ValueRange.Start, "Min range");
        //    Assert.AreEqual(end, collection.ValueRange.End, "Max range");
        //    Assert.AreEqual(count, collection.ValueCount, "Count");
        //    for (int i = start; i <= end; i++)
        //    {
        //        Assert.IsNull(collection[i], $"[{i}] is not null");
        //    }
        //}
        //#endregion Initialize_2_to_12

        [TestMethod]
        public void Add_1_to_6_OneItem()
        {
            OptionCollection collection = new OptionCollection("akey");
            collection.Add(3, 3, "This is a description");
            Assert.AreEqual("This is a description", collection[3].Description);
            Assert.AreEqual(3, collection.MinimumKey, nameof(collection.MinimumKey));
            Assert.AreEqual(3, collection.MaximumKey, nameof(collection.MaximumKey));
            Assert.AreEqual(1, collection.Count(), "option count");
            try
            {
                string s = collection[1].Description;
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException) { }
        }

        [TestMethod]
        public void Add_1_to_6_RangedItem()
        {
            OptionCollection collection = new OptionCollection("akey");
            collection.Add(3, 4, "This is a description");

            Assert.AreEqual("This is a description", collection[3].Description);
            Assert.AreEqual("This is a description", collection[4].Description);
            Assert.AreEqual(3, collection.MinimumKey, nameof(collection.MinimumKey));
            Assert.AreEqual(4, collection.MaximumKey, nameof(collection.MaximumKey));
            Assert.AreEqual(1, collection.Count(), "option count");
            try
            {
                string s = collection[1].Description;
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException) { }
        }

        [TestMethod]
        public void Add_1_to_6_TwoItems()
        {
            const string s1 = "This is a description";
            const string s2 = "This is a second description";
            OptionCollection collection = new OptionCollection("akey");
            collection.Add(3, 3, s1);
            collection.Add(4, 4, s2);

            Assert.AreEqual(s1, collection[3].Description);
            Assert.AreEqual(s2, collection[4].Description);
            Assert.AreEqual(3, collection.MinimumKey, nameof(collection.MinimumKey));
            Assert.AreEqual(4, collection.MaximumKey, nameof(collection.MaximumKey));
            Assert.AreEqual(2, collection.Count, "count");
        }

        [TestMethod]
        public void Add_1_to_6_OverrideItem()
        {
            const string s1 = "This is a description";
            const string s2 = "This is a second description";
            OptionCollection collection = new OptionCollection("akey");
            collection.Add(1, 3, s1);
            collection.Add(3, 4, s2);

            Assert.AreEqual(s1, collection[1].Description);
            Assert.AreEqual(s1, collection[2].Description);
            Assert.AreEqual(s2, collection[3].Description);
            Assert.AreEqual(s2, collection[4].Description);
            Assert.AreEqual(1, collection.MinimumKey, nameof(collection.MinimumKey));
            Assert.AreEqual(4, collection.MaximumKey, nameof(collection.MaximumKey));
            Assert.AreEqual(2, collection.Count, "count");
        }

        [TestMethod]
        public void Iterate_Empty()
        {
            OptionCollection collection = new OptionCollection("akey");
            Assert.AreEqual(0, collection.Count(), "option count");
        }

        [TestMethod]
        public void Iterate_1_to_6_OneAddedItem()
        {
            OptionCollection collection = new OptionCollection("akey");
            collection.Add(3, 4, "This is a description");
            
            var item = collection.First();
            Assert.AreEqual("This is a description", item.Description);
            Assert.AreEqual(3, item.Range.Start, "item 1 start");
            Assert.AreEqual(4, item.Range.End, "item 1 end");
            Assert.AreEqual(1, collection.Count(), "option count");
        }

        [TestMethod]
        public void Iterate_1_to_6_TwoAddedItems()
        {
            OptionCollection collection = new OptionCollection("akey");
            collection.Add(1, 2, "This is a description");
            collection.Add(3, 4, "This is a second description");
            int i = 0;
            foreach (var item in collection)
            {
                if (i == 0)
                {
                    Assert.AreEqual("This is a description", item.Description);
                    Assert.AreEqual(1, item.Range.Start, "item 1 start");
                    Assert.AreEqual(2, item.Range.End, "item 1 end");
                }
                else if (i == 1)
                {
                    Assert.AreEqual("This is a second description", item.Description);
                    Assert.AreEqual(3, item.Range.Start, "item 2 start");
                    Assert.AreEqual(4, item.Range.End, "item 2 end");
                }
                i++;
            }
            Assert.AreEqual(2, collection.Count(), "option count");
        }

        [TestMethod]
        public void Iterate_1_to_6_TwoAddedItems_AtEnd()
        {
            OptionCollection collection = new OptionCollection("akey");
            collection.Add(1, 3, "This is a description");
            collection.Add(4, 6, "This is a second description");
            int i = 0;
            foreach (var item in collection)
            {
                if (i == 0)
                {
                    Assert.AreEqual("This is a description", item.Description);
                    Assert.AreEqual(1, item.Range.Start, "item 1 start");
                    Assert.AreEqual(3, item.Range.End, "item 1 end");
                }
                else if (i == 1)
                {
                    Assert.AreEqual("This is a second description", item.Description);
                    Assert.AreEqual(4, item.Range.Start, "item 2 start");
                    Assert.AreEqual(6, item.Range.End, "item 2 end");
                }
                i++;
            }
            Assert.AreEqual(2, collection.Count(), "option count");
        }

        [TestMethod]
        public void Iterate_1_to_6_MissingItem()
        {
            OptionCollection collection = new("akey")
            {
                { 1, 2, "This is a description" },
                { 4, 6, "This is a second description" }
            };
            int i = 0;
            try
            {
                foreach (var item in collection)
                {
                    i++;
                }
                Assert.Fail($"Expected {nameof(MissingItemException)} to be thrown.");
            }
            catch (MissingItemException)
            {
            }
        }
    }
}
