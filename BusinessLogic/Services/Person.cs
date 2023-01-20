using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using Models.Account;
using DataAccess.EF;

namespace BusinessLogic.Services
{
    public class Person : IPerson
    {
        private readonly EventaContext _eventaContext;

        public Person(EventaContext eventaContext) 
        {
            _eventaContext = eventaContext;
        }

        public async Task<bool> AddPerson(ApplicationUser user)
        {
            try
            {
                var result = _eventaContext.Persons.Add(new DataAccess.EF.Person 
                {IsActive = false, UserId = user.Id 
                });
                await _eventaContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool PersonIsActive(string userId) 
        {
            var person =  _eventaContext.Persons.SingleOrDefault(i => i.UserId == userId);
            if (person == null) 
            {
                return false;
            }
            return person.IsActive;
        }

        public async Task<bool> ActivatePerson(string userId) 
        {
            try
            {
                var person = _eventaContext.Persons.SingleOrDefault(i => i.UserId == userId);
                if (person == null)
                {
                    return false;
                }
                person.IsActive = true;
                await _eventaContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
