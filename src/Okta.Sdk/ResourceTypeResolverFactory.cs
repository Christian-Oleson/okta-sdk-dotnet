﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Okta.Sdk.Abstractions;

namespace Okta.Sdk
{
    public class ResourceTypeResolverFactory : AbstractResourceTypeResolverFactory
    {
        public override IEnumerable<TypeInfo> GetAllResourceDefinedTypes()
        {
            return typeof(Resource).GetTypeInfo().Assembly.DefinedTypes.ToArray();
        }
    }
}
