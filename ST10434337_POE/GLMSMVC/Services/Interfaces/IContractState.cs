using GLMSMVC.Models.Domain;
using GLMSMVC.Models.Enums;

namespace GLMSMVC.Services.Interfaces
{
    public interface IContractState // (Geekific. 2021b) 
    {
        ContractStatus Status { get; }

        ContractStatus EvaluateStatus(Contract contract); // Auto Status Logic

        bool CanCreateServiceRequest(Contract contract); // Blocks when criteria not met

        bool CanEdit(Contract contract); // Checks if Draft/On Hold to be edited... Edit Btn disappear in view
    }
}
/*
 Geekific. 2021b. The State Pattern Explained and Implemented in Java | Behavioral 
Design Patterns | Geekific. [video online] Available at: 
<https://youtu.be/abX4xzaAsoc?si=MYqv2fg7g8fFL-rK> [Accessed 28 March 2026]. 
 
 */
