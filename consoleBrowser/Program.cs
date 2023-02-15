using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace WebBrowser
{
    internal class Program
    {
        static List<string> HTMLTags = new List<string>() { "h1", "span", "p", "a", "button", "h2" };
        static async Task Main(string[] args)
        {
            Uri res;
            while (true) 
            {
                //Check if you have entered a website correctly fx. "https://www.dr.dk", and tells you if you haven't.
                while (!Uri.TryCreate(Console.ReadLine(), UriKind.Absolute, out res))
                {
                    Console.WriteLine("You have not enteret the website correctly, please try again.");
                    Console.WriteLine();
                }

                HttpClient client = new HttpClient();
                //Takes the website and beautifies it, removing all of the html tags, only leaving plain text.
                try
                {
                    Console.Clear();
                    Console.WriteLine("Website: " + res.Host);
                    Console.WriteLine();
                    client.Timeout = TimeSpan.FromSeconds(10);
                    HttpResponseMessage response = await client.GetAsync(res);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    List<string> HtmlText = new List<string>();

                    foreach (string tag in HTMLTags)
                    {
                        Regex regex = new Regex($"<{tag}" + "[^\\>]{1,}>([^\\<]{1,})");
                        foreach (Match match in regex.Matches(responseBody))
                        {
                            for (int x = 1; x < match.Groups.Count; x++)
                            {
                                string text = match.Groups[x].Value.Replace("\n", "");
                                //print the names wherever there is a succesful match
                                if (!string.IsNullOrEmpty(text))
                                    HtmlText.Add(text);
                            }
                        }
                    }
                    //Makes sure you can read ÆØÅ.
                    foreach (string text in HtmlText) { Console.WriteLine(WebUtility.HtmlDecode(text)); }
                }
                //Checks if the website is alive.
                catch (HttpRequestException e)
                {
                    Console.WriteLine("Could not find the entered website");
                }
            }
        }
    }
}