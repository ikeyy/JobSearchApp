using FluentValidation;
using JobSearchApp.Application.Behavior;
using JobSearchApp.Application.Command.CreateContractor;
using JobSearchApp.Application.Command.CreateCustomer;
using JobSearchApp.Application.Command.CreateJob;
using JobSearchApp.Application.Command.CreateJobOffer;
using JobSearchApp.Application.Command.UpdateJob;
using JobSearchApp.Application.Command.UpdateJobOffer;
using JobSearchApp.Domain.Interfaces.Repository;
using JobSearchApp.Domain.Interfaces.Service;
using JobSearchApp.Domain.Services;
using JobSearchApp.Infrastructure.Repositories;
using MediatR;

namespace JobSearchApp.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            //CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // If you need to send cookies/authentication
                });
            });

            
            // MediatR
            //services.AddMediatR(cfg =>
            //cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

            // Register MediatR handlers, queries, and commands from Application assembly
            var applicationAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "JobSearchApp.Application");

            if (applicationAssembly != null)
            {
                services.AddMediatR(cfg =>
                    cfg.RegisterServicesFromAssembly(applicationAssembly));
            }

            // Register FluentValidation validators
            services.AddValidatorsFromAssembly(typeof(CreateContractorCommandValidator).Assembly);
            services.AddValidatorsFromAssembly(typeof(CreateCustomerCommandValidator).Assembly);
            services.AddValidatorsFromAssembly(typeof(CreateJobCommandValidator).Assembly);
            services.AddValidatorsFromAssembly(typeof(CreateJobOfferCommandValidator).Assembly);
            services.AddValidatorsFromAssembly(typeof(UpdateJobCommandValidator).Assembly);
            services.AddValidatorsFromAssembly(typeof(UpdateJobOfferCommandValidator).Assembly);

            // MediatR pipeline behavior to run FluentValidation validators for requests
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            
            //Services
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IJobService, JobService>();
            services.AddScoped<IJobOfferService, JobOfferService>();
            services.AddScoped<IContractorService, ContractorService>();

            // Unit of Work & Repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Repositories
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IJobOfferRepository, JobOfferRepository>();
            services.AddScoped<IContractorRepository, ContractorRepository>();

            return services;
        }
    }
}
