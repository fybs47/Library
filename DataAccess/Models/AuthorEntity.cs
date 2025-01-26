namespace DataAccess.Models
{
    public class AuthorEntity
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;  
        public string LastName { get; set; } = string.Empty;   
        public DateTime DateOfBirth { get; set; }              
        public string Country { get; set; } = string.Empty;    

        public ICollection<BookEntity> Books { get; set; }     
    }
}