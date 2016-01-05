using System;
using System.Collections.Generic;
using HipchatApiV2.Responses;
using StandupAggragation.Core.Models;

namespace StandupAggragation.Core.Services
{
    public interface IStandupService
    {
        IList<IStandupMessage> GetStandupHistory(string roomName, DateTime date, out bool hasMore);
        IList<IStandupMessage> GetAllStandupHistory(string roomName, string botName);
    }
}