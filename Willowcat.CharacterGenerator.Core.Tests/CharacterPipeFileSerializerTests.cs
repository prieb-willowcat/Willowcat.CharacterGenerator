using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using Willowcat.CharacterGenerator.Application.Interface;
using Willowcat.CharacterGenerator.FlatFile.TextRepository;
using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.Core.Tests
{
    [TestClass]
    public class CharacterPipeFileSerializerTests
    {
        private readonly CharacterPipeFileSerializer _Serializer = new CharacterPipeFileSerializer();
        private IChartRepository _ChartRepository = null;

        public void AssertEquals(CharacterModel expected, CharacterModel actual)
        {
            Assert.AreEqual(expected.Name, actual.Name, nameof(expected.Name));
            Assert.AreEqual(expected.Notes, actual.Notes, nameof(expected.Notes));

            var expectedDetails = expected.Details.GetEnumerator();
            var actualDetails = actual.Details.GetEnumerator();
            int expectedCount = 0;
            int actualCount = 0;
            while (expectedDetails.MoveNext())
            {
                expectedCount++;
                if (actualDetails.MoveNext())
                {
                    actualCount++;
                    var expectedDetail = expectedDetails.Current;
                    var actualDetail = actualDetails.Current;

                    Assert.AreEqual(expectedDetail.ChartKey, actualDetail.ChartKey, nameof(expectedDetail.ChartKey));
                    Assert.AreEqual(expectedDetail.ChartName, actualDetail.ChartName, nameof(expectedDetail.ChartName));
                    Assert.AreEqual(expectedDetail.ParentChartKey, actualDetail.ParentChartKey, nameof(expectedDetail.ParentChartKey));
                    Assert.AreEqual(expectedDetail.Description, actualDetail.Description, nameof(expectedDetail.Description));
                    Assert.AreEqual(expectedDetail.Range, actualDetail.Range, nameof(expectedDetail.Range));
                }
            }
            while (actualDetails.MoveNext())
            {
                actualCount++;
            }
            if (actualCount != expectedCount)
            {
                Assert.Fail($"Only {actualCount} actual items found. {expectedCount} were expected");
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            var mock = new Mock<IChartRepository>();

            mock.Setup(repo => repo.GetChart("xyz")).Returns(new ChartModel()
            {
                Key = "xyz",
                ChartName = "The XYZ Chart"
            });

            mock.Setup(repo => repo.GetChart("abc")).Returns(new ChartModel()
            {
                Key = "abc",
                ChartName = "The ANC Chart"
            });

            _ChartRepository = mock.Object;
        }

        [TestMethod]
        public void DeserializeTest_NegativeRange()
        {
            CharacterModel character = new CharacterModel
            {
                Name = "Keith",
                Notes = "One line of notes",
                Details = new List<SelectedOption>
                {
                    new SelectedOption()
                    {
                        ChartKey = "xyz",
                        Description = "This is the best description",
                        ChartName = "The XYZ Chart",
                        Range = new DiceRange(-1,2)
                    }
                }
            };
            string input = @"#Name=Keith
#Notes=One line of notes

#Option=xyz||-1-2|This is the best description
";

            var actual = _Serializer.Deserialize(_ChartRepository, input);
            AssertEquals(character, actual);
        }

        [TestMethod]
        public void DeserializeTest_NoNotes()
        {
            CharacterModel character = new CharacterModel
            {
                Name = "Keith",
                Notes = string.Empty,
                Details = new List<SelectedOption>
                {
                    new SelectedOption()
                    {
                        ChartKey = "xyz",
                        Description = "This is the best description",
                        ChartName = "The XYZ Chart",
                        Range = new DiceRange(1,2)
                    },
                    new SelectedOption()
                    {
                        ChartKey = "abc",
                        Description = "This is the best description",
                        ChartName = "The ANC Chart",
                        Range = new DiceRange(3,4)
                    }
                }
            };
            string input = @"#Name=Keith
#Notes=

#Option=xyz||1-2|This is the best description
#Option=abc||3-4|This is the best description
";

            var actual = _Serializer.Deserialize(_ChartRepository, input);
            AssertEquals(character, actual);
        }

        [TestMethod]
        public void DeserializeTest_MultiLineNote()
        {
            CharacterModel character = new CharacterModel
            {
                Name = "Keith",
                Notes = "One line of notes\r\nAnother line",
                Details = new List<SelectedOption>
                {
                    new SelectedOption()
                    {
                        ChartKey = "xyz",
                        Description = "This is the best description",
                        ChartName = "The XYZ Chart",
                        Range = new DiceRange(1,2)
                    },
                    new SelectedOption()
                    {
                        ChartKey = "abc",
                        Description = "This is the best description",
                        ChartName = "The ANC Chart",
                        Range = new DiceRange(3,4)
                    }
                }
            };
            string input = @"#Name=Keith
#Notes=One line of notes
Another line

#Option=xyz||1-2|This is the best description
#Option=abc||3-4|This is the best description
";

            var actual = _Serializer.Deserialize(_ChartRepository, input);
            AssertEquals(character, actual);
        }

        [TestMethod]
        public void DeserializeTest_ParentOption()
        {
            CharacterModel character = new CharacterModel
            {
                Name = "Keith",
                Notes = string.Empty,
                Details = new List<SelectedOption>
                {
                    new SelectedOption()
                    {
                        ChartKey = "xyz",
                        Description = "This is the best description",
                        ChartName = "The XYZ Chart",
                        Range = new DiceRange(1,2)
                    },
                    new SelectedOption()
                    {
                        ChartKey = "abc",
                        Description = "This is the best description",
                        ChartName = "The ANC Chart",
                        Range = new DiceRange(-3,-4),
                        ParentChartKey = "xyz"
                    }
                }
            };
            string input = @"#Name=Keith
#Notes=

#Option=xyz||1-2|This is the best description
#Option=abc|xyz|-3--4|This is the best description
";

            var actual = _Serializer.Deserialize(_ChartRepository, input);
            AssertEquals(character, actual);
        }

        [TestMethod]
        public void DeserializeTest_SingleLineNote()
        {
            CharacterModel character = new CharacterModel
            {
                Name = "Keith",
                Notes = "One line of notes",
                Details = new List<SelectedOption>
                {
                    new SelectedOption()
                    {
                        ChartKey = "xyz",
                        Description = "This is the best description",
                        ChartName = "The XYZ Chart",
                        Range = new DiceRange(1,2)
                    }
                }
            };
            string input = @"#Name=Keith
#Notes=One line of notes

#Option=xyz||1-2|This is the best description
";

            var actual = _Serializer.Deserialize(_ChartRepository, input);
            AssertEquals(character, actual);
        }

        [TestMethod]
        public void SerializeTest_NegativeRange()
        {
            CharacterModel character = new CharacterModel
            {
                Name = "Keith",
                Notes = "One line of notes",
                Details = new List<SelectedOption>
                {
                    new SelectedOption()
                    {
                        ChartKey = "xyz",
                        Description = "This is the best description",
                        ChartName = "The XYZ Chart",
                        Range = new DiceRange(-1,2)
                    }
                }
            };
            string expected = @"#Name=Keith
#Notes=One line of notes

#Option=xyz||-1-2|This is the best description
";

            string actual = _Serializer.Serialize(character);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SerializeTest_MultiLineNote()
        {
            CharacterModel character = new CharacterModel
            {
                Name = "Keith",
                Notes = "One line of notes\r\nAnother line",
                Details = new List<SelectedOption>
                {
                    new SelectedOption()
                    {
                        ChartKey = "xyz",
                        Description = "This is the best description",
                        ChartName = "The XYZ Chart",
                        Range = new DiceRange(1,2)
                    },
                    new SelectedOption()
                    {
                        ChartKey = "abc",
                        Description = "This is the best description",
                        ChartName = "The ANC Chart",
                        Range = new DiceRange(3,4)
                    }
                }
            };
            string expected = @"#Name=Keith
#Notes=One line of notes
Another line

#Option=xyz||1-2|This is the best description
#Option=abc||3-4|This is the best description
";

            string actual = _Serializer.Serialize(character);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SerializeTest_NoNotes()
        {
            CharacterModel character = new CharacterModel
            {
                Name = "Keith",
                Details = new List<SelectedOption>
                {
                    new SelectedOption()
                    {
                        ChartKey = "xyz",
                        Description = "This is the best description",
                        ChartName = "The XYZ Chart",
                        Range = new DiceRange(1,2)
                    }
                }
            };
            string expected = @"#Name=Keith
#Notes=

#Option=xyz||1-2|This is the best description
";

            string actual = _Serializer.Serialize(character);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SerializeTest_ParentOption()
        {
            CharacterModel character = new CharacterModel
            {
                Name = "Keith",
                Details = new List<SelectedOption>
                {
                    new SelectedOption()
                    {
                        ChartKey = "xyz",
                        Description = "This is the best description",
                        ChartName = "The XYZ Chart",
                        Range = new DiceRange(1,2)
                    },
                    new SelectedOption()
                    {
                        ChartKey = "abc",
                        Description = "This is the best description",
                        ChartName = "The ANC Chart",
                        Range = new DiceRange(-3,-4),
                        ParentChartKey = "xyz"
                    }
                }
            };
            string expected = @"#Name=Keith
#Notes=

#Option=xyz||1-2|This is the best description
#Option=abc|xyz|-4--3|This is the best description
";

            string actual = _Serializer.Serialize(character);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SerializeTest_SingleLineNote()
        {
            CharacterModel character = new CharacterModel
            {
                Name = "Keith",
                Notes = "One line of notes",
                Details = new List<SelectedOption>
                {
                    new SelectedOption()
                    {
                        ChartKey = "xyz",
                        Description = "This is the best description",
                        ChartName = "The XYZ Chart",
                        Range = new DiceRange(1,2)
                    }
                }
            };
            string expected = @"#Name=Keith
#Notes=One line of notes

#Option=xyz||1-2|This is the best description
";

            string actual = _Serializer.Serialize(character);
            Assert.AreEqual(expected, actual);
        }
    }
}
