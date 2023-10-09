namespace Service.Identity.Application.Policies.Contracts;

public class MinimalPolicyResponseModel
{
    public long Id { get; set; }
    public string Policy { get; set; }
    public byte Order { get; set; }
}