using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoS.Apps.SharePoint.SmartCAML.Model
{
    public class Web
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public IList<SList> Lists { get; set; }
        public Guid Id { get; set; }
    }
}
