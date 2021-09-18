using ProjetoOficinaWeb.Data.Entities;
using ProjetoOficinaWeb.Models;

namespace ProjetoOficinaWeb.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public Vehicle ToVehicle(VehicleViewModel model, string path, bool isNew)
        {
            return new Vehicle
            {
                Id = model.Id,
                ImageUrl = path,
                LicensePlate = model.LicensePlate,
                Brand = model.Brand,
                Model = model.Model,
                Color = model.Color,
                Year = model.Year,
                User = model.User
            };
        }

        public VehicleViewModel ToVehicleViewModel(Vehicle vehicle)
        {
            return new VehicleViewModel
            {
                Id = vehicle.Id,
                ImageUrl = vehicle.ImageUrl,
                LicensePlate = vehicle.LicensePlate,
                Brand = vehicle.Brand,
                Model = vehicle.Model,
                Color = vehicle.Color,
                Year = vehicle.Year,
                User = vehicle.User
            };
        }

        public Mechanic ToMechanic(MechanicViewModel model, string path, bool isNew)
        {
            return new Mechanic
            {
                Id = model.Id,
                ImageUrl = path,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Adress = model.Adress,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                User = model.User
            };
        }

        public MechanicViewModel ToMechanicViewModel(Mechanic mechanic)
        {
            return new MechanicViewModel
            {
                Id = mechanic.Id,
                ImageUrl = mechanic.ImageUrl,
                FirstName = mechanic.FirstName,
                LastName = mechanic.LastName,
                Adress = mechanic.Adress,
                PhoneNumber = mechanic.PhoneNumber,
                Email = mechanic.Email,
                User = mechanic.User
            };
        }

        public Receptionist ToReceptionist(ReceptionistViewModel model, string path, bool isNew)
        {
            return new Receptionist
            {
                Id = model.Id,
                ImageUrl = path,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Adress = model.Adress,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                User = model.User
            };
        }

        public ReceptionistViewModel ToReceptionistViewModel(Receptionist receptionist)
        {
            return new ReceptionistViewModel
            {
                Id = receptionist.Id,
                ImageUrl = receptionist.ImageUrl,
                FirstName = receptionist.FirstName,
                LastName = receptionist.LastName,
                Adress = receptionist.Adress,
                PhoneNumber = receptionist.PhoneNumber,
                Email = receptionist.Email,
                User = receptionist.User
            };
        }

        public Client ToClient(ClientViewModel model, string path, bool isNew)
        {
            return new Client
            {
                Id = model.Id,
                ImageUrl = path,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Adress = model.Adress,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                User = model.User
            };
        }

        public ClientViewModel ToClientViewModel(Client client)
        {
            return new ClientViewModel
            {
                Id = client.Id,
                ImageUrl = client.ImageUrl,
                FirstName = client.FirstName,
                LastName = client.LastName,
                Adress = client.Adress,
                PhoneNumber = client.PhoneNumber,
                Email = client.Email,
                User = client.User
            };
        }
    }
}
