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


              // CORS farkl� domainler aras� kaynak payla��m�
              // api:www.a.com (domain) (server)
              // mvc app: www.c.com (domain) beslenebilir. (client)
              // bu iki farkl� domain aras�nda kaynak payla��m� olur bir g�ven ili�kisi olmas� laz�m. CORS (Cross Origin Resource Sharing) bu g�veni buradaki poli�e sa�lar.
              // HTTPVerbs POST,GET,PUT,DELETE,PATCH

              // CORS sadece SPA uygulama API haberle�mesi i�in tercih ediriz. AJAX request i�in laz�m. API ile MVC iki backend teknolojis haberle�iyyorsa problem yok.
              opt.AddDefaultPolicy(policy =>
              {

                policy.AllowAnyHeader();
                  //policy.AllowAnyOrigin(); // t�m domainlere eri�imi a��k k�lar.
                  policy.WithOrigins("https://localhost:44347", "http://localhost:4200");
                policy.AllowAnyMethod(); // hepsini a�mak i�in 
                                         //policy.WithMethods("PUT","DELETE","PATCH"); // Default olarak Net Core API da GET ve POST a��kt�r. HTTPPut, HttpDelete, HttpPatch default kapal�.
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

      app.UseCors(); // Cors a�ma middleware
      app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
