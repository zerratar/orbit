﻿using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
                options.AddPolicy("AllowAllOrigins", builder => builder.WithOrigins("http://localhost:8080", "*"));// builder.AllowAnyOrigin());
                options.AddPolicy("AllowAllMethods", builder => builder.AllowAnyMethod());
                options.AddPolicy("AllowAllHeaders", builder => builder.AllowAnyHeader());
            });

            services.AddSingleton<IPlayerSessionProvider, UserSessionProvider>();
            services.AddSingleton<IUserSessionManager, UserSessionManager>();
            services.AddSingleton<IPlayerSessionBinder, PlayerSessionBinder>();
            services.AddSingleton<IPlayerAuthenticator, PlayerAuthenticator>();
            services.AddSingleton<IPlayerConnectionHandler, PlayerConnectionHandler>();
            services.AddSingleton<INodeRepository, MemoryCachedFileBasedNodeRepository>();
            services.AddSingleton<IPlayerRepository, MemoryBasedPlayerRepository>();
            services.AddSingleton<INodeObserver, SessionNodeObserver>();
            services.AddSingleton<IConnectionProvider, ConnectionProvider>();
            services.AddSingleton<IPacketDataSerializer, JsonPacketDataSerializer>();
            services.AddSingleton<IPlayerPacketHandler, PlayerPacketHandler>();
            services.AddSingleton<INodeChangeTracker, NodeChangeTracker>();
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

            var game = app.ApplicationServices.GetService<IGame>();
            game.Begin(); // let the game begin!

            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };

            app.UseCors(builder =>
                builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin());

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
                        if (playerSession == null)
                        {
                            await webSocket.CloseAsync(
                                WebSocketCloseStatus.InternalServerError,
                                "Nope",
                                CancellationToken.None);
                        }


                        await playerSession.KeepAlive();
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
    }
}
