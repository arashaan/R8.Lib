﻿using R8.Lib.Test.FakeObjects;

using System.ComponentModel.DataAnnotations;
using System.Linq;

using Xunit;

namespace R8.AspNetCore.Test
{
    public class ReflectionsTests
    {
        [Fact]
        public void CallGetValidatablePropertyModelMetadata()
        {
            var model = new FakeObjHasReq
            {
                LastName = "Shabbeh",
                Name = "Arash"
            };

            var act = model.GetMetadataForProperty(x => x.LastName);
            var attributes = act.Attributes.PropertyAttributes;
            var required = attributes.Any(x => x is RequiredAttribute);

            Assert.True(required);
        }
    }
}