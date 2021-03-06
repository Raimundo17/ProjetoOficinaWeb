using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using ProjetoOficinaWeb.Data;
using ProjetoOficinaWeb.Data.Entities;
using ProjetoOficinaWeb.Helpers;
using System.Text;

namespace ProjetoOficinaWeb
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
            services.AddIdentity<User, IdentityRole>(cfg =>
            {
                cfg.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider; //video 33 token necessario para login
                cfg.SignIn.RequireConfirmedEmail = true; //temos de ativar conta no email
                cfg.User.RequireUniqueEmail = true;
                cfg.Password.RequireDigit = false; // obrigatoriedade para as passwords terem varios caracters, numeros etc... COLOCAR A TRUE
                cfg.Password.RequiredUniqueChars = 0;
                cfg.Password.RequireUppercase = false;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequiredLength = 6;
            })
                 .AddDefaultTokenProviders() //video 33
                 .AddEntityFrameworkStores<DataContext>(); // separa o datacontext do "dele" do "meu"
                                                           //depois da pessoa fazer o login usa o DataContext "simples", o dele tem mais seguranša

            //Token
            services.AddAuthentication()//video 33 servišos para o token 
                .AddCookie()
                .AddJwtBearer(cfg =>
                {
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = this.Configuration["Tokens:Issuer"],
                        ValidAudience = this.Configuration["Tokens:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(this.Configuration["Tokens:Key"]))
                    };
                });

            services.AddDbContext<DataContext>(cfg => // cria um servišo do DataContext, injeto o meu
            {
                cfg.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"));   // usar o sqlsever com esta connection string
            });

            services.AddTransient<SeedDb>(); // quando alguem chamar pelo SeedDb cria-o na factory do Program.cs
                                             //AddTransient -> Usa e deita fora (deixa de ficar em memˇria) e nŃo pode ser mais usado 
            services.AddScoped<IUserHelper, UserHelper>();
            services.AddScoped<IImageHelper, ImageHelper>();
            services.AddScoped<IMailHelper, MailHelper>();
            services.AddScoped<IConverterHelper, ConverterHelper>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IRepairRepository, RepairRepository>();
            services.AddControllersWithViews();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/NotAuthorized"; // em vez de reedirecionar para o login envia para a action Not Authorize (anula o retorno que definimos no login)
                options.AccessDeniedPath = "/Account/NotAuthorized"; // quando houver um acesso negado (Authorize) corre esta
            });
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                app.UseExceptionHandler("/Errors/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseStatusCodePagesWithReExecute("/error/{0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // Middleware (Sistema operativo que mantŕm a aplicašŃo)
                                     // AtenšŃo ß sequŕncia 

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
