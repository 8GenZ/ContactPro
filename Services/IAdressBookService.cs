namespace ContactPro.Services
{
    public interface IAdressBookService
    {
        public Task AddCategoriesToContactAsync(List<int>categoryIds, int contactId);
        public Task RemoveCategoriesFromContactAsync(int contactId);
    }
}
