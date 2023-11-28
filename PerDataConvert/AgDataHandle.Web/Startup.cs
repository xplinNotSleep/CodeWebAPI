using AgDataHandle.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ServiceCenter.Core;
using ServiceCenter.Core.Aop;
using ServiceCenter.Core.Aop.Default;
using ServiceCenter.Core.Data;
using ServiceCenter.Core.Http;
using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace AgDataHandle.Web
{
    public class DateTimeConverterUsingDateTimeParse : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString());
        }
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }

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

            services.AddControllers();

            // If using Kestrel:
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            //services.AddPureProfiler();

            services.AddMvc(options =>
            {
                options.Filters.Add<CustomExceptionFilterAttribute>();
            }).AddJsonOptions(op =>
            {
                op.JsonSerializerOptions.PropertyNamingPolicy = null;
                op.JsonSerializerOptions.Converters.Add(new DateTimeConverterUsingDateTimeParse());
                op.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            })
    .AddControllersAsServices().AddRazorRuntimeCompilation();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AgDataHandle.Web", Version = "v1" });
                var xmlPath = Path.Combine(AppContext.BaseDirectory, typeof(Startup).Assembly.GetName().Name + ".xml");
                if (!File.Exists(xmlPath))
                {
                    throw new FileNotFoundException("�����������ļ���" + xmlPath);
                }
                c.IncludeXmlComments(xmlPath);
            });
            services.AddControllersWithViews();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowCorsOrigin", builder =>
                {
                    //�����������
                    builder
                     .AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader()
                     .SetPreflightMaxAge(TimeSpan.FromSeconds(2000000));
                });
            });

            //����ϴ��ļ�����
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = long.MaxValue; // In case of multipart
                x.MemoryBufferThreshold = int.MaxValue;
            });

            //AgCIM.Tools.GltfToI3s.ResourceReader.Init();
            string AppEnv = Configuration["AppEnv"];
            Bootstrapper.Start(b => b
               .UseDefaultDependencyInjection(services)
               .UseDefaultConfig()
               .UseRuntimeCache()
               .UseIdCreatorOfSnowflakeString()
               .UseIdCreatorOfTimestampLong()
               .UseDbContext(c =>
               {
                   c.Contexts.Add(typeof(FILEDbContext));
                   c.EntityAndServiceAssemblies.Add(typeof(FILEDbContext).Assembly);
                   //c.DbContextLifeStyle =  LifeStyle.Transient;

                   //�������ݿ����ӳ�
                   c.EnablePool = true;
                   c.DbContextLifeStyle = LifeStyle.Singleton;
                   c.ServiceLifeStyle = LifeStyle.Transient;
                   c.RepositoryLifeStyle = LifeStyle.Transient;
               }),
               config =>
               {
                   //Toolset.TinyLogger.WriteLog(AppEnv);
                   //���뵱ǰAssembly����ע��Controller
                   config.AppInfo.Init(GetType().Assembly);//��ʼ��Ӧ�û�����Ϣ
                   config.SetEnv(AppEnv);//����Ӧ�û���
                   config.LogWriter = (msg) => { Toolset.TinyLogger.WriteLog(msg); };
               });
            #region  ����ȫ������ע��
            services.AddGlobalHttpContextAccessor();
            services.AddHttpClient();
            #endregion

            //������ı�����
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddMemoryCache();//����MemoryCache
                                      // session ����
            services.AddSession(options =>
            {
                // ���� Session ����ʱ��
                options.IdleTimeout = TimeSpan.FromHours(3);
                options.Cookie.HttpOnly = true;
            });

            services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true)
                .Configure<IISServerOptions>(x => x.AllowSynchronousIO = true);

            //services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());
            //var provider = services.BuildDefaultServiceProvider();

            services.AddSingleton(Configuration);
            ServiceCenter.Core.Configs.ConfigurationManager.RegisterConfiguration(Configuration);
            Toolset.DIContainer.Build();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{

            //}
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AgDataHandle.Web v1");
            });
            app.UseRouting();
            app.UseAuthorization();
            app.UseCors("AllowCorsOrigin");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            #region ����ȫ�����õ���
            app.UseGlobalHttpContext();
            #endregion
            app.UseStaticFiles();
            var _3dtilesPath = Configuration["3dtilesPath"];
            if (!Directory.Exists(_3dtilesPath))
            {
                Directory.CreateDirectory(_3dtilesPath);
            }
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(_3dtilesPath),
                ServeUnknownFileTypes = true,
                RequestPath = new PathString("/3dtiles")
            });
            Task.Run(() =>
            {
                using (var dbContext = Toolset.DIContainer.Resolve<FILEDbContext>())
                {
                    dbContext.Close();
                }
            });
        }
    }
}
