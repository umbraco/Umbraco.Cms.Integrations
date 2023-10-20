namespace Umbraco.Cms.Integrations.Analytics.Heap.Models;

public class User
{
    public User(string id, string email, string name, string roles)
    {
        Id = id;
        Email = email;
        Name = name;
        Roles = roles;
    }

    public string Id { get; set; }

    public string Email { get; set; }

    public string Name { get; set; }

    public string Roles { get; set; }
}
