using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Mapping.Conventions
{
    public class TableNameConvention : IClassConvention
    {
        public void Apply(IClassInstance instance)
        {
            instance.LazyLoad();
            var typeName = instance.EntityType.Name;
            var pluralTableName = Inflector.Inflector.Pluralize(typeName);
            instance.Table(pluralTableName);
        }
    }
}