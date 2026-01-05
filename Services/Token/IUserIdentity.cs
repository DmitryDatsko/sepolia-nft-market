using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace SepoliaNftMarket.Services.Token;

public interface IUserIdentity
{
    string GetAddressByCookie(HttpContext context);
    string GetAddressByHub(ClaimsPrincipal? claims);
}