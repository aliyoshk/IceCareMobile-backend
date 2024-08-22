using System;
namespace IceCareNigLtd.Core.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(string email);
    }
}

