using HUST.Core.Constants;
using HUST.Core.Interfaces.Repository;
using HUST.Core.Interfaces.Service;
using HUST.Core.Services;
using HUST.Core.Utils;
using HUST.Infrastructure.Repositories;
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

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IDictionaryRepository, DictionaryRepository>();
            services.AddScoped<IConceptRepository, ConceptRepository>();
            services.AddScoped<IConceptRelationshipRepository, ConceptRelationshipRepository>();
            services.AddScoped<ICacheSqlRepository, CacheSqlRepository>();
            services.AddScoped<IExampleRepository, ExampleRepository>();
            services.AddScoped<ICacheExternalWordApiRepository, CacheExternalWordApiRepository>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            GlobalConfig.ContentRootPath = env.ContentRootPath;
            GlobalConfig.IsDevelopment = env.IsDevelopment();
            GlobalConfig.Environment = env.EnvironmentName;
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
