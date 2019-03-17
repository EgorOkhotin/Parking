using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Parking.Data
{
    public class ApplicationDbContext : IdentityDbContext, IDatabaseContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            InitializeTariffs();
        }

        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Tariff>(entity => {
                entity.HasIndex(e => e.Name).IsUnique();
            });
        }

        public async Task<bool> AddKey(Key key)
        {
            try
            {
                await Keys.AddAsync(key);
                //Tariffs.Update(key.Tariff);
                await SaveChangesAsync();
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteKey(Key key)
        {
            try
            {
                Keys.Remove(key);
                await SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Task<Key> GetKey(string token)
        {
            return GetKeyAsync(token);
        }

        public async Task<Tariff> GetTariff(string tariffName)
        {
            return await Tariffs.FirstOrDefaultAsync(x => x.Name == tariffName);
        }

        private async Task<Key> GetKeyAsync(string token)
        {
            var k = await Keys.FirstOrDefaultAsync(x => x.Token == token);
            k.Tariff = await Tariffs.FirstAsync(x => x.Id == k.TariffId);
            return k;
        }

        DbSet<Key> Keys { get; set; }
        DbSet<Tariff> Tariffs { get; set; }

        private async void InitializeTariffs()
        {
            if(await Tariffs.CountAsync()==0)
            {
                var names = new string[] { "LOW", "MIDDLE", "HIGH", "SPECIAL" };
                var costs = new int[] { 10, 20, 30, 0 };
                int i = 0;
                foreach (var n in names)
                {
                    if (Tariffs.FirstOrDefaultAsync(x => x.Name == n) == null)
                    {
                        Tariffs.Add(new Tariff() { Name = n, Cost = costs[i]});
                    }
                    i++;
                }
                SaveChangesAsync();
            }
        }
    }
}
