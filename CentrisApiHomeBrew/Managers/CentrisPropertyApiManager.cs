using CentrisApiHomeBrew.Utils;
using HtmlAgilityPack;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CentrisApiHomeBrew.Managers
{
    public class CentrisPropertyApiManager
    {
        readonly string BaseUrl = "https://www.centris.ca";
        static string Cookies = "";
        readonly string FileName = "SearchPayload.json";
        private static List<string> lstMSLAlreadySent = new List<string>();
        private string lastDate = DateTime.Now.ToString("yyyy-MM-dd");

        public List<Property> GetPropertyBaseOnJson(bool shouldAddTodayDate = false)
        {
            List<Property> lstProperty = new List<Property>();
            //Must try max 2 time if our session is not valid
            for (int i = 0; i < 2; i++)
            {
                QueryResponseDataString.QueryResponseDataString UpdatedQueryString = Post(shouldAddTodayDate);
                if (UpdatedQueryString.d.Succeeded)
                {
                    QueryResponseDataObject.QueryResponseDataObject UpdatedQueryObject = GetNewPage(0);
                    string pageHtml = UpdatedQueryObject.d.Result.html;
                    if (pageHtml != string.Empty)
                    {
                        int pageIncrement = UpdatedQueryObject.d.Result.inscNumberPerPage;
                        int pageIndex = 0;
                        UpdatedQueryObject.d.InitPagerCounter();

                        //Get all property on all pages
                        for (UpdatedQueryObject.d.PagerCounter.Current = 1; UpdatedQueryObject.d.PagerCounter.Current <= UpdatedQueryObject.d.PagerCounter.Last; UpdatedQueryObject.d.PagerCounter.Current++)
                        {
                            lstProperty.AddRange(FindAllPropertyInHTML(shouldAddTodayDate, pageHtml));

                            //Prevent the final call because we already pass every property
                            if (!(UpdatedQueryObject.d.PagerCounter.Current + 1 > UpdatedQueryObject.d.PagerCounter.Last))
                            {
                                pageIndex += pageIncrement;
                                QueryResponseDataObject.QueryResponseDataObject newPage = GetNewPage(pageIndex);

                                if (newPage.d.Succeeded)
                                {
                                    pageHtml = newPage.d.Result.html;
                                }
                            }
                        }
                    }
                    break;
                }
            }

            return lstProperty;
        }

        private QueryResponseDataString.QueryResponseDataString Post(bool shouldAddTodayDate)
        {
            string json = System.IO.File.ReadAllText(FileName);

            if (shouldAddTodayDate)
            {
                if (CheckIfShouldUpdateSavedDate(json))
                {
                    json = AddTodayDate(json);
                    System.IO.File.WriteAllText(FileName, json);
                }
            }

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
            return JsonConvert.DeserializeObject<QueryResponseDataString.QueryResponseDataString>(response.Content);
        }

        private List<Property> FindAllPropertyInHTML(bool shouldCheckInMLSList, string html)
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

                property.MLS = ValidateNode(docc, "//p[@style='z-index: 0; height: 3px; color: white;']");
                //If we have already sent the email, skip everything for this property and go to the next
                if (!shouldCheckInMLSList || (shouldCheckInMLSList && !lstMSLAlreadySent.Contains(property.MLS)))
                {
                    property.Price = ValidateNode(docc, "//meta[@itemprop='price']", true, "content");
                    property.Image = ValidateNode(docc, "//img[@itemprop='image']", true, "src");
                    property.NbBedroom = ValidateNode(docc, "//div[@class='cac']");
                    property.NbBathroom = ValidateNode(docc, "//div[@class='sdb']");
                    property.Link = BaseUrl + ValidateNode(docc, "//a[@class='a-more-detail']", true, "href");

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

                    if (shouldCheckInMLSList)
                    {
                        lstMSLAlreadySent.Add(property.MLS);
                    }
                    lstProperty.Add(property);
                }
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

        private QueryResponseDataObject.QueryResponseDataObject GetNewPage(int pageIndex)
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
            return JsonConvert.DeserializeObject<QueryResponseDataObject.QueryResponseDataObject>(response.Content);
        }

        public int AutomaticEmail()
        {
            List<Property> propertys = GetPropertyBaseOnJson(true);

            if (propertys.Count > 0)
            {
                propertys = propertys.OrderBy(x => x.Price).ToList();
                SendEmail(propertys);
            }

            return propertys.Count;
        }

        private void SendEmail(List<Property> propertys)
        {
            string email = ConfigurationManager.AppSettings.Get("email");
            var smtpClient = new SmtpClient(ConfigurationManager.AppSettings.Get("services"))
            {
                Port = 587,
                Credentials = new NetworkCredential(email, ConfigurationManager.AppSettings.Get("password")),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(email),
                Subject = "New places to look at!",
                Body = FormatBody(propertys),
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            smtpClient.Send(mailMessage);
        }

        private string FormatBody(List<Property> propertys)
        {
            string allProperty = string.Empty;

            foreach (Property property in propertys)
            {
                allProperty += $"<h1>{property.Address}</h1>" + Environment.NewLine +
                $"<h2>{property.Price}$</h2>" +
                "<h3><a href=\"" + property.Link + "\">Link</a></h3>" + Environment.NewLine +
                "<img src=\"" + property.Image + "\" />" + Environment.NewLine;
            }

            return allProperty;
        }

        private string AddTodayDate(string json)
        {
            CentrisSearchPayload centrisSearchPayload = JsonConvert.DeserializeObject<CentrisSearchPayload>(json);

            centrisSearchPayload.query.FieldsValues.Where(x => x.fieldId == "LastModifiedDate").FirstOrDefault().value = DateTime.Now.ToString("yyyy-MM-dd");

            return JsonConvert.SerializeObject(centrisSearchPayload);
        }

        private string GetDateTimeInFile(string json)
        {
            CentrisSearchPayload centrisSearchPayload = JsonConvert.DeserializeObject<CentrisSearchPayload>(json);

            return Convert.ToDateTime(centrisSearchPayload.query.FieldsValues.Where(x => x.fieldId == "LastModifiedDate").FirstOrDefault().value).ToString("yyy-MM-dd");
        }

        //Check if we should clear the MSL list, to keep only the property that were released today
        private bool CheckIfShouldUpdateSavedDate(string json)
        {
            if (Convert.ToDateTime(GetDateTimeInFile(json)) < Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")))
            {
                lastDate = DateTime.Now.ToString("yyyy-MM-dd");
                lstMSLAlreadySent = new List<string>();
                return true;
            }
            return false;
        }
    }
}
