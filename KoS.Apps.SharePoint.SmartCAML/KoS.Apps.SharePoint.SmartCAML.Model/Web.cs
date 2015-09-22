using System;
using System.Collections.Generic;

namespace KoS.Apps.SharePoint.SmartCAML.Model
{
    public class Web
    { 
        public Web(ISharePointProvider client)
        {
            Client = client;
        }

        public ISharePointProvider Client { get; private set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public IList<SList> Lists { get; set; }
        public Guid Id { get; set; }
    }
}
