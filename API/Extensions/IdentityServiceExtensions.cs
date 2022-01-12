using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Persistence;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            // Made a service for user registration. 
            services.AddIdentityCore<AppUser>(opt =>
            {
                // Passwords don't require a special character to be made and stored
                opt.Password.RequireNonAlphanumeric = false;
            })
            // Pointing to the database that we are storing our users in
            .AddEntityFrameworkStores<DataContext>()
            // Assinging a Sign in manager for user collection
            .AddSignInManager<SignInManager<AppUser>>();

            // Generate a key signatrue for our tokens. Points to dev appSettings for secret phrase.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
            // A service to put our jwt into bearer hearder in our http requests
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(opt =>
                     {   
                     // Adding validation options to our jwt
                        opt.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = key,
                            ValidateIssuer = false,
                            ValidateAudience = false
                        };
                    });
                    // Adds TokenService cause it the token layout we ar using to this service
                    services.AddScoped<TokenService>();
            // Return service to use in startup
            return services;
        }
    }
}