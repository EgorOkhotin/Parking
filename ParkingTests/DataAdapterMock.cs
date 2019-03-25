using System.Threading.Tasks;
using Parking.Data;
using Parking.Models;
using System;
//using System.Threading.Tasks;

namespace ParkingTests
{
    public class DataAdapterMock : IDataAdapter
    {
        DateTimeBuilder TimeBuilder { get; set; }

        public DataAdapterMock()
        {
            TimeBuilder = new DateTimeBuilder();
        }
        public async Task<Parking.Models.Key> CreateKey()
        {
            return await CreateTestKey();
        }

        public async Task<Parking.Models.Key> CreateKey(string tariffName)
        {
            if (tariffName == null) throw new ArgumentException("Tariff name is null");
            return await CreateTestKey();
        }

        public async Task<Parking.Models.Key> CreateKey(string tariffName, string autoId)
        {
            if (tariffName == null) throw new ArgumentException("Tariff name is null");
            if (autoId == null) throw new ArgumentException("AutoId is null");
            return await CreateTestKey();
        }

        public async Task<Parking.Models.Key> FindKeyByAutoId(string autoId)
        {
            if (autoId == null) throw new ArgumentNullException();
            return await CreateTestKey();
        }

        public async Task<bool> DeleteKey(Parking.Models.Key key)
        {
            return await Task.Run(() =>
            {
                if (key == null || key.Token == null)
                    return false;
                return true;
            }
            );
        }

        public async Task<Parking.Models.Key> FindKey(string token)
        {
            if (token == null) throw new ArgumentException();
            return await CreateTestKey();
        }

        public async Task<Parking.Models.Tariff> GetTariff(string tariffName)
        {
            if (tariffName == null) throw new ArgumentException();
            return await CreateTestTariff();
        }

        private async Task<Parking.Models.Key> CreateTestKey()
        {
            var k = new Parking.Models.Key();
            k.Tariff = await CreateTestTariff();
            k.TimeStamp = TimeBuilder.GetTimeBeforeMinutes(10 * 2);
            k.Token = "TEST_TOKEN";
            return k;
        }

        private async Task<Parking.Models.Tariff> CreateTestTariff()
        {
            return await Task.Run(() =>
            {
                var t = new Parking.Models.Tariff();
                t.Cost = 10;
                t.Name = "TEST_NAME";
                return t;
            });
        }
    }
}