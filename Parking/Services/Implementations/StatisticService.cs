using System;
using Parking.Data.Entites;
using Parking.Services.Api;
using Parking.Data.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Parking.Data.Entites.Statistic;
using System.Threading.Tasks;
using System.Linq;

namespace Parking.Services.Implementations
{
    public class StatisticService : IStatisticService, IObservable<(int, string)>
    {
        IStatisticDataService _data;
        ConcurrentDictionary<string, FreePlaces> _parkings;
        ConcurrentBag<IObserver<(int, string)>> _observers;
        int incomingInLastHour;
        int outcomingInListHour;
        public StatisticService([FromServices] IStatisticDataService statistic,
        [FromServices] IParkingDataService parkingData)
        {
            _data = statistic;
            InitializeDictionary(parkingData);

            incomingInLastHour = 0;
            outcomingInListHour = 0;

            _observers = new ConcurrentBag<IObserver<(int, string)>>();

            Timer timer = new Timer((object state)=>SendStatistic());
        }

        public bool EnterUser(string name, string tariff)
        {
            var parking = _parkings[tariff];
            if(parking.CurrentAuthorize > 0)
            {
                var currentFreePlaces = Interlocked.Decrement(ref parking.CurrentAuthorize);
                Interlocked.Increment(ref incomingInLastHour);

                NotifyAll((currentFreePlaces, tariff));

                return true;
            }
            return false;
        }

        public bool EnterAnonym(string name, string tariff)
        {
            var parking = _parkings[tariff];
            if(parking.CurrentAnonymous > 0)
            {
                Interlocked.Decrement(ref parking.CurrentAnonymous);
                Interlocked.Increment(ref incomingInLastHour);
                NotifyAll((parking.CurrentAuthorize, tariff));
                return true;
            }
            return false;
        }

        public bool LeaveUser(string name, string tariff)
        {
            var parking = _parkings[tariff];
            Interlocked.Increment(ref parking.CurrentAuthorize);
            Interlocked.Increment(ref outcomingInListHour);
            NotifyAll((parking.CurrentAuthorize, tariff));
            return true;
        }

        public bool LeaveAnonym(string name, string tariff)
        {
            var parking = _parkings[tariff];
            Interlocked.Increment(ref parking.CurrentAnonymous);
            Interlocked.Increment(ref outcomingInListHour);
            NotifyAll((parking.CurrentAuthorize, tariff));
            return true;
        }

        private void InitializeDictionary(IParkingDataService parkingData)
        {
            _parkings = new ConcurrentDictionary<string, FreePlaces>();
            foreach(var p in parkingData.GetParkingAreas())
                _parkings.TryAdd(p.Name, new FreePlaces(p));
        }

        private class FreePlaces{
            public FreePlaces(Data.Entites.Parking park)
            {
                AllAnonymous = park.AnonymousPlaces;
                AllAuthorize = park.AuthorizePlaces;

                CurrentAnonymous = 0;
                CurrentAuthorize = 0;
            }
            public int CurrentAnonymous;
            public int CurrentAuthorize;
            public int AllAnonymous;
            public int AllAuthorize;
        }

        private class Unsubscriber : IDisposable
        {
            private Action a;
            public Unsubscriber(Action a)
            {
                this.a = a;
            }
            public void Dispose()
            {
                a.Invoke();
            }
        }
        private void SendStatistic()
        {
            int incoming = Interlocked.Exchange(ref incomingInLastHour, 0);
            int outcoming = Interlocked.Exchange(ref outcomingInListHour, 0);
            _data.AddRecord(incoming, outcoming);
        }

        private async Task NotifyAll((int, string) value)
        {
            Task.Run(() => Parallel.ForEach(_observers, (IObserver<(int,string)> o) => o.OnNext(value)));
        }

        public IDisposable Subscribe(IObserver<(int, string)> observer)
        {
            _observers.Add(observer);
            return new Unsubscriber(new Action(() =>{
                IObserver<(int, string)> o;
                var r = _observers.TryTake(out o);
            }));
        }


    }
}