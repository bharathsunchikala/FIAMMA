using System.Threading.Tasks;

namespace Fiamma.ApplicationCore.Interfaces;

public interface ITokenClaimsService
{
    Task<string> GetTokenAsync(string userName);
}

