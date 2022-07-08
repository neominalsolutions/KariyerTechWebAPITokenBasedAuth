using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPISample.Infrastructure.JWT;

namespace WebAPISample
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

            services.AddCors(opt => {


              // CORS farklý domainler arasý kaynak paylaþýmý
              // api:www.a.com (domain) (server)
              // mvc app: www.c.com (domain) beslenebilir. (client)
              // bu iki farklý domain arasýnda kaynak paylaþýmý olur bir güven iliþkisi olmasý lazým. CORS (Cross Origin Resource Sharing) bu güveni buradaki poliçe saðlar.
              // HTTPVerbs POST,GET,PUT,DELETE,PATCH

              // CORS sadece SPA uygulama API haberleþmesi için tercih ediriz. AJAX request için lazým. API ile MVC iki backend teknolojis haberleþiyyorsa problem yok.
              opt.AddDefaultPolicy(policy =>
              {

                policy.AllowAnyHeader();
                  //policy.AllowAnyOrigin(); // tüm domainlere eriþimi açýk kýlar.
                  policy.WithOrigins("https://localhost:44347", "http://localhost:4200");
                policy.AllowAnyMethod(); // hepsini açmak için 
                                         //policy.WithMethods("PUT","DELETE","PATCH"); // Default olarak Net Core API da GET ve POST açýktýr. HTTPPut, HttpDelete, HttpPatch default kapalý.
                                         //policy.AllowAnyOrigin();

                });

              //opt.AddPolicy("SecondPolicy", policy =>
              //{
              //    policy.WithHeaders("x-www-formurlencoded");
              //    policy.WithMethods("GET", "PUT");
              //    policy.WithOrigins("www.a.com");
              //});


            });


            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPISample", Version = "v1" });
            });


            var key = Encoding.ASCII.GetBytes(JWTSettings.SecretKey);
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

            services.AddTransient<IJwtTokenService, JwtTokenService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPISample v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();

      app.UseCors(); // Cors açma middleware
      app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
