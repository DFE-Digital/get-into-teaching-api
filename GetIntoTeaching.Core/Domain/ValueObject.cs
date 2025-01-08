namespace GetIntoTeaching.Core.Domain
{
    /// <summary>
    /// Abstract Value Object base class which is used as the basis of any domain concept when the
    /// model's identity is not a key design consideration, rather equality is determined by assessing
    ///  whether all attributes are the same. The key aspects of a domain value object are as follows:
    /// <list type="bullet">
    /// <item>
    /// <term>Equals</term>
    /// <description>
    /// Used to determine whether all attributes of one value object are
    /// equivalent to all attributes of another comparable value object.
    /// </description>
    /// </item>
    /// <item>
    /// <term>Operator Overloading on == and !=</term>
    /// <description>
    /// Allows equality checking to be performed on the reference of the two objects.
    /// </description>
    /// </item>
    /// </list>
    /// </summary>
    /// <typeparam name="TValueObject"></typeparam>
    public abstract class ValueObject<TValueObject> where TValueObject : ValueObject<TValueObject>
    {
        /// <summary>
        /// Method used to check referential equality of all attributes
        /// associated between to two value objects.
        /// </summary>
        /// <param name="obj">The value object to compare against.</param>
        /// <returns>A result predicated on equivalence, or otherwise (i.e. true or false).</returns>
        public override bool Equals(object? obj)
        {
            if (obj is not TValueObject comparableValueObject) return false;

            return GetPropertiesForEqualityCheck()
                .SequenceEqual(comparableValueObject.GetPropertiesForEqualityCheck());
        }

        /// <summary>
        /// Equality operator overloading to allow attribute
        /// equivalence checking between two value objects.
        /// </summary>
        /// <param name="leftCompareValueObj">Value object to make equality check against.</param>
        /// <param name="rightCompareValueObj">Value object to make equality check from.</param>
        /// <returns>Boolean result predicated on equality check.</returns>
        public static bool operator ==(ValueObject<TValueObject> leftCompareValueObj, ValueObject<TValueObject> rightCompareValueObj)
        {
            if (leftCompareValueObj is null && rightCompareValueObj is null) return true;
            if (leftCompareValueObj is null || rightCompareValueObj is null) return false;

            return leftCompareValueObj.Equals(rightCompareValueObj);
        }

        /// <summary>
        /// Non-Equality operator overloading to allow attribute
        /// non-equivalence checking between two value objects.
        /// </summary>
        /// <param name="leftCompareValueObj">Value object to make equality check against.</param>
        /// <param name="rightCompareValueObj">Value object to make equality check from.</param>
        public static bool operator !=(ValueObject<TValueObject> leftCompareValueObj, ValueObject<TValueObject> rightCompareValueObj) =>
            !(leftCompareValueObj == rightCompareValueObj);

        /// <summary>
        /// Exposes all attributes made available for equality checking in a collection of objects.
        /// </summary>
        /// <returns>A collection of objects representing available attributes to compare.</returns>
        protected abstract IEnumerable<object> GetPropertiesForEqualityCheck();

        public override int GetHashCode()
        {
            int hash = 17;

            GetPropertiesForEqualityCheck().ToList().ForEach(obj =>
                hash = (hash * 31) + ((obj?.GetHashCode()) ?? 0));

            return hash;
        }
    }
}
