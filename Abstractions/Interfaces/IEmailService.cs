using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions.Interfaces
{
    public interface IEmailService
    {
        Task Send(string emailAddress, string body);
    }

}
