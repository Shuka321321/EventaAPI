using Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IPerson
    {
        Task<bool> AddPerson(ApplicationUser user);
        bool PersonIsActive(string userId);
        Task<bool> ActivatePerson(string userId);
    }
}
