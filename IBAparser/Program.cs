using HtmlAgilityPack;
using IBAparser;
using System.Text;
using System.Text.RegularExpressions;

string urlRecipe = "https://iba-world.com/alexander/";
var newDBCreator = new DatabaseCreator();
newDBCreator.CreateDB();
//ShowRecipeByURL(urlRecipe);
//Console.WriteLine(RecipeByURL(urlRecipe));

//string RecipeByURL(string urlRecipe)
//{
//    var httpClient = new HttpClient();

//    ///Takes html structure from URL and presents it as a string
//    var html = httpClient.GetStringAsync(urlRecipe).Result;

//    var htmlDocument = new HtmlDocument();
//    htmlDocument.LoadHtml(html);

//    var cocktailsHtml = htmlDocument.DocumentNode.Descendants("div")
//           .Where(node => node.GetAttributeValue("class", "")
//           .Equals("et_pb_module et_pb_post_content et_pb_post_content_0_tb_body blog-post-content")).ToList();

//    ///Creates List with individual entries for each cocktail
//    var result = cocktailsHtml[0].Descendants("p").ToList();
//    string ingridients = result[0].InnerText;
//    string method = result[1].InnerText;
//    string garnish = result[2].InnerText;
//    var other = new List<string>();
//    var imgUrl = ImgUrl(urlRecipe);
//    try
//    {
//        for (int i = 3; i < result.Count; i++)
//            other.Add(result[i].InnerText);
//    }
//    catch (IndexOutOfRangeException)
//    { }
//    var builder = new StringBuilder();
//    var recipe = string.Format("{3}\n\nINGRIDIENTS:\n\n{0}\n\nMETHOD:\n\n{1}\n\nGARNISH:\n\n{2}\n\n", ingridients, method, garnish, imgUrl);
//    builder.AppendLine(recipe);
//    foreach (var entry in other)
//        builder.Append(string.Format("{0}\n", entry));
//    return builder.ToString();
//}

//string ImgUrl(string urlRecipe)
//{
//    var httpClient = new HttpClient();

//    ///Takes html structure from URL and presents it as a string
//    var html = httpClient.GetStringAsync(urlRecipe).Result;
//    var ind = html.IndexOf("og:image");
//    var imgLine = html.Substring(ind, 500);
//    var split = imgLine.Split("\"");
//    return split[2];
//}

//<meta property="og:image"

//var page = htmlDocument.DocumentNode.Descendants("head").ToList();
//var maybe = page[0].Descendants("meta")
//    .Where(node => node.GetAttributeValue("property", "")
//    .Equals("og: image")).First().GetAttributeValue("content", "");


//var regex = new Regex(@"");

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//string urlMain = "https://iba-world.com/category/iba-cocktails/";

//var listMain = CocktailListFromURL(urlMain);
//var cocktailsDict = new Dictionary<string, string>();
//FillDict(cocktailsDict, listMain);

//var recipes = new List<string>();
//int counter=0;


//foreach (var cocktail in cocktailsDict)
//{
//    var builder = new StringBuilder();
//    builder.Append(String.Format("{0}\n{1}\n", cocktail.Key, cocktail.Value));
//    var newCocktail = new CocktailParser();
//    builder.AppendLine(newCocktail.RecipeByURL(cocktail.Value));
//    builder.AppendLine("\n\n**************************************************************************************\n\n");
//    recipes.Add(builder.ToString());
//    counter++;
//    Console.WriteLine(counter);
//    GC.Collect();
//}
//await File.AppendAllLinesAsync("RecipeBook.txt", recipes);

Console.WriteLine();
/// Parses whole html for individual cocktail entries
//List<HtmlAgilityPack.HtmlNode> CocktailListFromURL(string url)
//{
//    var httpClient = new HttpClient();

//    ///Takes html structure from URL and presents it as a string
//    var html = httpClient.GetStringAsync(url).Result;

//    var htmlDocument = new HtmlDocument();
//    htmlDocument.LoadHtml(html);

//    ///Extracts fragment of html with cocktails info
//    var cocktailsHtml = htmlDocument.DocumentNode.Descendants("div")
//        .Where(node => node.GetAttributeValue("class", "")
//        .Equals("et_pb_ajax_pagination_container")).ToList();

//    ///Creates List with individual entries for each cocktail
//    return cocktailsHtml[0].Descendants("article")
//        .Where(node => node.GetAttributeValue("id", "")
//        .Contains("post")).ToList();
//}

/// Parses each coctail entry for cocktail name & url to recipe page and puts info to given Dictionary
//void FillDict(Dictionary<string, string> cocktailsDict, List<HtmlAgilityPack.HtmlNode> cocktailsList)
//{ 
//    foreach (var cocktail in cocktailsList)
//    {
//        var name = cocktail.Descendants("h3")
//            .Where(node => node.GetAttributeValue("class", "")
//            .Equals("entry-title")).First().InnerText;
//        var urlLink = cocktail.Descendants("a").First().GetAttributeValue("href", "");
//        //var imgLink = cocktail.Descendants("div")
//        //    .Where(node => node.GetAttributeValue("class", "")
//        //    .Equals(""));

//        if (name.Contains("&#8217;"))
//            name = name.Replace("&#8217;", "'");

//        cocktailsDict.Add(name, urlLink);
//    }
//}