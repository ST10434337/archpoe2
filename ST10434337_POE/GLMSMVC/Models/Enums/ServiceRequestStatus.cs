namespace GLMSMVC.Models.Enums
{
    // Status that a clients Contract can be in. Tracks the compleation/ readiness for action to be taken.
    // Contract Status checks Progress of contract
    public enum ServiceRequestStatus
    {
        Pending,
        Completed,
        Cancelled,
        OnHold
    }
}
