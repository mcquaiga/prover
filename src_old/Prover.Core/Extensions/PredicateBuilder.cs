namespace Prover.Core.Extensions
{
    using System;

    public static class PredicateBuilder
    {
        public static Predicate<T> True<T>() { return f => true; }
        public static Predicate<T> False<T>() { return f => false; }

        public static Predicate<T> Or<T>(this Predicate<T> expr1,
            Predicate<T> expr2)
        {
            return delegate (T item)
            {                
                if (expr1(item) || expr2(item))
                {
                    return true;
                }
                
                return false;
            };
           
        }

        public static Predicate<T> And<T>(this Predicate<T> expr1,
            Predicate<T> expr2)
        {
            return delegate (T item)
            {
                if (expr1(item) && expr2(item))
                {
                    return true;
                }

                return false;
            };
        }
    }
}
