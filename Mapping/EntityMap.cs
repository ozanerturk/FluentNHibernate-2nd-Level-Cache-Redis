using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using FluentNHibernate.Mapping;

namespace Mapping
{
    public class HakanMap : ClassMap<Entity>
    {
        public HakanMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Data);
            References(x => x.SubEntity).Nullable();
        }
    }
}
