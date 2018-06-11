using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;

namespace IsiTrello.Infrastructures
{
    public interface IAzureClient
    {
        bool KnownToken();
        Task<bool> GetToken(IPlatformParameters platformParameters);
        void RefreshToken();
    }
}
