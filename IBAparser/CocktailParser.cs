using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBAparser
{
    public class CocktailParser
    {
        public string? Ingridients;
        public string? Method;
        public string? Garnish;
        public string? Other;
        public string? Image;

        public string RecipeByURL(string urlRecipe)
        {
            var httpClient = new HttpClient();

            ///Takes html structure from URL and presents it as a string
            var html = httpClient.GetStringAsync(urlRecipe).Result;

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var cocktailsHtml = htmlDocument.DocumentNode.Descendants("div")
                   .Where(node => node.GetAttributeValue("class", "")
                   .Equals("et_pb_module et_pb_post_content et_pb_post_content_0_tb_body blog-post-content")).ToList();

            ///Creates List with individual entries for each cocktail
            var result = cocktailsHtml[0].Descendants("p").ToList();
            Ingridients = result[0].InnerText;
            Method = result[1].InnerText;
            Garnish = result[2].InnerText;
            var other = new List<string>();
            Image = ImgUrl(html);
            try
            {
                for (int i = 3; i < result.Count; i++)
                    other.Add(result[i].InnerText);
            }
            catch (IndexOutOfRangeException)
            { }

            var otherBuilder = new StringBuilder();
            foreach (var entry in other)
                otherBuilder.Append(string.Format("{0}\n", entry));
            Other = otherBuilder.ToString();

            var builder = new StringBuilder();
            var recipe = string.Format("\n\nINGRIDIENTS:\n\n{0}\n\nMETHOD:\n\n{1}\n\nGARNISH:\n\n{2}\n\n", Ingridients, Method, Garnish);
            builder.AppendLine(recipe);            
            builder.AppendLine(Other);
            return builder.ToString();
        }

        public string ImgUrl(string html)
        {
            //var httpClient = new HttpClient();

            /////Takes html structure from URL and presents it as a string
            //var html = httpClient.GetStringAsync(urlRecipe).Result;
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
