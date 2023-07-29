using HUST.Core.Constants;
using HUST.Core.Interfaces.InfrastructureService;
using HUST.Core.Interfaces.Repository;
using HUST.Core.Interfaces.Service;
using HUST.Core.Services;
using HUST.Core.Settings;
using HUST.Core.Utils;
using HUST.Infrastructure.CacheService;
using HUST.Infrastructure.LogService;
using HUST.Infrastructure.MailService;
using HUST.Infrastructure.Repositories;
using HUST.Infrastructure.StorageService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace HUST.Api
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HUST.Api", Version = "v1" });
            });

            // Tránh lỗi CORS
            services.AddCors();

            // Cache mem
            services.AddMemoryCache();

            // Cache redis
            var redisCache = Configuration.GetConnectionString(ConnectionStringSettingKey.RedisCache);
            if (!string.IsNullOrEmpty(redisCache))
            {
                services.AddStackExchangeRedisCache(option =>
                {
                    option.Configuration = redisCache;
                    option.InstanceName = CacheKey.HustInstanceCache;
                });
            }
            else
            {
                services.AddDistributedMemoryCache();
            }

            // Distributed cache
            services.AddTransient<IDistributedCacheService, DistributedCacheService>();

            // Add send mail service
            services.Configure<MailSettings>(Configuration.GetSection(AppSettingKey.MailSettingsSection));
            services.AddTransient<IMailService, MailService>();

            // Storage
            services.AddScoped<IStorageService, FirebaseStorageService>();

            // Log
            services.AddTransient<ILogService, NLogService>();

            // Thiết lập các cấu hình theo base config
            BaseStartupConfig.ConfigureServices(ref services, Configuration);
            
            // Thiết lập Dependencies Inject
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<IDictionaryService, DictionaryService>();
            services.AddScoped<ITemplateService, TemplateService>();
            services.AddScoped<IConceptService, ConceptService>();
            services.AddScoped<IUserConfigService, UserConfigService>();
            services.AddScoped<IExampleService, ExampleService>();
            services.AddScoped<IExternalApiService, ExternalApiService>();
            services.AddScoped<IUserSettingService, UserSettingService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IDictionaryRepository, DictionaryRepository>();
            services.AddScoped<IConceptRepository, ConceptRepository>();
            services.AddScoped<IConceptRelationshipRepository, ConceptRelationshipRepository>();
            services.AddScoped<ICacheSqlRepository, CacheSqlRepository>();
            services.AddScoped<IExampleRepository, ExampleRepository>();
            services.AddScoped<ICacheExternalWordApiRepository, CacheExternalWordApiRepository>();
            services.AddScoped<IUserSettingRepository, UserSettingRepository>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            GlobalConfig.ContentRootPath = env.ContentRootPath;
            GlobalConfig.IsDevelopment = env.IsDevelopment();
            GlobalConfig.Environment = env.EnvironmentName;

            // PTHIEU 14.07.2023: Cấu hình show dev message (exception) bất kể môi trường (kể cả production)
            var convertRes = bool.TryParse(Configuration.GetSection(AppSettingKey.AppSettingsSection)[AppSettingKey.AlwaysShowDevMsg], out var isAlwaysShowDevMesg);
            GlobalConfig.AlwaysShowDevMesg = convertRes && isAlwaysShowDevMesg;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HUST.Api v1"));
            }

            // Tránh lỗi CORS
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Content-Disposition"));

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
