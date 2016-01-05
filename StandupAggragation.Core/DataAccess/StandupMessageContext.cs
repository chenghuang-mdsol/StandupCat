using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using HipchatApiV2;
using HipchatApiV2.Responses;
using Newtonsoft.Json;
using StandupAggragation.Core.Models;

namespace StandupAggragation.Core.DataAccess
{
    public class StandupMessageContext: IContext
    {
        //"rWhFopVMgXRBxHUQIzqvDlMHOLuYA5obelp3SOVx"

        public static StandupMessageContext Instance { get; private set; } = new StandupMessageContext();
        private static DateTime _lastRefreshDateTime = DateTime.Now;
        private static Dictionary<string, object> _cached = new Dictionary<string, object>();
        public IHipchatClient Client { get; private set; }
        private StandupMessageContext(string key)
        {
            Connect(key);
        }
        public void Dispose()
        {
            
        }

        private StandupMessageContext() { }

        public object Connect(string key)
        {
            return Client ?? (Client = new HipchatClient(key));
        }

        //private bool TryGetFromCache(string command, ExpandoObject filter, out object obj)
        //{
            
        //    string key = BuildKeyAndRemoveDateTimeTag(command, filter);
        //    if (_cached.ContainsKey(key) && (DateTime.Now - _lastRefreshDateTime).Seconds < 20)
        //    {
        //        obj = _cached[key];
        //        return true;
        //    }
        //    obj = null;
        //    return false;
        //}

        //private ExpandoObject CloneExpandoObj(ExpandoObject obj1)
        //{
        //    dynamic obj2= new ExpandoObject();
        //    foreach (var kvp in obj1)
        //    {
        //        ((IDictionary<string, object>)obj2).Add(kvp);
        //    }
        //    return (ExpandoObject) obj2;
        //}
        //private string BuildKeyAndRemoveDateTimeTag(string command, ExpandoObject filter)
        //{
        //    var filterClone = CloneExpandoObj(filter);
        //    var dict = (IDictionary<string, object>)filterClone;
        //    if (dict.ContainsKey("DateTime"))
        //    {
        //        dict.Remove("DateTime");
        //    }
        //    var json = JsonConvert.SerializeObject(filterClone);
        //    string key = $"{command}|{json}";
        //    return key;
        //}
        //private void SaveToCache(string command, ExpandoObject filter, object obj)
        //{
        //    string key = BuildKeyAndRemoveDateTimeTag(command, filter);
        //    if (_cached.ContainsKey(key))
        //    {
        //        _cached[key] = obj;
        //    }
        //    else
        //    {
        //        _cached.Add(key,obj);
        //    }
        //}
        public object Fetch(string command, ExpandoObject filter)
        {
            if (Client == null)
            {
                throw new ArgumentNullException("Please call Connect() first");
            }
            object result = null;

            //if (TryGetFromCache(command, filter, out result))
            //{
            //    return result;
            //}

            dynamic param = filter;
            if (command.ToLower() == "getlist" && param.Mode == "messagehistory")
            {
                string roomName = param.RoomName;
                DateTime dt = param.DateTime;
                result = GetMessageHistory(roomName, dt);
                //SaveToCache(command,filter,result);
                _lastRefreshDateTime = DateTime.Now;
                return result;
            }
            if (command.ToLower() == "getlist" && param.Mode == "allstanduphistory")
            {
                string roomName = param.RoomName;
                string botName = param.BotName;
                result = GetAllStandupHistory(roomName, botName);
                //SaveToCache(command,filter,result);
                _lastRefreshDateTime = DateTime.Now;
                return result;
            }
            return null;
        }

        private IList<HipchatViewRoomHistoryResponseItems> GetMessageHistory(string roomName, DateTime date)
        {
            //HipChat History is not realtime, allow 3 min delay.
            int timeStamp = UnixTimeStampUTC(date,0);
            var results = Client.ViewRoomHistory(roomName, timeStamp.ToString(),"EST", 0, 1000);
            return results.Items;
        }

        private int UnixTimeStampUTC(DateTime dateTime, int miniuteDelay)
        {
            Int32 unixTimeStamp;
            DateTime currentTime = dateTime - TimeSpan.FromMinutes(miniuteDelay);
            DateTime zuluTime = currentTime.ToUniversalTime();
            DateTime unixEpoch = new DateTime(1970, 1, 1);
            unixTimeStamp = (Int32)(zuluTime.Subtract(unixEpoch)).TotalSeconds;
            return unixTimeStamp;
        }



        private IList<HipchatViewRoomHistoryResponseItems> GetAllStandupHistory(string roomName, string botName)
        {

            var earliest = DateTime.Now;
            var items = new List<HipchatViewRoomHistoryResponseItems>();
            while (true)
            {
                var oneFetch = GetMessageHistory(roomName, earliest);
                items.AddRange(oneFetch);
                if (oneFetch.Count < 1000)
                {
                    break;
                }
                var earliestForOneFetch = oneFetch.First().Date;
                earliest = earliestForOneFetch < earliest ? earliestForOneFetch : earliest;
            }

            var standupReg = new Regex($@"/{botName} .*");
            var standups =
                items.Where(o => o.From != "Standup" && standupReg.IsMatch(o.Message)).ToList();

            //var latest = standups.GroupBy(o => o.Date.Date).ToDictionary(o => o.Key, o => o.GroupBy(p=>p.From).ToDictionary(w=>w.Key, q=>q.OrderBy(r=>r.Date).Last()));
            //{id: 101942, links: {self: https://api.hipchat.com/v2/user/101942}, mention_name: JunShao, name: Jun Shao, version: 00000000}
            return standups;
        }
    }
}