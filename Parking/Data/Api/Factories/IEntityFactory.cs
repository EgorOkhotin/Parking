using System;
using Parking.Data.Entites;

namespace Parking.Data.Api.Factories
{
    public interface IEntityFactory
    {
        Key CreateKey(string tariffName);
        Key CreateKey(string tariffName, string autoId);
    }
}