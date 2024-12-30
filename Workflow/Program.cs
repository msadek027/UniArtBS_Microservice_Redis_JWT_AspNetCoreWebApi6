using Workflow;
using Workflow.Cache;
using Workflow.WorkflowCommon;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using Workflow.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;


DBConnection conn = new DBConnection();
var builder = WebApplication.CreateBuilder(args);
// Access the configuration
var configuration = builder.Configuration;

// Add services to the container
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddDbContext<DbContextClass>(options => options.UseSqlServer(configuration.GetConnectionString("SAConn")));
builder.Services.AddMvc();
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder.AllowAnyMethod().AllowAnyHeader().AllowCredentials().SetIsOriginAllowed((hosts) => true));
    options.AddPolicy("AllowAll", builder => builder.AllowAnyMethod().AllowAnyHeader().AllowCredentials().SetIsOriginAllowed((hosts) => true));
});

// Add services to the container.
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", builder =>
//    {
//        builder.AllowAnyOrigin()
//               .AllowAnyMethod()
//               .AllowAnyHeader();
//    });
//});


//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSwaggerAggregator", policy =>
//    {
//        policy.AllowAnyHeader()
//              .AllowAnyMethod()
//              .WithOrigins("http://localhost:5069"); // Add the domain of your Swagger UI
//    });
//});

// JWT Authentication configuration
var key = "This is my first Test Key This is my first Test Key";// Use your actual secret key here, ideally from a secure source
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Set to true in production for secure communications
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false, // on production make it true
        ValidateAudience = false, // on production make it true
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)), //if var key = "This is my first Test Key";                                                                     // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
        ClockSkew = TimeSpan.Zero
        // IssuerSigningKey = new SymmetricSecurityKey(key) //if var key Encoding.UTF8.GetBytes(Configuration["JWT:Key"]);
        //or
        //ValidAudience = Configuration["JWT:ValidAudience"],
        //ValidIssuer = Configuration["JWT:ValidIssuer"],
        //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
    };
});

builder.Services.AddSingleton<IJwtAuth>(new Auth(key));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    //  c.SwaggerDoc("v1", new OpenApiInfo { Title = "JSON Web JWT Token ASP.NET Core Web Api 5.0", Version = "v1" });
    c.EnableAnnotations();
 
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWT Authorization Microservice Redis Cache AspNetCoreWebApi6",
        Version = "v1",
        Contact = new OpenApiContact
        {
            Name = "Documents v1",
            Url = new Uri("http://172.16.201.17:84/swagger/index.html")          
        },
      
    });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, new string[] { } } });

    //SwaggerControllerOrder<ControllerBase> swaggerControllerOrder = new SwaggerControllerOrder<ControllerBase>(Assembly.GetEntryAssembly());
    SwaggerControllerOrder<ControllerBase> swaggerControllerOrder = new SwaggerControllerOrder<ControllerBase>(Assembly.GetExecutingAssembly());
    //c.OrderActionsBy((apiDesc) => $"{swaggerControllerOrder.SortKey(apiDesc.ActionDescriptor.RouteValues["controller"])}");
    c.OrderActionsBy(apiDesc => $"{swaggerControllerOrder.SortKey(apiDesc.ActionDescriptor.RouteValues["controller"])}.{apiDesc.HttpMethod}");
});
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100MB (example)
});
var app = builder.Build();
app.UseStaticFiles();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DefaultModelsExpandDepth(-1); // Disables displaying models (schemas)     
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Workflow v1");
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Documents v1");    
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Security v1");
        options.InjectStylesheet("/swagger/swagger-ui.css");
        options.InjectJavascript("/swagger/custom-swagger.js"); // Path relative to wwwroot  
    });
}

app.UseRouting();
app.UseCors("CorsPolicy");
//app.UseCors("AllowAll");
//app.UseCors("AllowSpecificOrigins");
//app.UseCors("AllowSwaggerAggregator");
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
});
app.Run();
