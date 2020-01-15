using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace lyricsTrainer
{
    class Program
    {
        static void Main(string[] args)
        {
            bool wantsToPlay = true;
            do
            {
                char playAgain;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Test your knowledge on the lyrics of your favorite song...");
                Console.ResetColor();
                String name, author;
                bool working = false;
                do
                {
                    Console.Write("Song Name: ");
                    name = Console.ReadLine().Replace("\'", "").Replace(".", "");
                    Console.Write("Author: ");
                    author = Console.ReadLine().Replace("\'", "").Replace(".", "");
                    try
                    {
                        startGame(name, author);
                        working = true;
                    }
                    catch
                    {
                        Console.WriteLine("Invalid name or author, or Genius doesn't have the song");
                        working = false;
                    }
                } while (!working);
                Console.WriteLine("Play Again(y/n)? ");
                playAgain = Console.ReadKey().KeyChar;
                if(!(playAgain.ToString().ToLowerInvariant().CompareTo("y") == 0))
                {
                    wantsToPlay = false;
                }
                Console.ReadLine();
                Console.Clear();
            } while (wantsToPlay);
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("okay bye.");
            Console.ResetColor();
            Console.ReadLine();
        }
        static String[] scrapeLyrics(string author, string song)
        {
            List<string> lyrics = new List<string>();
            string url = "https://genius.com/" + (author.ToLowerInvariant() + "-" + song.ToLowerInvariant()).Replace(" ", "-") + "-lyrics";
            HttpClient client = new HttpClient();
            var html = client.GetStringAsync(url);
            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(html.Result);
            var lyricsList = htmlDocument.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Contains("lyrics")).ToList();
            var descendants = lyricsList[0].Descendants("p").ToList();
            return descendants[0].InnerText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        }
        static void startGame(string name, string author)
        {
            int points = 0;
            List<string> lyrics = scrapeLyrics(author, name).ToList();
            for(int lyricLine = 0; lyricLine < lyrics.Count; lyricLine++)
            {
                lyrics[lyricLine] = lyrics[lyricLine].Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "").Replace(",", "");
                if(lyrics[lyricLine].ToLowerInvariant().Contains("verse") || lyrics[lyricLine].ToLowerInvariant().Contains("corus") || lyrics[lyricLine].ToLowerInvariant().Contains("intro") || lyrics[lyricLine].ToLowerInvariant().Contains("outro") || lyrics[lyricLine].ToLowerInvariant().Contains("refrain"))
                {
                    lyrics.RemoveAt(lyricLine);
                }
            }
            foreach(string lyric in lyrics)
            {
                Console.Clear();
                string lyricWithMissingWord = "";
                string missingWord;
                string response;
                Random rnd = new Random();
                int tryNum = 0;
                string[] words = lyric.Split(' ');
                if (words.Length != 1)
                {
                    int index = rnd.Next(0, words.Length);
                    bool hasItCorrect = false;
                    missingWord = words[index].ToLowerInvariant().Replace(",","").Replace("'","").Replace("(","").Replace(")","").Replace("[","").Replace("]","");
                    words[index] = "";
                    for(int p = 0; p < missingWord.Length; p++)
                    {
                        words[index] += "?";
                    }
                    foreach (string word in words)
                    {
                        lyricWithMissingWord += word + " ";
                    }
                    Console.WriteLine(lyricWithMissingWord);
                    do
                    {
                        Console.Write("What is the missing part of the lyric? ");
                        response = Console.ReadLine();
                        if (response.ToLowerInvariant().CompareTo(missingWord) == 0)
                        {
                            Console.Write("Congratulations! You got it right! You earned ");
                            hasItCorrect = true;
                        }
                        else
                        {
                            Console.WriteLine("You got it wrong, here's a hint: the first letter is " + missingWord[0] + ".");
                            tryNum++;
                        }
                    } while (!hasItCorrect);
                    switch (tryNum)
                    {
                        case 0:
                            Console.WriteLine("You got 50 points!");
                            points += 50;
                            break;
                        case 1:
                            Console.WriteLine("You got 25 points!");
                            points += 25;
                            break;
                        case 2:
                            Console.WriteLine("You got 15 points!");
                            points += 15;
                            break;
                        case 3:
                            Console.WriteLine("You got 5 points!");
                            points += 5;
                            break;
                        default:
                            Console.WriteLine("You got 1 point.");
                            points += 1;
                            break;
                    }
                }
                System.Threading.Thread.Sleep(1000);
            }
            Console.WriteLine("Good Job! You ended up with " + points + " points");
        }
    }
}