using System;

namespace Parking.Data.Entites
{
    public class Parking
    {
        public int Id{get;set;}
        public string Name{get;set;}
        public int AnonymousPlaces {get;set;}
        public int AuthorizePlaces {get;set;}
    }
}