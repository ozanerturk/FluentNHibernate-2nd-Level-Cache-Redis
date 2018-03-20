using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Entity 
    {
        public virtual int Id { get; set; }
        public virtual int Data { get; set; }
        public virtual Entity SubEntity { get; set; }


    }
}
