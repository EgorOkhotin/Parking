using System;

namespace Parking.Data.Entites
{
    public class SellOut : Discount
    {
        public SellOutType SellOutType{get;set;}
        public string Tariffs {get;set;}
        public int Counter{get;set;}

        public override bool IsActive()
        {
            return base.IsActive() && (Counter > 0);
        }

        public override int GetDiscount(int cost)
        {
            throw new NotImplementedException();
        }

        public virtual SellOut CreateInstance(SellOut sellOut)
        {
            throw new NotImplementedException();
        }
    }
}