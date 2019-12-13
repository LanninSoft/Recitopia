

using Microsoft.AspNetCore.Mvc;

using System;
using System.Linq;

//[System.Web.Mvc.Authorize]
public abstract class AuthorizeController : Controller
{
        
    public int GetUserCustomerId(string customerId)
    {
        int x = 0;

        if (!Int32.TryParse(customerId, out x))
        {
            x = 0;
        }
        return (x);
    }
}
