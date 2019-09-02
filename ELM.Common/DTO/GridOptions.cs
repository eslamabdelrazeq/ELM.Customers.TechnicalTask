using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ELM.Common.DTO
{
    public class GridOptions<T> where T : class
    {
        public List<Expression<Func<T, bool>>> Filters { get; }
        public List<Expression<Func<T, Object>>> GroupBy { get; }
        public List<OrderBy<T>> Order { get; }

        public GridOptions()
        {
            Filters = new List<Expression<Func<T, bool>>>();
            GroupBy = new List<Expression<Func<T, Object>>>();
            Order = new List<OrderBy<T>>();
        }

        #region filters
        //public void AddFilter(string propertyName, object propertyValue)
        public void AddFilter(Expression<Func<T, bool>> predicate)
        {
            //Expression<Func<T, bool>> predicate = x => x.propertyName == propertyValue;
            //Expression<Func<T, bool>> filter = EF.Property<object>(e, propertyName) == propertyValue;

            Filters.Add(predicate);
        }

        public void AddSearch(Expression<Func<T, bool>> predicate)
        {
            //Expression<Func<T, bool>> predicate = null;

            Filters.Add(predicate);
        }
        #endregion

        #region order
        public void AddOrderBy(string column, bool desc = false)
        {
            OrderBy<T> orderItem = new OrderBy<T>()
            {
                Column = column,
                Desc = desc
            };
            Order.Add(orderItem);
        }

        public void AddOrderBy(Expression<Func<T, Object>> property, bool desc = false)
        {
            OrderBy<T> orderItem = new OrderBy<T>()
            {
                Property = property,
                Desc = desc
            };
            Order.Add(orderItem);
        }

        public void AddOrderBy(OrderBy<T> orderItem)
        {
            Order.Add(orderItem);
        }
        #endregion
    }
}
