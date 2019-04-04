using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Parking.Data.Entites;
using Parking.Data.Factories.Abstractions;

namespace Parking.Data.Factories
{
    public class DefaultSellOutFactory : SellOutFactory
    {
        SellOut[] _sells;
        public DefaultSellOutFactory()
        {
            _sells = new SellOut[3];
            _sells[0] = new NoneSellOut();
            _sells[1] = new Off50SellOut();
            _sells[2] = new Off25SellOut();
        }
        public override SellOut GetSellOut(SellOut sellOut)
        {
            int t = (int)sellOut.SellOutType;
            if(!((t >= 0) && (t < _sells.Length)))
                throw new ArgumentException();

            return _sells[t].CreateInstance(sellOut);
        }
    }
}