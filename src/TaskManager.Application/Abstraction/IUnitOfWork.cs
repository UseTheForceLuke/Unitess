namespace TaskManager.Application.Abstraction;

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken);
}
