using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsiTrello.Infrastructures
{
    public interface IMessageVisualizerService
    {
        Task<bool> ShowMessage(string title, string message,
                        string ok, string cancel = null);
    }
}
