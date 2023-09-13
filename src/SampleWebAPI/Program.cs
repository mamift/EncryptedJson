using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Miqo.EncryptedJsonConfiguration;

namespace SampleWebAPI
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) {
            var vars = Environment.GetEnvironmentVariables().Cast<DictionaryEntry>()
                .OrderBy(d => d.Key.ToString())
                .ToDictionary(k => k.Key.ToString(), v => v.Value?.ToString());
            var keyFromEnv = vars["Key"];

            if (string.IsNullOrWhiteSpace(keyFromEnv)) throw new InvalidOperationException("No key provided!");
            var key = Convert.FromBase64String(keyFromEnv);

            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) => {
                    config.AddEncryptedJsonFile("settings.ejson", key);
                })
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
