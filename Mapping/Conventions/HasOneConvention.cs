using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Mapping.Conventions
{
    public class HasOneConvention : IHasOneConvention
    {
        #region IHasOneConvention Members

        public void Apply(IOneToOneInstance instance)
        {
            instance.LazyLoad();
            instance.Cascade.All();
        }

        #endregion
    }
}