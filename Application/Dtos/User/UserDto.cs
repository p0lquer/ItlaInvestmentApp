﻿namespace InvestmentApp.Core.Application.Dtos.User
{
    public class UserDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string UserName { get; set; }
        public string? Phone { get; set; }
        public string? ProfileImage { get; set; }
        public required int Role { get; set; }
    }
}
