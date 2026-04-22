using GLMSMVC.Models.Domain;
using GLMSMVC.Models.Enums;
using GLMSMVC.Services.Interfaces;

namespace GLMSMVC.Services.States
{
    public class DraftState : IContractState // (Geekific. 2021b) 
    {
        public ContractStatus Status => ContractStatus.Draft;

        public ContractStatus EvaluateStatus(Contract contract)
        {
            if (contract.IsPaused)
                return ContractStatus.OnHold;

            if (contract.EndDate.HasValue && contract.EndDate.Value.Date < DateTime.UtcNow.Date)
                return ContractStatus.Expired;

            if (IsComplete(contract))
                return ContractStatus.Active;

            return ContractStatus.Draft;
        }

        public bool CanCreateServiceRequest(Contract contract)
        {
            return false;
        }

        public bool CanEdit(Contract contract)
        {
            return true;
        }

        private static bool IsComplete(Contract contract)
        {
            return contract.StartDate.HasValue
                && contract.EndDate.HasValue
                && contract.ServiceLevel.HasValue
                && !string.IsNullOrWhiteSpace(contract.FilePath);
        }
    }
}
/*
 Geekific. 2021b. The State Pattern Explained and Implemented in Java | Behavioral 
Design Patterns | Geekific. [video online] Available at: 
<https://youtu.be/abX4xzaAsoc?si=MYqv2fg7g8fFL-rK> [Accessed 28 March 2026]. 
 
 */