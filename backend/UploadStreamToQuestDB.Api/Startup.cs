using Common.RabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSSql.Infrastructure;
using Questdb.Net;
using Serilog;
using System;
using UploadStreamToQuestDB.API.Hub;
using UploadStreamToQuestDB.API.Middlewares.GlobalExceptions.Handler;
using UploadStreamToQuestDB.API.SwaggerFilters;
using UploadStreamToQuestDB.Application;
using UploadStreamToQuestDB.Infrastructure;

namespace UploadStreamToQuestDB.API {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddProblemDetails(options =>
                   options.CustomizeProblemDetails = ctx =>
                       ctx.ProblemDetails.Extensions.Add("nodeId", Environment.MachineName));
            services.AddExceptionHandler<GlobalExceptionHandler>();

            services.AddSignalR();
            services.AddCors();
            services.AddControllers();
            services.AddQuestDb(Configuration["ReadQuestDbAddress"]);
            services.AddInfrastructureDependencies(Configuration);
            services.AddMSSQLServices();
            services.AddApplicationDependencies(Configuration);
            services.AddRabbitMQServices();

            services.Configure<FormOptions>(x => {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = long.MaxValue;
            });

            services.AddSwaggerGen(options => {
                options.EnableAnnotations();
                options.OperationFilter<AddCustomHeaderParameter>();
                options.OperationFilter<FileUploadOperation>();
            });

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            services.AddHostedService<BackgroundNotifier>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            app.UseExceptionHandler();

            app.UseRouting();

            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseCors(p => {
                //p.WithOrigins("http://localhost:8081")
                    p.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapHub<AgentsStatusHub>("/agentsDetails");
            });


            var builder = WebApplication.CreateBuilder();
            builder.Host.UseSerilog();
        }
    }
}
