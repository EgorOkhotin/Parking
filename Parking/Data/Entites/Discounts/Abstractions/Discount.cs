using System;

namespace Parking.Data.Entites
{
    //! Add hierarchy
    public abstract class Discount
    {
        public int Id{get;set;}
        public string Name {get;set;}
        public DateTime Start{get;set;}
        public DateTime End{get;set;}

        public abstract int GetDiscount(int cost);
        public bool IsExpired()
        {
            return DateTime.Now > End;
        }
        public virtual bool IsActive()
        {
            return (Start <= DateTime.Now) && (DateTime.Now < End);
        }

    }
}