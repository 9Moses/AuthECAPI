using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace AuthECAPI.Models;

public class ApiUser:IdentityUser
{
    [PersonalData]
    [Column(TypeName = "varchar(100)")]
    public string FullName { get; set; }
    
    [PersonalData]
    [Column(TypeName = "varchar(10)")]
    public string Gender { get; set; }
    
    [PersonalData]
    public DateOnly DOB { get; set; }
    
    [PersonalData]
    public int? LibraryID { get; set; }
}