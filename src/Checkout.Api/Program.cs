using Checkout.Api.Extensions;
using Checkout.Api.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Once built we can access the ConfigurationManager as normal
var app = builder.Build();

#region JWT Configuration & Authentication
IConfigurationSection jwtOptions = app.Configuration.GetSection(nameof(JwtIssuerOptions));

// TODO:// The Secret should really be coming from a secure place and not appsettings.json
SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions[nameof(JwtIssuerOptions.SecretKey)]));

// Add services to the container.
builder.Services.Configure<JwtIssuerOptions>(options =>
{
    options.Issuer = jwtOptions[nameof(JwtIssuerOptions.Issuer)];
    options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
});

// Check we got a valid value from the appsettings.json file, if not default to 5 minutes
double.TryParse(jwtOptions[nameof(JwtIssuerOptions.ClockSkew)], out double clockSkew);
if (clockSkew <= 0.0D)
{
    clockSkew = 5;
}

// Use Policy Authentication and specify the policy requirements
builder.Services.AddAuthorization(options => { options.AddPolicy("KataCheckout", policy => policy.RequireClaim("KataCheckout", "True")); });

// Setup the authentication - Note this must be before the MVC call
builder.Services.AddAuthentication(options => { options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {

            // The signing key must match
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,

            // Validate the JWT Issuer claim
            ValidateIssuer = true,
            ValidIssuer = jwtOptions[nameof(JwtIssuerOptions.Issuer)],

            // Validate the JWT Audience claim
            ValidateAudience = false,

            // Validate the expiry token
            RequireExpirationTime = true,
            ValidateLifetime = true,

            // Set clock drift allowance - ideally zero but user clocks are never correct!
            ClockSkew = TimeSpan.FromMinutes(clockSkew)
        };

        options.SecurityTokenValidators.Clear();
        options.SecurityTokenValidators.Add(new RevokableJwtSecurityHandler());
    });
#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
