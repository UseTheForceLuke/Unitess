using HotChocolate.Data.Sorting;
using TaskManager.API.Mutations;
using TaskManager.Application.Users.Commands;

namespace TaskManager.API.GraphQL;

// Input Types
public class CreateTaskInputType : InputObjectType<CreateTaskInput>
{
    protected override void Configure(IInputObjectTypeDescriptor<CreateTaskInput> descriptor)
    {
        descriptor.Field(x => x.Title).Type<NonNullType<StringType>>();
        descriptor.Field(x => x.Description).Type<StringType>();
        descriptor.Field(x => x.Status).Type<NonNullType<TaskStatusType>>();
        descriptor.Field(x => x.AssignedUserIds).Type<NonNullType<ListType<NonNullType<UuidType>>>>();
    }
}

public class UserInputType : InputObjectType<UserDto>
{
    protected override void Configure(IInputObjectTypeDescriptor<UserDto> descriptor)
    {
        descriptor.Field(x => x.Id).Type<NonNullType<UuidType>>();
        descriptor.Field(x => x.Username).Type<NonNullType<StringType>>();
        // Add other fields as needed
    }
}
// Enum Types
public class TaskStatusType : EnumType<TaskManager.Domain.Tasks.TaskStatus>
{
    protected override void Configure(IEnumTypeDescriptor<TaskManager.Domain.Tasks.TaskStatus> descriptor)
    {
        descriptor.Name("TaskStatus");
        descriptor.Value(TaskManager.Domain.Tasks.TaskStatus.New).Description("New task");
        descriptor.Value(TaskManager.Domain.Tasks.TaskStatus.InProgress).Description("Task in progress");
        descriptor.Value(TaskManager.Domain.Tasks.TaskStatus.Completed).Description("Completed task");
        descriptor.Value(TaskManager.Domain.Tasks.TaskStatus.Archived).Description("Archived task");
    }
}

// Sort Types
public class TaskSortType : SortInputType<TaskManager.Domain.Tasks.Task>
{
    protected override void Configure(ISortInputTypeDescriptor<TaskManager.Domain.Tasks.Task> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Title);
        descriptor.Field(x => x.CreatedAt);
        descriptor.Field(x => x.Status);
    }
}

public class TaskType : ObjectType<TaskManager.Domain.Tasks.Task>
{
    protected override void Configure(IObjectTypeDescriptor<TaskManager.Domain.Tasks.Task> descriptor)
    {
        descriptor.Field(t => t.Id).Type<NonNullType<IdType>>();
        descriptor.Field(t => t.Title).Type<NonNullType<StringType>>();
        descriptor.Field(t => t.Description).Type<StringType>();
        descriptor.Field(t => t.Status).Type<NonNullType<TaskStatusType>>();
        descriptor.Field(t => t.CreatedAt).Type<NonNullType<DateTimeType>>();

        // For navigation properties, either:
        // 1. Exclude them
        descriptor.Ignore(t => t.Creator);
        descriptor.Ignore(t => t.UserTasks);

        // OR 2. Configure them properly
        // descriptor.Field(t => t.Creator).Type<UserType>();
    }
}