using GLMSMVC.Data;
using GLMSMVC.Repositories;
using GLMSMVC.Repositories.Interfaces;
using GLMSMVC.Services;
using GLMSMVC.Services.Core_Services;
using GLMSMVC.Services.CurrencyConversion;
using GLMSMVC.Services.Factories;
using GLMSMVC.Services.Interfaces;
using GLMSMVC.Services.States;
using Microsoft.EntityFrameworkCore;

namespace GLMSMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // SQL Express 2022 local used 
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));//(Teddy Smith, 2022)

            // Database Access Repos
            builder.Services.AddScoped<IClientRepository, ClientRepository>();
            builder.Services.AddScoped<IContractRepository, ContractRepository>();
            builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();

            // File Handling Service - Download, Upload
            builder.Services.AddScoped<IContractFileService, ContractFileService>();

            // Currency Conversion Services (Facade Pattern)
            builder.Services.AddScoped<ICurrencyCalculator, CurrencyCalculator>();
            builder.Services.AddScoped<ICurrencyConversionFacade, CurrencyConversionFacade>();
            builder.Services.AddHttpClient<IExchangeRateProvider, ExchangeRateProvider>();

            // Factories for Contract and Service Request
            builder.Services.AddScoped<IContractFactory, ContractFactory>();
            builder.Services.AddScoped<IServiceRequestFactory, ServiceRequestFactory>();

            // States for Contract
            builder.Services.AddScoped<ContractStateResolver>();
            builder.Services.AddScoped<IContractState, DraftState>();
            builder.Services.AddScoped<IContractState, ActiveState>();
            builder.Services.AddScoped<IContractState, ExpiredState>();
            builder.Services.AddScoped<IContractState, OnHoldState>();

            // Core Services (Client, Contract, ServiceRequest)
            builder.Services.AddScoped<IClientService, ClientService>();
            builder.Services.AddScoped<IContractService, ContractService>();
            builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
/*
 https://www.youtube.com/watch?v=kFGSjTSCTCQ - sql express 2022
 https://www.youtube.com/watch?v=af_tK9LUiX0 - set up + migrations
 */