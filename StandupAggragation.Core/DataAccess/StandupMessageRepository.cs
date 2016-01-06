using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Policy;
using System.Text.RegularExpressions;
using HipchatApiV2.Responses;
using ImpromptuInterface;
using ServiceStack;
using StandupAggragation.Core.Models;

namespace StandupAggragation.Core.DataAccess
{

    public class StandupMessageRepository : BaseRepository<HipchatViewRoomHistoryResponseItems>
    {

        public StandupMessageRepository(string key) : base(StandupMessageContext.Instance)
        {
            StandupMessageContext.Instance.Connect(key);
        }

        public override IStandupMessage Convert<IStandupMessage>(HipchatViewRoomHistoryResponseItems obj)
        {
            return (IStandupMessage) ConvertMessage(obj);
            
        }




        private IStandupMessage ConvertMessage(HipchatViewRoomHistoryResponseItems item)
        {
            var regex = new Regex(@"\bid: (\d{0,20})\b");
            var userId = regex.Match(item.From).Groups[1].Value;

            regex = new Regex(@"\bname: \b(.*)\b,");
            var username = regex.Match(item.From).Groups[1].Value;
            //Match tags
            regex = new Regex(@"\[([^\[\]]*)]");
            var message = item.Message.TrimPrefixes("/standup ");
            var date = item.Date;
            var tags = regex.Matches(item.Message).Cast<Match>().Select(o => o.Value.TrimStart('[').TrimEnd(']')).ToList();
            return
                new { UserId = userId, UserName = username, Message = message, Date = date, Tags = tags }
                    .ActLike<IStandupMessage>();
        }
    }
}