namespace Otlob.Core.Entities;

public sealed class BankAccount : AuditEntity
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }
    public string BankCertificateId { get; set; } = string.Empty;

    public string AccountHolderName { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public EgyptianBanks BankName { get; set; }
    public string Iban { get; set; } = string.Empty;
    public DateOnly BankCertificateIssueDate { get; set; }
    public DocumentStatus Status { get; set; } = DocumentStatus.Pending;

    public Restaurant Restaurant { get; set; } = null!;
    public UploadedFile BankCertificate { get; set; } = null!;
}

public enum AccountType
{
    [Display(Name = "Saving Account")]
    SavingsAccount,

    [Display(Name = "Checking Account")]
    CheckingAccount
}

public enum EgyptianBanks
{
    [Display(Name = "National Bank of Egypt (NBE)")]
    NationalBankOfEgypt_NBE,

    [Display(Name = "Banque Misr (BM)")]
    BanqueMisr_BM,

    [Display(Name = "Banque du Caire")]
    BanqueDuCaire,

    [Display(Name = "Commercial International Bank (CIB)")]
    CommercialInternationalBank_CIB,

    [Display(Name = "QNB Alahli")]
    QNBAlahli,

    [Display(Name = "Bank of Alexandria")]
    BankOfAlexandria,

    [Display(Name = "Credit Agricole Egypt")]
    CreditAgricoleEgypt,

    [Display(Name = "Arab African International Bank")]
    ArabAfricanInternationalBank,

    [Display(Name = "HSBC Bank Egypt")]
    HSBCBankEgypt,

    [Display(Name = "Emirates NBD Egypt")]
    EmiratesNBDEgypt,

    [Display(Name = "Abu Dhabi Islamic Bank (ADIB)")]
    AbuDhabiIslamicBank_ADIB,

    [Display(Name = "Abu Dhabi Commercial Bank (ADCB)")]
    AbuDhabiCommercialBank_ADCB,

    [Display(Name = "First Abu Dhabi Bank (FAB)")]
    FirstAbuDhabiBank_FAB,

    [Display(Name = "Mashreq Bank Egypt")]
    MashreqBankEgypt,

    [Display(Name = "Al Baraka Bank Egypt")]
    AlBarakaBankEgypt,

    [Display(Name = "Faisal Islamic Bank of Egypt")]
    FaisalIslamicBankOfEgypt,

    [Display(Name = "Suez Canal Bank")]
    SuezCanalBank,

    [Display(Name = "Egyptian Gulf Bank (EGBank)")]
    EgyptianGulfBank_EGBank,

    [Display(Name = "Arab International Bank (AIB)")]
    ArabInternationalBank_AIB,

    [Display(Name = "The United Bank")]
    TheUnitedBank,

    [Display(Name = "Housing and Development Bank (HDB)")]
    HousingAndDevelopmentBank_HDB,

    [Display(Name = "Egyptian Arab Land Bank (EALB)")]
    EgyptianArabLandBank_EALB,

    [Display(Name = "Industrial Development Bank (IDB)")]
    IndustrialDevelopmentBank_IDB,

    [Display(Name = "Bank ABC Egypt")]
    BankABCEgypt,

    [Display(Name = "Ahli United Bank (AUB)")]
    AhliUnitedBank_AUB,

    [Display(Name = "SAIB Bank")]
    SAIBBank,

    [Display(Name = "MIDBANK")]
    MIDBANK,

    [Display(Name = "National Bank of Kuwait (NBK)")]
    NationalBankOfKuwait_NBK,

    [Display(Name = "Arab Bank PLC - Egypt")]
    ArabBankPLC_Egypt,

    [Display(Name = "Citibank Egypt")]
    CitibankEgypt,

    [Display(Name = "Export Development Bank of Egypt (EBE)")]
    ExportDevelopmentBankOfEgypt_EBE,

    [Display(Name = "Agricultural Bank of Egypt (ABE)")]
    AgriculturalBankOfEgypt_ABE,

    [Display(Name = "Capital Bank of Egypt")]
    CapitalBankOfEgypt,

    [Display(Name = "Housing & Mortgage Finance Bank")]
    HousingAndMortgageFinanceBank,

    [Display(Name = "Arab Investment Bank (aiBANK)")]
    ArabInvestmentBank_aiBANK,

    [Display(Name = "Bank of China Egypt")]
    BankOfChinaEgypt
}
