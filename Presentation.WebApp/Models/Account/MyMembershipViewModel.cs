namespace Presentation.WebApp.Models.Account;

public class MyMembershipViewModel
{
    public bool HasMembership { get; set; }
    public string? MembershipPlan { get; set; }
    public bool IsActive { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string? MemberId { get; set; }
}
