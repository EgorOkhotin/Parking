using System;

namespace Parking.Data.Entites
{
    public class Parking
    {
        public int Id{get;set;}
        public string Name{get;set;}
        public int FreePlaces {get;set;}

        public bool IsHavePlace()
        {
            return FreePlaces>0;
        }

        public bool GetPlace()
        {
            if(!IsHavePlace())
                throw new InvalidOperationException();
            
            FreePlaces--;
            return true;
        }
    }
}