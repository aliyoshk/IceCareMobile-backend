using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Request;
using IceCareNigLtd.Api.Models.Response;
using IceCareNigLtd.Api.Models.Users;
using IceCareNigLtd.Core.Entities.Users;

namespace IceCareNigLtd.Core.Interfaces.Users
{
	public interface IUserService
	{
        Task<Response<Registration>> RegisterUserAsync(RegistrationDto registrationDto);
        Task<Response<LoginResponse>> LoginUserAsync(LoginDto loginDto);
        Task<Response<bool>> ResetUserLoginAsync(ResetPasswordRequest resetPasswordRequest);
        Task<Response<bool>> FundTransferAsync(TransferRequest transferRequest);
        Task<Response<bool>> AccountPaymentAsync(AccountPaymentRequest accountPaymentRequest);
        Task<Response<bool>> ThirdPartyPaymentAsync(ThirdPartyPaymentRequest thirdPartyPaymentRequest);
    }
}

