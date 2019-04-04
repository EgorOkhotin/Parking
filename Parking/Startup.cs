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
using Parking.Data.Implementations;
using Parking.Data.Entites;

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
            services.AddScoped<IKeyService, KeyDataService>();
            services.AddScoped<ITariffService, TariffDataService>();
            
            services.AddScoped<IApplicationDataContext, ApplicationDbContext>();
            services.AddScoped<IKeyDataContext, ApplicationDbContext>();
            services.AddScoped<ITariffDataContext, ApplicationDbContext>();
            services.AddScoped<IEnterService, EnterService>();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IDataProperties, DataProperties>();
            services.AddSingleton<IKeyFactory>(new KeyFactory());
            services.AddSingleton<ICostCalculation, CostCalculationService>();

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
                    var sellouts = new SellOut[]{
                        new SellOut(){
                            Name = names[0],
                            SellOutType = SellOutType.Off50,
                            Start = DateTime.MinValue,
                            End = DateTime.MaxValue,
                            Tariffs = "LOW MIDDLE"
                        },
                        new SellOut(){
                            Name = names[1],
                            SellOutType = SellOutType.Off25,
                            Start = DateTime.MinValue,
                            End = DateTime.MaxValue,
                            Tariffs = "LOW MIDDLE HIGH"
                        }
                    };
                    foreach(var s in sellouts)
                    {
                        context.SellOuts.Add(s);
                    }
                }
            }
        }
    }
}
