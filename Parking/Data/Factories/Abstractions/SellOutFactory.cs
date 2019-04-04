using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Parking.Data.Entites;

namespace Parking.Data.Factories.Abstractions
{
    public abstract class SellOutFactory : ISellOutFactory
    {        
        public abstract SellOut GetSellOut(SellOut sellOut);
    }
}