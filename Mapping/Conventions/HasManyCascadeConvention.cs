using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Mapping.Conventions
{
    public class HasManyCascadeConvention : IHasManyConvention
    {
        #region IHasManyConvention Members

        public void Apply(IOneToManyCollectionInstance instance)
        {
            instance.AsSet();
            instance.Inverse();
            instance.Where("StatusType!=-1");
            instance.Cascade.AllDeleteOrphan();
            instance.Generic();
        }

        #endregion
    }
}