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
            CleanTable();
            var mainListParser = new CocktailParser();
            var listMain =  mainListParser.CocktailListFromURL(urlMain);
            var cocktailsDict = new Dictionary<string, string>();
            FillDict(cocktailsDict, listMain);
            await FillBase(cocktailsDict);            
        }

        private void CleanTable()
        {
            var connectionString = new SQLiteConnectionStringBuilder();
            connectionString.DataSource = "./CocktailDB.db";

            using (var connection = new SQLiteConnection(connectionString.ConnectionString))
            {

                connection.Open();


                using (var transaction = connection.BeginTransaction())
                {
                    var dropCMD = connection.CreateCommand();

                    dropCMD.CommandText = "DELETE FROM CocktailRecipes";

                    dropCMD.ExecuteNonQuery();

                    //dropCMD.CommandText = @"CREATE TABLE 'CocktailRecipes' 
                    //                        ( 
                    //                            'ID' INTEGER NOT NULL UNIQUE, 
                    //                            'Name'  TEXT NOT NULL UNIQUE,
                    //                            'Image' BLOB,
	                   //                         'Ingridients'   TEXT,
	                   //                         'Method'    TEXT,
	                   //                         'Garnish'   TEXT,
	                   //                         'Notes' TEXT,
	                   //                         PRIMARY KEY('ID' AUTOINCREMENT)
                    //                        )";
                    //dropCMD.ExecuteNonQuery();

                    transaction.Commit();
                }

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

            var cockTaskList = new List<Task>();
            var cockList = new List<CocktailParser>();
            foreach (var cocktail in cocktailsDict)
            {

                var newCocktail = new CocktailParser();
                newCocktail.Name = cocktail.Key;
                cockTaskList.Add(newCocktail.RecipeByURL(cocktail.Value));
                cockList.Add(newCocktail);


            }
            await Task.WhenAll(cockTaskList);

            var taskList = new List<Task>();

                
            foreach (var cocktail in cockList)
                
            {
                                
                taskList.Add(InsertCoctailToDB(connectionString, cocktail));

                counter++;
                Console.WriteLine(counter);
                GC.Collect();

            }
            //await File.AppendAllLinesAsync("RecipeBookNew.txt", recipes);
            await Task.WhenAll(taskList);
        }

        private async Task InsertCoctailToDB(SQLiteConnectionStringBuilder connectionString, CocktailParser cocktail)
        {
            //var name = cocktail.Key;


            //var newCocktail = new CocktailParser();
            //await newCocktail.RecipeByURL(cocktail.Value);
            //using (WebClient client = new WebClient())
            //{
            //    client.DownloadFile(new Uri(newCocktail.Image), String.Format("./RecipeImages/{0}.png", name));
            //}
            //var img = new FileInfo((String.Format("./RecipeImages/{0}.png", name)));
            //newCocktail.ImageBytes = File.ReadAllBytes(img.FullName);


            using (var connection = new SQLiteConnection(connectionString.ConnectionString))
            {

                connection.Open();


                using (var transaction = connection.BeginTransaction())
                {
                    var insertCMD = connection.CreateCommand();

                    insertCMD.CommandText = String.Format(
                        "INSERT INTO CocktailRecipes (Name, Ingridients, Method, Garnish, Notes, Image) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')"
                        , cocktail.Name, cocktail.Ingridients, cocktail.Method, cocktail.Garnish, cocktail.Notes, cocktail.ImageBytes);

                    await insertCMD.ExecuteNonQueryAsync();

                    transaction.Commit();
                }

            }

        }
    }
}
