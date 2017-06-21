#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
#region usings
using System;
using NodaTime.Globalization;
using NodaTime.Text;
using NodaTime.Utility;

#endregion

namespace NodaTime
{
    /// <summary>
    /// Represents an instant on the global timeline.
    /// </summary>
    /// <remarks>
    /// <para>
    /// An instant is defined by an integral number of 'ticks' since the Unix epoch (typically described as January 1st
    /// 1970, midnight, UTC, ISO calendar), where a tick is equal to 100 nanoseconds. There are 10,000 ticks in a
    /// millisecond.
    /// </para>
    /// <para>
    /// An <see cref="Instant"/> has no concept of a particular time zone or calendar: it simply represents a point in
    /// time that can be globally agreed-upon.
    /// </para>
    /// </remarks>
    /// <threadsafety>This type is an immutable value type. See the thread safety section of the user guide for more information.</threadsafety>
    public struct Instant : IEquatable<Instant>, IComparable<Instant>, IFormattable, IComparable
    {
        /// <summary>
        /// String used to represent "the beginning of time" (as far as Noda Time is concerned).
        /// </summary>
        internal const string BeginningOfTimeLabel = "BOT";
        /// <summary>
        /// String used to represent "the end of time" (as far as Noda Time is concerned).
        /// </summary>
        internal const string EndOfTimeLabel = "EOT";

        /// <summary>
        /// Represents the smallest possible <see cref="Instant"/>.
        /// </summary>
        /// <remarks>
        /// Within Noda Time, this is also used to represent 'the beginning of time'.
        /// </remarks>
        public static readonly Instant MinValue = new Instant(Int64.MinValue);
        /// <summary>
        /// Represents the largest possible <see cref="Instant"/>.
        /// </summary>
        /// <remarks>
        /// Within Noda Time, this is also used to represent 'the end of time'.
        /// </remarks>
        public static readonly Instant MaxValue = new Instant(Int64.MaxValue);

        private readonly long ticks;

        /// <summary>
        /// Initializes a new instance of the <see cref="Instant" /> struct.
        /// </summary>
        /// <remarks>
        /// Note that while the Noda Time <see cref="Instant"/> type and BCL <see cref="DateTime"/> and
        /// <see cref="DateTimeOffset"/> types are all defined in terms of a number of ticks, they use different
        /// origins: the Noda Time types count ticks from the Unix epoch (the start of 1970 AD), while the BCL types
        /// count from the start of 1 AD. This constructor requires the former; to convert from a number-of-ticks since
        /// the BCL epoch, construct a <see cref="DateTime"/> first, then use <see cref="FromDateTimeUtc"/>.
        /// </remarks>
        /// <param name="ticks">The number of ticks since the Unix epoch. Negative values represent instants before the
        /// Unix epoch.</param>
        public Instant(long ticks)
        {
            this.ticks = ticks;
        }

        /// <summary>
        /// The number of ticks since the Unix epoch. Negative values represent instants before the Unix epoch.
        /// </summary>
        /// <remarks>
        /// A tick is equal to 100 nanoseconds. There are 10,000 ticks in a millisecond.
        /// </remarks>
        public long Ticks { get { return ticks; } }

        #region IComparable<Instant> and IComparable Members
        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///   A 32-bit signed integer that indicates the relative order of the objects being compared.
        ///   The return value has the following meanings:
        ///   <list type = "table">
        ///     <listheader>
        ///       <term>Value</term>
        ///       <description>Meaning</description>
        ///     </listheader>
        ///     <item>
        ///       <term>&lt; 0</term>
        ///       <description>This object is less than the <paramref name = "other" /> parameter.</description>
        ///     </item>
        ///     <item>
        ///       <term>0</term>
        ///       <description>This object is equal to <paramref name = "other" />.</description>
        ///     </item>
        ///     <item>
        ///       <term>&gt; 0</term>
        ///       <description>This object is greater than <paramref name = "other" />.</description>
        ///     </item>
        ///   </list>
        /// </returns>
        public int CompareTo(Instant other)
        {
            return Ticks.CompareTo(other.Ticks);
        }

        /// <summary>
        /// Implementation of <see cref="IComparable.CompareTo"/> to compare two instants.
        /// </summary>
        /// <remarks>
        /// This uses explicit interface implementation to avoid it being called accidentally. The generic implementation should usually be preferred.
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="obj"/> is non-null but does not refer to an instance of <see cref="Instant"/>.</exception>
        /// <param name="obj">The object to compare this value with.</param>
        /// <returns>The result of comparing this instant with another one; see <see cref="CompareTo(NodaTime.Instant)"/> for general details.
        /// If <paramref name="obj"/> is null, this method returns a value greater than 0.
        /// </returns>
        int IComparable.CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            Preconditions.CheckArgument(obj is Instant, "obj", "Object must be of type NodaTime.Instant.");
            return CompareTo((Instant)obj);
        }
        #endregion

        #region Object overrides
        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Instant)
            {
                return Equals((Instant)obj);
            }
            return false;
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing algorithms and data
        ///   structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Ticks.GetHashCode();
        }
        #endregion  // Object overrides

        /// <summary>
        /// Returns a new value of this instant with the given number of ticks added to it.
        /// </summary>
        /// <param name="ticksToAdd">The ticks to add to this instant to create the return value.</param>
        /// <returns>The result of adding the given number of ticks to this instant.</returns>
        public Instant PlusTicks(long ticksToAdd)
        {
            return new Instant(this.ticks + ticksToAdd);
        }

        #region Operators
        /// <summary>
        /// Implements the operator + (addition) for <see cref="Instant" /> + <see cref="Duration" />.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Instant" /> representing the sum of the given values.</returns>
        public static Instant operator +(Instant left, Duration right)
        {
            return new Instant(left.Ticks + right.Ticks);
        }

        /// <summary>
        /// Adds the given offset to this instant, to return a <see cref="LocalInstant" />.
        /// </summary>
        /// <remarks>
        /// This was previously an operator+ implementation, but operators can't be internal.
        /// </remarks>
        /// <param name="offset">The right hand side of the operator.</param>
        /// <returns>A new <see cref="LocalInstant" /> representing the sum of the given values.</returns>
        internal LocalInstant Plus(Offset offset)
        {
            return new LocalInstant(Ticks + offset.Ticks);
        }

        /// <summary>
        /// Adds a duration to an instant. Friendly alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Instant" /> representing the sum of the given values.</returns>
        public static Instant Add(Instant left, Duration right)
        {
            return left + right;
        }

        /// <summary>
        /// Returns the result of adding a duration to this instant, for a fluent alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="duration">The duration to add</param>
        /// <returns>A new <see cref="Instant" /> representing the result of the addition.</returns>
        public Instant Plus(Duration duration)
        {
            return this + duration;
        }

        /// <summary>
        ///   Implements the operator - (subtraction) for <see cref="Instant" /> - <see cref="Instant" />.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Instant" /> representing the difference of the given values.</returns>
        public static Duration operator -(Instant left, Instant right)
        {
            return new Duration(left.Ticks - right.Ticks);
        }

        /// <summary>
        /// Implements the operator - (subtraction) for <see cref="Instant" /> - <see cref="Duration" />.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Instant" /> representing the difference of the given values.</returns>
        public static Instant operator -(Instant left, Duration right)
        {
            return new Instant(left.Ticks - right.Ticks);
        }

        /// <summary>
        ///   Subtracts one instant from another. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration" /> representing the difference of the given values.</returns>
        public static Duration Subtract(Instant left, Instant right)
        {
            return left - right;
        }

        /// <summary>
        /// Returns the result of subtracting another instant from this one, for a fluent alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="other">The other instant to subtract</param>
        /// <returns>A new <see cref="Instant" /> representing the result of the subtraction.</returns>
        public Duration Minus(Instant other)
        {
            return this - other;
        }

        /// <summary>
        /// Subtracts a duration from an instant. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Instant" /> representing the difference of the given values.</returns>
        public static Instant Subtract(Instant left, Duration right)
        {
            return left - right;
        }

        /// <summary>
        /// Returns the result of subtracting a duration from this instant, for a fluent alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="duration">The duration to subtract</param>
        /// <returns>A new <see cref="Instant" /> representing the result of the subtraction.</returns>
        public Instant Minus(Duration duration)
        {
            return this - duration;
        }

        /// <summary>
        ///   Implements the operator == (equality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator ==(Instant left, Instant right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///   Implements the operator != (inequality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator !=(Instant left, Instant right)
        {
            return !(left == right);
        }

        /// <summary>
        ///   Implements the operator &lt; (less than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than the right value, otherwise <c>false</c>.</returns>
        public static bool operator <(Instant left, Instant right)
        {
            return left.Ticks < right.Ticks;
        }

        /// <summary>
        ///   Implements the operator &lt;= (less than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator <=(Instant left, Instant right)
        {
            return left.Ticks <= right.Ticks;
        }

        /// <summary>
        ///   Implements the operator &gt; (greater than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than the right value, otherwise <c>false</c>.</returns>
        public static bool operator >(Instant left, Instant right)
        {
            return left.Ticks > right.Ticks;
        }

        /// <summary>
        ///   Implements the operator &gt;= (greater than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator >=(Instant left, Instant right)
        {
            return left.Ticks >= right.Ticks;
        }
        #endregion // Operators

        #region Convenience methods
        /// <summary>
        /// Returns a new instant corresponding to the given UTC date and time in the ISO calendar.
        /// In most cases applications should use <see cref="ZonedDateTime" /> to represent a date
        /// and time, but this method is useful in some situations where an <see cref="Instant" /> is
        /// required, such as time zone testing.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year",
        /// so a value of 0 means 1 BC, for example.</param>
        /// <param name="monthOfYear">The month of year.</param>
        /// <param name="dayOfMonth">The day of month.</param>
        /// <param name="hourOfDay">The hour.</param>
        /// <param name="minuteOfHour">The minute.</param>
        /// <returns>An <see cref="Instant"/> value representing the given date and time in UTC and the ISO calendar.</returns>
        public static Instant FromUtc(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour)
        {
            var local = CalendarSystem.Iso.GetLocalInstant(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour);
            return new Instant(local.Ticks);
        }

        /// <summary>
        /// Returns a new instant corresponding to the given UTC date and
        /// time in the ISO calendar. In most cases applications should 
        /// use <see cref="ZonedDateTime" />
        /// to represent a date and time, but this method is useful in some 
        /// situations where an Instant is required, such as time zone testing.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year",
        /// so a value of 0 means 1 BC, for example.</param>
        /// <param name="monthOfYear">The month of year.</param>
        /// <param name="dayOfMonth">The day of month.</param>
        /// <param name="hourOfDay">The hour.</param>
        /// <param name="minuteOfHour">The minute.</param>
        /// <param name="secondOfMinute">The second.</param>
        /// <returns>An <see cref="Instant"/> value representing the given date and time in UTC and the ISO calendar.</returns>
        public static Instant FromUtc(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour, int secondOfMinute)
        {
            var local = CalendarSystem.Iso.GetLocalInstant(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, secondOfMinute);
            return new Instant(local.Ticks);
        }

        /// <summary>
        /// Returns the later instant of the given two.
        /// </summary>
        /// <param name="x">The first instant to compare.</param>
        /// <param name="y">The second instant to compare.</param>
        /// <returns>The later instant of <paramref name="x"/> or <paramref name="y"/>.</returns>
        public static Instant Max(Instant x, Instant y)
        {
            return x > y ? x : y;
        }

        /// <summary>
        /// Returns the earlier instant of the given two.
        /// </summary>
        /// <param name="x">The first instant to compare.</param>
        /// <param name="y">The second instant to compare.</param>
        /// <returns>The earlier instant of <paramref name="x"/> or <paramref name="y"/>.</returns>
        public static Instant Min(Instant x, Instant y)
        {
            return x < y ? x : y;
        }
        #endregion

        #region Formatting
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// The value of the current instance in the standard format pattern, using the current thread's
        /// culture to obtain a format provider.
        /// </returns>
        public override string ToString()
        {
            return InstantPattern.BclSupport.Format(this, null, NodaFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Formats the value of the current instance using the specified pattern.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> containing the value of the current instance in the specified format.
        /// </returns>
        /// <param name="patternText">The <see cref="T:System.String" /> specifying the pattern to use,
        /// or null to use the default format pattern.
        /// </param>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use when formatting the value,
        /// or null to use the current thread's culture to obtain a format provider.
        /// </param>
        /// <filterpriority>2</filterpriority>
        public string ToString(string patternText, IFormatProvider formatProvider)
        {
            return InstantPattern.BclSupport.Format(this, patternText, NodaFormatInfo.GetInstance(formatProvider));
        }
        #endregion Formatting

        #region IEquatable<Instant> Members
        /// <summary>
        /// Indicates whether the value of this instant is equal to the value of the specified instant.
        /// </summary>
        /// <param name="other">The value to compare with this instance.</param>
        /// <returns>
        /// true if the value of this instant is equal to the value of the <paramref name="other" /> parameter;
        /// otherwise, false.
        /// </returns>
        public bool Equals(Instant other)
        {
            return Ticks == other.Ticks;
        }
        #endregion

        /// <summary>
        /// Constructs a <see cref="DateTime"/> from this Instant which has a <see cref="DateTime.Kind" />
        /// of <see cref="DateTimeKind.Utc"/> and represents the same instant of time as this value.
        /// </summary>
        /// <returns>A <see cref="DateTime"/> representing the same instant in time as this value, with a kind of "universal".</returns>
        public DateTime ToDateTimeUtc()
        {
            return new DateTime((this - NodaConstants.BclEpoch).Ticks, DateTimeKind.Utc);
        }

        /// <summary>
        /// Constructs a <see cref="DateTimeOffset"/> from this Instant which has an offset of zero.
        /// </summary>
        /// <returns>A <see cref="DateTimeOffset"/> representing the same instant in time as this value.</returns>
        public DateTimeOffset ToDateTimeOffset()
        {
            return new DateTimeOffset((this - NodaConstants.BclEpoch).Ticks, TimeSpan.Zero);
        }

        /// <summary>
        /// Converts a <see cref="DateTimeOffset"/> into a new Instant representing the same instant in time. Note that
        /// the offset information is not preserved in the returned Instant.
        /// </summary>
        /// <returns>An <see cref="Instant"/> value representing the same instant in time as the given <see cref="DateTimeOffset"/>.</returns>
        /// <param name="dateTimeOffset">Date and time value with an offset.</param>
        public static Instant FromDateTimeOffset(DateTimeOffset dateTimeOffset)
        {
            return NodaConstants.BclEpoch.PlusTicks(dateTimeOffset.Ticks - dateTimeOffset.Offset.Ticks);
        }

        /// <summary>
        /// Converts a <see cref="DateTime"/> into a new Instant representing the same instant in time.
        /// </summary>
        /// <returns>An <see cref="Instant"/> value representing the same instant in time as the given universal <see cref="DateTime"/>.</returns>
        /// <param name="dateTime">Date and time value which must have a <see cref="DateTime.Kind"/> of <see cref="DateTimeKind.Utc"/></param>
        /// <exception cref="ArgumentException"><paramref name="dateTime"/> is not of <see cref="DateTime.Kind"/>
        /// <see cref="DateTimeKind.Utc"/>.</exception>
        public static Instant FromDateTimeUtc(DateTime dateTime)
        {
            Preconditions.CheckArgument(dateTime.Kind == DateTimeKind.Utc, "dateTime", "Invalid DateTime.Kind for Instant.FromDateTimeUtc");
            return NodaConstants.BclEpoch.PlusTicks(dateTime.Ticks);
        }

        /// <summary>
        /// Returns the <see cref="ZonedDateTime"/> representing the same point in time as this instant, in the UTC time
        /// zone and ISO-8601 calendar. This is a shortcut for calling <see cref="InZone(DateTimeZone)" /> with an
        /// argument of <see cref="DateTimeZone.Utc"/>.
        /// </summary>
        /// <returns>A <see cref="ZonedDateTime"/> for the same instant, in the UTC time zone
        /// and the ISO-8601 calendar</returns>
        public ZonedDateTime InUtc()
        {
            return new ZonedDateTime(this, DateTimeZone.Utc, CalendarSystem.Iso);
        }

        /// <summary>
        /// Returns the <see cref="ZonedDateTime"/> representing the same point in time as this instant, in the
        /// specified time zone and ISO-8601 calendar.
        /// </summary>
        /// <param name="zone">The time zone in which to represent this instant.</param>
        /// <returns>A <see cref="ZonedDateTime"/> for the same instant, in the given time zone
        /// and the ISO-8601 calendar</returns>
        /// <exception cref="ArgumentNullException"><paramref name="zone"/> is null.</exception>
        public ZonedDateTime InZone(DateTimeZone zone)
        {
            Preconditions.CheckNotNull(zone, "zone");
            return new ZonedDateTime(this, zone, CalendarSystem.Iso);
        }

        /// <summary>
        /// Returns the <see cref="ZonedDateTime"/> representing the same point in time as this instant, in the
        /// specified time zone and calendar system.
        /// </summary>
        /// <param name="zone">The time zone in which to represent this instant.</param>
        /// <param name="calendar">The calendar system in which to represent this instant.</param>
        /// <returns>A <see cref="ZonedDateTime"/> for the same instant, in the given time zone
        /// and calendar</returns>
        /// <exception cref="ArgumentNullException"><paramref name="zone"/> or <paramref name="calendar"/> is null.</exception>
        public ZonedDateTime InZone(DateTimeZone zone, CalendarSystem calendar)
        {
            Preconditions.CheckNotNull(zone, "zone");
            Preconditions.CheckNotNull(calendar, "calendar");
            return new ZonedDateTime(this, zone, calendar);
        }
    }
}