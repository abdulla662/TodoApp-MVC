using TodoApp.DataAccess;
using TodoApp.Models;
using TodoApp.Reposotory.IRepository;

namespace TodoApp.Reposotory
{
    public class TaskItemRepository : Repository<TaskItem>,ITaskItemRepository
    {
        ApplicationDbContext dbcontext;
        public TaskItemRepository(ApplicationDbContext dbContext) : base(dbContext)

        {
            this.dbcontext = dbContext;
        }
    }
}

