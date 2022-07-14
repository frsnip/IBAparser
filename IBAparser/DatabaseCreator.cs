using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;

namespace IBAparser
{
    public class DatabaseCreator
    {
        string urlMain = "https://iba-world.com/category/iba-cocktails/";
        public async Task CreateDBAsync()
        {
            //DeleteDB();
            var mainListParser = new CocktailParser();
            var listMain =  mainListParser.CocktailListFromURL(urlMain);
            var cocktailsDict = new Dictionary<string, string>();
            FillDict(cocktailsDict, listMain);
            await FillBase(cocktailsDict);            
        }

        private void DeleteDB()
        {
            if (File.Exists("./CocktailDB.db"))
            {
                File.Delete("./CocktailDB.db");
            }

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
                CorrectUniciodeChars(ref name);
                

                cocktailsDict.Add(name, urlLink);
            }
            Console.WriteLine("Main page have been parsed");
        }

        public void InsertIMGToDB()
        {

        }

        private void CorrectUniciodeChars(ref string name)
        {
            if (name.Contains("&#8217;"))
                name = name.Replace("&#8217;", "’");
            if (name.Contains("&#8216;"))
                name = name.Replace("&#8216;", "‘");
        }

        public async Task FillBase(Dictionary<string, string>  cocktailsDict)
        {
            var recipes = new List<string>();
            int counter = 0;

            var connectionString = new SQLiteConnectionStringBuilder();
            connectionString.DataSource = "./CocktailDB.db";

            var cockList = new List<Task>();
            //foreach (var cocktail in cocktailsDict)
            //{

            //    var newCocktail = new CocktailParser();
            //    newCocktail.RecipeByURL(cocktail.Value));

            //}

            var taskList = new List<Task>();

                
            foreach (var cocktail in cocktailsDict)
                
            {
                                
                taskList.Add(InsertCoctailToDB(connectionString, cocktail));

                counter++;
                Console.WriteLine(counter);
                GC.Collect();

            }
            //await File.AppendAllLinesAsync("RecipeBookNew.txt", recipes);
            await Task.WhenAll(taskList);
        }

        private async Task InsertCoctailToDB(SQLiteConnectionStringBuilder connectionString, KeyValuePair<string, string> cocktail)
        {
            var name = cocktail.Key;


            var newCocktail = new CocktailParser();
            newCocktail.RecipeByURL(cocktail.Value);
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(new Uri(newCocktail.Image), String.Format("./RecipeImages/{0}.png", name));
            }
            var img = new FileInfo((String.Format("./RecipeImages/{0}.png", name)));
            newCocktail.ImageBytes = File.ReadAllBytes(img.FullName);


            using (var connection = new SQLiteConnection(connectionString.ConnectionString))
            {

                connection.Open();


                using (var transaction = connection.BeginTransaction())
                {
                    var insertCMD = connection.CreateCommand();

                    insertCMD.CommandText = String.Format(
                        "INSERT INTO CocktailRecipes (Name, Ingridients, Method, Garnish, Notes, Image) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')"
                        , name, newCocktail.Ingridients, newCocktail.Method, newCocktail.Garnish, newCocktail.Notes, newCocktail.ImageBytes);

                    await insertCMD.ExecuteNonQueryAsync();

                    transaction.Commit();
                }

            }

        }
    }
}
