
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinotpTokenDelete
{
    public class TokenDetail
    {
        [JsonProperty("LinOtp.TokenId")]
        public int LinOtpTokenId { get; set; }

        [JsonProperty("LinOtp.TokenInfo")]
        public string LinOtpTokenInfo { get; set; }

        [JsonProperty("LinOtp.OtpLen")]
        public int LinOtpOtpLen { get; set; }

        [JsonProperty("LinOtp.TokenType")]
        public string LinOtpTokenType { get; set; }

        [JsonProperty("LinOtp.TokenSerialnumber")]
        public string LinOtpTokenSerialnumber { get; set; }

        [JsonProperty("LinOtp.CountWindow")]
        public int LinOtpCountWindow { get; set; }

        [JsonProperty("LinOtp.MaxFail")]
        public int LinOtpMaxFail { get; set; }

        [JsonProperty("User.description")]
        public string UserDescription { get; set; }

        [JsonProperty("LinOtp.IdResClass")]
        public string LinOtpIdResClass { get; set; }

        [JsonProperty("LinOtp.RealmNames")]
        public List<string> LinOtpRealmNames { get; set; }

        [JsonProperty("LinOtp.Count")]
        public int LinOtpCount { get; set; }

        [JsonProperty("User.username")]
        public string UserUsername { get; set; }

        [JsonProperty("LinOtp.SyncWindow")]
        public int LinOtpSyncWindow { get; set; }

        [JsonProperty("LinOtp.FailCount")]
        public int LinOtpFailCount { get; set; }

        [JsonProperty("LinOtp.TokenDesc")]
        public string LinOtpTokenDesc { get; set; }

        [JsonProperty("User.userid")]
        public string UserUserid { get; set; }

        [JsonProperty("LinOtp.IdResolver")]
        public string LinOtpIdResolver { get; set; }

        [JsonProperty("LinOtp.Userid")]
        public string LinOtpUserid { get; set; }

        [JsonProperty("LinOtp.Isactive")]
        public bool LinOtpIsactive { get; set; }
    }

    public class Result
    {
        public bool status { get; set; }
        public Value value { get; set; }
    }

    public class Resultset
    {
        public int tokens { get; set; }
        public int pages { get; set; }
        public int pagesize { get; set; }
        public int page { get; set; }
    }

    public class TokenListModel
    {
        public string version { get; set; }
        public string jsonrpc { get; set; }
        public Result result { get; set; }
        public int id { get; set; }
    }

    public class Value
    {
        public Resultset resultset { get; set; }
        public List<TokenDetail> data { get; set; }
    }

}