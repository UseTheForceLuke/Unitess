using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstraction;
using Task = TaskManager.Domain.Tasks.Task;
using Task_ = System.Threading.Tasks.Task;

namespace TaskManager.Application.Tasks.Queries;

public record GetTasksQuery : IRequest<IQueryable<Task>>;

public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, IQueryable<Task>>
{
    private readonly IApplicationDbContext _context;

    public GetTasksQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<IQueryable<Task>> Handle(
        GetTasksQuery request,
        CancellationToken cancellationToken)
    {
        return Task_.FromResult(_context.Tasks.AsQueryable().AsNoTracking());
    }
}