using System;

namespace LucasClassManagementApi.Models;

public class Student {
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? Name { get; set; }
    public string? Password { get; set; }
    public string? Role { get; set; }
    public string? StudentNumber { get; set; }
    public uint PhoneNumber { get; set; }
}