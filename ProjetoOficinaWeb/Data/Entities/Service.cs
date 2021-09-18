namespace ProjetoOficinaWeb.Data.Entities
{
    public class Service : IEntity
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public int Price { get; set; }
    }
}
