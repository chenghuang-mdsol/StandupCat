using System.Collections.Generic;
using System.Dynamic;

namespace StandupAggragation.Core.DataAccess
{
    public interface IRepository<TObject, TCollection> where TCollection : ICollection<TObject>
    {
        RepositoryResult<TCollection> GetItems(ExpandoObject filter);
        RepositoryResult<TObject> GetItem(ExpandoObject filter);
    }
}