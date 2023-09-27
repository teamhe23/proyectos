using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Properties;
using Microsoft.Extensions.Options;

namespace Jobs.Services
{
    public class SettignsService: ISettignsService
    {
        private readonly Settigns _setting;
        public SettignsService(IOptions<Settigns> setting)
        {
            _setting = setting.Value;
        }

        public Settigns GetSettings()
        {
            return _setting;
        }
    }
}
