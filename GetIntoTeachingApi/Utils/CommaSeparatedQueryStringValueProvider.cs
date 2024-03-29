﻿using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;

namespace GetIntoTeachingApi.Utils
{
    public class CommaSeparatedQueryStringValueProvider : QueryStringValueProvider
    {
        private readonly string[] _keys;
        private readonly string _separator;

        public CommaSeparatedQueryStringValueProvider(IQueryCollection values, string separator)
            : this(null, values, separator)
        {
            _separator = separator;
        }

        public CommaSeparatedQueryStringValueProvider(string[] keys, IQueryCollection values, string separator)
            : base(BindingSource.Query, values, CultureInfo.InvariantCulture)
        {
            _keys = keys;
            _separator = separator;
        }

        public override ValueProviderResult GetValue(string key)
        {
            var result = base.GetValue(key);

            if (!_keys.Contains(key))
            {
                return result;
            }

            if (result != ValueProviderResult.None && result.Values.Any(x => x.Contains(_separator)))
            {
                var splitValues = new StringValues(result.Values
                    .SelectMany(x => x.Split(new[] { _separator }, StringSplitOptions.None)).ToArray());
                return new ValueProviderResult(splitValues, result.Culture);
            }

            return result;
        }
    }
}
