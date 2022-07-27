using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ChainLinkLogging.Middleware;
using ChainLinkLogging.Objects;
using ChainLinkUtils.Filters;
using ChainLinkUtils.Utils;
using NotificationsService.Objects.DAO;
using NotificationsService.Objects.DAO.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core.Enrichers;
using Serilog.Enrichers.AspNetCore.HttpContext;

namespace ConfigurationService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddMvc(options => options.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services
                .AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressConsumesConstraintForFormFileParameters = true;
                    options.SuppressInferBindingSourcesForParameters = true;
                    options.SuppressModelStateInvalidFilter = true;
                    options.SuppressMapClientErrors = true;
                    options.ClientErrorMapping[404].Link =
                        "https://httpstatuses.com/404";
                })
                .AddJsonOptions(opts =>
                {
                    opts.JsonSerializerOptions.PropertyNamingPolicy = null;
                });

           
            services.AddSingleton<INotificationDAO, NotificationDAO>();
            services.AddSingleton<INotificationDetailsDAO, NotificationDetailsDAO>();
            services.AddSingleton<INotificationHistoryDAO, NotificationHistoryDAO>();
            services.AddSingleton<INotificationMethodDAO, NotificationMethodDAO>();
            services.AddSingleton<INotificationRecipientDAO, NotificationRecipientDAO>();
            services.AddSingleton<IHeaderMethodRefDAO, HeaderMethodRefDAO>();
            services.AddSingleton<INotificationStoredProcedureDAO, NotificationStoredProcedureDAO>();

            services.AddSingleton(Configuration);

            services.AddScoped<ValidateJwtTokenFilter>();

            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseSerilogLogContext(options =>
            {
                options.EnrichersForContextFactory = context =>
                {
                    string user = "";

                    if (context.Request.Headers.ContainsKey(SysConstants.JWT_TS_JWT_TOKEN_HEADER))
                    {
                        string token = context.Request.Headers[SysConstants.JWT_TS_JWT_TOKEN_HEADER].ToString().Split(' ')[1];
                        JwtSecurityToken jwtToken = new JwtSecurityToken(token);
                        List<Claim> tokenClaims = new List<Claim>(jwtToken.Claims);

                        user = tokenClaims.Find(c => c.Type == JwtClaimTypes.Subject).Value.ToString();
                    }

                    return new[]
                    {
                        new PropertyEnricher("User", user),
                        new PropertyEnricher("Address", context.Connection.RemoteIpAddress.ToString())
                    };
                };
            });

            app.UseRequestLogging(new LoggingSettings(Configuration.GetSection("Serilog")));
            app.UseResponseLogging(new LoggingSettings(Configuration.GetSection("Serilog")));

            app.UseCors(builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
            );

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}