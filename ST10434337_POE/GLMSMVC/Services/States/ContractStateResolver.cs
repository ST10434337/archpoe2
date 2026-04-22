using GLMSMVC.Models.Domain;
using GLMSMVC.Models.Enums;
using GLMSMVC.Services.Interfaces;

namespace GLMSMVC.Services.States
{
    // Decides which concrete state object to use
    public class ContractStateResolver // (Geekific. 2021b) 
    {
        private readonly DraftState _draftState;
        private readonly ActiveState _activeState;
        private readonly ExpiredState _expiredState;
        private readonly OnHoldState _onHoldState;

        public ContractStateResolver()
        {
            _draftState = new DraftState();
            _activeState = new ActiveState();
            _expiredState = new ExpiredState();
            _onHoldState = new OnHoldState();
        }

        public IContractState Resolve(Contract contract)
        {
            return contract.Status switch
            {
                ContractStatus.Active => _activeState,
                ContractStatus.Expired => _expiredState,
                ContractStatus.OnHold => _onHoldState,
                _ => _draftState
            };
        }

        public ContractStatus Evaluate(Contract contract)
        {
            var state = Resolve(contract);
            return state.EvaluateStatus(contract);
        }

        public bool CanCreateServiceRequest(Contract contract)
        {
            var state = Resolve(contract);
            return state.CanCreateServiceRequest(contract);
        }

        public bool CanEdit(Contract contract)
        {
            var state = Resolve(contract);
            return state.CanEdit(contract);
        }
    }
}
/*
 UNIT TEST ideas
incomplete contract returns Draft

valid complete contract returns Active

expired end date returns Expired

paused contract returns OnHold

active contract can create SR

draft contract cannot create SR

on-hold contract cannot create SR

only draft/on-hold can edit
 
 */


/*
 Geekific. 2021b. The State Pattern Explained and Implemented in Java | Behavioral 
Design Patterns | Geekific. [video online] Available at: 
<https://youtu.be/abX4xzaAsoc?si=MYqv2fg7g8fFL-rK> [Accessed 28 March 2026]. 
 
 */