
namespace Models
{
    public class User(string name)
    {
        public int Id { get; }
        public required string Name { get; set; } = name;
        public required string Email { get; set; } = email;
    }
}