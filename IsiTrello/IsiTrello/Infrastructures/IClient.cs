using IsiTrello.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IsiTrello.Infrastructures
{
    public interface IClient
    {
        Task<string> GetBoardname(string boardID);
        Task<IEnumerable<List>> GetBoard(string boardID);
        Task<IEnumerable<Blist>> GetBoardlist();
        Task<IEnumerable<Attachment>> GetAttachments(string Shortlink);
        Task<IEnumerable<Checklist>> GetChecklist(string idc);
    }
}
