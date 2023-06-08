using Microsoft.Extensions.DependencyInjection;
using HUST.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using HUST.Core.Constants;
using System.Diagnostics;
using System;
using HUST.Core.Interfaces.Service;

namespace HUST.Core.Extensions
{
    /// <summary>
    /// Extension jwt authen
    /// </summary>
    public static class JwtAuthorizationExtension
    {
        /// <summary>
        /// Thêm authorize thông qua jwt
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddJwtAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettings = configuration.GetSection(AppSettingKey.AppSettingsSection);
            var secretKey = appSettings[AppSettingKey.JwtSecretKey];
            var issuer = appSettings[AppSettingKey.JwtIssuer];
            var audience = appSettings[AppSettingKey.JwtAudience];


            if (!string.IsNullOrEmpty(secretKey))
            {
                var encodingKey = Encoding.UTF8.GetBytes(secretKey);
                services.AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.SaveToken = true;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateIssuerSigningKey = true,
                            ValidateLifetime = true,
                            ValidIssuer = issuer,
                            ValidAudience = audience,
                            IssuerSigningKey = new SymmetricSecurityKey(encodingKey),
                            ClockSkew = Debugger.IsAttached ? TimeSpan.Zero : TimeSpan.FromMinutes(2) // Nếu đang debug thì để độ trể = 0, thực tế thì cho phép trễ 2p
                        };

                        options.Events = new JwtBearerEvents()
                        {
                            OnMessageReceived = context =>
                            {
                                string sessionId = null;
                                if(context.HttpContext.Request.Cookies.ContainsKey(AuthKey.SessionId))
                                {
                                    var cookieSessionId = context.HttpContext.Request.Cookies[AuthKey.SessionId];
                                    context.HttpContext.Request.Headers.Add(AuthKey.SessionId, cookieSessionId);
                                }
                                
                                if(context.HttpContext.Request.Headers.ContainsKey(AuthKey.SessionId))
                                {
                                    sessionId = context.HttpContext.Request.Headers[AuthKey.SessionId];
                                }

                                if(!string.IsNullOrEmpty(sessionId))
                                {
                                    var sp = services.BuildServiceProvider();
                                    var sessionService = sp.GetService<ISessionService>();
                                    var token = sessionService.GetToken(sessionId);

                                    if(!string.IsNullOrEmpty(token))
                                    {
                                        context.Request.Headers.Add(AuthKey.Authorization, $"{AuthKey.Bearer} {token}");
                                    }
                                }
                                return Task.CompletedTask;
                            },
                            OnAuthenticationFailed = context =>
                            {
                                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                                {
                                    context.Response.Headers.Add(AuthKey.TokenExpired, "true");
                                }
                                return Task.CompletedTask;
                            }
                        };
                    });
            }

            return services;
        }

    }
}
