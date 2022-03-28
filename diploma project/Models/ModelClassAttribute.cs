using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tanks.Models
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModelClassAttribute : System.Attribute
    {
        public ModelClassAttribute()
        {
        }
    }
}
