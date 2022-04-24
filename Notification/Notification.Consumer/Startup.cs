using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notification.Commom.Settings;
using Notification.Messenger.Interfaces;
using Notification.Messenger.Models;
using Notification.Messenger.Services;

namespace Notification.Consumer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<Consumer>();
            services.AddTransient<IMessengerService<Email>, SendGridService>();

            services.Configure<RabbitMqConfig>(Configuration.GetSection(nameof(RabbitMqConfig)));
            services.Configure<SendGridConfig>(Configuration.GetSection(nameof(SendGridConfig)));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
        }
    }
}
