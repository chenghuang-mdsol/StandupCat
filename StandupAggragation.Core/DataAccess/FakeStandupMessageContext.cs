using System;
using System.Collections.Generic;
using System.Dynamic;
using ImpromptuInterface;
using StandupAggragation.Core.Models;

namespace StandupAggragation.Core.DataAccess
{
    public class FakeStandupMessageContext : IContext
    {
        public object Fetch(string command, ExpandoObject filter)
        {
            if (command.ToLower() == "getlist" && ((dynamic) filter).GetAll == true)
            {
                return new List<IStandupMessage>
                {
                    new
                    {
                        UserId = "cheng",
                        UserName = "chuang",
                        Message = "Test Message",
                        Date = DateTime.Now,
                        Tags = new List<string> {"TSDV", "Misc"}
                    }.ActLike<IStandupMessage>(),
                    new
                    {
                        UserId = "cheng2",
                        UserName = "chuang2",
                        Message = "Test Message2",
                        Date = DateTime.Now,
                        Tags = new List<string> {"Rave", "RaveX"}
                    }.ActLike<IStandupMessage>()
                };
            }

            if (command.ToLower() == "get")
            {
                return new
                {
                    UserId = "cheng",
                    UserName = "chuang",
                    Message = "Test Message",
                    Date = DateTime.Now,
                    Tags = new List<string> {"TSDV", "Misc"}
                }.ActLike<IStandupMessage>();
            }

            return null;
        }
        
        public void Dispose()
        {
        }

        public object Connect(string key)
        {
            return null;
        }
    }
}