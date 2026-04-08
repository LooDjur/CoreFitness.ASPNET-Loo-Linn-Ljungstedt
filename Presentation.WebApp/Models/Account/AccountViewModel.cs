namespace Presentation.WebApp.Models.Account;

public class AccountViewModel
{
    public AboutMeViewModel Profile { get; set; } = new();
    public MyMembershipViewModel Membership { get; set; } = new();
}
