using System;
using System.Collections.Generic;
using System.Dynamic;
using HipchatApiV2.Responses;
using StandupAggragation.Core.DataAccess;
using StandupAggragation.Core.Models;

namespace StandupAggragation.Core.Services
{
    public class FakeStandupService : IStandupService
    {
        private readonly BaseRepository<IStandupMessage> _repository;

        public FakeStandupService()
        {
            _repository = new FakeStandupMessageRepository();
        }


        public IList<IStandupMessage> GetStandupHistory(string roomName, DateTime date, out bool hasMore)
        {
            throw new NotImplementedException();
        }

        public IList<IStandupMessage> GetAllStandupHistory(string roomName, string botName)
        {
            dynamic expando = new ExpandoObject();
            expando.GetAll = true;
            var result = _repository.GetItems(expando);
            return result.Succeed ? result.Result : null;
        }
    }
}