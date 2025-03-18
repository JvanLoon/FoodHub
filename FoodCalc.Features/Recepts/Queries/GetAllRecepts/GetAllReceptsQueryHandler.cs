using ErrorOr;
using MediatR;

using FoodHub.Persistence.Entities;
using FoodHub.Persistence.Persistence;
using Microsoft.Extensions.Logging;


namespace FoodCalc.Features.Recepts.Queries.GetAllRecepts;
public class GetAllReceptsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllReceptsQueryHandler> logger) : IRequestHandler<GetAllReceptsQuery, ErrorOr<List<Recept>>>
{
	public async Task<ErrorOr<List<Recept>>> Handle(GetAllReceptsQuery request, CancellationToken cancellationToken)
	{
		try
		{
			return await unitOfWork.ReceptRepository.GetAllAsync(cancellationToken);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to get all recepts");
			return Error.Failure("Failed to get all recepts");
		}
	}
}
