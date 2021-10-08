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

        public Service ToService(ServiceViewModel model, string path, bool isNew)
        {
            return new Service
            {
                Id = model.Id,
                Description = model.Description,
                Price = model.Price,
            };
        }

        public ServiceViewModel ToServiceViewModel(Service service)
        {
            return new ServiceViewModel
            {
                Id = service.Id,
                Description = service.Description,
                Price = service.Price,
            };
        }
    }
}
