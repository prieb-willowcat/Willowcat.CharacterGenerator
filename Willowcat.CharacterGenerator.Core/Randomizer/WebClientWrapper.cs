using System;
using System.Net;
using System.Text;

namespace Willowcat.CharacterGenerator.Core.Randomizer
{

    public class WebClientWrapper
    {
        public virtual string Download(string url)
        {
            string response = null;
            Console.WriteLine("Downloading from " + url);
            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8;
                response = webClient.DownloadString(url);
            }
            return response;
        }
    }
}
