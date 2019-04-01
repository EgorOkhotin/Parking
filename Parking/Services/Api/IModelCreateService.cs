using System;
using Parking.Data.Entites;

namespace Parking.Services.Api
{
    public interface IModelCreateService
    {
        Models.Key CreateKeyModel(Key k);
    }
}