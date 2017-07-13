using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Casestudy.ViewModels;
using Newtonsoft.Json;

namespace Casestudy.Utils
{
    public class SessionUtils
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public SessionUtils(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool UserExists()
        {
            return _session.GetString(SessionVars.User) != null;
        }

        public bool CartContainsItems()
        {
            return _session.Get<Dictionary<string, object>>(SessionVars.Cart) != null;
        }

        public int GetCartSize()
        {
            var cart = _session.Get<Dictionary<string, object>>(SessionVars.Cart);
            var sum = 0;
            foreach (var key in cart.Keys)
            {
                ProductViewModel product = JsonConvert.DeserializeObject<ProductViewModel>(Convert.ToString(cart[key]));
                sum += product.Qty;
            }
            return sum;
        }

    }
}
