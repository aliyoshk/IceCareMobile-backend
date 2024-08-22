using System;
namespace IceCareNigLtd.Api.Middleware
{
    public class CustomerNotFoundException : Exception
    {
        public string EntityName { get; }
        public int EntityId { get; }

        public CustomerNotFoundException(string entityName, int entityId)
            : base($"{entityName} with ID {entityId} was not found.")
        {
            EntityName = entityName;
            EntityId = entityId;
        }


        //public CustomerNotFoundException(int customerId)
        //    : base($"Customer with ID {customerId} was not found.")
        //{ }
    }
}
