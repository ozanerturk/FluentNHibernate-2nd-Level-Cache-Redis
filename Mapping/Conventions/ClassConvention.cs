using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;

namespace Mapping.Conventions
{
    public class ClassConvention : IClassConventionAcceptance
    {
        public void Accept(IAcceptanceCriteria<IClassInspector> criteria)
        {
            //L2 cache edilmesini istemediğimiz entity type ekliyoruz.
            //var noCache = new[]
            //                  {
            //                      typeof (RewardBalanceAction),
            //                      typeof (RewardBalanceActionDetail),
            //                      typeof (CustomerBalance),
            //                      typeof (CustomerBalanceHistory)
            //                      ,typeof (SaleOrderTransactionDetail),
            //                  };

            //criteria.Expect(x => x.EntityType.BaseType != null &&
            //                     x.EntityType.BaseType.BaseType == typeof(EntityBase<>) &&
            //                     x.EntityType.IsNotAny(noCache));
        }
    }
}