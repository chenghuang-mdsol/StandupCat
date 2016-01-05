using StandupAggragation.Core.Models;

namespace StandupAggragation.Core.DataAccess
{
    public class FakeStandupMessageRepository : BaseRepository<IStandupMessage>
    {
        public FakeStandupMessageRepository() : base(new FakeStandupMessageContext())
        {
        }
    }
}