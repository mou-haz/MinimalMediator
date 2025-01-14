using System.Threading;
using System.Threading.Tasks;
using MinimalMediator.Abstractions.Messages;

namespace MinimalMediator.Abstractions.Interceptors;

public interface IValidatePipeline<in TRequest>
    where TRequest : IIdentifiableRequest
{
    ValueTask Validate(TRequest request, CancellationToken cancellationToken);
}