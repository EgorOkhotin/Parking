using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Parking.Data.Entites;

namespace Parking.Data.Factories.Abstractions
{
    public interface ISellOutFactory
    {
        SellOut GetSellOut(SellOut sellOut);
    }
}