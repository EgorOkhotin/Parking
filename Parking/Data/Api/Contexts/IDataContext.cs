using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Parking.Data;
using System.Threading;

namespace Parking.Data.Api.Contexts
{
    public interface IDataContext
    {
        Task<int> SaveChangesAsync(CancellationToken token = default(CancellationToken));
    }
}