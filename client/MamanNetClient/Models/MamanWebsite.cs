using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class MamanWebsite
    {
        public string WebsiteName{ get; set; }
        public Uri WebsiteUri { get; set; }

        public override string ToString()
        {
            return WebsiteName;
        }
    }
}
