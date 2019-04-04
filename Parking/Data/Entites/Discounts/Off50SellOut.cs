using System;

namespace Parking.Data.Entites
{
    public class Off50SellOut : SellOut
    {
        public Off50SellOut()
        {
            this.SellOutType = SellOutType.Off50;
        }

        public override int GetDiscount(int cost)
        {
            if(!IsActive()) throw new InvalidOperationException();
            return (int)(cost*0.5);
        }

        public override SellOut CreateInstance(SellOut sellOut)
        {
            if(sellOut.SellOutType == SellOutType.Off50)
            {
                return new Off50SellOut()
                {
                    Counter = sellOut.Counter,
                    Start = sellOut.Start,
                    End = sellOut.End,
                    Id = sellOut.Id,
                    Name = sellOut.Name,
                    SellOutType = this.SellOutType
                };
            }
            else throw new ArgumentException("SellOut type is Invalid!");
        }
    }
}