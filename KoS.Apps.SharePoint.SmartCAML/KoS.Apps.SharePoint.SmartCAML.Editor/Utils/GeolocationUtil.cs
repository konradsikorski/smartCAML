using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KoS.Apps.SharePoint.SmartCAML.Editor.Utils
{
    class GeolocationUtil
    {
        public class GeoLoc
        {
            public string City { get; set; }

            public string Country { get; set; }

            public string Code { get; set; }

            public string Host { get; set; }

            public string Ip { get; set; }

            public string Latitude { get; set; }

            public string Lognitude { get; set; }

            public object State { get; set; }
        }

        internal GeoLoc GetMyGeoLocation()
        {
            var geoLoc = new GeoLoc();
            try
            {
                //create a request to geoiptool.com
                var request = WebRequest.Create(new Uri("http://geoiptool.com/data.php")) as HttpWebRequest;

                if (request != null)
                {
                    //set the request user agent
                    request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0; SLCC1; .NET CLR 2.0.50727)";

                    //get the response
                    using (var webResponse = (HttpWebResponse)request.GetResponse())
                    {
                        using (var reader = new StreamReader(webResponse.GetResponseStream()))
                        {
                            //get the XML document
                            var doc = new XmlDocument();
                            doc.Load(reader);

                            //now we parse the XML document
                            var nodes = doc.GetElementsByTagName("marker");
                            var marker = (XmlElement) nodes[0];

                            //get the data and return it
                            geoLoc.City = marker.GetAttribute("city");
                            geoLoc.Country = marker.GetAttribute("country");
                            geoLoc.Code = marker.GetAttribute("code");
                            geoLoc.Host = marker.GetAttribute("host");
                            geoLoc.Ip = marker.GetAttribute("ip");
                            geoLoc.Latitude = marker.GetAttribute("lat");
                            geoLoc.Lognitude = marker.GetAttribute("lng");

                            return geoLoc;
                        }
                    }
                }
            }
            catch{}

            return new GeoLoc();
        }
    }
}
