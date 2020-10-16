using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Elsa.Activities.Email.Extensions;
using Elsa.Activities.Http.Extensions;
using Elsa.Activities.Timers.Extensions;
using Elsa.Dashboard.Extensions;
using Elsa.Persistence.EntityFrameworkCore.DbContexts;
using Elsa.Persistence.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Elsa.Sample.Business;
using Elsa.Persistence.MongoDb.Extensions;
using Elsa.Sample.Models;
using Elsa.Extensions;
using Elsa.Samples.UserRegistration.Web.Handlers;
using Elsa.Samples.UserRegistration.Web.Activities;
using Elsa.Activities.Console.Extensions;

namespace elsa_sample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMvc();
            services.AddServerSideBlazor();

            services
                .AddElsa(
                    elsa =>
                    {

                        elsa.AddMongoDbStores(Configuration, databaseName: "SampleUserRegistration", connectionStringName: "MongoDb");
                    })
                .AddElsaDashboard()
                .AddConsoleActivities()
                .AddHttpActivities(options => options.Bind(Configuration.GetSection("Elsa:Http")))
                .AddEmailActivities(options => options.Bind(Configuration.GetSection("Elsa:Smtp")))
                .AddTimerActivities(options => options.Bind(Configuration.GetSection("Elsa:Timers")));


            services.AddScoped<IUserBusiness, UserBusiness>();
            services.AddMongoDbCollection<User>("Users");
            services.AddNotificationHandlers(typeof(LiquidConfigurationHandler));

            services
                .AddActivity<CreateUser>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }
            //app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseHttpActivities();
            app.UseRouting();

            //app.UseAuthorization();
            app.UseHttpActivities();

            app.UseEndpoints(endpoints => endpoints.MapControllers());

            // app.UseWelcomePage();
        }
    }
}
