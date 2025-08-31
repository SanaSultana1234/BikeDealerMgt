using BikeDealerMgtAPI.Models;
using BikeDealerMgtAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;

// Add services to the container.

//builder.Services.AddControllers();

//To avoid cycles in Include()
builder.Services.AddControllers().AddJsonOptions(x =>
		x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
	);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Add AuthDbContext
builder.Services.AddDbContext<AuthDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("BikeCon"))); // use a separate DB or same DB with prefix tables

builder.Services.AddIdentity<AuthUser, IdentityRole>()
	.AddEntityFrameworkStores<AuthDbContext>()
	.AddDefaultTokenProviders();


//Add BikeDealerMgtDbContext
builder.Services.AddDbContext<BikeDealerMgmtDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("BikeCon")));

//Add Services for the DI
builder.Services.AddScoped<IBikeService, BikeService>();
builder.Services.AddScoped<IDealerService, DealerService>();
builder.Services.AddScoped<IDealerMasterService, DealerMasterService>();

//Authentication
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
	options.SaveToken = true;
	options.RequireHttpsMetadata = false;
	options.TokenValidationParameters = new TokenValidationParameters()
	{
		ValidateIssuer = false,
		ValidateAudience = false,
		ValidAudience = configuration["JWT:ValidAudience"],
		ValidIssuer = configuration["JWT:ValidIssuer"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
	};
});

var app = builder.Build();

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


/*
BIKE STORE DBFIRST
Scaffold-DbContext "Data Source=LAPTOP-5C1CRU1C;Initial Catalog=BikeDealerMgmtDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True" -Provider Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -verbose

Scaffold-DbContext "Data Source=LAPTOP-5C1CRU1C;Initial Catalog=BikeDealerMgmtDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Context BikeDealerMgmtDbContext -DataAnnotations -Force

IDENTITY CODEFIRST
Add-Migration AddIdentityTables -Context AuthDbContext
Update-Database -Context AuthDbContext
*/
