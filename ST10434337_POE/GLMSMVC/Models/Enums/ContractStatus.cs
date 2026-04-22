namespace GLMSMVC.Models.Enums
{
    // Seperte Status to Contract Status. Tracks status of the 'action' of what has taken place.
    // Service Request Status checks progress of delivery activity
    public enum ContractStatus
    {
        Draft,
        Active,
        Expired,
        OnHold
    }
}
