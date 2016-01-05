namespace StandupAggragation.Core.DataAccess
{
    public class RepositoryResult<T>
    {
        public T Result { get; set; }
        public bool Succeed { get; set; }
        public object Id { get; set; }
    }
}