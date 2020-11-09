﻿using System;
using System.Linq;
using NodaTime;
using R8.Lib.Enums;

namespace R8.Lib.AspNetCore
{
    public class CurrentUser
    {
        public string Id { get; set; }
        public Guid GuidId => !string.IsNullOrEmpty(Id) && Guid.TryParse(Id, out var guid) ? guid : Guid.Empty;
        public Roles Role { get; set; }
        public string Username { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTimeZone TimeZone { get; set; }

        public string GetFullName()
        {
            return $"{FirstName} {LastName}";
        }

        public bool HasPermission(params Roles[] roles)
        {
            return roles?.Any() != true || roles.Any(x => x == Role);
        }

        public override string ToString()
        {
            return $"{Role.ToString().ToUpper()} => {FirstName} {LastName}";
        }
    }
}