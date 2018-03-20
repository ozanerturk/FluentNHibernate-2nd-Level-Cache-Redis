using Domain;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Mapping.Conventions
{
    public class CollectionConvention : ICollectionConvention
    {
        public void Apply(ICollectionInstance instance)
        {
            instance.LazyLoad();
            if (typeof (INotCacheableLazyLoad).IsAssignableFrom(instance.Relationship.Class.GetUnderlyingSystemType()))
            {
            }
            instance.Cache.ReadWrite();
        }
    }
}