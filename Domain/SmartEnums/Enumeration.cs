using System.Reflection;

namespace Domain.SmartEnums
{
    public abstract class Enumeration : IComparable
    {
        public string Name { get; private set; }
        public int Id { get; private set; }

        protected Enumeration(int id, string name) => (Id, Name) = (id, name);

        public override string ToString() => Name;

        /// <summary>
        /// Gets all items of a specific enumeration type.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <returns>An IEnumerable of all defined enumeration items.</returns>
        public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
            typeof(T).GetFields(BindingFlags.Public |
                                BindingFlags.Static |
                                BindingFlags.DeclaredOnly)
                     .Select(f => f.GetValue(null))
                     .Cast<T>();

        /// <summary>
        /// Determines whether the specified object is equal to the current instance.
        /// Equality is determined by comparing the type and the Id of the enumeration items.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>True if the specified object is equal to the current instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is not Enumeration otherValue)
            {
                return false;
            }

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type based on their Id.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>An integer that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(object other) => Id.CompareTo(((Enumeration)other).Id);

        /// <summary>
        /// Returns the hash code for this instance, which is the hash code of its Id.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode() => Id.GetHashCode();

        /// <summary>
        /// Creates an enumeration item from its integer value.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="value">The integer value representing the enumeration item.</param>
        /// <returns>The matching enumeration item.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the value does not correspond to a valid enumeration item.</exception>
        public static T FromValue<T>(int value) where T : Enumeration
        {
            var matchingItem = GetAll<T>().FirstOrDefault(item => item.Id == value);
            if (matchingItem == null)
            {
                throw new InvalidOperationException($"'{value}' is not a valid id for {typeof(T).Name}");
            }
            return matchingItem;
        }

        /// <summary>
        /// Creates an enumeration item from its display name.
        /// The comparison is case-insensitive.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="name">The display name of the enumeration item.</param>
        /// <returns>The matching enumeration item.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the name does not correspond to a valid enumeration item.</exception>
        public static T FromName<T>(string name) where T : Enumeration
        {
            var matchingItem = GetAll<T>().FirstOrDefault(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
            if (matchingItem == null)
            {
                throw new InvalidOperationException($"'{name}' is not a valid name for {typeof(T).Name}");
            }
            return matchingItem;
        }
    }
}