﻿using System.ComponentModel.DataAnnotations;
using R8.Lib.Validatable;

namespace R8.Lib.Test.FakeObjects
{
    public class FakeValidatableObjectTest : ValidatableObject
    {
        [Required]
        public string Name { get; set; }

        public string this[string key] => string.Empty;
    }
}