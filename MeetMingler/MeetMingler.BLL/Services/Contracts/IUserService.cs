using MeetMingler.BLL.Models.User;
using MeetMingler.BLL.Services.Contracts.Generic;

namespace MeetMingler.BLL.Services.Contracts;

public interface IUserService : IGenericCreateService<UserVM, UserIM>
{
    public Task<UserVM?> GetByIdAsync(Guid id, CancellationToken cf = default);
    
    public Task<bool> UpdateUserPassword(Guid id, PasswordUM passwordUm);
}
