namespace Kanini.LMP.Database.Enums
{
    public enum CreditBureau
    {
        CIBIL,
        Experian,
        Equifax,
        CRIF
    }

    public enum CreditScoreRange
    {
        Poor = 300,      // 300-549
        Fair = 550,      // 550-649
        Good = 650,      // 650-749
        VeryGood = 750,  // 750-799
        Excellent = 800  // 800-850
    }

    public enum CreditStatus
    {
        Active,
        Closed,
        Settled,
        WrittenOff,
        Overdue
    }
}