using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Inspections;

namespace Mapping.Conventions
{
    public class CustomManyToManyTableNameConvention : ManyToManyTableNameConvention
    {
        protected override string GetBiDirectionalTableName(IManyToManyCollectionInspector collection,
            IManyToManyCollectionInspector otherSide)
        {
            return collection.EntityType.Name + "_" + otherSide.EntityType.Name + "BiMapping";
        }

        protected override string GetUniDirectionalTableName(IManyToManyCollectionInspector collection)
        {
            return collection.EntityType.Name + "_" + collection.ChildType.Name + "UniMapping";
        }
    }
}