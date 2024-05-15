﻿using Microsoft.EntityFrameworkCore;

namespace Todo.Api
{
    class TodoDb : DbContext
    {
        public TodoDb(DbContextOptions<TodoDb> options)
        : base(options) { }

        public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    }
}
