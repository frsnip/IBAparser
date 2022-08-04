using HtmlAgilityPack;
using System.Net;

namespace IBAparser
{
    public class CocktailParser
    {
        public string? Name;
        public string? Ingridients;
        public string? Method;
        public string? Garnish;
        public string? Notes;
        public string? Image;
        public byte[]? ImageBytes;

        public async Task RecipeByURL(string urlRecipe)
        {
            var httpClient = new HttpClient();

            ///Takes html structure from URL and presents it as a string
            var html = httpClient.GetStringAsync(urlRecipe);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html.Result);

            var cocktailsHtml = htmlDocument.DocumentNode.Descendants("div")
                   .Where(node => node.GetAttributeValue("class", "")
                   .Equals("et_pb_module et_pb_post_content et_pb_post_content_0_tb_body blog-post-content")).ToList();
            
            var result = cocktailsHtml[0].Descendants("p").ToList();
            Ingridients = result[0].InnerText;
            
            var methodList = new List<string>();

            var splittedRecipe = cocktailsHtml[0].InnerText.Split("METHOD");
            
            Method = splittedRecipe[1].Trim();

            Image = ImgUrl(html.Result);

            using (WebClient client = new WebClient())
            {
                client.DownloadFile(new Uri(Image), String.Format("./RecipeImages/{0}.png", Name));
            }
            var img = new FileInfo((String.Format("./RecipeImages/{0}.png", Name)));
            ImageBytes = File.ReadAllBytes(img.FullName);
        }

        public string ImgUrl(string html)
        {
            var ind = html.IndexOf("og:image");
            var imgLine = html.Substring(ind, 500);
            var split = imgLine.Split("\"");
            return split[2];
        }

        public List<HtmlAgilityPack.HtmlNode> CocktailListFromURL(string url)
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
    }
}
