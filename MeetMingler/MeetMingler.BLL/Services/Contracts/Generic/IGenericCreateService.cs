namespace MeetMingler.BLL.Services.Contracts.Generic;

public interface IGenericCreateService<TEntityVM, TEntityIM>
    where TEntityVM : class
    where TEntityIM : class
{
    public Task<TEntityVM?> CreateAsync(TEntityIM im, CancellationToken cf = default);
}
