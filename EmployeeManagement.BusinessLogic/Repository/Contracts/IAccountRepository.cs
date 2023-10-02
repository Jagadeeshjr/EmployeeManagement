using EmployeeManagement.Models.Models.Authentication;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.BusinessLogic.Repository.Contracts
{
    public interface IAccountRepository
    {
        Task<IdentityResult> SignUpAsync(SignUpModel signUpModel);

        Task<string> LoginAsync(SignInModel signInModel);
    }
}