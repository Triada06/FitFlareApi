using FitFlare.Application.Contracts.Requests;
using FitFlare.Application.DTOs.Admin.Admins;
using FitFlare.Application.DTOs.Ban;

namespace FitFlare.Application.Services.Interfaces;

public interface IAdminsService
{
    public Task<IEnumerable<AdminDto>> GetAllAdminsAsync();
    public Task MakeAppOwnerAsync(string id);
    public Task ConfirmOwnerTransferAsync(string token);
    public Task PromoteToAdminAsync(string id);
    public Task RemoveAdminAsync(string id);
    public Task<AdminDto> FindByEmail(string email);
}