using Domain;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Mapping.Conventions
{
    public class CacheableConvention : IClassConventionAcceptance, IClassConvention
    {
        public void Apply(IClassInstance instance)
        {
            instance.Cache.ReadWrite();

            instance.Cache.Region(instance.EntityType.FullName);

            //instance.Cache.IncludeAll();
        }

        public void Accept(IAcceptanceCriteria<IClassInspector> criteria)
        {
            criteria.Expect(x => !typeof (INotCacheable).IsAssignableFrom(x.EntityType));
        }
    }
}