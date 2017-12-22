using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Configuration;
using System.Web.Http;
using Newtonsoft.Json;

namespace API.Controllers
{
    public class CalculatorController : ApiController
    {
        [HttpGet]
        [System.Web.Http.Route("Api/Search")]
        public object Search(string str, int start = 0)
        {
            string url = WebConfigurationManager.ConnectionStrings["ConnStringSolr"].ConnectionString + "/select?indent=on&wt=json&fl=title,url,thumbnail,description&rows=16&q=title:";
            WebClient c = new WebClient();
            c.Encoding = Encoding.UTF8;
            string data = "";
            str = str.Trim();
            try
            {
                string[] part;
                string msg = str;
                if (str.Contains(" "))
                {
                    part = str.Split(' ');
                    msg = "(" + part[0];
                    for (int i = 1; i < part.Length; i++)
                        msg += " AND " + part[i];
                    msg += ")";
                }
                data = c.DownloadString(url + msg + " OR content:" + msg + " OR image:" + msg + "&start=" +start+ "&rscore=abs(similar(%22" + str + "%22,content))&sort=$rscore%20desc");
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Error get result from Solr");
            }
            object jsonObject = JsonConvert.DeserializeObject(data);
            return jsonObject;
        }

        [HttpGet]
        [System.Web.Http.Route("Api/Suggest")]
        public object Suggest(string str)
        {
            //http://13.228.252.30:8983/solr/dft/select?q=title:Accessori*&rscore=abs(similar(%22Accessori%22,title))&fl=title,$rscore&wt=json&rows=20&sort=$rscore%20desc
            string url = WebConfigurationManager.ConnectionStrings["ConnStringSolr"].ConnectionString + "/select?indent=on&wt=json&fl=title&rows=10&q=";
            WebClient c = new WebClient();
            c.Encoding = Encoding.UTF8;
            string data = "";
            str = str.Trim();
            try
            {
                string[] part;
                string msg = "title:" + str + "*";
                if (str.Contains(" "))
                {
                    part = str.Split(' ');
                    msg = "title:(" + part[0];
                    for (int i = 1; i < part.Length; i++)
                        msg += " AND " + part[i] ;
                    msg += "*)";
                }
                data = c.DownloadString(url + msg + "&rscore=abs(similar(%22" + str + "%22,title))&sort=$rscore%20desc");
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Error get result from Solr");
            }
            object jsonObject = JsonConvert.DeserializeObject(data);
            return jsonObject;
        }
      
        [HttpGet]
        [System.Web.Http.Route("Api/SearchSpecific")]
        public object Search(string str, int start=0, string type="*", string country="*", string lang="*")
        {
            string url = WebConfigurationManager.ConnectionStrings["ConnStringSolr"].ConnectionString + "/select?indent=on&wt=json&fl=title,url,thumbnail,description&rows=16&q=title:";
            WebClient c = new WebClient();
            c.Encoding = Encoding.UTF8;
            string data = "";
            try
            {
                data = c.DownloadString(url + "\"" + str + "\"" + " OR content:" + "\"" + str + "\"&start=" + start + "&fq=type:" + type + "&fq=country:" + country + "&fq=lang:" + lang);
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Error get result from Solr");
            }
            object jsonObject = JsonConvert.DeserializeObject(data);
            return jsonObject;
        }
    }  
}
