using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.AspNetCore.Builder;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace ABPCodeGenerator
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            #region ���ÿ��
            //ConfigurationDemo();
            #endregion

            #region �����������ṩ����
            ConfigurationCommandLineDemo(args);
            #endregion

            #region �������������ṩ����

            #endregion

            #region �ļ������ṩ����
            //ConfigurationFileDemo();
            #endregion

            CreateHostBuilder(args).Build().Run();
        }

        private static void ConfigurationFileDemo()
        {
            var configurationBuilder = new ConfigurationBuilder();

            configurationBuilder.AddJsonFile("", optional: true, reloadOnChange: false);
            configurationBuilder.AddIniFile("", optional: true, reloadOnChange: false);


            //������ӵ��ļ��Ḳ��ǰ����ļ��������
        }

        private static void ConfigurationCommandLineDemo(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder();

            //configurationBuilder.AddCommandLine(args);

            #region �����滻
            var mapper = new Dictionary<string, string>() { { "--k1", "Ckey1" } };
            configurationBuilder.AddCommandLine(args, mapper);
            #endregion

            IConfigurationRoot configurationRoot = configurationBuilder.Build();

            Console.WriteLine($"Ckey1:{configurationRoot["CKey1"]}");
            Console.WriteLine($"Ckey2:{configurationRoot["CKey2"]}");
        }

        private static void ConfigurationDemo()
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>()
            {
                {"key1","value1"},
                {"key2","value2"},
                {"section1:key3","value3"},
                {"section1:key4","value4"},
                {"section1:key5","value5"},
                {"section2:key6","value6"},
                {"section2:key7","value7"},
                {"section2:key8","value8"},
                {"section2:section3:key9","value9"},
                {"section2:section3:key10","value10"},
            });

            IConfigurationRoot configurationRoot = configurationBuilder.Build();

            Console.WriteLine(configurationRoot["key1"]);
            Console.WriteLine(configurationRoot["key2"]);

            IConfigurationSection configurationSection = configurationRoot.GetSection("section1");


            Console.WriteLine(configurationSection["key322"]);
            Console.WriteLine(configurationSection["key4"]);
            Console.WriteLine(configurationSection["key5"]);
            Console.WriteLine(configurationSection["key6"]);
            Console.WriteLine(configurationSection["key7"]);
            Console.WriteLine(configurationSection["key8"]);


            IConfigurationSection configurationSection2 = configurationRoot.GetSection("section2");

            Console.WriteLine(configurationSection2["key3"]);
            Console.WriteLine(configurationSection2["key4"]);
            Console.WriteLine(configurationSection2["key5"]);
            Console.WriteLine(configurationSection2["key6"]);
            Console.WriteLine(configurationSection2["key7"]);
            Console.WriteLine(configurationSection2["key8"]);

            IConfigurationSection configurationSection31 = configurationSection.GetSection("section3");
            Console.WriteLine(configurationSection31["key9"]);
            Console.WriteLine(configurationSection31["key10"]);

            IConfigurationSection configurationSection32 = configurationSection2.GetSection("section3");
            Console.WriteLine(configurationSection32["key9"]);
            Console.WriteLine(configurationSection32["key10"]);
        }

        #region Ĭ������ע��ʵ��
        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //  Host.CreateDefaultBuilder(args)
        //  .ConfigureWebHost(builder =>
        //  {
        //      Console.WriteLine("ConfigureWebHost");
        //  })
        //  .ConfigureAppConfiguration(builder =>
        //  {
        //      Console.WriteLine("ConfigureAppConfiguration");
        //  })
        //  .ConfigureLogging(webBuilder =>
        //  {
        //      Console.WriteLine("ConfigureLogging");
        //  })
        //  .ConfigureServices(builder =>
        //  {
        //      Console.WriteLine("ConfigureServices");
        //  })
        //  .ConfigureHostConfiguration(webBuilder =>
        //  {
        //      Console.WriteLine("ConfigureHostConfiguration");
        //  })
        //  .ConfigureWebHostDefaults(webBuilder =>
        //  {
        //      Console.WriteLine("ConfigureWebHostDefaults");

        //      webBuilder.UseStartup<Startup>();
        //  })
        //  ;
        #endregion

        #region Autofac����ע��ʵ��

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHostDefaults(webBuilder =>
            {
                Console.WriteLine("ConfigureWebHostDefaults");

                webBuilder.UseStartup<Startup>();
            })
            ;
        #endregion
    }
}
