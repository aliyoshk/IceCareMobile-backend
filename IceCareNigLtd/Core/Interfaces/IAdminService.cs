using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Request;
using IceCareNigLtd.Api.Models.Response;
using IceCareNigLtd.Api.Models.Users;
using IceCareNigLtd.Core.Entities.Users;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Core.Interfaces
{
    public interface IAdminService
    {
        Task<Response<AdminDto>> AddAdminAsync(AdminDto adminDto);
        Task<Response<List<AdminDto>>> GetAdminsAsync();
        Task<Response<object>> DeleteAdminAsync(int adminId);
        Task<Response<AdminResponseDto>> LoginAsync(AdminLoginDto adminLoginDto);


        // Add mobile onboarding 
        Task<Response<List<UserDto>>> GetUsersByStatusAsync(string status);
        Task<Response<string>> ChangeUserStatusAsync(ChangeUserStatusRequest request, string adminName = null);
        Task<Response<List<UserDto>>> GetApprovedUsersAsync();
        Task<Response<List<UserDto>>> GetRejectedUsersAsync();
        Task<Response<object>> DeleteUserAsync(int userId);

        Task<Response<List<TransferResponse>>> GetPendingTransferAsync();
        Task<Response<List<TransferResponse>>> GetApprovedTransferAsync();
        Task<Response<string>> ConfirmTransferAsync(ConfirmationRequest request, string adminName = null);
        Task<Response<List<TransferResponse>>> GetUsersByTransferStatusAsync(string status);
        Task<Response<object>> DeleteTransferRecordAsync(int id);

        Task<Response<List<TransferResponse>>> GetAccountPaymentAsync(string status);
        Task<Response<string>> ConfirmAccountPaymentAsync(int id, string adminName = null);
        Task<Response<object>> DeleteAccountPaymentAsync(int id);

        Task<Response<List<ThirdPartyPaymentResponse>>> GetThirdPartyTransfer(string status);
        Task<Response<string>> ThirdPartyTransferCompleted(int id, string adminName = null);
        Task<Response<object>> DeleteThirdPartyTransferAsync(int id);

        Task<Response<List<AccountTopUpResponse>>> GetAccountTopUpsAsync(string status);
        Task<Response<string>> ConfirmAccountTopUp(ConfirmationRequest request, string adminName = null);
        Task<Response<object>> DeleteAccountTopUpAsync(int id);
    }
}
