using ProjetoOficinaWeb.Data.Entities;
using ProjetoOficinaWeb.Models;

namespace ProjetoOficinaWeb.Helpers
{
    public interface IConverterHelper
    {
        Vehicle ToVehicle(VehicleViewModel model, string path, bool isNew); // o bool é para o id quando se for editar, se nao for novo coloca por aqui o id

        VehicleViewModel ToVehicleViewModel(Vehicle vehicle);

        Mechanic ToMechanic(MechanicViewModel model, string path, bool isNew); // o bool é para o id quando se for editar, se nao for novo coloca por aqui o id

        MechanicViewModel ToMechanicViewModel(Mechanic mechanic);

        Receptionist ToReceptionist(ReceptionistViewModel model, string path, bool isNew); // o bool é para o id quando se for editar, se nao for novo coloca por aqui o id

        ReceptionistViewModel ToReceptionistViewModel(Receptionist receptionist);

        Client ToClient(ClientViewModel model, string path, bool isNew); // o bool é para o id quando se for editar, se nao for novo coloca por aqui o id

        ClientViewModel ToClientViewModel(Client client);

        Service ToService(ServiceViewModel model, string path, bool isNew); // o bool é para o id quando se for editar, se nao for novo coloca por aqui o id

        ServiceViewModel ToServiceViewModel(Service service);
    }
}
