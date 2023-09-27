using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Oracle.Helpers
{
    public class ExceptionInternal : Exception
    {
        public ExceptionInternal(String message) : base(message)
        {
        }

        public override String Message
        {
            get
            {
                String messageFull = base.Message;
                String resultado = "";
                if (messageFull.StartsWith("ORA-"))
                {
                    messageFull = messageFull.Substring(11);
                }
                int pos = messageFull.IndexOf("\n");
                if (pos >= 0)
                {
                    resultado = messageFull.Substring(0, pos);
                }
                else
                {
                    resultado = messageFull;
                }
                return resultado;
            }
        }
    }
}
