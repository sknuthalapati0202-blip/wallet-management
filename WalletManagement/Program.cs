using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using System.Text.Json;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;
using WalletManagement.Core.Domain.Models;
using WalletManagement.Core.Domain.Repositories;
using WalletManagement.Core.Domain.Services;
using WalletManagement.Core.Persistence.Repositories;
using WalletManagement.Core.Services;
using WalletManagement.Core.Utilities;
using WalletManagement.Middleware;
using WalletManagement.Utilities;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Info("Init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    await ConfigureServices(builder);

    builder.Host.UseNLog();

    builder.Services.AddProblemDetails();

    builder.Services.AddEndpointsApiExplorer();
    //builder.Services.AddSwaggerGen();
    builder.Services.AddSwaggerGen(options =>
    {
        // This is the most important line for syncing your code logic to the JSON file
        options.SupportNonNullableReferenceTypes();
    });
    builder.Services.AddSignalR();

    var app = builder.Build();

    logger.Info("WebApplication build successful");

    // For Proxy Servers
    string basePath = builder.Configuration["BasePath"] ?? "";
    if (!string.IsNullOrEmpty(basePath))
    {
        app.Use(async (context, next) =>
        {
            context.Request.PathBase = basePath;
            await next.Invoke();
        });
    }

    app.UseForwardedHeaders();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.ContentType = "application/json";

                var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();

                var response = new
                {
                    success = false,
                    message = exception?.Error.Message ?? "Unexpected error",
                    result = (object?)null
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            });
        });
    }

    app.UseRouting();

    app.UseStatusCodePages(async context =>
    {
        var request = context.HttpContext.Request;
        var response = context.HttpContext.Response;

        // Only intercept API routes
        if (request.Path.StartsWithSegments("/api"))
        {
            response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = response.StatusCode switch
                {
                    404 => "The requested resource was not found.",
                    400 => "Bad request.",
                    401 => "Unauthorized.",
                    403 => "Forbidden.",
                    415 => "Unsupported media type.",
                    _ => "An error occurred."
                },
                result = (object?)null
            });
            await response.WriteAsync(result);
        }
    });

    app.UseStaticFiles();

    app.UseCookiePolicy();

    //app.UseRouting();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseMiddleware<ExceptionHandlingMiddleWare>();

    app.UseCors("AllowAll");

    app.UseAuthentication();

    app.UseAuthorization();

    app.UseSession();

    app.MapControllers();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Login}/{action=Index}/{id?}");

    await app.RunAsync();
}
catch (Exception ex)
{
    logger.Error(ex, ex.Message);
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}

async Task ConfigureServices(WebApplicationBuilder builder)
{
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders =
            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        // Only loopback proxies are allowed by default.
        // Clear that restriction because forwarders are enabled by explicit 
        // configuration.
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
    });

    var environment = builder.Environment;
    // Load secrets from Vault only in Staging or Production
    if (environment.IsStaging() || environment.IsProduction())
    {
        var vaultAddress = builder.Configuration["Vault:Address"];
        var vaultToken = builder.Configuration["Vault:Token"];
        var secretPath = builder.Configuration["Vault:SecretPath"];

        // Initialize Vault client
        var authMethod = new TokenAuthMethodInfo(vaultToken);
        var vaultClientSettings = new VaultClientSettings(vaultAddress, authMethod);
        var vaultClient = new VaultClient(vaultClientSettings);

        // Fetch secret data from Vault
        Secret<SecretData> secret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(
            path: secretPath,
            mountPoint: "secret"
        );

        var data = secret.Data.Data;

        // Override configuration values
        var memoryConfig = new Dictionary<string, string>
        {
            ["ConnectionStrings:IDPConnString"] = data["ConnectionStrings:WalletConnString"]?.ToString() ?? ""
        };

        // Inject Vault secrets into configuration
        builder.Configuration.AddInMemoryCollection(memoryConfig);
    }
    else
    {
        Console.WriteLine("Skipping Vault secrets loading (Development environment).");
    }
    var idpConnectionString = builder.Configuration.GetConnectionString("IDPConnString");
    var redisConn = builder.Configuration["RedisConnString"];

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            policy => policy.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
    });


    builder.Services.AddRouting(options => options.LowercaseUrls = true);
    //builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
    // After:
    builder.Services.AddControllersWithViews()
        .AddRazorRuntimeCompilation()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.NumberHandling =
                System.Text.Json.Serialization.JsonNumberHandling.Strict;
        });


    builder.Services.AddAntiforgery(options => options.HeaderName = "XSRF-TOKEN");

    builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    });

    builder.Services.AddSession();

    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddCookie(Config =>
               {
                   Config.LoginPath = "/Login";
                   Config.Cookie.Name = "DTPlatform";
                   Config.LogoutPath = "/Logout";
                   Config.AccessDeniedPath = new PathString("/Error/401");
               });


    builder.Services.AddTransient<ExceptionHandlingMiddleWare>();
    builder.Services.AddScoped<Microsoft.Extensions.Logging.ILogger, Microsoft.Extensions.Logging.Logger<UnitOfWork>>();
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    builder.Services.AddScoped<IMessageLocalizer, MessageLocalizer>();
    builder.Services.AddScoped<Helper>();



    if (builder.Configuration.GetValue<bool>("EncryptionEnabled"))
    {
        idpConnectionString = PKIMethods.Instance.PKIDecryptSecureWireData(idpConnectionString);
    }

    builder.Services.AddDbContext<idp_dtplatformContext>(options =>
        options.UseNpgsql(idpConnectionString));

    builder.Services.AddHttpClient("ignoreSSL");

    var context = new CustomAssemblyLoadContext();

    if (builder.Environment.IsDevelopment())
    {
        context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), "libwkhtmltox.dll"));
    }
    else
    {
        context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), "libwkhtmltox.so"));
    }

    builder.Services.AddScoped<ITokenHelper, TokenHelper>();

    builder.Services.AddScoped<IOrganizationService,
        WalletManagement.Core.Services.OrganizationService>();
    builder.Services.AddSingleton<WalletManagement.Core.Utilities.BackgroundService>();
    //builder.Services.AddScoped<ICacheClient, CacheClient>();
    builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
    builder.Services.AddScoped<WalletManagement.Core.Utilities.IGlobalConfiguration, WalletManagement.Core.Utilities.GlobalConfiguration>();
    builder.Services.AddSingleton<IKafkaConfigProvider, KafkaConfigProvider>();
    builder.Services.AddScoped<IUserDataService, UserDataService>();
    builder.Services.AddScoped<IWalletDomainService, WalletDomainService>();
    builder.Services.AddScoped<IPasswordHasher<object>, PasswordHasher<object>>();

    builder.Services.AddScoped<Helper>();
    builder.Services.AddHttpClient<ISelfServiceConfigurationService, SelfServiceConfigurationService>();
    builder.Services.AddScoped<IRazorRendererHelper, RazorRendererHelper>();
    builder.Services.AddScoped<IHelper, Helper>();
    builder.Services.AddScoped<IWalletConfigurationService, WalletConfigurationService>();
    builder.Services.AddScoped<ICredentialService, CredentialService>();
    builder.Services.AddScoped<IProvisionStatusService, ProvisionStatusService>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<ICredentialVerifiersService, CredentialVerifiersService>();
    builder.Services.AddScoped<IQrCredentialService, QrCredentialService>();
    builder.Services.AddScoped<IQrCredentialVerifiersService, QrCredentialVerifiersService>();
    builder.Services.AddScoped<IWalletConsentService, WalletConsentService>();
}