using CentrisApiHomeBrew.Utils;
using HtmlAgilityPack;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CentrisApiHomeBrew.Managers
{
    public class CentrisPropertyApiManager
    {
        readonly string BaseUrl = "https://www.centris.ca";
        static string Cookies = "";
        readonly string FileName = "laval.json";

        public List<Property> GetPropertyBaseOnJson()
        {
            List<Property> lstProperty = new List<Property>();
            //Must try max 2 time if our session is not valid
            for (int i = 0; i < 2; i++)
            {
                QueryResponseDataString UpdatedQueryString = Post();
                if (UpdatedQueryString.d.Succeeded)
                {
                    //Get(UpdatedQuery.d.Result);
                    QueryResponseDataObject UpdatedQueryObject = GetNewPage(0);
                    int pageIncrement = UpdatedQueryObject.d.Result.inscNumberPerPage;
                    int pageIndex = 0;
                    string pageHtml = UpdatedQueryObject.d.Result.html;
                    UpdatedQueryObject.d.InitPagerCounter();
                    for (UpdatedQueryObject.d.PagerCounter.Current = 1; UpdatedQueryObject.d.PagerCounter.Current <= UpdatedQueryObject.d.PagerCounter.Last; UpdatedQueryObject.d.PagerCounter.Current++)
                    {
                        lstProperty.AddRange(FindAllPropertyInHTML(pageHtml));

                        //Prevent the final call because we already pass every property
                        if (!(UpdatedQueryObject.d.PagerCounter.Current + 1 > UpdatedQueryObject.d.PagerCounter.Last))
                        {
                            pageIndex += pageIncrement;
                            QueryResponseDataObject newPage = GetNewPage(pageIndex);

                            if (newPage.d.Succeeded)
                            {
                                pageHtml = newPage.d.Result.html;
                            }
                        }
                    }
                    break;
                }
            }

            return lstProperty;
        }

        private QueryResponseDataString Post()
        {
            string json = System.IO.File.ReadAllText(FileName);
            var client = new RestClient($"{BaseUrl}/property/UpdateQuery");
            client.Timeout = -1;
            client.CookieContainer = new CookieContainer();
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Cookie", Cookies);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode.ToString() == "555")
            {
                Cookies = client.CookieContainer.GetCookieHeader(new Uri("https://www.centris.ca/property/UpdateQuery"));
            }
            return JsonConvert.DeserializeObject<QueryResponseDataString>(response.Content);
        }

        private List<Property> FindAllPropertyInHTML(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            List<Property> lstProperty = new List<Property>();

            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div[@class='shell']"))
            {
                Property property = new Property();
                string infoProperty = node.OuterHtml;
                HtmlDocument docc = new HtmlDocument();
                docc.LoadHtml(infoProperty);

                property.Price = ValidateNode(doc, "//span[@itemprop='price']");
                property.MLS = ValidateNode(doc, "//p[@style='z-index: 0; height: 3px; color: white;']");
                property.Image = ValidateNode(doc, "//img[@itemprop='image']", true, "src");
                property.NbBedroom = ValidateNode(doc, "//div[@class='cac']");
                property.NbBathroom = ValidateNode(doc, "//div[@class='sdb']");
                property.Link = ValidateNode(doc, "//a[@class='a-more-detail']", true, "href");

                try
                {
                    //Crash if not found
                    docc.DocumentNode.SelectNodes("//div[@class='banner new-property']").FirstOrDefault();
                    property.IsNewProperty = true;
                }
                catch
                {
                    property.IsNewProperty = false;
                }

                var addressNode = docc.DocumentNode.SelectNodes("//span[@class='address']").FirstOrDefault();
                infoProperty = addressNode.OuterHtml;
                docc = new HtmlDocument();
                docc.LoadHtml(infoProperty);

                string address = "";

                var addressContentNode = docc­.DocumentNode.ChildNodes.FirstOrDefault();
                address += addressContentNode.FirstChild.NextSibling.InnerHtml + " ";
                address += addressContentNode.FirstChild.NextSibling.NextSibling.NextSibling.InnerHtml;

                property.Address = StringManipulation.ReplaceEncodingString(address);

                lstProperty.Add(property);
            }

            return lstProperty;
        }

        private string ValidateNode(HtmlDocument doc, string queryNode, bool shouldGetAttribute = false, string attribute = "")
        {
            try
            {
                var node = doc.DocumentNode.SelectNodes(queryNode).FirstOrDefault();
                if (node != null)
                {
                    if (shouldGetAttribute)
                    {
                       return StringManipulation.ReplaceEncodingString(node.GetAttributeValue(attribute, ""));
                    }
                    else
                    {
                        return StringManipulation.ReplaceEncodingString(node.InnerHtml);
                    }
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        private QueryResponseDataObject GetNewPage(int pageIndex)
        {
            string json = "{\"startPosition\": TOREPLACE}";
            json = json.Replace("TOREPLACE", pageIndex.ToString());
            var client = new RestClient($"{BaseUrl}/Property/GetInscriptions");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Cookie", Cookies);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return JsonConvert.DeserializeObject<QueryResponseDataObject>(response.Content);
        }
    }
}
