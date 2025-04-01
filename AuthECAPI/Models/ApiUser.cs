using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace AuthECAPI.Models;

public class ApiUser:IdentityUser
{
    [PersonalData]
    [Column(TypeName = "varchar(100)")]
    public string FullName { get; set; }
}