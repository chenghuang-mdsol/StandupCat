using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using HipchatApiV2;
using HipchatApiV2.Responses;
using ImpromptuInterface;
using ServiceStack;
using StandupAggragation.Core.DataAccess;
using StandupAggragation.Core.Models;

namespace StandupAggragation.Core.Services
{
    public class IntegrationCredential
    {
        public string AuthToken { get; set; }
    }

    public class StandupService : IStandupService
    {
        private StandupMessageRepository _repository;
        public StandupService(string key)
        {
            _repository = new StandupMessageRepository(key);
        }

        public IList<IStandupMessage> GetStandupHistory(string roomName, DateTime date, out bool hasMore)
        {
            dynamic expando = new ExpandoObject();
            expando.Mode = "messagehistory";
            expando.RoomName = roomName;
            expando.DateTime = date;
            var result = _repository.GetItems((ExpandoObject)expando);
            if (result.Succeed)
            {
                var standupReg = new Regex($@"/standup .*");
                hasMore = result.Result.Count == 1000;
                return result.Result.Where(o=>o.From!="Standup" && standupReg.IsMatch(o.Message)).Select(o => _repository.Convert<IStandupMessage>(o)).ToList();
            }
            hasMore = false;
            return null;
        }

        public IList<IStandupMessage> GetAllStandupHistory(string roomName, string botName)
        {
            //Allow 3 min hipchat
            var earliest = DateTime.Now- TimeSpan.FromMinutes(3);
            var items = new List<IStandupMessage>();
            while (true)
            {
                bool hasMore = false;
                var oneFetch = GetStandupHistory(roomName, earliest, out hasMore);
                items.AddRange(oneFetch);
                if (!hasMore)
                {
                    break;
                }
                var earliestForOneFetch = oneFetch.Min(o => o.Date);
                earliest = earliestForOneFetch < earliest ? earliestForOneFetch : earliest;
            }
            //var latest = standups.GroupBy(o => o.Date.Date).ToDictionary(o => o.Key, o => o.GroupBy(p=>p.From).ToDictionary(w=>w.Key, q=>q.OrderBy(r=>r.Date).Last()));
            //{id: 101942, links: {self: https://api.hipchat.com/v2/user/101942}, mention_name: JunShao, name: Jun Shao, version: 00000000}
            return items;
        }


    }
}