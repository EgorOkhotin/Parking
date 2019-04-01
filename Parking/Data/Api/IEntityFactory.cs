using System;
using Parking.Data.Entites;

namespace Parking.Data.Api
{
    public interface IEntityFactory
    {
        Key CreateKey(string tariffName);
        Key CreateKey(string tariffName, string autoId);
    }
}