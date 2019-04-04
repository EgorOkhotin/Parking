using System;

namespace Parking.Data.Entites
{
    public class NoneSellOut : SellOut
    {
        public NoneSellOut()
        {
            this.SellOutType = SellOutType.None;
        }

        public override int GetDiscount(int cost)
        {
            return cost;
        }

        public override SellOut CreateInstance(SellOut sellOut)
        {
            if(sellOut.SellOutType != this.SellOutType)
                throw new ArgumentException("Invalid sellOut type");
            return new NoneSellOut{
                SellOutType = this.SellOutType,
                Start = Start,
                End = End,
                Id = sellOut.Id,
                Name = sellOut.Name,
                Counter = sellOut.Counter
            };
        }
    }
}