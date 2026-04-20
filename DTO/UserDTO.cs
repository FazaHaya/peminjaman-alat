namespace PeminjamanAlat.DTO
{
    public class UserDTO
    {
        public int IdUser { get; set; }
        public string Nama { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class UserCreateDTO
    {
        public string Nama { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class UserUpdateDTO
    {
        public string Nama { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}