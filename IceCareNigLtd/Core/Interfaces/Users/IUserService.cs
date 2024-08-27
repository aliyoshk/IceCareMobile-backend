using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Response;
using IceCareNigLtd.Api.Models.Users;
using IceCareNigLtd.Core.Entities.Users;

namespace IceCareNigLtd.Core.Interfaces.Users
{
	public interface IUserService
	{
        Task<Response<Registration>> RegisterUserAsync(RegistrationDto registrationDto);
        Task<Response<LoginResponse>> LoginUserAsync(LoginDto loginDto);
    }
}

