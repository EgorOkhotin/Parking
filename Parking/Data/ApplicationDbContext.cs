﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Parking.Data.Api;
namespace Parking.Data
{
    public delegate void SaveChanges();
    public class ApplicationDbContext : IdentityDbContext, IDatabaseContext
    {
        private readonly ILogger _logger;
        private event SaveChanges _storageChanged;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, [FromServices] ILogger<ApplicationDbContext> logger)
            : base(options)
        {
            Database.EnsureCreated();
            _storageChanged = async ()=> await SaveChangesAsync();
            _logger = logger;
        }
        public DbSet<Key> Keys { get; set; }
        public SaveChanges StorageChangedEvent => _storageChanged;
        public DbSet<Tariff> Tariffs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Tariff>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
            });
        }

        public async Task<bool> AddKey(Key key)
        {
            try
            {
                _logger.LogInformation($"Try to add new key: {key}");
                var k = await Keys.AddAsync(key);
                //Tariffs.Update(key.Tariff);
                await SaveChangesAsync();
                _logger.LogInformation($"Key added successful");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Can't add key {key} \n Exception message:{ex.Message} \n Exception stack trace:{ex.StackTrace}");
                return false;
            }
        }

        public async Task<bool> DeleteKey(Key key)
        {
            try
            {
                _logger.LogInformation($"Try to delete key: {key}");
                Keys.Remove(key);
                await SaveChangesAsync();
                _logger.LogInformation($"Key: {key} deleted successful");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Cant delete key:{key} Exception message:{ex.Message}\n Exception stack trace:{ex.StackTrace}");
                return false;
            }
        }

        public Task<Key> GetKey(string token)
        {
            return GetKeyByTokenAsync(token);
        }

        public Task<Key> GetKeyByAutoId(string autoId)
        {
            return GetKeyByAutoIdAsync(autoId);
        }

        public async Task<Tariff> GetTariff(string tariffName)
        {
            _logger.LogInformation($"Try to get tariff by name: {tariffName}");
            return await Tariffs.FirstAsync(x => x.Name == tariffName)
            .ContinueWith(task =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    return task.Result;
                }
                else
                {
                    _logger.LogError($"Didn't find tariff with name: {tariffName}");
                    return null;
                }
            });
        }

        private async Task<Key> GetKeyByTokenAsync(string token)
        {
            _logger.LogInformation($"Try to get key by token: {token}");
            return await await Keys.FirstAsync(x => x.Token == token)
            .ContinueWith(task =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    return Tariffs.FirstAsync(x => x.Id == task.Result.TariffId).ContinueWith(t =>
                    {
                        if (t.Status == TaskStatus.RanToCompletion)
                        {
                            task.Result.Tariff = t.Result;
                            return task.Result;
                        }
                        else
                        {
                            _logger.LogError($"Tariff for key: {task.Result} didn't found");
                            return null;
                        }
                    });
                }
                else
                {
                    _logger.LogError($"Key for token:{token} \t isn't exist!");
                    return null;
                }
            });
        }

        private async Task<Key> GetKeyByAutoIdAsync(string id)
        {
            _logger.LogInformation($"Try to get key for auto id: {id}");
            return await await Keys.FirstAsync(x => x.AutoId == id)
            .ContinueWith(task =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    return Tariffs.FirstAsync(x => x.Id == task.Result.TariffId).ContinueWith(t =>
                    {
                        if (t.Status == TaskStatus.RanToCompletion)
                        {
                            task.Result.Tariff = t.Result;
                            return task.Result;
                        }
                        else
                        {
                            _logger.LogError($"Tariff for key: {task.Result} didn't found");
                            return null;
                        }
                    });
                }
                else
                {
                    _logger.LogError($"Key for auto id:{id} \t isn't exist!");
                    return null;
                }
            });
        }
    }
}
