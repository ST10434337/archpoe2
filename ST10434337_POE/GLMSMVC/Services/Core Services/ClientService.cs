using GLMSMVC.Models.Domain;
using GLMSMVC.Repositories.Interfaces;
using GLMSMVC.Services.Interfaces;
using GLMSMVC.Services.Results;

namespace GLMSMVC.Services.Core_Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<ServiceResult<IEnumerable<Client>>> GetAllAsync()
        {
            var clients = await _clientRepository.GetAllAsync();
            return ServiceResult<IEnumerable<Client>>.Success(clients);
        }

        public async Task<ServiceResult<Client>> GetByIdAsync(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);

            if (client == null)
            {
                return ServiceResult<Client>.Failure("Client not found.");
            }

            return ServiceResult<Client>.Success(client);
        }

        public async Task<ServiceResult<Client>> CreateAsync(Client client)
        {
            var errors = ValidateClient(client);
            if (errors.Any())
            {
                return ServiceResult<Client>.Failure(errors);
            }

            await _clientRepository.AddAsync(client);
            await _clientRepository.SaveChangesAsync();

            return ServiceResult<Client>.Success(client);
        }

        // Updates an existing client
        public async Task<ServiceResult<Client>> UpdateAsync(Client client)
        {
            var existingClient = await _clientRepository.GetByIdAsync(client.Id);
            if (existingClient == null)
            {
                return ServiceResult<Client>.Failure("Client not found.");
            }

            var errors = ValidateClient(client);
            if (errors.Any())
            {
                return ServiceResult<Client>.Failure(errors);
            }

            existingClient.Name = client.Name;
            existingClient.ContactDetails = client.ContactDetails;
            existingClient.Region = client.Region;

            _clientRepository.Update(existingClient);
            await _clientRepository.SaveChangesAsync();

            return ServiceResult<Client>.Success(existingClient);
        }

        // Performs basic validation rules for Client entities and returns list of error messages
        private static List<string> ValidateClient(Client client)
        {
            var errors = new List<string>();

            // Name is Required
            if (string.IsNullOrWhiteSpace(client.Name))
                errors.Add("Client name is required.");

            // Region is Required
            if (string.IsNullOrWhiteSpace(client.Region))
                errors.Add("Region is required.");

            // Enforce maximum length constraints on fields to match persistence/schema limits
            if (!string.IsNullOrWhiteSpace(client.Name) && client.Name.Length > 100)
                errors.Add("Client name cannot exceed 100 characters.");

            if (!string.IsNullOrWhiteSpace(client.ContactDetails) && client.ContactDetails.Length > 100)
                errors.Add("Contact details cannot exceed 100 characters.");

            if (!string.IsNullOrWhiteSpace(client.Region) && client.Region.Length > 75)
                errors.Add("Region cannot exceed 75 characters.");

            return errors;
        }
    }
}
