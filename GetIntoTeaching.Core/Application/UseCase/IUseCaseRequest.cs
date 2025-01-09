namespace GetIntoTeaching.Core.Application.UseCase
{
    /// <summary>
    /// Contract which defines the response object to be associated with a use-case request.
    /// </summary>
    /// <typeparam name="TUseCaseResponse">The runtime type definition of the use-case response object.</typeparam>
    public interface IUseCaseRequest<out TUseCaseResponse>{
    }
}
