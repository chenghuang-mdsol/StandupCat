using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using HipchatApiV2;
using HipchatApiV2.Responses;
using Newtonsoft.Json;

namespace StandupAggragation.Core.Services
{
    public interface IHipChatLoginService
    {
        HipchatUser Login(string userName, string password);
        //HipchatUser GetUser(string userName);
    }

    
    public class HipChatLoginService: IHipChatLoginService
    {
        
        //HipchatClient client = new HipchatClient();
        public HipchatUser Login(string userName, string password)
        {
            
            string authToken = ConfigurationManager.AppSettings["AuthTokens.LoginBridge"];
            int groupId = int.Parse(ConfigurationManager.AppSettings["CompanyGroupId"]); 
            string url = "https://api.hipchat.com/v2/oauth/token".AddHipchatAuthentication(authToken);
            string body = $"grant_type=password&username={userName}&password={password}";
            var bodyBytes = Encoding.UTF8.GetBytes(body);
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bodyBytes.Length;
            request.Accept = @"text/html,application/xhtml+xml,application/xml,application/json";
           
            using (var stream = request.GetRequestStream())
            {
                stream.Write(bodyBytes, 0, bodyBytes.Length);
            }
            try
            {
                using (var response = request.GetResponse())
                {
                    using (var stream = new StreamReader(response.GetResponseStream()))
                    {
                        var result = JsonConvert.DeserializeObject<HipChatLoginResult>(stream.ReadToEnd());
                        if (result.GroupId == groupId)
                        {
                            return GetUser(userName);
}
                        throw new AuthenticationException("Not Authorized to Medidata Solutions");
                    }
                }
            }
            catch (WebException ex)
            {
                throw new AuthenticationException(ExceptionHelpers.WebExceptionHelper(ex).Message);
            }
            


        }

        public HipchatUser GetUser(string userName)
        {
            string authToken = ConfigurationManager.AppSettings["AuthTokens.Admin"];
            try
            {
                HipchatClient client = new HipchatClient(authToken);
                var response = client.GetUserInfo(userName);
               
                return new HipchatUser() {Id = response.Id, MentionName = response.MentionName, Name = response.Name};
            }
            catch
            {
                return null;
            }

            
        }
    }

    public class HipChatLoginResult
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public long Expires_in { get; set; }

        [JsonProperty("group_id")]
        public int GroupId { get; set; }

        [JsonProperty("group_name")]
        public string GroupName { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        
        [JsonProperty("scope")]
        public string Scope { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }

    }
}
