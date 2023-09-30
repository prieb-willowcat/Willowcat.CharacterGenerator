using Willowcat.CharacterGenerator.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Willowcat.CharacterGenerator.Core.Tests
{
    //[TestClass]
    //TODO: public class StringFormatterTests
    //{
    //    private Dictionary<string, ChartModel> _Charts = new Dictionary<string, ChartModel>
    //    {
    //        ["link348"] = new ChartModel
    //        {
    //            ChartName = "Personality"
    //        },
    //        ["link600"] = new ChartModel
    //        {
    //            ChartName = "Businesss"
    //        }
    //    };

    //    [TestMethod]
    //    public void AddLocalLinksToChartKeys_Simple()
    //    {
    //        string link = "link348";
    //        string text = "Personality";
    //        string input = $"This is a test [#{link}] of the system";
    //        string expected = $"{{\\rtf1\\ansi This is a test {{\\field{{\\*\\fldinst HYPERLINK \"#{link}\"}}{{\\fldrslt {text}}}}} of the system}}";

    //        Assert.AreEqual(expected, RichTextStringFormatters.AddLocalLinksToChartKeys(_Charts, input));
    //    }

    //    [TestMethod]
    //    public void AddLocalLinksToChartKeys_Two()
    //    {
    //        string link = "link348";
    //        string text = "Personality";
    //        string link2 = "link600";
    //        string text2 = "Businesss";
    //        string input = $"This is a test [#{link}] of the system [#{link2}] TEST";
    //        string expected = $"{{\\rtf1\\ansi This is a test {{\\field{{\\*\\fldinst HYPERLINK \"#{link}\"}}{{\\fldrslt {text}}}}} of the system {{\\field{{\\*\\fldinst HYPERLINK \"#{link2}\"}}{{\\fldrslt {text2}}}}} TEST}}";

    //        Assert.AreEqual(expected, RichTextStringFormatters.AddLocalLinksToChartKeys(_Charts, input));
    //    }

    //    [TestMethod]
    //    public void AddLocalLinksToChartKeys_Two_NoMatch()
    //    {
    //        string link = "link348";
    //        string text = "Personality";
    //        string link2 = "link601";
    //        string input = $"This is a test [#{link}] of the system [#{link2}] TEST";
    //        string expected = $"{{\\rtf1\\ansi This is a test {{\\field{{\\*\\fldinst HYPERLINK \"#{link}\"}}{{\\fldrslt {text}}}}} of the system {{\\field{{\\*\\fldinst HYPERLINK \"#{link2}\"}}{{\\fldrslt {link2}}}}} TEST}}";

    //        Assert.AreEqual(expected, RichTextStringFormatters.AddLocalLinksToChartKeys(_Charts, input));
    //    }
    //}
}
