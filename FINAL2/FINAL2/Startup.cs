using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FINAL2.Data;
using FINAL2.Helper;
using FINAL2.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;
using NLog.Web;

namespace FINAL2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            // LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            //Configuration = configuration;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<PersonDbContext>(opt =>
                         opt.UseSqlServer(Configuration.GetConnectionString("FINAL2"),
                         b => b.MigrationsAssembly("FINAL2"))
                         .EnableSensitiveDataLogging()
                         .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            services.AddControllers();
            services.AddCors();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FINAL2", Version = "v1" });
            });
            var appsettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appsettingsSection);
            var appSettings = appsettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
               .AddJwtBearer(x =>
               {

                   x.RequireHttpsMetadata = false;
                   x.SaveToken = true;
                   x.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(key),
                       ValidateIssuer = false,
                       ValidateAudience = false
                   };
               });
            //services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<ILoanservice, Loanservice>();
            services.AddScoped<IUserService, UserService>();
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders(); // Clear other logging providers
                loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace); // Set minimum log level
                loggingBuilder.AddNLog(Configuration); // Use NLog from appsettings.json
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FINAL2 v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

