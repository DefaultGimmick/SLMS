using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using SLMS.Application.Books;
using SLMS.Application.Categories;
using SLMS.Application.Users;
using SLMS.Infrastructure.Caching;
using SLMS.Infrastructure.MessageQueue;
using SLMS.Models.SLMS.EntityFrameworkCore;
using SLMS.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SLMS.API
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
            #region ����Jwt
            //��ȡappsettings.json�ļ���������֤����Կ��Secret�������ڣ�Aud����Ϣ
            var audienceConfig = Configuration.GetSection("Audience");
            //��ȡ��ȫ��Կ
            var signingKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(audienceConfig["Secret"]));
            //tokenҪ��֤�Ĳ�������
            var tokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,//������֤��ȫ��Կ
                IssuerSigningKey = signingKey,//��ֵ��ȫ��Կ
                ValidateIssuer = true,//������֤ǩ����
                ValidIssuer = audienceConfig["Iss"],//��ֵǩ����
                ValidateAudience = true,//������֤����
                ValidAudience = audienceConfig["Aud"],//��ֵ����
                ValidateLifetime = true,//�Ƿ���֤Token��Ч�ڣ�ʹ�õ�ǰʱ����Token��Claims�е�NotBefore��Expires�Ա�
                ClockSkew = TimeSpan.Zero,//����ķ�����ʱ��ƫ����
                RequireExpirationTime = true,//�Ƿ�Ҫ��Token��Claims�б������Expires
            };
            //��ӷ�����֤������ΪTestKey
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = "TestKey";
            })
            .AddJwtBearer("TestKey", x =>
            {
                x.RequireHttpsMetadata = false;
                //��JwtBearerOptions�����У�IssuerSigningKey(ǩ����Կ)��ValidIssuer(Token�䷢����)��ValidAudience(�䷢��˭)���������Ǳ���ġ�
                x.TokenValidationParameters = tokenValidationParameters;
            });

            services.AddOptions();
            services.Configure<Audience>(Configuration.GetSection("Audience"));
            #endregion

            #region ���ÿ���
            services.AddCors(options =>
                {
                    options.AddPolicy("CorsPolicy", builder =>
                    {
                        builder.AllowAnyOrigin() //��������Origin����

                               //�����������󷽷���Get,Post,Put,Delete
                               .AllowAnyMethod()

                               //������������ͷ:application/json
                               .AllowAnyHeader();
                    });
                });
            #endregion

            #region ����Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("V1", new OpenApiInfo
                {
                    Title = "Swagger�ӿ��ĵ�",
                    Version = "V1",
                    Description = "����Web API",
                });
            });
            #endregion

            #region ������־
            services.AddLogging(logBuilder => {
                logBuilder.ClearProviders();
            });
            #endregion

            #region ����Sql Server
            services.AddDbContext<SLMSDBContext>(opt =>
               opt.UseSqlServer(Configuration.GetConnectionString("SLMSDB")));
            #endregion

            #region ����Redis
            services.AddSingleton<RedisContext>();
            #endregion

            #region ����RabbitMQ
            services.Configure<RabbitMQOptions>(Configuration.GetSection("RabbitMQOptions"));
            services.AddSingleton<RabbitMQClient>();
            services.AddSingleton<RabbitMQOptions>();
            services.AddSingleton<MessageProducer>();
            services.AddSingleton<MessageConsumer>();
            #endregion

            #region ���÷�����
            services.AddScoped<IBookAppService, BookAppService>();
            services.AddScoped<IUserAppService, UserAppService>();
            services.AddScoped<ICategoryAppService, CategoryAppService>();
            services.AddScoped<JwtUtils>();
            #endregion

            #region ���Session
            services.AddSession(config =>
            {
                config.Cookie.IsEssential = true;
                config.Cookie.Name = "SLMS.session";
                config.Cookie.HttpOnly = true;
                config.IdleTimeout = TimeSpan.FromMinutes(30);
            });
            services.AddDistributedMemoryCache();
            #endregion
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSession();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint($"/swagger/V1/swagger.json", "V1");
                opt.RoutePrefix = string.Empty;
            });
            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseCors();

            app.UseAuthentication(); // ��֤
            app.UseAuthorization();  // ��Ȩ

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
