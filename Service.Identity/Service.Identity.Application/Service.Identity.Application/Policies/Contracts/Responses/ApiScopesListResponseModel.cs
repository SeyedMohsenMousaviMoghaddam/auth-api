namespace Service.Identity.Application.Policies.Contracts;

public class ApiScopesListResponseModel
{
    public string ApiScope { get; set; }
    public List<MinimalPolicyResponseModel> Policies { get; set; }
}