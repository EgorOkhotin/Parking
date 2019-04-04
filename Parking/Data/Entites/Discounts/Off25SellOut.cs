using System;

namespace Parking.Data.Entites
{
    public class Off25SellOut : SellOut
    {
        public Off25SellOut()
        {
            this.SellOutType = SellOutType.Off25;
        }

        public override int GetDiscount(int cost)
        {
            if(!IsActive()) throw new InvalidOperationException("Sell out is inactive");
            return (int)(cost * 0.75);
        }

        public override SellOut CreateInstance(SellOut sellOut)
        {
            if(sellOut.SellOutType != this.SellOutType)
                throw new ArgumentException("Sell Out have invalid type!");
            
            return new Off25SellOut(){
                Counter = sellOut.Counter,
                Start = sellOut.Start,
                End = sellOut.End,
                Id = sellOut.Id,
                Name = sellOut.Name,
                SellOutType = this.SellOutType
            };
        }
    }
}