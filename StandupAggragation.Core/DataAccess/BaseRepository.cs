using System.Collections.Generic;
using System.Dynamic;

namespace StandupAggragation.Core.DataAccess
{
    public class BaseRepository<TObject> : IRepository<TObject, IList<TObject>>
    {
        private readonly IContext _context;

        public BaseRepository(IContext context)
        {
            _context = context;
        }

        public virtual RepositoryResult<IList<TObject>> GetItems(ExpandoObject filter)
        {
            return GetFromContext<IList<TObject>>("GetList", filter);
        }

        public virtual RepositoryResult<TObject> GetItem(ExpandoObject filter)
        {
            return GetFromContext<TObject>("Get", filter);
        }

        public virtual TConverted Convert<TConverted>(TObject obj) where TConverted : class
        {
            if (typeof (TConverted) == typeof (TObject))
            {
                return obj as TConverted;
            }
            return null;
        }

        private RepositoryResult<T> GetFromContext<T>(string command, ExpandoObject filter)
        {
            using (_context)
            {
                var rawResult = _context.Fetch(command, filter);
                var result = new RepositoryResult<T>();
                if (rawResult == null)
                {
                    result.Succeed = false;
                    result.Result = default(T);
                    result.Id = null;
                }
                else
                {
                    result.Succeed = true;
                    result.Result = (T) rawResult;
                    //result.Id 
                    //TODO: Implement result.ID in Context level
                }
                return result;
            }
        }
    }
}