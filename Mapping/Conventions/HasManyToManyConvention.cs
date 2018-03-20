using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Mapping.Conventions
{
    public class HasManyToManyCascadeConvention : IHasManyToManyConvention
    {
        #region IHasManyToManyConvention Members

        public void Apply(IManyToManyCollectionInstance instance)
        {
            instance.AsSet();
            instance.Cascade.AllDeleteOrphan();
            instance.Generic();
        }

        #endregion
    }
}