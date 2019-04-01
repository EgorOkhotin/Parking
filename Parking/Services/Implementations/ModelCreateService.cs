using System;
using Parking.Data.Entites;
using Parking.Services.Api;

namespace Parking.Services.Implementations
{
    class ModelCreateService : IModelCreateService
    {
        public ModelCreateService()
        {

        }

        public Models.Key CreateKeyModel(Key k)
        {
            return new Models.Key(){
                Token = k.Token,
                TimeStamp = k.TimeStamp,
                Tariff = new Models.Tariff(){
                    Name = k.Tariff.Name,
                    Cost = k.Tariff.Cost,
                }
            };
        }
    }
}