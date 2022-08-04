using System.Data.SQLite;
using System.Data;
using System.Diagnostics;

namespace IBAparser
{
    public class DatabaseCreator
    {
        private string urlMain = "https://iba-world.com/category/iba-cocktails/";
        public async Task CreateDBAsync()
        {
            var timerGlobal = new Stopwatch();
            timerGlobal.Start();

            CleanTable();
            var mainListParser = new CocktailParser();
            var listMain =  mainListParser.CocktailListFromURL(urlMain);
            var cocktailsDict = new Dictionary<string, string>();
            FillDict(cocktailsDict, listMain);
            await FillBase(cocktailsDict);  
            
            timerGlobal.Stop();
            Console.WriteLine(String.Format("\nAll cocktails have been parsed in {0}", timerGlobal.Elapsed));
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
                    transaction.Commit();
                }
            }
        }

        private void FillDict(Dictionary<string, string> cocktailsDict, List<HtmlAgilityPack.HtmlNode> cocktailsList)
        {
            var timer = new Stopwatch();
            timer.Start();

            foreach (var cocktail in cocktailsList)
            {
                var name = cocktail.Descendants("h3")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("entry-title")).First().InnerText;
                var urlLink = cocktail.Descendants("a").First().GetAttributeValue("href", "");
                
                CorrectUniciodeChars(ref name);                

                cocktailsDict.Add(name, urlLink);
            }

            timer.Stop();
            Console.WriteLine(String.Format("\rMain page have been parsed in {0} seconds", (double)timer.ElapsedMilliseconds/1000));
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

        private async Task FillBase(Dictionary<string, string>  cocktailsDict)
        {
            var recipes = new List<string>();
            int counter = 0;
            int cocktailsAmmount = cocktailsDict.Count();
            var cockTaskList = new List<Task>();
            var cockList = new List<CocktailParser>();

            var connectionString = new SQLiteConnectionStringBuilder();
            connectionString.DataSource = "./CocktailDB.db";

            foreach (var cocktail in cocktailsDict)
            {
                var newCocktail = new CocktailParser();
                cockTaskList.Add(newCocktail.RecipeByURL(cocktail));
                cockList.Add(newCocktail);
                counter++;
                Console.Write(String.Format("\rIndividual cocktails parsed: \t{0} of {1}", counter, cocktailsAmmount));
            }
            await Task.WhenAll(cockTaskList);

            var taskList = new List<Task>();
                
            foreach (var cocktail in cockList)                
            {                                
                taskList.Add(InsertCoctailToDB(connectionString, cocktail));                
                GC.Collect();
            }
            
            await Task.WhenAll(taskList);
        }

        private async Task InsertCoctailToDB(SQLiteConnectionStringBuilder connectionString, CocktailParser cocktail)
        {
            using (var connection = new SQLiteConnection(connectionString.ConnectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var insertCMD = connection.CreateCommand();
                    insertCMD.CommandText = String.Format(
                        "INSERT INTO CocktailRecipes (Name, Ingridients, Method, Image) VALUES ('{0}', '{1}', '{2}', '{3}')"
                        , cocktail.Name, cocktail.Ingridients, cocktail.Method,  cocktail.ImageBytes);
                    await insertCMD.ExecuteNonQueryAsync();
                    transaction.Commit();
                }
            }
        }
    }
}
