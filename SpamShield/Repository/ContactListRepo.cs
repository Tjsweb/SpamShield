using InstaHyreSDETest.Data;
using InstaHyreSDETest.Entities;
using Microsoft.EntityFrameworkCore;

namespace InstaHyreSDETest.Repository
{
    public interface IContactListRepo
    {
        Task<Contacts> CreateContact(Contacts contact);
        Task<bool> DeleteAllContacts();
        Task<Contacts> DeleteContact(string phoneNumber);
        Task<List<Contacts>> GetAllContacts();
        Task<List<Contacts>> GetAllContacts(string phoneNumber);
        Task<bool> IsUserXInPersonalContacts(string phoneNumber, string userId);
        Task<Contacts> UpdateContact(Contacts contact);
    }

    public class ContactListRepo : IContactListRepo
    {
        private readonly ApplicationDBContext applicationDBContext;

        public ContactListRepo(ApplicationDBContext applicationDBContext)
        {
            this.applicationDBContext = applicationDBContext;
        }

        public async Task<List<Contacts>> GetAllContacts()
        {
            return await applicationDBContext.Contacts.ToListAsync();
        }

        public async Task<List<Contacts>> GetAllContacts(string phoneNumber)
        {
            return await applicationDBContext.Contacts.Where(x => x.PhoneNumber == phoneNumber).ToListAsync();
        }

        public async Task<Contacts> CreateContact(Contacts contact)
        {
            await applicationDBContext.Contacts.AddAsync(contact);
            await applicationDBContext.SaveChangesAsync();
            return contact;
        }

        public async Task<Contacts> UpdateContact(Contacts contact)
        {
            applicationDBContext.Contacts.Update(contact);
            await applicationDBContext.SaveChangesAsync();
            return contact;
        }

        public async Task<Contacts> DeleteContact(string phoneNumber)
        {
            Contacts contact = await applicationDBContext.Contacts.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            applicationDBContext.Contacts.Remove(contact);
            await applicationDBContext.SaveChangesAsync();
            return contact;
        }

        public async Task<Boolean> DeleteAllContacts()
        {
            List<Contacts> contacts = await applicationDBContext.Contacts.ToListAsync();
            applicationDBContext.Contacts.RemoveRange(contacts);
            await applicationDBContext.SaveChangesAsync();

            return true;
        }

        public async Task<Boolean> IsUserXInPersonalContacts(string phoneNumber, string userId)
        {
            List<Contacts> contacts = await applicationDBContext.Contacts.Where(x => x.PhoneNumber == phoneNumber).ToListAsync();
            foreach (var contact in contacts)
            {
                if (contact.ContactUserId == userId)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
