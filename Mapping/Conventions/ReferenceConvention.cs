using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Mapping.Conventions
{
    public class ReferenceConvention : IReferenceConvention
    {
        #region IReferenceConvention Members

        public void Apply(IManyToOneInstance instance)
        {
            instance.Not.Nullable();
        }

        #endregion
    }
}