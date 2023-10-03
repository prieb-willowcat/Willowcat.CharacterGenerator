using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using Unidecode.NET;
using Willowcat.CharacterGenerator.Application.Interface;
using Willowcat.CharacterGenerator.OnlineGenerators.Generator;

namespace Willowcat.CharacterGenerator.Core.Tests.IntegrationTests
{
    [TestClass]
    public class RandomAPITests
    {
        private readonly string _SampleJson = "[{\"name\":\"Κτησιφών\",\"surname\":\"Γεωργιάδης\",\"gender\":\"male\",\"region\":\"Greece\"},{\"name\":\"Εύμηλος\",\"surname\":\"Κορωναίος\",\"gender\":\"male\",\"region\":\"Greece\"},{\"name\":\"Κλεομήδης\",\"surname\":\"Μαγγίνας\",\"gender\":\"male\",\"region\":\"Greece\"},{\"name\":\"’Ευα\",\"surname\":\"Αντωνοπούλου\",\"gender\":\"female\",\"region\":\"Greece\"},{\"name\":\"Λεωνίδας\",\"surname\":\"Βαρουξής\",\"gender\":\"male\",\"region\":\"Greece\"},{\"name\":\"Σωτήριος\",\"surname\":\"Λαγός\",\"gender\":\"male\",\"region\":\"Greece\"},{\"name\":\"Διόδοτος\",\"surname\":\"Αλεξόπουλος\",\"gender\":\"male\",\"region\":\"Greece\"},{\"name\":\"Αρτέμης\",\"surname\":\"Ελευθερίου\",\"gender\":\"male\",\"region\":\"Greece\"},{\"name\":\"Ανδρέας\",\"surname\":\"Κούνδουρος\",\"gender\":\"male\",\"region\":\"Greece\"},{\"name\":\"Κλείταρχος\",\"surname\":\"Δεσποτόπουλος\",\"gender\":\"male\",\"region\":\"Greece\"},{\"name\":\"Ασκάλαφος\",\"surname\":\"Κουβέλης\",\"gender\":\"male\",\"region\":\"Greece\"},{\"name\":\"Ιαπετός\",\"surname\":\"Μπλέτσας\",\"gender\":\"male\",\"region\":\"Greece\"},{\"name\":\"Ερμής\",\"surname\":\"Πρωτονοτάριος\",\"gender\":\"male\",\"region\":\"Greece\"},{\"name\":\"Ηγήσιππος\",\"surname\":\"Βασιλικός\",\"gender\":\"male\",\"region\":\"Greece\"},{\"name\":\"Ιπποκράτης\",\"surname\":\"Βουγιουκλάκης\",\"gender\":\"male\",\"region\":\"Greece\"},{\"name\":\"Καλλίνικος\",\"surname\":\"Δράκος\",\"gender\":\"male\",\"region\":\"Greece\"},{\"name\":\"Βελλεροφόντης\",\"surname\":\"Γιαννακόπουλος\",\"gender\":\"male\",\"region\":\"Greece\"},{\"name\":\"Εύδοξος\",\"surname\":\"Ζωγράφος\",\"gender\":\"male\",\"region\":\"Greece\"},{\"name\":\"Εύμολπος\",\"surname\":\"Ζάχος\",\"gender\":\"male\",\"region\":\"Greece\"},{\"name\":\"Ηριδανός\",\"surname\":\"Κοσμόπουλος\",\"gender\":\"male\",\"region\":\"Greece\"},{\"name\":\"Ηγήσιππος\",\"surname\":\"Γιάγκος\",\"gender\":\"male\",\"region\":\"Greece\"},{\"name\":\"Γεώργιος\",\"surname\":\"Αγγελίδου\",\"gender\":\"male\",\"region\":\"Greece\"},{\"name\":\"Μόψος\",\"surname\":\"Κρεστενίτης\",\"gender\":\"male\",\"region\":\"Greece\"},{\"name\":\"Σοφία\",\"surname\":\"Βέργας\",\"gender\":\"female\",\"region\":\"Greece\"},{\"name\":\"Ευρυμέδων\",\"surname\":\"Κασιδιάρης\",\"gender\":\"male\",\"region\":\"Greece\"}]";

        [TestMethod]
        public void RandomUiName()
        {
            var mockWebClient = new Mock<IHttpJsonClient>();
            mockWebClient.Setup(client => client.DownloadJson(It.IsAny<string>())).Returns(_SampleJson);

            RandomUiNames randomNames = new RandomUiNames(mockWebClient.Object);

            string value = randomNames.NextHumanName();
            Console.WriteLine("Random Name: " + value.Unidecode());
            Assert.IsTrue(!string.IsNullOrEmpty(value));
        }

        [TestMethod]
        public void RandomUiNames()
        {
            var mockWebClient = new Mock<IHttpJsonClient>();
            mockWebClient.Setup(client => client.DownloadJson(It.IsAny<string>())).Returns(_SampleJson);

            RandomUiNames randomNames = new RandomUiNames(mockWebClient.Object);

            var value = randomNames.NextHumanNames(10);
            Assert.AreEqual(10, value.Count);
            foreach (var name in value)
            {
                Console.WriteLine("Random Name: " + name.Unidecode());
            }
        }

        //[TestMethod]
        //[Ignore]
        //public void RandomBehindTheName_SimpleTest()
        //{
        //    RandomBehindTheName randomNames = new RandomBehindTheName(Gender.Female, 10);

        //    string value = randomNames.GetNextName();
        //    Assert.IsTrue(!string.IsNullOrEmpty(value));
        //    Console.WriteLine("Random Name: " + value.Unidecode());
        //}

        //[TestMethod]
        //[Ignore]
        //public void RandomBehindTheName_RegionTest()
        //{
        //    RandomBehindTheName randomNames = new RandomBehindTheName(Gender.Female, 10);

        //    string value = randomNames.GetNextName(RandomBehindTheName.Regions.Keys.First());
        //    Assert.IsTrue(!string.IsNullOrEmpty(value));
        //    Console.WriteLine("Random Name: " + value.Unidecode());
        //}
    }
}
