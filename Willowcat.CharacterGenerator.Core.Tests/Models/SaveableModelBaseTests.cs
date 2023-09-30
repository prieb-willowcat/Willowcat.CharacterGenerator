using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.Linq;

namespace Willowcat.CharacterGenerator.Core.Models.Tests
{
    [TestClass()]
    public class SaveableModelBaseTests
    {
        [TestMethod()]
        public void AcceptChangesTest_NoChanges()
        {
            CharacterModel model = new CharacterModel();
            model.AcceptChanges();
            Assert.IsFalse(model.HasChanges());
        }

        [TestMethod()]
        public void AcceptChangesTest_WithListChangesBefore()
        {
            CharacterModel model = new CharacterModel
            {
                Details = new[]
                {
                    new SelectedOption()
                }
            };
            model.AcceptChanges();
            Assert.IsFalse(model.HasChanges());
        }

        [TestMethod()]
        public void AcceptChangesTest_WithStringChangesBefore()
        {
            CharacterModel model = new CharacterModel
            {
                Name = "a name"
            };
            model.AcceptChanges();
            Assert.IsFalse(model.HasChanges());
        }

        [TestMethod()]
        public void AcceptChangesTest_WithStringChangesBeforeAndAfter()
        {
            CharacterModel model = new CharacterModel
            {
                Name = "a name"
            };
            model.AcceptChanges();
            model.Name = "a different name";
            Assert.IsTrue(model.HasChanges());
        }

        [TestMethod()]
        public void AcceptChangesTest_WithIdenticalStringChangesBeforeAndAfter()
        {
            CharacterModel model = new CharacterModel
            {
                Name = "a name"
            };
            model.AcceptChanges();
            model.Name = "a name";
            Assert.IsFalse(model.HasChanges());
        }

        [TestMethod()]
        public void HasChangesTest_NoChanges()
        {
            CharacterModel model = new CharacterModel();
            Assert.IsFalse(model.HasChanges());
        }

        [TestMethod()]
        public void HasChangesTest_WithListChanges()
        {
            CharacterModel model = new CharacterModel
            {
                Details = new[]
                {
                    new SelectedOption()
                }
            };
            Assert.IsTrue(model.HasChanges());
        }

        [TestMethod()]
        public void HasChangesTest_WithListItemAdded()
        {
            CharacterModel model = new CharacterModel
            {
                Details = new[]
                {
                    new SelectedOption()
                }
            };
            model.AcceptChanges();
            if (model.Details is ObservableCollection<SelectedOption> collection)
            {
                collection.Add(new SelectedOption());
                Assert.AreEqual(2, model.Details.Count());
                Assert.IsTrue(model.HasChanges());
            }
            else
            {
                Assert.Fail("not an observable collection");
            }
        }

        [TestMethod()]
        public void HasChangesTest_WithListItemChanged()
        {
            CharacterModel model = new CharacterModel
            {
                Details = new[]
                {
                    new SelectedOption(),
                    new SelectedOption()
                }
            };
            model.AcceptChanges();
            if (model.Details is ObservableCollection<SelectedOption> collection)
            {
                collection[0].Description = "test description";
                Assert.AreEqual(2, model.Details.Count());
                Assert.IsTrue(model.HasChanges());
            }
            else
            {
                Assert.Fail("not an observable collection");
            }
        }

        [TestMethod()]
        public void HasChangesTest_WithListItemRemoved()
        {
            CharacterModel model = new CharacterModel
            {
                Details = new[]
                {
                    new SelectedOption(),
                    new SelectedOption()
                }
            };
            model.AcceptChanges();
            if (model.Details is ObservableCollection<SelectedOption> collection)
            {
                collection.RemoveAt(1);
                Assert.AreEqual(1, model.Details.Count());
                Assert.IsTrue(model.HasChanges());
            }
            else
            {
                Assert.Fail("not an observable collection");
            }
        }

        [TestMethod()]
        public void HasChangesTest_WithStringChanges()
        {
            CharacterModel model = new CharacterModel
            {
                Name = "a name"
            };
            Assert.IsTrue(model.HasChanges());
        }

        [TestMethod()]
        public void HasChangesTest_WithString2Changes()
        {
            CharacterModel model = new CharacterModel
            {
                Notes = "a note"
            };
            Assert.IsTrue(model.HasChanges());
        }
    }
}