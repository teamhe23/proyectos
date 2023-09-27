using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegracionBbook.Api.Repositories.Utils
{
    public static class ExceptionHelpers
    {
        public static int LineNumber(this Exception ex)
        {
            int n;
            int i = ex.StackTrace.LastIndexOf(" ");
            if (i > -1)
            {
                string s = ex.StackTrace.Substring(i + 1);
                if (int.TryParse(s, out n))
                    return n;
            }
            return -1;
        }
    }
}
