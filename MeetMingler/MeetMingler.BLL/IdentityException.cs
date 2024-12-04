using System.Collections;
using Microsoft.AspNetCore.Identity;

namespace MeetMingler.BLL;

public class IdentityException(IEnumerable<IdentityError> errors) : Exception
{
    public override IDictionary Data
    {
        get
        {
            return errors
                .Select((item, index) => new { item, index })
                .ToDictionary(x => x.index, x => x.item);
        }
    }

    public override string Message
    {
        get
        {
            return errors
                .Aggregate("", (s, error) => s += $"{error.Code}: {error.Description}\n");
        }
    }
}
