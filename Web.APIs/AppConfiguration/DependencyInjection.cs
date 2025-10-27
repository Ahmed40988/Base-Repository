
using Microsoft.AspNetCore.Identity.UI.Services;
using Web.Application.Settings;

namespace Web.APIs
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddControllersConfig()
                .AddCorsConfig()
                .AddSwaggerWithAuth()
                .AddDatabaseConfig(configuration)
                .AddIdentityConfig()
                .AddEmailConfig(configuration)
                .AddAppServices()
                .AddAuthorizationConfig()
                .AddFluentValidationConfig()
                .AddMapsterConfig(configuration)
                .AddJwtAuthConfig(configuration);

            return services;
        }


        private static IServiceCollection AddControllersConfig(this IServiceCollection services)
        {
            services.AddControllers();
            return services;
        }


        private static IServiceCollection AddCorsConfig(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            return services;
        }

        private static IServiceCollection AddSwaggerWithAuth(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Web API",
                    Version = "v1"
                });

                // ✅ Enable JWT Auth in Swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token with Bearer prefix (Example: 'Bearer eyJhbGciOi...')",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }

        private static IServiceCollection AddDatabaseConfig(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }

        private static IServiceCollection AddIdentityConfig(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }

        private static IServiceCollection AddAuthorizationConfig(this IServiceCollection services)
        {
            services.AddAuthorization();
            return services;
        }


        private static IServiceCollection AddEmailConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
           
            return services;
        }

        private static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailSender, EmailService>();
            services.AddMemoryCache();

            return services;
        }

        private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation()
                    .AddValidatorsFromAssemblies(new[]
                    {
                        Assembly.GetExecutingAssembly(),
                        typeof(LoginDTO).Assembly,
                        typeof(AppDbContext).Assembly
                    });

            return services;
        }

        private static IServiceCollection AddMapsterConfig(this IServiceCollection services, IConfiguration configuration)
        {
            var baseUrl = configuration["BaseURL"];
            var mappingConfig = TypeAdapterConfig.GlobalSettings;
            MapsterConfiguration.RegisterMappings(mappingConfig, baseUrl);
            services.AddSingleton<IMapper>(new Mapper(mappingConfig));

            return services;
        }

        private static IServiceCollection AddJwtAuthConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JWT:Key"] ??
                            throw new InvalidOperationException("JWT Key is missing")))
                };
            });

            return services;
        }
    }
}
