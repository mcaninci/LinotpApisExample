using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinotpTokenDelete
{
    public class TokenModel
    {
        public string version;
        public string jsonrpc;
        public result result;
        public int id;
        public string session;
        public Dictionary<string, string> headers;
    }

    public class result
    {
        public bool status;
        public bool value;
    }



}
