using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Geowerkstatt.TestTools
{
    /// <summary>
    /// Provides extension methods for collections to verify
    /// true/false propositions with every item.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Verifies that the number of elements in <paramref name="collection"/>
        /// is within the interval <c>[<paramref name="min"/>,<paramref name="max"/>]</c>.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="min">The expected minimum number of elements in <paramref name="collection"/>.</param>
        /// <param name="max">The expected maximum number of elements in <paramref name="collection"/>.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// The number of elements in <paramref name="collection"/> is less
        /// than <paramref name="min"/> or more than <paramref name="max"/>,
        /// or <paramref name="collection"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="min"/> is bigger than <paramref name="max"/>, or either value is negative.</exception>
        public static IEnumerable<T> AssertCountInRange<T>(this IEnumerable<T> collection, int min, int max = int.MaxValue)
        {
            return AssertCountInRange(collection, min, max, null);
        }

        /// <summary>
        /// Verifies that the number of elements in <paramref name="collection"/>
        /// is within the interval <c>[<paramref name="min"/>,int.MaxValue</c>.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="min">The expected minimum number of elements in <paramref name="collection"/>.</param>
        /// <param name="message">The message to describe the assertion in case of failure.</param>
        /// <param name="parameters">Parameters for the <paramref name="message"/>.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// The number of elements in <paramref name="collection"/> is less
        /// than <paramref name="min"/> or more than int.MaxValue,
        /// or <paramref name="collection"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="min"/> is bigger than int.MaxValue, or either value is negative.</exception>
        public static IEnumerable<T> AssertCountInRange<T>(this IEnumerable<T> collection, int min, string message, params object[] parameters)
        {
            return AssertCountInRange(collection, min, int.MaxValue, message, parameters);
        }

        /// <summary>
        /// Verifies that the number of elements in <paramref name="collection"/>
        /// is within the interval <c>[<paramref name="min"/>,<paramref name="max"/>]</c>.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="min">The expected minimum number of elements in <paramref name="collection"/>.</param>
        /// <param name="max">The expected maximum number of elements in <paramref name="collection"/>.</param>
        /// <param name="message">The message to describe the assertion in case of failure.</param>
        /// <param name="parameters">Parameters for the <paramref name="message"/>.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// The number of elements in <paramref name="collection"/> is less
        /// than <paramref name="min"/> or more than <paramref name="max"/>,
        /// or <paramref name="collection"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="min"/> is bigger than <paramref name="max"/>, or either value is negative.</exception>
        public static IEnumerable<T> AssertCountInRange<T>(this IEnumerable<T> collection, int min, int max, string message, params object[] parameters)
        {
            if (min > max) throw new ArgumentOutOfRangeException(nameof(max), MessageHelper.CombineDefaultAndCustomMessage("The expected maximum value must be no less than the expected minimum.", message, parameters));
            if (min < 0) throw new ArgumentOutOfRangeException(nameof(min), MessageHelper.CombineDefaultAndCustomMessage("A collection cannot be expected to be of negative length.", message, parameters));

            Assert.IsNotNull(collection, MessageHelper.CombineDefaultAndCustomMessage("The collection must not be null.", message, parameters));

            var count = collection.Count();
            if (count < min || count > max)
            {
                var msg = MessageHelper.CombineDefaultAndCustomMessage(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The collection contains {0} items, which is outside the expected range of [{1},{2}].",
                        count,
                        min,
                        max),
                    message,
                    parameters);

                throw new AssertFailedException(msg);
            }

            return collection;
        }

        /// <summary>
        /// Verifies that <paramref name="collection"/> contains at least <paramref name="min"/> items.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="min">The expected minimum number of elements in <paramref name="collection"/>.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// The number of elements in <paramref name="collection"/> is less
        /// than <paramref name="min"/>, or <paramref name="collection"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="min"/> is negative.</exception>
        public static IEnumerable<T> AssertContainsAtLeast<T>(this IEnumerable<T> collection, int min)
        {
            return AssertContainsAtLeast(collection, min, null);
        }

        /// <summary>
        /// Verifies that <paramref name="collection"/> contains at least <paramref name="min"/> items.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="min">The expected minimum number of elements in <paramref name="collection"/>.</param>
        /// <param name="message">The message to describe the assertion in case of failure.</param>
        /// <param name="parameters">Parameters for the <paramref name="message"/>.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// The number of elements in <paramref name="collection"/> is less
        /// than <paramref name="min"/>, or <paramref name="collection"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="min"/> is negative.</exception>
        public static IEnumerable<T> AssertContainsAtLeast<T>(this IEnumerable<T> collection, int min, string message, params object[] parameters)
        {
            if (min < 0) throw new ArgumentOutOfRangeException(nameof(min), MessageHelper.CombineDefaultAndCustomMessage("A collection cannot be expected to be of negative length.", message, parameters));

            Assert.IsNotNull(collection, MessageHelper.CombineDefaultAndCustomMessage("The collection must not be null.", message, parameters));

            var count = collection.Count();
            if (count < min)
            {
                var msg = MessageHelper.CombineDefaultAndCustomMessage(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The collection contains {0} items, which is below the expected minimum of <{1}>.",
                        count,
                        min),
                    message,
                    parameters);

                throw new AssertFailedException(msg);
            }

            return collection;
        }

        /// <summary>
        /// Verifies that <paramref name="collection"/> contains a particular <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="item">The expected item.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">If <paramref name="collection"/> or <paramref name="item"/> is
        /// <c>null</c> or <paramref name="item"/> is not part of <paramref name="collection"/>.</exception>
        public static IEnumerable<T> AssertContains<T>(this IEnumerable<T> collection, T item)
        {
            return AssertContains(collection, item, null);
        }

        /// <summary>
        /// Verifies that <paramref name="collection"/> contains a particular <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="item">The expected item.</param>
        /// <param name="message">The message to describe the assertion in case of failure.</param>
        /// <param name="parameters">Parameters for the <paramref name="message"/>.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">If <paramref name="collection"/> or <paramref name="item"/> is
        /// <c>null</c> or <paramref name="item"/> is not part of <paramref name="collection"/>.</exception>
        public static IEnumerable<T> AssertContains<T>(this IEnumerable<T> collection, T item, string message, params object[] parameters)
        {
            Assert.IsNotNull(collection, MessageHelper.CombineDefaultAndCustomMessage("The collection must not be null.", message, parameters));
            Assert.IsNotNull(item, MessageHelper.CombineDefaultAndCustomMessage("The item must not be null.", message, parameters));

            var comparer = EqualityComparer<T>.Default;
            foreach (T element in collection)
            {
                if (comparer.Equals(element, item))
                {
                    return collection;
                }
            }

            var defaultMessage = string.Format(
                CultureInfo.InvariantCulture,
                "The expected item <{0}> was not part of source collection.",
                item);

            throw new AssertFailedException(
                MessageHelper.CombineDefaultAndCustomMessage(defaultMessage, message, parameters));
        }

        /// <summary>
        /// Verifies that <paramref name="collection"/> contains exactly <paramref name="expected"/> items.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="expected">The expected number of elements in <paramref name="collection"/>.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// The number of elements in <paramref name="collection"/> is not <paramref name="expected"/>,
        /// or <paramref name="collection"/> is <c>null</c>.
        /// </exception>
        public static IEnumerable<T> AssertCount<T>(this IEnumerable<T> collection, int expected)
        {
            return AssertCount(collection, expected, null);
        }

        /// <summary>
        /// Verifies that <paramref name="collection"/> contains exactly <paramref name="expected"/> items.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="expected">The expected number of elements in <paramref name="collection"/>.</param>
        /// <param name="message">The message to describe the assertion in case of failure.</param>
        /// <param name="parameters">Parameters for the <paramref name="message"/>.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// The number of elements in <paramref name="collection"/> is not <paramref name="expected"/>,
        /// or <paramref name="collection"/> is <c>null</c>.
        /// </exception>
        public static IEnumerable<T> AssertCount<T>(this IEnumerable<T> collection, int expected, string message, params object[] parameters)
        {
            return AssertItems(collection, null, null, expected, message, parameters);
        }

        /// <summary>
        /// Performs an assertion for each item in a collection.
        /// The advantage over plainly iterating in a test is that the
        /// error message contains a description of the failing element.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="asserter">The asserting method that is called with each item in <paramref name="collection"/>.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// Asserting failed for at least one item in <paramref name="collection"/>,
        /// or <paramref name="collection"/> is <c>null</c>.
        /// </exception>
        public static IEnumerable<T> AssertAll<T>(this IEnumerable<T> collection, Action<T> asserter)
        {
            return AssertAll(collection, asserter, null);
        }

        /// <summary>
        /// Performs an assertion for each item in a collection.
        /// The advantage over plainly iterating in a test is that the
        /// error message contains a description of the failing element.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="asserter">The asserting method that is called with each item in <paramref name="collection"/>.</param>
        /// <param name="message">The message to describe the assertion in case of failure.</param>
        /// <param name="parameters">Parameters for the <paramref name="message"/>.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// Asserting failed for at least one item in <paramref name="collection"/>,
        /// or <paramref name="collection"/> is <c>null</c>.
        /// </exception>
        public static IEnumerable<T> AssertAll<T>(this IEnumerable<T> collection, Action<T> asserter, string message, params object[] parameters)
        {
            if (asserter == null) throw new ArgumentNullException(nameof(asserter));

            return AssertItems(collection, asserter, message, parameters);
        }

        /// <summary>
        /// Verifies a single, specific element in a sequence.
        /// If the item cannot be found, or the predicate is not unique,
        /// the verification is considered failed.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="predicate">A function to test an element for a condition.</param>
        /// <param name="asserter">The asserting method that is called with the specified item, if found.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// Asserting failed for the found item in <paramref name="collection"/>,
        /// <paramref name="collection"/> is <c>null</c>,
        /// no element satisfies the condition in <paramref name="predicate"/>,
        /// or more than one element satisfies the condition in <paramref name="predicate"/>.
        /// </exception>
        public static IEnumerable<T> AssertSingleItem<T>(this IEnumerable<T> collection, Func<T, bool> predicate, Action<T> asserter)
        {
            return AssertSingleItem(collection, predicate, asserter, null);
        }

        /// <summary>
        /// Verifies a single, specific element in a sequence.
        /// If the item cannot be found, or the predicate is not unique,
        /// the verification is considered failed.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="predicate">A function to test an element for a condition.</param>
        /// <param name="asserter">The asserting method that is called with the specified item, if found.</param>
        /// <param name="message">The message to describe the assertion in case of failure.</param>
        /// <param name="parameters">Parameters for the <paramref name="message"/>.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// Asserting failed for the found item in <paramref name="collection"/>,
        /// <paramref name="collection"/> is <c>null</c>,
        /// no element satisfies the condition in <paramref name="predicate"/>,
        /// or more than one element satisfies the condition in <paramref name="predicate"/>.
        /// </exception>
        public static IEnumerable<T> AssertSingleItem<T>(this IEnumerable<T> collection, Func<T, bool> predicate, Action<T> asserter, string message, params object[] parameters)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (asserter == null) throw new ArgumentNullException(nameof(asserter));

            return AssertItems(collection, predicate, asserter, 1, message, parameters);
        }

        /// <summary>
        /// Verifies that the sequence contains a single matching item.
        /// The item itself is not verified.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="predicate">A function to test an element for a condition.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// <paramref name="collection"/> is <c>null</c>,
        /// no element satisfies the condition in <paramref name="predicate"/>,
        /// or more than one element satisfies the condition in <paramref name="predicate"/>.
        /// </exception>
        public static IEnumerable<T> AssertSingleItem<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            return AssertItems(collection, predicate, null, 1);
        }

        /// <summary>
        /// Verifies that the sequence contains a single matching item.
        /// The item itself is not verified.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="predicate">A function to test an element for a condition.</param>
        /// <param name="message">The message to describe the assertion in case of failure.</param>
        /// <param name="parameters">Parameters for the <paramref name="message"/>.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// <paramref name="collection"/> is <c>null</c>,
        /// no element satisfies the condition in <paramref name="predicate"/>,
        /// or more than one element satisfies the condition in <paramref name="predicate"/>.
        /// </exception>
        public static IEnumerable<T> AssertSingleItem<T>(this IEnumerable<T> collection, Func<T, bool> predicate, string message, params object[] parameters)
        {
            return AssertItems(collection, predicate, null, 1, message, parameters);
        }

        /// <summary>
        /// Verifies that the sequence contains a single item and verifies it.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="asserter">The asserting method that is called with the item, if found.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// Asserting failed for the found item in <paramref name="collection"/>,
        /// <paramref name="collection"/> is <c>null</c>,
        /// <paramref name="collection"/> is empty,
        /// or <paramref name="collection"/> contains more than one item.
        /// </exception>
        public static IEnumerable<T> AssertSingleItem<T>(this IEnumerable<T> collection, Action<T> asserter)
        {
            return AssertItems(collection, null, asserter, 1);
        }

        /// <summary>
        /// Verifies that the sequence contains a single item and verifies it.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="asserter">The asserting method that is called with the item, if found.</param>
        /// <param name="message">The message to describe the assertion in case of failure.</param>
        /// <param name="parameters">Parameters for the <paramref name="message"/>.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// Asserting failed for the found item in <paramref name="collection"/>,
        /// <paramref name="collection"/> is <c>null</c>,
        /// <paramref name="collection"/> is empty,
        /// or <paramref name="collection"/> contains more than one item.
        /// </exception>
        public static IEnumerable<T> AssertSingleItem<T>(this IEnumerable<T> collection, Action<T> asserter, string message, params object[] parameters)
        {
            return AssertItems(collection, null, asserter, 1, message, parameters);
        }

        /// <summary>
        /// Verifies that the sequence contains a single item and verifies it.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <typeparam name="TKey">The equatable type of the key to find the item.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="getKey">The function to get key of an item.</param>
        /// <param name="expectedKey">The key which identifies the item to check its existence.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// Asserting failed for the found item in <paramref name="collection"/>,
        /// <paramref name="collection"/> is <c>null</c>,
        /// <paramref name="getKey"/> is <c>null</c>,
        /// <paramref name="collection"/> is empty,
        /// or <paramref name="collection"/> contains more than one item.
        /// </exception>
        public static IEnumerable<T> AssertSingleItem<T, TKey>(this IEnumerable<T> collection, Func<T, TKey> getKey, TKey expectedKey)
            where TKey : IEquatable<TKey>
        {
            return AssertSingleItem(collection, getKey, expectedKey, null, (string)null);
        }

        /// <summary>
        /// Verifies that the sequence contains a single item and verifies it.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <typeparam name="TKey">The equatable type of the key to find the item.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="getKey">The function to get key of an item.</param>
        /// <param name="expectedKey">The key which identifies the item to check its existence.</param>
        /// <param name="message">The message to describe the assertion in case of failure.</param>
        /// <param name="parameters">Parameters for the <paramref name="message"/>.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// Asserting failed for the found item in <paramref name="collection"/>,
        /// <paramref name="collection"/> is <c>null</c>,
        /// <paramref name="getKey"/> is <c>null</c>,
        /// <paramref name="collection"/> is empty,
        /// or <paramref name="collection"/> contains more than one item.
        /// </exception>
        public static IEnumerable<T> AssertSingleItem<T, TKey>(this IEnumerable<T> collection, Func<T, TKey> getKey, TKey expectedKey, string message, params object[] parameters)
            where TKey : IEquatable<TKey>
        {
            return AssertSingleItem(collection, getKey, expectedKey, null, message, parameters);
        }

        /// <summary>
        /// Verifies that the sequence contains a single item and verifies it.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <typeparam name="TKey">The equatable type of the key to find the item.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="getKey">The function to get key of an item.</param>
        /// <param name="expectedKey">The key which identifies the item to check its existence.</param>
        /// <param name="asserter">The asserting method that is called with the item, if found.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// Asserting failed for the found item in <paramref name="collection"/>,
        /// <paramref name="collection"/> is <c>null</c>,
        /// <paramref name="getKey"/> is <c>null</c>,
        /// <paramref name="collection"/> is empty,
        /// <paramref name="asserter"/> is not <c>null</c> and assertment of found item fails,
        /// or <paramref name="collection"/> contains more than one item.
        /// </exception>
        public static IEnumerable<T> AssertSingleItem<T, TKey>(this IEnumerable<T> collection, Func<T, TKey> getKey, TKey expectedKey, Action<T> asserter)
            where TKey : IEquatable<TKey>
        {
            return AssertSingleItem(collection, getKey, expectedKey, asserter, null);
        }

        /// <summary>
        /// Verifies that the sequence contains a single item and verifies it.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <typeparam name="TKey">The equatable type of the key to find the item.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="getKey">The function to get key of an item.</param>
        /// <param name="expectedKey">The key which identifies the item to check its existence.</param>
        /// <param name="asserter">The asserting method that is called with the item, if found.</param>
        /// <param name="message">The message to describe the assertion in case of failure.</param>
        /// <param name="parameters">Parameters for the <paramref name="message"/>.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// Asserting failed for the found item in <paramref name="collection"/>,
        /// <paramref name="collection"/> is <c>null</c>,
        /// <paramref name="getKey"/> is <c>null</c>,
        /// <paramref name="collection"/> is empty,
        /// <paramref name="asserter"/> is not <c>null</c> and assertment of found item fails,
        /// or <paramref name="collection"/> contains more than one item.
        /// </exception>
        public static IEnumerable<T> AssertSingleItem<T, TKey>(this IEnumerable<T> collection, Func<T, TKey> getKey, TKey expectedKey, Action<T> asserter, string message, params object[] parameters)
            where TKey : IEquatable<TKey>
        {
            Assert.IsNotNull(collection, MessageHelper.CombineDefaultAndCustomMessage("The collection must not be null.", message, parameters));
            if (getKey == null) throw new ArgumentNullException(nameof(getKey), MessageHelper.CombineDefaultAndCustomMessage("The 'getKey'-function must not be null.", message, parameters));

            var selected = collection.Where((item) => Equals(getKey(item), expectedKey));

            int itemCount = selected.Count();
            if (itemCount == 0)
            {
                throw new AssertFailedException(
                    MessageHelper.CombineDefaultAndCustomMessage(
                        "Sequence contains no matching element. Expected key <{0}>.",
                        new object[] { expectedKey },
                        message,
                        parameters));
            }
            else if (itemCount > 1)
            {
                throw new AssertFailedException(
                    MessageHelper.CombineDefaultAndCustomMessage(
                        "Sequence contains <{0}> items for the key <{1}>. Expected was exactly one.",
                        new object[] { itemCount, expectedKey },
                        message,
                        parameters));
            }

            if (asserter != null)
            {
                var item = selected.Single();

                MessageHelper.Assert(
                    () => asserter(item),
                    MessageHelper.CombineDefaultAndCustomMessage("The assertion failed for item <{0}>.", new object[] { item }, message, parameters));
            }

            return collection;
        }

        /// <summary>
        /// Verifies every item in the collection against the asserter.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="asserter">The asserting method that is called with the specified item, if found.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// Asserting failed for a found item in <paramref name="collection"/>,
        /// <paramref name="collection"/> is <c>null</c>.
        /// </exception>
        public static IEnumerable<T> AssertItems<T>(this IEnumerable<T> collection, Action<T> asserter)
        {
            return AssertItems(collection, null, asserter, int.MinValue, null);
        }

        /// <summary>
        /// Verifies every item in the collection against the asserter.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="asserter">The asserting method that is called with the specified item, if found.</param>
        /// <param name="message">The message to describe the assertion in case of failure.</param>
        /// <param name="parameters">Parameters for the <paramref name="message"/>.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// Asserting failed for a found item in <paramref name="collection"/>,
        /// <paramref name="collection"/> is <c>null</c>.
        /// </exception>
        public static IEnumerable<T> AssertItems<T>(this IEnumerable<T> collection, Action<T> asserter, string message, params object[] parameters)
        {
            return AssertItems(collection, null, asserter, int.MinValue, message, parameters);
        }

        /// <summary>
        /// Verifies every item selected by the predicate against the asserter.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="predicate">A function to test an element for a condition.</param>
        /// <param name="asserter">The asserting method that is called with the specified item, if found.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// Asserting failed for a found item in <paramref name="collection"/>,
        /// <paramref name="collection"/> is <c>null</c>.
        /// </exception>
        public static IEnumerable<T> AssertItems<T>(this IEnumerable<T> collection, Func<T, bool> predicate, Action<T> asserter)
        {
            return AssertItems(collection, predicate, asserter, int.MinValue, null);
        }

        /// <summary>
        /// Verifies every item selected by the predicate against the asserter.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="predicate">A function to test an element for a condition.</param>
        /// <param name="asserter">The asserting method that is called with the specified item, if found.</param>
        /// <param name="message">The message to describe the assertion in case of failure.</param>
        /// <param name="parameters">Parameters for the <paramref name="message"/>.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// Asserting failed for a found item in <paramref name="collection"/>,
        /// <paramref name="collection"/> is <c>null</c>.
        /// </exception>
        public static IEnumerable<T> AssertItems<T>(this IEnumerable<T> collection, Func<T, bool> predicate, Action<T> asserter, string message, params object[] parameters)
        {
            return AssertItems(collection, predicate, asserter, int.MinValue, message, parameters);
        }

        /// <summary>
        /// Verifies every item selected by the predicate against the asserter.
        /// The count of selected items must exactly match <paramref name="expectedCount"/>.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="predicate">A function to test an element for a condition.</param>
        /// <param name="asserter">The asserting method that is called with the specified item, if found.</param>
        /// <param name="expectedCount">The expected number of elements in <paramref name="collection"/>.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// Asserting failed for a found item in <paramref name="collection"/>,
        /// <paramref name="collection"/> is <c>null</c>,
        /// not exactly <paramref name="expectedCount"/> elements satisfy the condition in <paramref name="predicate"/>.
        /// </exception>
        public static IEnumerable<T> AssertItems<T>(this IEnumerable<T> collection, Func<T, bool> predicate, Action<T> asserter, int expectedCount)
        {
            return AssertItems(collection, predicate, asserter, expectedCount, null);
        }

        /// <summary>
        /// Verifies every item selected by the predicate against the asserter.
        /// The count of selected items must exactly match <paramref name="expectedCount"/>.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="predicate">A function to test an element for a condition.</param>
        /// <param name="asserter">The asserting method that is called with the specified item, if found.</param>
        /// <param name="expectedCount">The expected number of elements in <paramref name="collection"/>.</param>
        /// <param name="message">The message to describe the assertion in case of failure.</param>
        /// <param name="parameters">Parameters for the <paramref name="message"/>.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// Asserting failed for a found item in <paramref name="collection"/>,
        /// <paramref name="collection"/> is <c>null</c>,
        /// not exactly <paramref name="expectedCount"/> elements satisfy the condition in <paramref name="predicate"/>.
        /// </exception>
        public static IEnumerable<T> AssertItems<T>(this IEnumerable<T> collection, Func<T, bool> predicate, Action<T> asserter, int expectedCount, string message, params object[] parameters)
        {
            Assert.IsNotNull(collection, MessageHelper.CombineDefaultAndCustomMessage("The collection must not be null.", message, parameters));
            Assert.IsTrue(
                expectedCount == int.MinValue || expectedCount >= 0,
                MessageHelper.CombineDefaultAndCustomMessage("ExpectedCount must be '>= 0' or 'int.MinValue' to ignore.", message, parameters));

            IEnumerable<T> selected;

            if (predicate != null)
                selected = collection.Where(predicate);
            else
                selected = collection.ToList();

            if (expectedCount != int.MinValue && expectedCount != selected.Count())
            {
                throw new AssertFailedException(
                    MessageHelper.CombineDefaultAndCustomMessage(
                        "Expected <{0}> selected elements, but was <{1}>.",
                        new object[] { expectedCount, selected.Count() },
                        message,
                        parameters));
            }

            if (asserter != null)
            {
                foreach (var item in selected)
                {
                    MessageHelper.Assert(
                        () => asserter(item),
                        MessageHelper.CombineDefaultAndCustomMessage("The assertion failed for item <{0}>.", new object[] { item }, message, parameters));
                }
            }

            return collection;
        }

        /// <summary>
        /// Verifies that a sequence does not contain any items matching the
        /// specified <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="predicate">A function to test an element for a condition.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// One or more elements in <paramref name="collection"/> match the <paramref name="predicate"/>,
        /// or <paramref name="collection"/> is <c>null</c>.
        /// </exception>
        public static IEnumerable<T> AssertContainsNot<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            return AssertContainsNot(collection, predicate, null);
        }

        /// <summary>
        /// Verifies that a sequence does not contain any items matching the
        /// specified <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="predicate">A function to test an element for a condition.</param>
        /// <param name="message">The message to describe the assertion in case of failure.</param>
        /// <param name="parameters">Parameters for the <paramref name="message"/>.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// One or more elements in <paramref name="collection"/> match the <paramref name="predicate"/>,
        /// or <paramref name="collection"/> is <c>null</c>.
        /// </exception>
        public static IEnumerable<T> AssertContainsNot<T>(this IEnumerable<T> collection, Func<T, bool> predicate, string message, params object[] parameters)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate), MessageHelper.CombineDefaultAndCustomMessage("The predicate must not be null.", message, parameters));

            Assert.IsNotNull(collection, MessageHelper.CombineDefaultAndCustomMessage("The collection must not be null.", message, parameters));

            var n = collection.Count(predicate);
            if (n > 0)
            {
                throw new AssertFailedException(
                    MessageHelper.CombineDefaultAndCustomMessage(
                        "The collection contains {0} items that match the predicate; expected were 0.",
                        new object[] { n },
                        message,
                        parameters));
            }

            return collection;
        }

        /// <summary>
        /// Verifies that each item in <paramref name="collection"/> is non-null.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// At least one item in <paramref name="collection"/> is null,
        /// or <paramref name="collection"/> is <c>null</c>.
        /// </exception>
        public static IEnumerable<T> AssertAllNotNull<T>(this IEnumerable<T> collection)
        {
            return AssertAllNotNull(collection, null);
        }

        /// <summary>
        /// Verifies that each item in <paramref name="collection"/> is non-null.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="message">The message to describe the assertion in case of failure.</param>
        /// <param name="parameters">Parameters for the <paramref name="message"/>.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="AssertFailedException">
        /// At least one item in <paramref name="collection"/> is null,
        /// or <paramref name="collection"/> is <c>null</c>.
        /// </exception>
        public static IEnumerable<T> AssertAllNotNull<T>(this IEnumerable<T> collection, string message, params object[] parameters)
        {
            Assert.IsNotNull(collection, MessageHelper.CombineDefaultAndCustomMessage("The collection must not be null.", message, parameters));

            var errorIndexes = collection
                .Select((o, i) => new { Item = o, Index = i })
                .Where(x => x.Item == null)
                .Select(x => x.Index)
                .ToList();

            if (errorIndexes.Count > 0)
            {
                throw new AssertFailedException(
                    MessageHelper.CombineDefaultAndCustomMessage(
                        "The collection contains {0} items which are null; their indexes are <{1}>.",
                        new object[] { errorIndexes.Count, string.Join(", ", errorIndexes.Select(n => n.ToString(CultureInfo.InvariantCulture)))},
                        message,
                        parameters));
            }

            return collection;
        }

        /// <summary>
        /// Verifies that two sequences are equal, that is, their elements
        /// are considered equal and appear in the same order.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="expected">The collection the unit test expects.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <c>null</c>.</exception>
        public static IEnumerable<T> AssertSequenceEquals<T>(this IEnumerable<T> collection, IEnumerable<T> expected)
        {
            return AssertSequenceEquals(collection, expected, null);
        }

        /// <summary>
        /// Verifies that two sequences are equal, that is, their elements
        /// are considered equal and appear in the same order.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="expected">The collection the unit test expects.</param>
        /// <param name="message">The message to describe the assertion in case of failure.</param>
        /// <param name="parameters">Parameters for the <paramref name="message"/>.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <c>null</c>.</exception>
        public static IEnumerable<T> AssertSequenceEquals<T>(this IEnumerable<T> collection, IEnumerable<T> expected, string message, params object[] parameters)
        {
            if (expected == null)
                throw new ArgumentNullException(nameof(expected), MessageHelper.CombineDefaultAndCustomMessage("Expected must not be null.", message, parameters));

            Assert.IsNotNull(collection);
            Assert.AreEqual(expected.Count(), collection.Count(), MessageHelper.CombineDefaultAndCustomMessage("The number of elements differ.", message, parameters));
            Assert.IsTrue(collection.SequenceEqual(expected), MessageHelper.CombineDefaultAndCustomMessage("The sequences are not equal.", message, parameters));

            return collection;
        }

        /// <summary>
        /// Verifies that two sequences are equivalent, that is, their elements
        /// are considered equal and in the same quantity, but appear in any order.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="expected">The collection the unit test expects.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <c>null</c>.</exception>
        public static IEnumerable<T> AssertSequenceIsEquivalent<T>(this IEnumerable<T> collection, IEnumerable<T> expected)
        {
            return AssertSequenceIsEquivalent(collection, expected, null);
        }

        /// <summary>
        /// Verifies that two sequences are equivalent, that is, their elements
        /// are considered equal and in the same quantity, but appear in any order.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="expected">The collection the unit test expects.</param>
        /// <param name="message">The message to describe the assertion in case of failure.</param>
        /// <param name="parameters">Parameters for the <paramref name="message"/>.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <c>null</c>.</exception>
        public static IEnumerable<T> AssertSequenceIsEquivalent<T>(this IEnumerable<T> collection, IEnumerable<T> expected, string message, params object[] parameters)
        {
            if (expected == null)
                throw new ArgumentNullException(nameof(expected), MessageHelper.CombineDefaultAndCustomMessage("Expected must not be null.", message, parameters));

            try
            {
                Assert.IsNotNull(collection);
                CollectionAssert.AreEquivalent(expected.ToArray(), collection.ToArray());
            }
            catch (AssertFailedException ex)
            {
                throw new AssertFailedException(
                    MessageHelper.CombineDefaultAndCustomMessage(
                        ex.Message,
                        message,
                        parameters));
            }

            return collection;
        }

        /// <summary>
        /// Verifies that two sequences are equal, that is, their elements
        /// are considered equal and appear in the same order.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="expected">The collection the unit test expects.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <c>null</c>.</exception>
        public static IEnumerable<T> AssertSequenceEquals<T>(this IEnumerable<T> collection, params T[] expected)
        {
            return AssertSequenceEquals(collection, expected, null);
        }

        /// <summary>
        /// Verifies that two sequences are equal, that is, their elements
        /// are considered equal and appear in the same order.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="expected">The collection the unit test expects.</param>
        /// <param name="message">The message to describe the assertion in case of failure.</param>
        /// <param name="parameters">Parameters for the <paramref name="message"/>.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <c>null</c>.</exception>
        public static IEnumerable<T> AssertSequenceEquals<T>(this IEnumerable<T> collection, T[] expected, string message, params object[] parameters)
        {
            return collection.AssertSequenceEquals<T>((IEnumerable<T>)expected, message, parameters);
        }

        /// <summary>
        /// Verifies that two sequences are equivalent, that is, their elements
        /// are considered equal and in the same quantity, but appear in any order.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="expected">The collection the unit test expects.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <c>null</c>.</exception>
        public static IEnumerable<T> AssertSequenceIsEquivalent<T>(this IEnumerable<T> collection, params T[] expected)
        {
            return AssertSequenceIsEquivalent(collection, expected, null);
        }

        /// <summary>
        /// Verifies that two sequences are equivalent, that is, their elements
        /// are considered equal and in the same quantity, but appear in any order.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <param name="expected">The collection the unit test expects.</param>
        /// <param name="message">The message to describe the assertion in case of failure.</param>
        /// <param name="parameters">Parameters for the <paramref name="message"/>.</param>
        /// <returns><paramref name="collection"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <c>null</c>.</exception>
        public static IEnumerable<T> AssertSequenceIsEquivalent<T>(this IEnumerable<T> collection, T[] expected, string message, params object[] parameters)
        {
            return collection.AssertSequenceIsEquivalent((IEnumerable<T>)expected, message, parameters);
        }
    }
}
