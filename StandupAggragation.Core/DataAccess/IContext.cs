using System;
using System.Dynamic;

namespace StandupAggragation.Core.DataAccess
{
    public interface IContext : IDisposable
    {
        object Connect(string key);
        object Fetch(string command, ExpandoObject filter);
    }
}