using Microsoft.EntityFrameworkCore;
using Persistencia.Models;
using Constatacion.Servicios.Contrato;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Constatacion.Servicios.Implementacion;

namespace Constatacion;

public class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        ////xxxxxxx

        builder.Services.AddAuthentication()
            .AddMicrosoftAccount(opciones =>
        {
            opciones.ClientId = builder.Configuration["MicrosoftClientId"]!;
            opciones.ClientSecret = builder.Configuration["MicrosoftSecretId"]!;
        });
        builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
                                opciones.UseSqlServer("name=DefaultConnection"));

        builder.Services.AddIdentity<IdentityUser, IdentityRole>(opciones =>
        {
            opciones.SignIn.RequireConfirmedAccount = false;
        })
                       .AddEntityFrameworkStores<ApplicationDbContext>()
                       .AddDefaultTokenProviders();
        builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
    opciones =>
    {
        opciones.LoginPath = "/usuarios/IniciarSesion";
        opciones.AccessDeniedPath = "/usuarios/IniciarSesion";
       

    });

        ///xxx
        // Configuiración cadena de conexión
        var conn = builder.Configuration.GetConnectionString("DefaultConnection");
        var dbOptions = new DbContextOptionsBuilder<ConstatacionContext>()
                .UseSqlServer(conn)
                .Options;
        builder.Services.AddSingleton(dbOptions);

        //cookies
        builder.Services.AddScoped<IUsuarioService, UsuarioService>();//servicios
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/usuarios/IniciarSesion";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

        ///cokies
        builder.Services.AddDbContext<ConstatacionContext>(options => {
            options.UseSqlServer(conn);
        });

        builder.Services.AddScoped<IUsuarioService, UsuarioService>();
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }


        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();



        app.MapControllerRoute(
            name: "default",

    pattern: "{controller=Usuarios}/{action=IniciarSesion}/{id?}");
        IWebHostEnvironment env = app.Environment;
        Rotativa.AspNetCore.RotativaConfiguration.Setup(env.WebRootPath, "../Rotativa/Windows");


        app.Run();
    }
}