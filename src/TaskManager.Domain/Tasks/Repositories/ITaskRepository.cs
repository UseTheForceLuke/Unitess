namespace TaskManager.Domain.Tasks.Repositories;

public interface ITaskRepository
{
    Task<Task> GetByIdAsync(int id);
    Task<IEnumerable<Task>> GetAllAsync();
    Task AddAsync(Task Task);
    void Update(Task Task);
    void Delete(Task Task);
    Task<bool> IsUserAssignedToTask(int userId, int TaskId);
    Task<bool> IsUserCreatorOfTask(int userId, int TaskId);
}
