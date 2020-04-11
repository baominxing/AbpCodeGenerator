using ABPCodeGenerator.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;

namespace ABPCodeGenerator
{
    public class Startup
    {

        public Startup(
            IConfiguration configuration)
        {
            Console.WriteLine("StartUp");

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine("StartUp.ConfigureServices");

            services.AddControllersWithViews();

            #region 注册不同生命周期的服务

            //services.AddSingleton<ICPS8xCodeGeneratorService, CPS8xCodeGeneratorService>();

            //services.AddScoped<ICPS8xCodeGeneratorService, CPS8xCodeGeneratorService>();

            services.AddTransient<ICPS8xCodeGeneratorService, CPS8xCodeGeneratorService>();

            services.AddSingleton<IOrderService, OrderService>();

            //services.AddScoped<IOrderService, OrderService>();

            //services.AddTransient<IOrderService, OrderService>();

            #endregion

            #region 花式注册
            //var instance = new CPS8xCodeGeneratorService(this.razorViewEngine, this.tempDataProvider, this.serviceProvider);

            //services.AddSingleton<ICPS8xCodeGeneratorService>(instance);//直接注册服务实例

            //services.AddSingleton<ICPS8xCodeGeneratorService, CPS8xCodeGeneratorService>();

            //services.AddTransient<ICPS8xCodeGeneratorService>(factory => { return instance; });//工厂模式直接注册服务实例
            #endregion

            #region 尝试注册
            //services.TryAddTransient<ICPS8xCodeGeneratorService, CPS8xCodeGeneratorService>();//如果该接口已有注册过，则不再注册

            services.TryAddEnumerable(ServiceDescriptor.Singleton<ICPS8xCodeGeneratorService, CPS8xCodeGeneratorService>());//如果注册是是该接口的不同实现类，则可以注册

            services.TryAddEnumerable(ServiceDescriptor.Singleton<ICPS8xCodeGeneratorService, CPS8xCodeGeneratorService>());//如果注册是是该接口的不同实现类，则可以注册

            //services.TryAddEnumerable(ServiceDescriptor.Singleton<ICPS8xCodeGeneratorService>(new CPS8xCodeGeneratorService()));//如果注册是是该接口的不同实现类，则可以注册

            //services.TryAddEnumerable(ServiceDescriptor.Singleton<ICPS8xCodeGeneratorService>(p => { return new CPS8xCodeGeneratorService(); }));//如果注册是是该接口的不同实现类，则可以注册
            #endregion

            #region 移除和替换注册
            //services.RemoveAll<ICPS8xCodeGeneratorService>();//从容器中移除所有该接口的服务实例

            //services.Replace(ServiceDescriptor.Singleton<ICPS8xCodeGeneratorService, CPS8xCodeGeneratorService>());//从容器中替换原本实现了该接口的实现类为目标类
            #endregion

            #region 注册泛型模板
            services.AddSingleton(typeof(IGenerateService<>), typeof(GenerateService<>));
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {

            Console.WriteLine("StartUp.Configure");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            //app.UseIdentity();

            app.UseRouting();

            app.UseCors(builder => builder.WithOrigins("https://localhost:5001/"));

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Dashboard}/{action=Index}/{id?}");
            });
        }
    }
}
