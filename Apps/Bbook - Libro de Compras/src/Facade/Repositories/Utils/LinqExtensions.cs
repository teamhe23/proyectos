using IntegracionBbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegracionBbook.Api.Repositories.Utils
{
    public static class LinqExtensions
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int parts) 
        { int i = 0; 
            
            var splits = from item in list 
                         group item by i++ % parts 
                         into part 
                         select part.AsEnumerable(); 
            return splits; 
        }
        public static IEnumerable<Product> SplitWithCharacter<T>(this IEnumerable<Product> list, string palabra)
        {
            var splits = from item in list
                         where item.brand_id.StartsWith(palabra)
                         select item;
            return splits;
        }
    }
}
