﻿using QRMenuAPI.Data;
using Microsoft.EntityFrameworkCore;
using QRMenuAPI.Models;
using Microsoft.AspNetCore.Identity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Security.Claims;

namespace QRMenuAPI;

public class Program
{
    public static void Main(string[] args)
    {
        State state;
        IdentityRole identityRole;
        ApplicationUser applicationUser;
        Company? company = null;

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<ApplicationDBContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDatabase")));
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDBContext>().AddDefaultTokenProviders();
        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();
        {
            ApplicationDBContext? context = app.Services.CreateScope().ServiceProvider.GetService<ApplicationDBContext>();
            if (context != null)
            {
                context.Database.Migrate();
                if (context.States.Count() == 0)
                {
                    state = new State();
                    state.Id = 0;
                    state.Name = "Deleted";
                    context.States.Add(state);
                    state = new State();
                    state.Id = 1;
                    state.Name = "Active";
                    context.States.Add(state);
                    state = new State();
                    state.Id = 2;
                    state.Name = "Passive";
                    context.States.Add(state);
                }
                if (context.Companies.Count() == 0)
                {
                    company = new Company();
                    company.Address = "adres";
                    company.EMail = "abc@def.com";
                    company.Name = "Company";
                    company.Phone = "1112223344";
                    company.PostalCode = "12345";
                    company.RegisterDate = DateTime.Today;
                    company.StateId = 1;
                    company.TaxNumber = "11111111111";
                    context.Companies.Add(company);
                }
                context.SaveChanges();
                RoleManager<IdentityRole>? roleManager = app.Services.CreateScope().ServiceProvider.GetService<RoleManager<IdentityRole>>();
                if (roleManager != null)
                {
                    if (roleManager.Roles.Count() == 0)
                    {
                        identityRole = new IdentityRole("Administrator");
                        roleManager.CreateAsync(identityRole).Wait();
                        identityRole = new IdentityRole("CompanyAdministrator");
                        roleManager.CreateAsync(identityRole).Wait();
                        identityRole = new IdentityRole("RestaurantAdministrator");
                        roleManager.CreateAsync(identityRole).Wait();
                    }
                }
                UserManager<ApplicationUser>? userManager = app.Services.CreateScope().ServiceProvider.GetService<UserManager<ApplicationUser>>();
                if (userManager != null)
                {
                    if (userManager.Users.Count() == 0)
                    {
                        if (company != null)
                        {
                            Claim claim;
                            
                            applicationUser = new ApplicationUser();
                            applicationUser.UserName = "Administrator";
                            applicationUser.CompanyId = company.Id;
                            applicationUser.Name = "Administrator";
                            applicationUser.Email = "abc@def.com";
                            applicationUser.PhoneNumber = "1112223344";
                            applicationUser.RegisterDate = DateTime.Today;
                            applicationUser.StateId = 1;
                            userManager.CreateAsync(applicationUser, "Admin123!").Wait();
                            userManager.AddToRoleAsync(applicationUser, "Administrator").Wait();
                            claim = new Claim("CompanyId", company.Id.ToString());
                            userManager.AddClaimAsync(applicationUser, claim).Wait();
                        }
                    }
                }
            }

        }
        app.Run();
    }
}