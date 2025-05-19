using Newtonsoft.Json.Linq;

namespace GetIntoTeachingApiTests.Contracts.TestUtilities;

/// <summary>
/// Provides utility methods for comparing JSON arrays and objects using Newtonsoft.Json.
/// This class is designed to identify structural and value differences between two JSON arrays,
/// including detailed reporting of mismatches at the object and property level.
/// </summary>
internal static class JsonComparer
{
    /// <summary>
    /// Compares two JSON arrays for equality.
    /// </summary>
    /// <param name="array1">First JSON array.</param>
    /// <param name="array2">Second JSON array.</param>
    /// <returns>A ComparisonResult indicating whether the arrays are equal and a message describing the result.</returns>
    public static ComparisonResult CompareJsonArrays(JArray array1, JArray array2)
    {
        // Check if arrays have different lengths
        if (array1.Count != array2.Count)
        {
            return new ComparisonResult(
                areEqual: false,
                message: $"Arrays have different lengths: {array1.Count} vs {array2.Count}");
        }

        // Compare each element in the arrays
        for (int i = 0; i < array1.Count; i++)
        {
            // If both elements are JSON objects, compare them using the object comparison method
            if (array1[i].Type == JTokenType.Object && array2[i].Type == JTokenType.Object)
            {
                ComparisonResult objectComparisonResult =
                    CompareJsonObjects((JObject)array1[i], (JObject)array2[i], i);

                if (!objectComparisonResult.AreEqual)
                {
                    return objectComparisonResult;
                }
            }

            // Use DeepEquals to compare non-object elements
            if (!JToken.DeepEquals(array1[i], array2[i]))
            {
                return new ComparisonResult(
                    areEqual: false,
                    message: $"Array items at index {i} differ: '{array1[i]}' - '{array2[i]}'");
            }
        }

        // If all elements match
        return new ComparisonResult(areEqual: true, message: "Arrays are equal.");
    }

    /// <summary>
    /// Compares two JSON objects for equality.
    /// </summary>
    /// <param name="object1">First JSON object.</param>
    /// <param name="object2">Second JSON object.</param>
    /// <param name="index">Index of the object in the array (for error reporting).</param>
    /// <returns>A ComparisonResult indicating whether the objects are equal and a message describing the result.</returns>
    public static ComparisonResult CompareJsonObjects(JObject object1, JObject object2, int index)
    {
        // Check for missing or differing properties in the second object
        foreach (JProperty property in object1.Properties())
        {
            if (object2[property.Name] == null)
            {
                return new ComparisonResult(
                    areEqual: false,
                    message: $"Property '{property.Name}' is missing in the second JSON at index {index}.");
            }

            if (!JToken.DeepEquals(property.Value, object2[property.Name]))
            {
                return new ComparisonResult(
                    areEqual: false,
                    message: $"Property '{property.Name}' values differ at index {index}: '{property.Value}' - '{object2[property.Name]}'");
            }
        }

        // Check for extra properties in the second object
        foreach (JProperty property in object2.Properties())
        {
            if (object1[property.Name] == null)
            {
                return new ComparisonResult(
                    areEqual: false,
                    message: $"Property '{property.Name}' is missing in the first JSON at index {index}.");
            }
        }

        // If all properties match
        return new ComparisonResult(areEqual: true, message: "Objects are equal.");
    }

    /// <summary>
    /// Represents the result of a JSON comparison operation.
    /// This class encapsulates whether two JSON structures are equal and provides a descriptive message
    /// explaining the result of the comparison.
    /// </summary>
    internal sealed class ComparisonResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether the compared JSON structures are equal.
        /// </summary>
        public bool AreEqual { get; }

        /// <summary>
        /// Gets or sets a message describing the result of the comparison.
        /// This message typically includes details about mismatches or confirms equality.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComparisonResult"/> class.
        /// </summary>
        /// <param name="areEqual">A boolean value indicating whether the JSON structures are equal.</param>
        /// <param name="message">A descriptive message about the comparison result.</param>
        public ComparisonResult(bool areEqual, string message)
        {
            AreEqual = areEqual;
            Message = message;
        }
    }
}
