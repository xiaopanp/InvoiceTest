using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebSite.Common
{
    public interface IHttpAuthorization
    {
        void Set(HttpWebRequest request);
    }
}
