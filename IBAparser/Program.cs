using HtmlAgilityPack;



string urlMain = "https://iba-world.com/category/iba-cocktails/";

var listMain = CocktailListFromURL(urlMain);
var cocktailsDict = new Dictionary<string, string>();
FillDict(cocktailsDict, listMain);

foreach (var cocktail in cocktailsDict)
    Console.WriteLine("{0} \n{1}", cocktail.Key, cocktail.Value);

/// Parses whole html for individual cocktail entries
List<HtmlAgilityPack.HtmlNode> CocktailListFromURL(string url)
{
    var httpClient = new HttpClient();

    ///Takes html structure from URL and presents it as a string
    var html = httpClient.GetStringAsync(url).Result;

    var htmlDocument = new HtmlDocument();
    htmlDocument.LoadHtml(html);

    ///Extracts fragment of html with cocktails info
    var cocktailsHtml = htmlDocument.DocumentNode.Descendants("div")
        .Where(node => node.GetAttributeValue("class", "")
        .Equals("et_pb_ajax_pagination_container")).ToList();

    ///Creates List with individual entries for each cocktail
    return cocktailsHtml[0].Descendants("article")
        .Where(node => node.GetAttributeValue("id", "")
        .Contains("post")).ToList();
}

/// Parses each coctail entry for cocktail name & url to recipe page and puts info to given Dictionary
void FillDict(Dictionary<string, string> cocktailsDict, List<HtmlAgilityPack.HtmlNode> cocktailsList)
{ 
    foreach (var cocktail in cocktailsList)
    {
        var name = cocktail.Descendants("h3")
            .Where(node => node.GetAttributeValue("class", "")
            .Equals("entry-title")).First().InnerText;
        var urlLink = cocktail.Descendants("a").First().GetAttributeValue("href", "");

        if (name.Contains("&#8217;"))
            name = name.Replace("&#8217;", "'");

        cocktailsDict.Add(name, urlLink);
    }
}