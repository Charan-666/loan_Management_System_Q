namespace Kanini.LMP.Database.Enums
{
    public enum DocumentStatus
    {
        Pending,
        Verified,
        Rejected,
        RequiresResubmission
    }

    public enum DocumentType
    {
        IdentityProof,
        AddressProof,
        IncomeProof,
        BankStatement,
        PropertyDocuments,
        VehicleDocuments,
        Other
    }
}