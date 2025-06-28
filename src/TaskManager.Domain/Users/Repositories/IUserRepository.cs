﻿namespace TaskManager.Domain.Users.Repositories;

public interface IUserRepository
{
    Task<User> GetByIdAsync(int id);
    Task<User> GetByUsernameAsync(string username);
    Task AddAsync(User user);
}
