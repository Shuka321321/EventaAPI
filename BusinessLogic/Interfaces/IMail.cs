using Models.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IMail
    {
        Task<bool> SendEmailAsync(MailRequest mailRequest);
    }
}
