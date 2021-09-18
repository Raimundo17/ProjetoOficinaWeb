﻿using System;
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
            }

            var isInRole = await _userHelper.IsUserInRoleAsync(user, "Admin");
            if (!isInRole)
            {
                await _userHelper.AddUserToRoleAsync(user, "Admin");
            }

            if (!_context.vehicles.Any()) // se nao existirem veículos
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

            if (!_context.Mechanics.Any()) // se nao existirem serviços
            {
                AddMechanic("Ronald","Reagen","Rua R",123456789,"ronaldregan@gmail.com",user);
                AddMechanic("Jimmy", "Carter", "Rua J", 987654321, "jimmycarter@gmail.com", user);
                AddMechanic("Gerald", "Ford", "Rua G", 135791357, "geraldford@gmail.com", user);
                AddMechanic("Richard", "Nixon", "Rua R", 246802468, "richardnixon@gmail.com", user);
                AddMechanic("Lyndon", "Johnson", "Rua L", 357903579, "lyndonjohnson@gmail.com", user);
                AddMechanic("John", "Kennedy", "Rua J", 954739578, "johnkennedy@gmail.com", user);
                await _context.SaveChangesAsync(); // grava na base de dados
            }

            if (!_context.Receptionists.Any()) // se nao existirem serviços
            {
                AddReceptionist("Madeleine", "Swann", "Rua M", 123456789, "madeleineswann@gmail.com", user);
                AddReceptionist("Lucia", "Sciarra", "Rua L", 123456789, "luciasciarra@gmail.com", user);
                AddReceptionist("Eve", "Moneypenny", "Rua E", 123456789, "evemoneypenny@gmail.com", user);
                AddReceptionist("Camille", "Montes", "Rua C", 123456789, "camillemontes@gmail.com", user);
                await _context.SaveChangesAsync(); // grava na base de dados
            }

            if (!_context.Clients.Any()) // se nao existirem serviços
            {
                AddClient("Bill", "Gates", "Rua B", 123456789, "billgates@gmail.com", user);
                AddClient("Jeff", "Bezzos", "Rua J", 987654321, "jeffbezzos@gmail.com", user);
                AddClient("Warren", "Buffett", "Rua W", 135791357, "warrenbuffett@gmail.com", user);
                AddClient("Bernad", "Arnault", "Rua B", 246802468, "bernardarnault@gmail.com", user);
                AddClient("Larry", "Ellison", "Rua L", 357903579, "larryellison@gmail.com", user);
                AddClient("Carlos", "Slim", "Rua C", 954739578, "carlosslim@gmail.com", user);
                await _context.SaveChangesAsync(); // grava na base de dados
            }
        }

        private void AddVehicle(string licensePlate, string brand, string model, string color, User user)
        {
            _context.vehicles.Add(new Vehicle
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

        private void AddMechanic(string firstName, string lastName, string adress, int phoneNumber, string email,User user)
        {
            _context.Mechanics.Add(new Mechanic
            {
                FirstName = firstName,
                LastName = lastName,
                Adress = adress,
                PhoneNumber = phoneNumber,
                Email = email,
                User = user
            });
        }

        private void AddReceptionist(string firstName, string lastName, string adress, int phoneNumber, string email, User user)
        {
            _context.Receptionists.Add(new Receptionist
            {
                FirstName = firstName,
                LastName = lastName,
                Adress = adress,
                PhoneNumber = phoneNumber,
                Email = email,
                User = user
            });
        }

        private void AddClient(string firstName, string lastName, string adress, int phoneNumber, string email, User user)
        {
            _context.Clients.Add(new Client
            {
                FirstName = firstName,
                LastName = lastName,
                Adress = adress,
                PhoneNumber = phoneNumber,
                Email = email,
                User = user
            });
        }
    }
}