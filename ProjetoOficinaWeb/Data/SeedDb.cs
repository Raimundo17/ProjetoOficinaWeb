using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjetoOficinaWeb.Data.Entities;
using ProjetoOficinaWeb.Helpers;

namespace ProjetoOficinaWeb.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private Random _random;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync(); // ver se a base de dados está criada (o seed ao correr cria a tabela das migrações)

            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Customer");
            await _userHelper.CheckRoleAsync("Mechanic");
            await _userHelper.CheckRoleAsync("Receptionist");

            var user = await _userHelper.GetUserByEmailAsync("daniel.raimundo.21229@formandos.cinel.pt"); // ver se ja existe utilizador, o primeiro a ser criado é o admin pela propria aplicacao
            if (user == null) // se nao existir
            {
                user = new User
                {
                    FirstName = "Daniel",
                    LastName = "Raimundo",
                    Email = "daniel.raimundo.21229@formandos.cinel.pt",
                    UserName = "daniel.raimundo.21229@formandos.cinel.pt",
                    PhoneNumber = "123456789"
                };

                var result = await _userHelper.AddUserAsync(user, "123456");
                // os dois paramentros a passar é o user e a password á parte do objeto para poder ser
                // encriptada
                if (result != IdentityResult.Success) // se houve algum problema a criar
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                await _userHelper.AddUserToRoleAsync(user, "Admin");
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);   //video 33
                await _userHelper.ConfirmEmailAsync(user, token);  //video 33
            }

            var isInRole = await _userHelper.IsUserInRoleAsync(user, "Admin");
            if (!isInRole)
            {
                await _userHelper.AddUserToRoleAsync(user, "Admin");
            }

            if (!_context.Vehicles.Any()) // se nao existirem veículos
            {
                AddVehicle("22-GG-44", "Opel", "Astra", "White", user);
                AddVehicle("07-DF-21", "Fiat", "500", "Red", user);
                AddVehicle("89-XX-37", "Toyota", "Yaris", "Grey", user);
                AddVehicle("15-PR-12", "Renault", "Clio", "Blue", user);
                AddVehicle("57-QP-56", "Ford", "Focus", "White", user);
                AddVehicle("06-TB-44", "Volkswagen", "Golf", "Yellow", user);
                await _context.SaveChangesAsync(); // grava na base de dados
            }

            if (!_context.Services.Any()) // se nao existirem serviços
            {
                AddService("Steering alignment",30);
                AddService("Alternator", 300);
                AddService("shock absorbers", 400);
                AddService("Air Conditioner", 70);
                AddService("Batterie", 60);
                AddService("Free Checkup", 0);
                AddService("Clutch", 500);
                AddService("Exhaust System", 250);
                AddService("Tires", 100);
                AddService("Brake System", 180);
                AddService("Oil Change", 30);
                AddService("Lighting", 25);
                await _context.SaveChangesAsync(); // grava na base de dados
            }
        }

        private void AddVehicle(string licensePlate, string brand, string model, string color, User user)
        {
            _context.Vehicles.Add(new Vehicle
            {
                LicensePlate = licensePlate,
                Brand = brand,
                Model = model,
                Color = color,
                Year = _random.Next(1995, 2020),
                User = user
            });
        }

        private void AddService(string description, int price)
        {
            _context.Services.Add(new Service
            {
                Description = description,
                Price = price,
            });
        }
    }
}
