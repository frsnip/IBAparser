using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBAparser
{
    public class DatabaseCreator
    {
        string urlMain = "https://iba-world.com/category/iba-cocktails/";
        public void CreateDB()
        {
            var mainListParser = new CocktailParser();
            var listMain =  mainListParser.CocktailListFromURL(urlMain);
            var cocktailsDict = new Dictionary<string, string>();
            FillDict(cocktailsDict, listMain);
            FillBase(cocktailsDict);            
        }
        void FillDict(Dictionary<string, string> cocktailsDict, List<HtmlAgilityPack.HtmlNode> cocktailsList)
        {
            foreach (var cocktail in cocktailsList)
            {
                var name = cocktail.Descendants("h3")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("entry-title")).First().InnerText;
                var urlLink = cocktail.Descendants("a").First().GetAttributeValue("href", "");
                //var imgLink = cocktail.Descendants("div")
                //    .Where(node => node.GetAttributeValue("class", "")
                //    .Equals(""));

                if (name.Contains("&#8217;"))
                    name = name.Replace("&#8217;", "'");

                cocktailsDict.Add(name, urlLink);
            }
        }
        public async Task FillBase(Dictionary<string, string>  cocktailsDict)
        {
            var recipes = new List<string>();
            int counter = 0;
            foreach (var cocktail in cocktailsDict)
            {
                var builder = new StringBuilder();
                builder.Append(String.Format("{0}\n{1}\n", cocktail.Key, cocktail.Value));
                var newCocktail = new CocktailParser();
                builder.AppendLine(newCocktail.RecipeByURL(cocktail.Value));
                builder.AppendLine("\n\n**************************************************************************************\n\n");
                recipes.Add(builder.ToString());
                counter++;
                Console.WriteLine(counter);
                GC.Collect();
            }
            await File.AppendAllLinesAsync("RecipeBookNew.txt", recipes);
        }


    }
}
