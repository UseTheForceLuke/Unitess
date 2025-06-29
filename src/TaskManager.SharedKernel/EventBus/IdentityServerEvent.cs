namespace TaskManager.SharedKernel.EventBus;

public class IdentityServerEvent
{
    public string EventType { get; set; }
    public string SubjectId { get; set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}