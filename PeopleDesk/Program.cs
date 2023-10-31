using CorePush.Apple;
using CorePush.Google;
using DinkToPdf;
using DinkToPdf.Contracts;
using Elastic.Apm.Api;
using Elastic.Apm.NetCoreAll;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;
using PeopleDesk;
using PeopleDesk.Data;
using PeopleDesk.Extensions;
using PeopleDesk.Helper;
using PeopleDesk.Middlewares;
using PeopleDesk.Models.PushNotify;
using PeopleDesk.Models.SignalR;
using System.Data;
using System.Net;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

var connectionString = string.Empty;

builder.WebHost.ConfigureKestrel(c =>
{
    c.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(30);
});

if (builder.Environment.IsDevelopment())
{
    connectionString = builder.Configuration.GetConnectionString("Development");
}
else
{
    connectionString = Environment.GetEnvironmentVariable("ConnectionString");
}

builder.Services.AddDbContext<PeopleDeskContext>(options =>
options.UseSqlServer(connectionString));

Connection.iPEOPLE_HCM = connectionString;


#region J W T - - - T O K E N - - - &&&& - - - A U T H - - - C O N F I G

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PeopleDesk-Web-Api", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Jwt Authorization",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id= "Bearer"
                }
        },
        new string[]{}
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        //ValidAudience = builder.Configuration["Jwt:Audience"],
        //ValidAudiences =
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"])),
        ClockSkew = TimeSpan.Zero
    };

    options.RequireHttpsMetadata = false;
    options.Events = new JwtBearerEvents();

    options.Events.OnChallenge = context =>
    {
        context.HandleResponse();

        var payload = new JObject
        {
            ["error"] = context.Error,
            ["statusCode"] = 401,
            ["error_description"] = context.ErrorDescription,
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 401;

        return context.Response.WriteAsync(payload.ToString());
    };
});

builder.Services.AddHttpContextAccessor();

IHttpContextAccessor httpContextAccessor = builder.Services.BuildServiceProvider().GetService<IHttpContextAccessor>();

AuthExtension.SetHttpContextAccessor(httpContextAccessor);

builder.Services.AddControllers(opts =>
{
    //opts.Filters.Add<AllowAnonymousFilter>();

    if (builder.Environment.IsDevelopment())
    {
        opts.Filters.Add<AllowAnonymousFilter>();
    }
    else
    {
        var authenticatedUserPolicy = new AuthorizationPolicyBuilder()
                  .RequireAuthenticatedUser()
                  .Build();
        opts.Filters.Add(new AuthorizeFilter(authenticatedUserPolicy));
    }
});

#endregion J W T - - - T O K E N - - - &&&& - - - A U T H - - - C O N F I G

#region PDF DI

var OsPlatform = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
var context = new CustomAssemblyLoadContext();
if (OsPlatform.Contains("Windows"))
{
    /* ==================Windows server===================*/
    context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), "libwkhtmltox.dll"));
}
else if (OsPlatform.Contains("linux"))
{
    /* ==================Linux server===================*/
    context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), "libwkhtmltox.so"));
}
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

#endregion PDF DI

DependencyContainer.RegisterServices(builder.Services, builder, builder.Configuration);


builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddDistributedMemoryCache();
//builder.Services.AddMemoryCache();
builder.Services.AddSignalR();

//builder.Services.Configure<FormOptions>(x =>
//{
//    x.ValueLengthLimit = int.MaxValue;
//    x.MultipartBodyLengthLimit = int.MaxValue;
//    x.MultipartHeadersLengthLimit = int.MaxValue;
//});

ServicePointManager.UseNagleAlgorithm = true;
ServicePointManager.Expect100Continue = true;
ServicePointManager.DefaultConnectionLimit = 5000;

#region ============= Push Notify ==================

builder.Services.AddHttpClient<FcmSender>();
builder.Services.AddHttpClient<ApnSender>();
var appSettingsSection = builder.Configuration.GetSection("FcmNotification");
builder.Services.Configure<FcmNotificationSetting>(appSettingsSection);

#endregion ============= Push Notify ==================

if (builder.Environment.IsProduction() || builder.Environment.IsStaging())
{
    builder.Services.StartQuartzJob();
}


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || !app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "api/peopleDesk/swagger/{documentName}/swagger.json";
    });

    //specifying the Swagger JSON endpoint.

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/api/peopleDesk/swagger/v1/swagger.json", "PeopleDesk API v1");
        c.RoutePrefix = "api/peopleDesk/swagger";
    });

    app.UseDeveloperExceptionPage();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

if (builder.Environment.IsProduction())
{
    app.UseMiddleware<UserValidityCheckedMiddleware>();
    //app.UseHangfireDashBoardExtension();
    //app.UseMiddleware<RateLimitMiddleware>();
}

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

if (builder.Environment.IsProduction() || !builder.Environment.IsDevelopment())
{
    app.UseWhen(httpContext => !httpContext.Request.Path.StartsWithSegments("/api/Document")
                                 && !httpContext.Request.Path.StartsWithSegments("/api/AuthApps")
                                 && !httpContext.Request.Path.StartsWithSegments("/api/PdfAndExcelReport")
                                 //&& !httpContext.Request.Path.StartsWithSegments("/api/MasterData/GetAllDistrictAllowAnonymous")
                                 && !httpContext.Request.Path.StartsWithSegments("/api/MasterData/POST_TO_DATA_SAVE"),
    // && ! httpContext.Request.Path.StartsWithSegments("/watchdog"),
    subApp => subApp.UseMiddleware<EnycriptionMiddleware>());
}

//app.MapRazorPages();

app.MapHub<NotificationHub>("/NotificationHub");

app.UseAllElasticApm(builder.Configuration);

app.Run();