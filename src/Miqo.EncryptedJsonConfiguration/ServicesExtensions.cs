using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miqo.EncryptedJsonConfiguration
{
    /// <summary>
    /// Extension methods for making the application settings available as an injectable singleton.
    /// </summary>
    public static class ServicesExtensions
    {
        /// <summary>
        /// Returns the encrypted JSON settings as;
        /// </summary>
        /// <typeparam name="TSettings">The application's settings</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/></param>
        /// <param name="configuration">The <see cref="IConfiguration"/></param>
        /// <param name="filter">Optional. Set if you only want to load part of the options into your settings application's</param>
        public static void GetEncryptedConfig<TSettings>(this IServiceCollection services, IConfiguration configuration, string filter = null)
            where TSettings : class, new()
        {
            TSettings settings = string.IsNullOrEmpty(filter)
                ? configuration.Get<TSettings>()
                : configuration.GetSection(filter).Get<TSettings>();

            services.AddSingleton(settings);
        }

        /// <summary>
        /// Loads the application settings into an injectable singleton.
        /// </summary>
        /// <typeparam name="TSettings">The application's settings</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/></param>
        /// <param name="configuration">The <see cref="IConfiguration"/></param>
        /// <param name="filter">Optional. Set if you only want to load part of the options into your settings application's</param>
        public static void AddJsonEncryptedSettings<TSettings>(this IServiceCollection services, IConfiguration configuration, string filter = null)
            where TSettings : class, new()
        {
            TSettings settings = string.IsNullOrEmpty(filter)
                ? configuration.Get<TSettings>()
                : configuration.GetSection(filter).Get<TSettings>();

            services.AddSingleton(settings);
        }
    }
}
