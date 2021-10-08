using ProjetoOficinaWeb.Data.Entities;
using ProjetoOficinaWeb.Models;

namespace ProjetoOficinaWeb.Helpers
{
    public interface IConverterHelper
    {
        Vehicle ToVehicle(VehicleViewModel model, string path, bool isNew); // o bool é para o id quando se for editar, se nao for novo coloca por aqui o id

        VehicleViewModel ToVehicleViewModel(Vehicle vehicle);

        Service ToService(ServiceViewModel model, string path, bool isNew); // o bool é para o id quando se for editar, se nao for novo coloca por aqui o id

        ServiceViewModel ToServiceViewModel(Service service);
    }
}
