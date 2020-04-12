using ABPCodeGenerator.Interceptors;
using ABPCodeGenerator.Services;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;

namespace ABPCodeGenerator
{
    public class Startup
    {
        public ILifetimeScope AutofacContainer { get; set; }

        public Startup(
            IConfiguration configuration)
        {
            Console.WriteLine("StartUp");

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        #region 默认依赖注入实现
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
        //public void Configure(IApplicationBuilder app, IHostEnvironment env)
        //{

        //    Console.WriteLine("StartUp.Configure");

        //    if (env.IsDevelopment())
        //    {
        //        app.UseDeveloperExceptionPage();
        //    }
        //    else
        //    {
        //        app.UseExceptionHandler("/Home/Error");
        //        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        //        app.UseHsts();
        //    }

        //    app.UseHttpsRedirection();

        //    app.UseStaticFiles();

        //    //app.UseIdentity();

        //    app.UseRouting();

        //    app.UseCors(builder => builder.WithOrigins("https://localhost:5001/"));

        //    app.UseAuthorization();

        //    app.UseEndpoints(endpoints =>
        //    {
        //        endpoints.MapControllerRoute(
        //            name: "default",
        //            pattern: "{controller=Dashboard}/{action=Index}/{id?}");
        //    });
        //}
        #endregion

        #region Autofac依赖注入实现
        public void ConfigureContainer(ContainerBuilder builder)
        {
            ////默认注册
            //builder.RegisterType<OrderService>().As<IOrderService>();
            ////命名注册
            //builder.RegisterType<OrderService>().Named<IOrderService>("orderservice");
            ////属性注册
            //builder.RegisterType<OrderNameService>();
            //builder.RegisterType<OrderService>().Named<IOrderService>("orderservice2").PropertiesAutowired();
            ////使用拦截器
            //builder.RegisterType<MyInterceptor>();
            //builder.RegisterType<OrderService>().Named<IOrderService>("orderservice3").PropertiesAutowired().InterceptedBy(typeof(MyInterceptor)).EnableClassInterceptors();

            #region 子容器
            builder.RegisterType<OrderNameService>().InstancePerMatchingLifetimeScope("myscope");//myscope的容器对象会被myscope2的容器对象覆盖
            builder.RegisterType<OrderNameService>().InstancePerMatchingLifetimeScope("myscope2");
            #endregion
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            //var service1 = this.AutofacContainer.Resolve<IOrderService>();
            //service1.Show();

            //var orderservice = this.AutofacContainer.ResolveNamed<IOrderService>("orderservice");
            //orderservice.Show();

            //var orderservice2 = this.AutofacContainer.ResolveNamed<IOrderService>("orderservice2");
            //orderservice2.Show();

            //var orderservice3 = this.AutofacContainer.ResolveNamed<IOrderService>("orderservice3");
            //orderservice3.Show();


            using (var myscpoe = this.AutofacContainer.BeginLifetimeScope("myscope2"))
            {
                var s4 = myscpoe.Resolve<OrderNameService>();

                using (var myscpoe2 = myscpoe.BeginLifetimeScope("myscope"))
                {
                    var s5 = myscpoe2.Resolve<OrderNameService>();
                    var s6 = myscpoe2.Resolve<OrderNameService>();

                    Console.WriteLine($"s4==s5?{s4 == s5}");
                    Console.WriteLine($"s4==s6?{s4 == s6}");
                    Console.WriteLine($"s5==s6?{s5 == s6}");
                }
            }

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
        #endregion
    }
}
