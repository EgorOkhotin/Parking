using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Parking.Data;
using System.Threading;

namespace Parking.Data.Api
{
    public interface IKeyDataContext : IDataContext
    {
        DbSet<Key> Keys {get;}
    }
}