using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shinobytes.Core;
using Shinobytes.Orbit.Server;

namespace Orbit
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

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder => builder.AllowAnyOrigin());
                options.AddPolicy("AllowAllMethods", builder => builder.AllowAnyMethod());
                options.AddPolicy("AllowAllHeaders", builder => builder.AllowAnyHeader());
            });

            services.AddSingleton<IPlayerSessionProvider, PlayerSessionProvider>();
            services.AddSingleton<IPlayerSessionManager, PlayerSessionManager>();
            services.AddSingleton<IPlayerSessionBinder, PlayerSessionBinder>();
            services.AddSingleton<IPlayerAuthenticator, PlayerAuthenticator>();
            services.AddSingleton<IPlayerRepository, MemoryBasedPlayerRepository>();
            services.AddSingleton<IConnectionProvider, ConnectionProvider>();
            services.AddSingleton<Shinobytes.Core.ILogger, Shinobytes.Core.SyntaxHighlightedConsoleLogger>();
            services.AddSingleton<IGame, Game>();

            services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
            services.AddSession();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ApplicationServices.GetService<IGame>().Begin(); // let the game begin!

            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };

            app.UseSession();
            app.UseWebSockets(webSocketOptions);
            app.UseMvc();
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    var playerSessionProvider = context.RequestServices.GetService<IPlayerSessionProvider>();
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        var playerSession = await playerSessionProvider.GetAsync(webSocket);

                        await Echo(context, webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }
            });
        }

        private async Task Echo(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
