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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSVenta.Models.Common;
using WSVenta.Services;

namespace WSVenta
{
    public class Startup
    {
        readonly string MiCors = "MiCors"; // se agrego este codigo
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => 
            {
                options.AddPolicy(name: MiCors, builder =>      
                { 
                    builder.WithOrigins("*");  // para permitir los metodos Get
                    builder.WithHeaders("*");  // para permitir los metodos Post
                    builder.WithMethods("*");  // para permitir los metodos Put y Delete
                });
            });

            // Para agregar al backend global las clases DecimalToString y IntToString

            services.AddControllers()    // hay que quitarle el punto y coma primero para que no de error...
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new IntToStringConverter());
                    options.JsonSerializerOptions.Converters.Add(new DecimalToStringConverter());
                });

            var appSettingsSection = Configuration.GetSection("AppSettings");
            
            services.Configure<AppSettings>(appSettingsSection);

            // Json Web Token

            var appSettings = appSettingsSection.Get<AppSettings>();

            var llave = Encoding.ASCII.GetBytes(appSettings.Secreto);

            services.AddAuthentication(d =>
            {
                d.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

                d.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
                .AddJwtBearer(d =>
                {                                  
                    d.RequireHttpsMetadata = false;
                    d.SaveToken = true;
                    d.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(llave),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
              
            services.AddScoped<IUserService, UserService>(); // para hacer inyeción
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(MiCors); // se agrego este codigo para el aceso a los metodos get, post, put, delete

            app.UseAuthentication(); // se agrego para el json token

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
