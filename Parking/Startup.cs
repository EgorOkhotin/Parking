using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parking.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Parking.Services.Api;
using Parking.Services.Implementations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using Parking.Data.Api;
using Parking.Data.Api.Contexts;
using Parking.Data.Api.Factories;
using Parking.Data.Api.Services;
using Parking.Data.Services;
using Parking.Data.Entites;
using Parking.Data.Factories.Abstractions;
using Parking.Data.Factories;

namespace Parking
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddRoleStore<RoleStore<IdentityRole, ApplicationDbContext>>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();



            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            
            services.AddScoped<IApplicationDataContext, ApplicationDbContext>();
            services.AddScoped<IKeyDataContext, ApplicationDbContext>();
            services.AddScoped<ITariffDataContext, ApplicationDbContext>();
            services.AddScoped<IStatisticDataContext, ApplicationDbContext>();
            services.AddScoped<ICouponDataContext, ApplicationDbContext>();
            services.AddScoped<ISellOutDataContext, ApplicationDbContext>();
            services.AddScoped<ISubscriptionDataContext, ApplicationDbContext>();

            services.AddScoped<ISellOutFactory, DefaultSellOutFactory>();
            services.AddScoped<ICouponFactory, DefaultCouponFactory>();
            services.AddSingleton<IRecordFactory, RecordFactory>();
            services.AddSingleton<IKeyFactory>(new KeyFactory());

            services.AddScoped<IKeyDataService, KeyDataService>();
            services.AddScoped<ITariffDataService, TariffDataService>();
            services.AddScoped<ICouponDataService, CouponDataService>();
            services.AddScoped<ISellOutDataService, SellOutDataService>();
            services.AddScoped<ISubscriptionDataService, SubscriptionDataService>();
            services.AddScoped<IDiscountDataService, DiscountDataService>();
            services.AddScoped<IStatisticDataService, StatisticDataService>();
            
            services.AddScoped<IEnterService, EnterService>();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IDataProperties, DataProperties>();
            services.AddSingleton<ICostCalculation, CostCalculationService>();
            services.AddSingleton<IDiscountService, DiscountService>();


            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration)
                .CreateLogger();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            CreateUserRoles(app.ApplicationServices).Wait();
            CreateTariffs(app.ApplicationServices).Wait();
            CreateSubscriptions(app.ApplicationServices).Wait();
            AddSubscriptionToUser(app.ApplicationServices).Wait();
        }

        private async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
                var names = new string[] { "user", "employee" };
                foreach (var n in names)
                {
                    //IdentityResult roleResult;
                    if (!await roleManager.RoleExistsAsync(n))
                    {
                        //create the roles and seed them to the database
                        await roleManager.CreateAsync(new IdentityRole(n));
                    }
                }
            }
        }

        private async Task CreateTariffs(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                if (await context.Tariffs.CountAsync() == 0)
                {
                    var names = new string[] { "LOW", "MIDDLE", "HIGH", "SPECIAL" };
                    var costs = new int[] { 10, 20, 30, 0 };
                    int i = 0;
                    foreach (var n in names)
                    {
                        if ((await context.Tariffs.FirstOrDefaultAsync(x => x.Name == n)) == null)
                        {
                            context.Tariffs.Add(new Tariff() { Name = n, Cost = costs[i] });
                        }
                        i++;
                    }
                    await context.SaveChangesAsync();
                }
            }
        }

        private async Task CreateSubscriptions(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                if (await context.Subscriptions.CountAsync() == 0)
                {
                    var names = new string[]{"Off50", "Off25"};
                    var sellouts = new Subscription[]{
                        new Subscription(){
                            Name = names[0],
                            Start = DateTime.MinValue,
                            End = DateTime.MaxValue,
                            TariffNames = "LOW MIDDLE"
                        },
                        new Subscription(){
                            Name = names[1],
                            Start = DateTime.MinValue,
                            End = DateTime.MaxValue,
                            TariffNames = "LOW MIDDLE HIGH"
                        }
                    };
                    foreach(var s in sellouts)
                    {
                        context.Subscriptions.Add(s);
                    }
                    context.SaveChanges();
                }
            }
        }

        private async Task AddSubscriptionToUser(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                var subs = context.Subscriptions.First();
                var user = context.Users.First();
                var registration = new UserSubscription()
                {
                    User = user,
                    Subscription = subs
                };
                context.UserSubscriptions.Add(registration);
                await context.SaveChangesAsync();
            }
        }

        private async Task CheckAdding(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                var logger = scope.ServiceProvider.GetService<ILogger<Startup>>();

                var registration = await context.UserSubscriptions.Include(x => x.User == context.Users.First(u => u.UserSubscriptionId==x.Id))
                                                                .Include(x => x.Subscription)
                                                                .FirstAsync();

                var user = registration.User;
                var subscription = registration.Subscription;
                logger.LogInformation($"User is null: {user == null}");
                logger.LogInformation($"Subscription is null: {subscription == null}");
            }
        }
    }
}
