﻿using System.Security.Claims;
using System.Threading.Tasks;

namespace BankApp.Interfaces
{
    public interface IJwtFactory
    {
        Task<string> GenerateEncodedToken(string email, ClaimsIdentity claimsIdentity);
        ClaimsIdentity GenerateClaimsIdentity(string email, string userId);
    }
}
