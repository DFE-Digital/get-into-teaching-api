namespace GetIntoTeaching.Core.CrossCuttingConcerns.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    public delegate object ServiceFactory(Type serviceType);

    /// <summary>
    /// 
    /// </summary>
    internal static class ServiceFactoryExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TCast"></typeparam>
        /// <param name="factory"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        public static TCast GetInstanceWithCast<TCast>(this ServiceFactory factory, Type type)
            where TCast : class =>
                factory(type) as TCast ??
                throw new InvalidCastException(
                    $"Unable to cast type of {type.Name} to {typeof(TCast).Name}");
    }
}
