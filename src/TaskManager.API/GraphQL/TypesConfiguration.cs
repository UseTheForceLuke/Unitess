using HotChocolate.Data.Filters;
using HotChocolate.Data.Sorting;
using TaskManager.API.GraphQL.Mutations;
using TaskManager.Application.Tasks.Commands;
using TaskManager.Application.Users.Commands;

namespace TaskManager.API.GraphQL;

// TODO: move to separate files

public class CreateTaskInputType : InputObjectType<CreateTaskInput>
{
    protected override void Configure(IInputObjectTypeDescriptor<CreateTaskInput> descriptor)
    {
        descriptor.Field(x => x.Title).Type<NonNullType<StringType>>();
        descriptor.Field(x => x.Description).Type<StringType>();
        descriptor.Field(x => x.Status).Type<NonNullType<TaskStatusType>>();
        descriptor.Field(x => x.AssignedUserIds)
            .Type<NonNullType<ListType<NonNullType<StringType>>>>()
            .Name("assignedUserIds");
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

public class TaskDtoSortType : SortInputType<TaskDto>
{
    protected override void Configure(ISortInputTypeDescriptor<TaskDto> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Id).Name("id");
        descriptor.Field(x => x.Title).Name("title");
        descriptor.Field(x => x.Status).Name("status");
        descriptor.Field(x => x.CreatedAt).Name("createdAt");
    }
}

public class TaskFilterInputType : FilterInputType<TaskDto>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<TaskDto> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Id);
        descriptor.Field(x => x.Status); 
    }
}

// Dtos
public class TaskDtoType : ObjectType<TaskDto>
{
    protected override void Configure(IObjectTypeDescriptor<TaskDto> descriptor)
    {
        descriptor.Field(x => x.Id).Type<NonNullType<IdType>>();
        descriptor.Field(x => x.Title).Type<NonNullType<StringType>>();
        descriptor.Field(x => x.Description).Type<StringType>();
        descriptor.Field(x => x.Status).Type<NonNullType<TaskStatusType>>();
        descriptor.Field(x => x.CreatedAt).Type<NonNullType<DateTimeType>>();
        descriptor.Field(x => x.Creator).Type<UserDtoType>();
        descriptor.Field(x => x.AssignedUsers).Type<ListType<UserDtoType>>();
    }
}

public class UserDtoType : ObjectType<UserDto>
{
    protected override void Configure(IObjectTypeDescriptor<UserDto> descriptor)
    {
        descriptor.Field(x => x.Id).Type<NonNullType<IdType>>();
        descriptor.Field(x => x.Username).Type<NonNullType<StringType>>();
        // Add other UserDto fields as needed
    }
}