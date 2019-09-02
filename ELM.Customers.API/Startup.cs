using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELM.Common;
using ELM.Common.DTO;
using ELM.Customers.API.Filters;
using ELM.Customers.API.Validators;
using ELM.Customers.Database.Context;
using ELM.Customers.Database.DAL;
using ELM.Customers.Services.Customer;
using ELM.Customers.Services.Queue;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;

namespace ELM.Customers.API
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

            services.AddMvc()
            .AddFluentValidation(fvc => {
                fvc.RegisterValidatorsFromAssemblyContaining<Startup>();
                fvc.ImplicitlyValidateChildProperties = true;
                fvc.RunDefaultMvcValidationAfterFluentValidationExecutes = true;

            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            SystemConstants.ConnectionString = Configuration.GetConnectionString("DbConnection");
            services.AddDbContext<ELMCustomersDbContext>(options =>
                 options.UseSqlServer(SystemConstants.ConnectionString));

            services.TryAddScoped(typeof(ELMCustomersDbContext), typeof(ELMCustomersDbContext));
            services.TryAddScoped(typeof(DbContext), typeof(ELMCustomersDbContext));
            services.TryAddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.TryAddScoped(typeof(ICustomerService), typeof(CustomerService));
            services.TryAddScoped(typeof(ICustomerPublisher), typeof(CustomerPublisher));
            //services.AddTransient<IValidator<CustomerDTO>, CreateCustomerValidator>();
            services.AddTransient<IValidator<List<CustomerDTO>>, CreateCustomerListValidator>();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Customers API", Version = "v1" });
            });

            services.AddLogging(builder =>
            {
                builder.AddConfiguration(Configuration.GetSection("Logging"))
                  .AddSerilog(new LoggerConfiguration().WriteTo.File($"{Configuration.GetSection("Logging").GetSection("Path").Value}").CreateLogger())
                  .AddConsole();
#if DEBUG
                builder.AddDebug();
#endif
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMiddleware<Middleware.RequestResponseLoggingMiddleware>();
            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Customers API V1");
                c.RoutePrefix = string.Empty;
            });


            #region Init DB
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            using (var context = scope.ServiceProvider.GetService<ELMCustomersDbContext>())
                context.Database.EnsureCreated();
            #endregion
        }
    }
}
