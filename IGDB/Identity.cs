using System.Collections.Generic;

namespace IGDB
{
    public class IdentityOrValue<T> where T : class
    {
        public long? Id { get; private set; }

        public T Value { get; private set; }

        public IdentityOrValue()
        {
        }

        public IdentityOrValue(long id)
        {
            Id = id;
        }

        public IdentityOrValue(T value)
        {
            Value = value;
        }

        public IdentityOrValue(object value)
        {
            Value = value as T;
        }
    }

    public class IdentitiesOrValues<T> where T : class
    {
        public List<long> Ids { get; private set; }

        public List<T> Values { get; private set; }

        public IdentitiesOrValues()
        {
        }

        public IdentitiesOrValues(List<long> ids)
        {
            Ids = ids;
        }

        public IdentitiesOrValues(List<T> values)
        {
            Values = values;
        }
    }
}