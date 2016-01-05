using System;
using System.Collections.Generic;

namespace StandupAggragation.Core.Models
{
    public interface IStandupMessage
    {
        string UserId { get; set; }
        string UserName { get; set; }
        string Message { get; set; }
        DateTime Date { get; set; }
        List<string> Tags { get; set; }
    }
}