using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GetIntoTeachingApi.ModelBinders
{
    public class TrimStringModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(string))
            {
                return new TrimStringModelBinder();
            }

            return null;
        }
    }
}
