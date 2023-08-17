using ContactPro.Data;
using ContactPro.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactPro.Services.Interfaces
{
    public class AdressBookService : IAdressBookService
    {
        private readonly ApplicationDbContext _context;
        public AdressBookService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task AddCategoriesToContactAsync(List<int> categoryIds, int contactId)
        {
            try
            {
                //get the contact to add categories to
                Contact? contact = await _context.Contacts.Include(c => c.Categories).FirstOrDefaultAsync(c => c.Id == contactId);
                //if contact doset exist
                if (contact == null)
                {
                    return;
                }
                //loop through each category Id
                foreach(int categoryId in categoryIds) 
                {
                    //-make sure each category exists
                    Category? category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
                    //-if so add the contact to that category 
                    if (category != null)
                    {
                        contact.Categories.Add(category);
                    }
                }                      

                //When done save changes to database
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task RemoveCategoriesFromContactAsync(int contactId)
        {
            try
            {
                //find contact by id 
                Contact? contact = await _context.Contacts.Include(c=> c.Categories).FirstOrDefaultAsync(c => c.Id == contactId);
                if (contact != null)
                {
                //remove all categories
                    contact.Categories.Clear();
                //save changes to database
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
