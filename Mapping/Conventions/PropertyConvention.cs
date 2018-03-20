using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Mapping.Conventions
{
    public class PropertyConvention : IPropertyConvention
    {
        #region IPropertyConvention Members

        public void Apply(IPropertyInstance instance)
        {
            instance.Not.Nullable();
            instance.Length(100);

            // # DateTime alanları veritabanına yazılırken milisaniye bilgisi kaybedildiği için bu çözüm uygulandı
            var propertyType = instance.Property.PropertyType;
            if (propertyType == typeof (DateTime) || propertyType == typeof (DateTime?))
            {
                instance.CustomType("Timestamp");
            }
        }

        #endregion
    }
}