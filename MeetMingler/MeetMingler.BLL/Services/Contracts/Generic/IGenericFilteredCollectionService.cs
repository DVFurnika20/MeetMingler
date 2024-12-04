using MeetMingler.BLL.Models;

namespace MeetMingler.BLL.Services.Contracts.Generic;

public interface IGenericFilteredCollectionService<TEntityVM, in TFilter>
    where TEntityVM : class
    where TFilter : class
{
    public Task<BaseCollectionVM<TEntityVM>> GetAllPaginatedAndFilteredAsync(PaginationOptions pagination,
        TFilter filter, CancellationToken cf = default);
}