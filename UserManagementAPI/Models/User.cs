using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public required string Name { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(254)]
    public required string Email { get; set; }
}