/// <reference path="$$rhino.d.ts" />

declare type GlideRecordOperationType = "delete" | "insert" | "update";

/**
 * Type representing Java packages.
 * Since access to Packages.* is not allowed in scoped apps, all classes will be represented as interfaces.
 * This was done to avoid a scenario where the source code may reference a Java class using Packages.* and transpile
 * successfully, but fail when used in ServiceNow.
 * @namespace Packages
 */
declare namespace Packages {
    /**
     * Represents a Java array.
     * @typedef {(ArrayLike<E> & java.lang.Object)} Array
     * @template E - The type of elements in this array.
     */
    export type Array<E> = ArrayLike<E> & java.lang.Object;

    export namespace java {
        export namespace io {
            /**
             * Represents the "InputStream" class.
             * @export
             * @interface InputStream
             * @extends {java.lang.Object}
             */
            export interface InputStream extends java.lang.Object {
                read(): $$rhino.Number;
                close(): void;
                reset(): void;
            }
        }

        export namespace lang {
            /**
             * Represents the base Java object class.
             * @export
             * @interface Object
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/lang/Object.html}
             */
            export interface Object {
                /**
                 * Indicates whether some other object is "equal to" this one.
                 * @param obj {*}
                 * @returns {$$rhino.Boolean}
                 */
                equals(obj: any): $$rhino.Boolean;

                /**
                 * Returns a hash code value for the object.
                 * @returns {$$rhino.Number}
                 */
                hashCode(): $$rhino.Number;

                /**
                 * Returns a string representation of the object.
                 * @returns {$$rhino.String}
                 */
                toString(): $$rhino.String;
            }

            /**
             * This interface imposes a total ordering on the objects of each class that implements it.
             * @export
             * @interface Comparable
             * @template T - The type of objects that this object may be compared to.
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/lang/Comparable.html}
             */
            export interface Comparable<T> {
                /**
                 * Compares this object with the specified object for order.
                 * @param o {T} The object to be compared.
                 * @returns {$$rhino.Number} A negative value if this object is less than the specified object;
                 * A non-zero positive number if this object is grater than the specified object;
                 * otherwise, zero if this object is equal to the specified object.
                 */
                compareTo(o: T): $$rhino.Number;
            }

            /**
             * A readable sequence of char values.
             * @export
             * @interface CharSequence
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/lang/CharSequence.html}
             */
            export interface CharSequence {
                /**
                 * Returns the char value at the specified index.
                 * @param index {$$rhino.Number}
                 * @returns {Character}
                 */
                charAt(index: $$rhino.Number): Character;

                /**
                 * Returns the length of this character sequence.
                 * @returns {$$rhino.Number}
                 */
                length(): $$rhino.Number;

                /**
                 * Returns a new CharSequence that is a subsequence of this sequence.
                 * @param start {$$rhino.Number}
                 * @param end {$$rhino.Number}
                 * @returns {CharSequence}
                 */
                subSequence(start: $$rhino.Number, end: $$rhino.Number): CharSequence;

                /**
                 * Returns a string containing the characters in this sequence in the same order as this sequence.
                 * @returns {$$rhino.String}
                 */
                toString(): $$rhino.String;
            }

            /**
             * Allows an object to be the target of the "for-each loop" statement.
             * @export
             * @interface Iterable
             * @template T - The type of elements returned by the iterator.
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/lang/Iterable.html}
             */
            export interface Iterable<T> {
                /**
                 * Returns an iterator over the elements in this collection in proper sequence.
                 * @returns {Iterator<E>}
                 */
                iterator(): util.Iterator<T>;
            }

            /**
             * Represents a java.lang.Character class or the primitive java char type.
             * @export
             * @interface Character
             * @extends {Object}
             * @extends {Comparable<Character>}
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/lang/Character.html}
             */
            export interface Character extends Object, Comparable<Character> {
                /**
                 * Returns the value of this Character object.
                 * @returns {Character}
                 */
                charValue(): Character;

                /**
                 * Compares two Character objects numerically.
                 * @param anotherCharacter {$$rhino.String}
                 * @returns {$$rhino.Number}
                 */
                compareTo(anotherCharacter: $$rhino.StringLike): $$rhino.Number;
            }

            /**
             * Represents the java.lang.String class.
             * @export
             * @interface String
             * @extends {Object}
             * @extends {Comparable<String>}
             * @extends {CharSequence}
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/lang/String.html}
             */
            export interface String extends Object, Comparable<String>, CharSequence {
                /**
                 * Returns the char value at the specified index.
                 * @param index {$$rhino.Number}
                 * @returns {Character}
                 */
                charAt(index: $$rhino.Number): Character;

                /**
                 * Returns the character (Unicode code point) at the specified index.
                 * @param index {$$rhino.Number}
                 * @returns {$$rhino.Number}
                 */
                codePointAt(index: $$rhino.Number): $$rhino.Number;

                /**
                 * Returns the character (Unicode code point) before the specified index.
                 * @param index {$$rhino.Number}
                 * @returns {$$rhino.Number}
                 */
                codePointBefore(index: $$rhino.Number): $$rhino.Number;

                /**
                 * Returns the number of Unicode code points in the specified text range of this String.
                 * @param beginIndex {$$rhino.Number}
                 * @param endIndex {$$rhino.Number}
                 * @returns {$$rhino.Number}
                 */
                codePointCount(beginIndex: $$rhino.Number, endIndex: $$rhino.Number): $$rhino.Number;

                /**
                 * Compares two strings lexicographically.
                 * @param anotherString {$$rhino.String}
                 * @returns {$$rhino.Number}
                 */
                compareTo(anotherString: $$rhino.String): $$rhino.Number;

                /**
                 * Compares two strings lexicographically, ignoring case differences.
                 * @param str {$$rhino.String}
                 * @returns {$$rhino.Number}
                 */
                compareToIgnoreCase(str: $$rhino.String): $$rhino.Number;

                /**
                 * Concatenates the specified string to the end of this string.
                 * @param str {$$rhino.String}
                 * @returns {$$rhino.String}
                 */
                concat(str: $$rhino.String): $$rhino.String;

                /**
                 * Returns true if and only if this string contains the specified sequence of char values.
                 * @param s {CharSequence}
                 * @returns {$$rhino.Boolean}
                 */
                contains(s: CharSequence): $$rhino.Boolean;

                /**
                 * Compares this string to the specified CharSequence.
                 * @param cs {CharSequence}
                 * @returns {$$rhino.Boolean}
                 */
                contentEquals(cs: CharSequence): $$rhino.Boolean;

                /**
                 * Compares this string to the specified StringBuffer.
                 * @param sb {StringBuffer}
                 * @returns {$$rhino.Boolean}
                 */
                contentEquals(sb: StringBuffer): $$rhino.Boolean;

                /**
                 * Tests if this string ends with the specified suffix.
                 * @param suffix {$$rhino.String}
                 * @returns {$$rhino.Boolean}
                 */
                endsWith(suffix: $$rhino.String): $$rhino.Boolean;

                /**
                 * Compares this String to another String, ignoring case considerations.
                 * @param anotherString {$$rhino.String}
                 * @returns {$$rhino.Boolean}
                 */
                equalsIgnoreCase(anotherString: $$rhino.String): $$rhino.Boolean;

                /**
                 * Encodes this String into a sequence of bytes using the platform's default charset, storing the result into a new byte array.
                 * @returns {Array<Byte>}
                 */
                getBytes(): Array<Byte>;

                /**
                 * Encodes this String into a sequence of bytes using the named charset, storing the result into a new byte array.
                 * @param charsetName {$$rhino.String}
                 * @returns {Array<Byte>}
                 */
                getBytes(charsetName: $$rhino.String): Array<Byte>;

                /**
                 * Copies characters from this string into the destination character array.
                 * @param srcBegin {$$rhino.Number}
                 * @param srcEnd {$$rhino.Number}
                 * @param dst {Array<$$rhino.String>}
                 * @param dstBegin {$$rhino.Number}
                 */
                getChars(srcBegin: $$rhino.Number, srcEnd: $$rhino.Number, dst: Array<$$rhino.String>, dstBegin: $$rhino.Number): void;

                /**
                 * Returns the index within this string of the first occurrence of the specified character.
                 * @param ch {$$rhino.Number}
                 * @returns {$$rhino.Number}
                 */
                indexOf(ch: $$rhino.Number): $$rhino.Number;

                /**
                 * Returns the index within this string of the first occurrence of the specified character, starting the search at the specified index.
                 * @param ch {$$rhino.Number}
                 * @param fromIndex {$$rhino.Number}
                 * @returns {$$rhino.Number}
                 */
                indexOf(ch: $$rhino.Number, fromIndex: $$rhino.Number): $$rhino.Number;

                /**
                 * Returns the index within this string of the first occurrence of the specified substring.
                 * @param str {$$rhino.String}
                 * @returns {$$rhino.Number}
                 */
                indexOf(str: $$rhino.String): $$rhino.Number;

                /**
                 * Returns the index within this string of the first occurrence of the specified substring, starting at the specified index.
                 * @param str {$$rhino.String}
                 * @param fromIndex {$$rhino.Number}
                 * @returns {$$rhino.Number}
                 */
                indexOf(str: $$rhino.String, fromIndex: $$rhino.Number): $$rhino.Number;

                /**
                 * Returns a canonical representation for the string object.
                 * @returns {$$rhino.String}
                 */
                intern(): $$rhino.String;

                /**
                 * Returns true if, and only if, length() is 0.
                 * @returns {$$rhino.Boolean}
                 */
                isEmpty(): $$rhino.Boolean;

                /**
                 * Returns the index within this string of the last occurrence of the specified character.
                 * @param ch {$$rhino.Number}
                 * @returns {$$rhino.Number}
                 */
                lastIndexOf(ch: $$rhino.Number): $$rhino.Number;

                /**
                 * Returns the index within this string of the last occurrence of the specified character, searching backward starting at the specified index.
                 * @param ch {$$rhino.Number}
                 * @param fromIndex {$$rhino.Number}
                 * @returns {$$rhino.Number}
                 */
                lastIndexOf(ch: $$rhino.Number, fromIndex: $$rhino.Number): $$rhino.Number;

                /**
                 * Returns the index within this string of the last occurrence of the specified substring.
                 * @param str {$$rhino.String}
                 * @returns {$$rhino.Number}
                 */
                lastIndexOf(str: $$rhino.String): $$rhino.Number;

                /**
                 * Returns the index within this string of the last occurrence of the specified substring, searching backward starting at the specified index.
                 * @param str {$$rhino.String}
                 * @param fromIndex {$$rhino.Number}
                 * @returns {$$rhino.Number}
                 */
                lastIndexOf(str: $$rhino.String, fromIndex: $$rhino.Number): $$rhino.Number;

                /**
                 * Returns the length of this string.
                 * @returns {$$rhino.Number}
                 */
                length(): $$rhino.Number;

                /**
                 * Tells whether or not this string matches the given regular expression.
                 * @param regex {$$rhino.String}
                 * @returns {$$rhino.Boolean}
                 */
                matches(regex: $$rhino.String): $$rhino.Boolean;

                /**
                 * Returns the index within this String that is offset from the given index by codePointOffset code points.
                 * @param index {$$rhino.Number}
                 * @param codePointOffset {$$rhino.Number}
                 * @returns {$$rhino.Number}
                 */
                offsetByCodePoints(index: $$rhino.Number, codePointOffset: $$rhino.Number): $$rhino.Number;

                /**
                 * Tests if two string regions are equal.
                 * @param ignoreCase {$$rhino.Boolean}
                 * @param toffset {$$rhino.Number}
                 * @param other {$$rhino.String}
                 * @param ooffset {$$rhino.Number}
                 * @param len {$$rhino.Number}
                 * @returns {$$rhino.Boolean}
                 */
                regionMatches(ignoreCase: $$rhino.Boolean, toffset: $$rhino.Number, other: $$rhino.String, ooffset: $$rhino.Number, len: $$rhino.Number): $$rhino.Boolean;

                /**
                 * Tests if two string regions are equal.
                 * @param toffset {$$rhino.Number}
                 * @param other {$$rhino.String}
                 * @param ooffset {$$rhino.Number}
                 * @param len {$$rhino.Number}
                 * @returns {$$rhino.Boolean}
                 */
                regionMatches(toffset: $$rhino.Number, other: $$rhino.String, ooffset: $$rhino.Number, len: $$rhino.Number): $$rhino.Boolean;

                /**
                 * Returns a new string resulting from replacing all occurrences of oldChar in this string with newChar.
                 * @param oldChar {$$rhino.String}
                 * @param newChar {$$rhino.String}
                 * @returns {$$rhino.String}
                 */
                replace(oldChar: $$rhino.String, newChar: $$rhino.String): $$rhino.String;

                /**
                 * Replaces each substring of this string that matches the literal target sequence with the specified literal replacement sequence.
                 * @param target {CharSequence}
                 * @param replacement {CharSequence}
                 * @returns {$$rhino.String}
                 */
                replace(target: CharSequence, replacement: CharSequence): $$rhino.String;

                /**
                 * Replaces each substring of this string that matches the given regular expression with the given replacement.
                 * @param regex {$$rhino.String}
                 * @param replacement {$$rhino.String}
                 * @returns {$$rhino.String}
                 */
                replaceAll(regex: $$rhino.String, replacement: $$rhino.String): $$rhino.String;

                /**
                 * Replaces the first substring of this string that matches the given regular expression with the given replacement.
                 * @param regex {$$rhino.String}
                 * @param replacement {$$rhino.String}
                 * @returns {$$rhino.String}
                 */
                replaceFirst(regex: $$rhino.String, replacement: $$rhino.String): $$rhino.String;

                /**
                 * Splits this string around matches of the given regular expression.
                 * @param regex {$$rhino.String}
                 * @returns {Array<$$rhino.String>}
                 */
                split(regex: $$rhino.String): Array<$$rhino.String>;

                /**
                 * Splits this string around matches of the given regular expression.
                 * @param regex {$$rhino.String}
                 * @param limit {$$rhino.Number}
                 * @returns {Array<$$rhino.String>}
                 */
                split(regex: $$rhino.String, limit: $$rhino.Number): Array<$$rhino.String>;

                /**
                 * Tests if this string starts with the specified prefix.
                 * @param prefix {$$rhino.String}
                 * @returns {$$rhino.Boolean}
                 */
                startsWith(prefix: $$rhino.String): $$rhino.Boolean;

                /**
                 * Tests if the substring of this string beginning at the specified index starts with the specified prefix.
                 * @param prefix {$$rhino.String}
                 * @param toffset {$$rhino.Number}
                 * @returns {$$rhino.Boolean}
                 */
                startsWith(prefix: $$rhino.String, toffset: $$rhino.Number): $$rhino.Boolean;

                /**
                 * Returns a new character sequence that is a subsequence of this sequence.
                 * @param beginIndex {$$rhino.Number}
                 * @param endIndex {$$rhino.Number}
                 * @returns {CharSequence}
                 */
                subSequence(beginIndex: $$rhino.Number, endIndex: $$rhino.Number): CharSequence;

                /**
                 * Returns a new string that is a substring of this string.
                 * @param beginIndex {$$rhino.Number}
                 * @returns {$$rhino.String}
                 */
                substring(beginIndex: $$rhino.Number): $$rhino.String;

                /**
                 * Returns a new string that is a substring of this string.
                 * @param beginIndex {$$rhino.Number}
                 * @param endIndex {$$rhino.Number}
                 * @returns {$$rhino.String}
                 */
                substring(beginIndex: $$rhino.Number, endIndex: $$rhino.Number): $$rhino.String;

                /**
                 * Converts this string to a new character array.
                 * @returns {Array<Character>}
                 */
                toCharArray(): Array<Character>;

                /**
                 * Converts all of the characters in this string to lower case using the rules of the default locale.
                 * @returns {$$rhino.String}
                 */
                toLowerCase(): $$rhino.String;

                /**
                 * Converts all of the characters in this string to lower case using the rules of the given Locale.
                 * @param locale {Locale}
                 * @returns {$$rhino.String}
                 */
                toLowerCase(locale: util.Locale): $$rhino.String;

                /**
                 * Converts all of the characters in this string to upper case using the rules of the default locale.
                 * @returns {$$rhino.String}
                 */
                toUpperCase(): $$rhino.String;

                /**
                 * Converts all of the characters in this string to upper case using the rules of the given Locale.
                 * @param locale {Locale}
                 * @returns {$$rhino.String}
                 */
                toUpperCase(locale: util.Locale): $$rhino.String;

                /**
                 * Returns a copy of the string, with leading and trailing whitespace omitted.
                 * @returns {$$rhino.String}
                 */
                trim(): $$rhino.String;
            }

            /**
             * Represents the java.lang.Number class or a primitive java number type.
             * @export
             * @interface Number
             * @extends {Object}
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/lang/Number.html}
             */
            export interface Number extends Object {
                /**
                 * Returns the value of the specified number as a byte.
                 * @returns {Byte}
                 */
                byteValue(): Byte;

                /**
                 * Returns the value of the specified number as a double.
                 * @returns {Double}
                 */
                doubleValue(): Double;

                /**
                 * Returns the value of the specified number as a float.
                 * @returns {Float}
                 */
                floatValue(): Float;

                /**
                 * Returns the value of the specified number as an int.
                 * @returns {Integer}
                 */
                intValue(): Integer;

                /**
                 * Returns the value of the specified number as a long.
                 * @returns {Long}
                 */
                longValue(): Long;

                /**
                 * Returns the value of the specified number as a short.
                 * @returns {Short}
                 */
                shortValue(): Short;
            }

            /**
             * Represents the java.lang.Boolean class or the primitive java $$rhino.Boolean type.
             * @export
             * @interface Boolean
             * @extends {Object}
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/lang/Boolean.html}
             */
            export interface Boolean extends Object {
                /**
                 * Returns the value of this Boolean object as a $$rhino.Boolean primitive.
                 * @returns {$$rhino.Boolean}
                 * @memberof {Boolean}
                 */
                booleanValue(): $$rhino.Boolean;

                /**
                 * Compares this Boolean instance with another.
                 * @param {Boolean} b -
                 * @returns {$$rhino.Number}
                 * @memberof {Boolean}
                 */
                compareTo(b: Boolean): $$rhino.Number;
            }

            /**
             * Represents the java.lang.Integer class or the primitive java int type.
             * @export
             * @interface Integer
             * @extends {Number}
             * @extends {Comparable<Integer>}
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/lang/Integer.html}
             */
            export interface Integer extends Number, Comparable<Integer> {
                /**
                 * Compares two Integer objects numerically.
                 * @param anotherInteger {$$rhino.Number}
                 * @returns {$$rhino.Number}
                 */
                compareTo(anotherInteger: $$rhino.Number): $$rhino.Number;
            }

            /**
             * Represents the java.lang.Long class or the primitive java long type.
             * @export
             * @interface Long
             * @extends {Number}
             * @extends {Comparable<Long>}
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/lang/Long.html}
             */
            export interface Long extends Number, Comparable<Long> {
                /**
                 * Compares two Long objects numerically.
                 * @param anotherLong {$$rhino.Number}
                 * @returns {$$rhino.Number}
                 */
                compareTo(anotherLong: $$rhino.Number): $$rhino.Number;
            }

            /**
             * Represents the java.lang.Double class or the primitive java double type.
             * @export
             * @interface Double
             * @extends {Number}
             * @extends {Comparable<Double>}
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/lang/Double.html}
             */
            export interface Double extends Number, Comparable<Double> {
                /**
                 * Compares two Double objects numerically.
                 * @param anotherDouble {$$rhino.Number}
                 * @returns {$$rhino.Number}
                 */
                compareTo(anotherDouble: $$rhino.Number): $$rhino.Number;

                /**
                 * Returns true if this Double value is infinitely large in magnitude, false otherwise.
                 * @returns {$$rhino.Boolean}
                 */
                isInfinite(): $$rhino.Boolean;

                /**
                 * Returns true if this Double value is a Not-a-Number (NaN), false otherwise.
                 * @returns {$$rhino.Boolean}
                 */
                isNaN(): $$rhino.Boolean;
            }

            /**
             * Represents the java.lang.Byte class or the primitive java byte type.
             * @export
             * @interface Byte
             * @extends {Number}
             * @extends {Comparable<Byte>}
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/lang/Byte.html}
             */
            export interface Byte extends Number, Comparable<Byte> {
                /**
                 * Compares two Byte objects numerically.
                 * @param anotherByte {$$rhino.Number}
                 * @returns {$$rhino.Number}
                 */
                compareTo(anotherByte: $$rhino.Number): $$rhino.Number;
            }

            /**
             * Represents the java.lang.Float class or the primitive java float type.
             * @export
             * @interface Float
             * @extends {Number}
             * @extends {Comparable<Float>}
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/lang/Float.html}
             */
            export interface Float extends Number, Comparable<Float> {
                /**
                 * Compares two Float objects numerically.
                 * @param anotherFloat {$$rhino.Number}
                 * @returns {$$rhino.Number}
                 */
                compareTo(anotherFloat: $$rhino.Number): $$rhino.Number;

                /**
                 * Returns true if this Float value is infinitely large in magnitude, false otherwise.
                 * @returns {$$rhino.Boolean}
                 */
                isInfinite(): $$rhino.Boolean;

                /**
                 * Returns true if this Float value is a Not-a-Number (NaN), false otherwise.
                 * @returns {$$rhino.Boolean}
                 */
                isNaN(): $$rhino.Boolean;
            }

            /**
             * Represents the java.lang.Short class or the primitive java short type.
             * @export
             * @interface Short
             * @extends {Number}
             * @extends {Comparable<Short>}
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/lang/Short.html}
             */
            export interface Short extends Number, Comparable<Short> {
                /**
                 * Compares two Short objects numerically.
                 * @param anotherShort {$$rhino.Number}
                 * @returns {$$rhino.Number}
                 */
                compareTo(anotherShort: $$rhino.Number): $$rhino.Number;
            }

            /**
             * Represents the java.lang.StringBuffer class.
             * @export
             * @interface StringBuffer
             * @extends {Object}
             * @extends {CharSequence}
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/lang/StringBuffer.html}
             */
            export interface StringBuffer extends Object, CharSequence {
                /**
                 * Appends the string representation of the $$rhino.Boolean argument to the sequence.
                 * @param b {$$rhino.Boolean}
                 * @returns {StringBuffer}
                 */
                append(b: $$rhino.Boolean): StringBuffer;

                /**
                 * Appends the string representation of the char argument to this sequence.
                 * @param c {$$rhino.String}
                 * @returns {StringBuffer}
                 */
                append(c: $$rhino.String): StringBuffer;

                /**
                 * Appends the string representation of the char array argument to this sequence.
                 * @param str {Array<$$rhino.String>}
                 * @returns {StringBuffer}
                 */
                append(str: Array<$$rhino.String>): StringBuffer;

                /**
                 * Appends the string representation of a subarray of the char array argument to this sequence.
                 * @param str {Array<$$rhino.String>}
                 * @param offset {$$rhino.Number}
                 * @param len {$$rhino.Number}
                 * @returns {StringBuffer}
                 */
                append(str: Array<$$rhino.String>, offset: $$rhino.Number, len: $$rhino.Number): StringBuffer;

                /**
                 * Appends the specified CharSequence to this sequence.
                 * @param s {CharSequence}
                 * @returns {StringBuffer}
                 */
                append(s: CharSequence): StringBuffer;

                /**
                 * Appends a subsequence of the specified CharSequence to this sequence.
                 * @param s {CharSequence}
                 * @param start {$$rhino.Number}
                 * @param end {$$rhino.Number}
                 * @returns {StringBuffer}
                 */
                append(s: CharSequence, start: $$rhino.Number, end: $$rhino.Number): StringBuffer;

                /**
                 * Appends the string representation of the double argument to this sequence.
                 * @param d {$$rhino.Number}
                 * @returns {StringBuffer}
                 */
                append(d: $$rhino.Number): StringBuffer;

                /**
                 * Appends the string representation of the float argument to this sequence.
                 * @param f {$$rhino.Number}
                 * @returns {StringBuffer}
                 */
                append(f: $$rhino.Number): StringBuffer;

                /**
                 * Appends the string representation of the int argument to this sequence.
                 * @param i {$$rhino.Number}
                 * @returns {StringBuffer}
                 */
                append(i: $$rhino.Number): StringBuffer;

                /**
                 * Appends the string representation of the long argument to this sequence.
                 * @param lng {$$rhino.Number}
                 * @returns {StringBuffer}
                 */
                append(lng: $$rhino.Number): StringBuffer;

                /**
                 * Appends the string representation of the Object argument.
                 * @param obj {Object}
                 * @returns {StringBuffer}
                 */
                append(obj: Object): StringBuffer;

                /**
                 * Appends the specified string to this character sequence.
                 * @param str {$$rhino.String}
                 * @returns {StringBuffer}
                 */
                append(str: $$rhino.String): StringBuffer;

                /**
                 * Appends the specified StringBuffer to this sequence.
                 * @param sb {StringBuffer}
                 * @returns {StringBuffer}
                 */
                append(sb: StringBuffer): StringBuffer;

                /**
                 * Appends the string representation of the codePoint argument to this sequence.
                 * @param codePoint {$$rhino.Number}
                 * @returns {StringBuffer}
                 */
                appendCodePoint(codePoint: $$rhino.Number): StringBuffer;

                /**
                 * Returns the current capacity.
                 * @returns {$$rhino.Number}
                 */
                capacity(): $$rhino.Number;

                /**
                 * Returns the char value in this sequence at the specified index.
                 * @param index {$$rhino.Number}
                 * @returns {Character}
                 */
                charAt(index: $$rhino.Number): lang.Character;

                /**
                 * Returns the character (Unicode code point) at the specified index.
                 * @param index {$$rhino.Number}
                 * @returns {$$rhino.Number}
                 */
                codePointAt(index: $$rhino.Number): $$rhino.Number;

                /**
                 * Returns the character (Unicode code point) before the specified index.
                 * @param index {$$rhino.Number}
                 * @returns {$$rhino.Number}
                 */
                codePointBefore(index: $$rhino.Number): $$rhino.Number;

                /**
                 * Returns the number of Unicode code points in the specified text range of this sequence.
                 * @param beginIndex {$$rhino.Number}
                 * @param endIndex {$$rhino.Number}
                 * @returns {$$rhino.Number}
                 */
                codePointCount(beginIndex: $$rhino.Number, endIndex: $$rhino.Number): $$rhino.Number;

                /**
                 * Removes the characters in a substring of this sequence.
                 * @param start {$$rhino.Number}
                 * @param end {$$rhino.Number}
                 * @returns {StringBuffer}
                 */
                delete(start: $$rhino.Number, end: $$rhino.Number): StringBuffer;

                /**
                 * Removes the char at the specified position in this sequence.
                 * @param index {$$rhino.Number}
                 * @returns {StringBuffer}
                 */
                deleteCharAt(index: $$rhino.Number): StringBuffer;

                /**
                 * Ensures that the capacity is at least equal to the specified minimum.
                 * @param minimumCapacity {$$rhino.Number}
                 */
                ensureCapacity(minimumCapacity: $$rhino.Number): void;

                /**
                 * Characters are copied from this sequence into the destination character array dst.
                 * @param srcBegin {$$rhino.Number}
                 * @param srcEnd {$$rhino.Number}
                 * @param dst {Array<$$rhino.String>}
                 * @param dstBegin {$$rhino.Number}
                 */
                getChars(srcBegin: $$rhino.Number, srcEnd: $$rhino.Number, dst: Array<$$rhino.String>, dstBegin: $$rhino.Number): void;

                /**
                 * Returns the index within this string of the first occurrence of the specified substring.
                 * @param str {$$rhino.String}
                 * @returns {$$rhino.Number}
                 */
                indexOf(str: $$rhino.String): $$rhino.Number;

                /**
                 * Returns the index within this string of the first occurrence of the specified substring, starting at the specified index.
                 * @param str {$$rhino.String}
                 * @param fromIndex {$$rhino.Number}
                 * @returns {$$rhino.Number}
                 */
                indexOf(str: $$rhino.String, fromIndex: $$rhino.Number): $$rhino.Number;

                /**
                 * Inserts the string representation of the $$rhino.Boolean argument into this sequence.
                 * @param offset {$$rhino.Number}
                 * @param b {$$rhino.Boolean}
                 * @returns {StringBuffer}
                 */
                insert(offset: $$rhino.Number, b: $$rhino.Boolean): StringBuffer;

                /**
                 * Inserts the string representation of the char argument into this sequence.
                 * @param offset {$$rhino.Number}
                 * @param c {$$rhino.String}
                 * @returns {StringBuffer}
                 */
                insert(offset: $$rhino.Number, c: $$rhino.String): StringBuffer;

                /**
                 * Inserts the string representation of the char array argument into this sequence.
                 * @param offset {$$rhino.Number}
                 * @param str {Array<$$rhino.String>}
                 * @returns {StringBuffer}
                 */
                insert(offset: $$rhino.Number, str: Array<$$rhino.String>): StringBuffer;

                /**
                 * Inserts the string representation of a subarray of the str array argument into this sequence.
                 * @param index {$$rhino.Number}
                 * @param str {Array<$$rhino.String>}
                 * @param offset {$$rhino.Number}
                 * @param len {$$rhino.Number}
                 * @returns {StringBuffer}
                 */
                insert(index: $$rhino.Number, str: Array<$$rhino.String>, offset: $$rhino.Number, len: $$rhino.Number): StringBuffer;

                /**
                 * Inserts the specified CharSequence into this sequence.
                 * @param dstOffset {$$rhino.Number}
                 * @param s {CharSequence}
                 * @returns {StringBuffer}
                 */
                insert(dstOffset: $$rhino.Number, s: lang.CharSequence): StringBuffer;

                /**
                 * Inserts a subsequence of the specified CharSequence into this sequence.
                 * @param dstOffset {$$rhino.Number}
                 * @param s {CharSequence}
                 * @param start {$$rhino.Number}
                 * @param end {$$rhino.Number}
                 * @returns {StringBuffer}
                 */
                insert(dstOffset: $$rhino.Number, s: lang.CharSequence, start: $$rhino.Number, end: $$rhino.Number): StringBuffer;

                /**
                 * Inserts the string representation of the double argument into this sequence.
                 * @param offset {$$rhino.Number}
                 * @param d {$$rhino.Number}
                 * @returns {StringBuffer}
                 */
                insert(offset: $$rhino.Number, d: $$rhino.Number): StringBuffer;

                /**
                 * Inserts the string representation of the float argument into this sequence.
                 * @param offset {$$rhino.Number}
                 * @param f {$$rhino.Number}
                 * @returns {StringBuffer}
                 */
                insert(offset: $$rhino.Number, f: $$rhino.Number): StringBuffer;

                /**
                 * Inserts the string representation of the second int argument into this sequence.
                 * @param offset {$$rhino.Number}
                 * @param i {$$rhino.Number}
                 * @returns {StringBuffer}
                 */
                insert(offset: $$rhino.Number, i: $$rhino.Number): StringBuffer;

                /**
                 * Inserts the string representation of the long argument into this sequence.
                 * @param offset {$$rhino.Number}
                 * @param l {$$rhino.Number}
                 * @returns {StringBuffer}
                 */
                insert(offset: $$rhino.Number, l: $$rhino.Number): StringBuffer;

                /**
                 * Inserts the string representation of the Object argument into this character sequence.
                 * @param offset {$$rhino.Number}
                 * @param obj {Object}
                 * @returns {StringBuffer}
                 */
                insert(offset: $$rhino.Number, obj: Object): StringBuffer;

                /**
                 * Inserts the string into this character sequence.
                 * @param offset {$$rhino.Number}
                 * @param str {$$rhino.String}
                 * @returns {StringBuffer}
                 */
                insert(offset: $$rhino.Number, str: $$rhino.String): StringBuffer;

                /**
                 * Returns the index within this string of the rightmost occurrence of the specified substring.
                 * @param str {$$rhino.String}
                 * @returns {$$rhino.Number}
                 */
                lastIndexOf(str: $$rhino.String): $$rhino.Number;

                /**
                 * Returns the index within this string of the last occurrence of the specified substring.
                 * @param str {$$rhino.String}
                 * @param fromIndex {$$rhino.Number}
                 * @returns {$$rhino.Number}
                 */
                lastIndexOf(str: $$rhino.String, fromIndex: $$rhino.Number): $$rhino.Number;

                /**
                 * Returns the length (character count).
                 * @returns {$$rhino.Number}
                 */
                length(): $$rhino.Number;

                /**
                 * Returns the index within this sequence that is offset from the given index by codePointOffset code points.
                 * @param index {$$rhino.Number}
                 * @param codePointOffset {$$rhino.Number}
                 * @returns {$$rhino.Number}
                 */
                offsetByCodePoints(index: $$rhino.Number, codePointOffset: $$rhino.Number): $$rhino.Number;

                /**
                 * Replaces the characters in a substring of this sequence with characters in the specified string.
                 * @param start {$$rhino.Number}
                 * @param end {$$rhino.Number}
                 * @param str {$$rhino.String}
                 * @returns {StringBuffer}
                 */
                replace(start: $$rhino.Number, end: $$rhino.Number, str: $$rhino.String): StringBuffer;

                /**
                 * Causes this character sequence to be replaced by the reverse of the sequence.
                 * @returns {StringBuffer}
                 */
                reverse(): StringBuffer;

                /**
                 * The character at the specified index is set to ch.
                 * @param index {$$rhino.Number}
                 * @param ch {$$rhino.String}
                 */
                setCharAt(index: $$rhino.Number, ch: $$rhino.String): void;

                /**
                 * Sets the length of the character sequence.
                 * @param newLength {$$rhino.Number}
                 */
                setLength(newLength: $$rhino.Number): void;

                /**
                 * Returns a new character sequence that is a subsequence of this sequence.
                 * @param start {$$rhino.Number}
                 * @param end {$$rhino.Number}
                 * @returns {CharSequence}
                 */
                subSequence(start: $$rhino.Number, end: $$rhino.Number): lang.CharSequence;

                /**
                 * Returns a new string that contains a subsequence of characters currently contained in this character sequence.
                 * @param start {$$rhino.Number}
                 * @returns {$$rhino.String}
                 */
                substring(start: $$rhino.Number): $$rhino.String;

                /**
                 * Returns a new string that contains a subsequence of characters currently contained in this sequence.
                 * @param start {$$rhino.Number}
                 * @param end {$$rhino.Number}
                 * @returns {$$rhino.String}
                 */
                substring(start: $$rhino.Number, end: $$rhino.Number): $$rhino.String;

                /**
                 * Attempts to reduce storage used for the character sequence.
                 */
                trimToSize(): void;
            }
        }

        export namespace util {
            /**
             * Represents a specific geographical, political, or cultural region.
             * @export
             * @interface Locale
             * @extends {lang.Object}
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/util/Locale.html}
             */
            export interface Locale extends java.lang.Object {
                /**
                 * Overrides Cloneable.
                 * @returns {lang.Object}
                 */
                clone(): lang.Object;

                /**
                 * Returns the country/region code for this locale, which should either be the empty string, an uppercase ISO 3166 2-letter code, or a UN M.49 3-digit code.
                 * @returns {$$rhino.String}
                 */
                getCountry(): $$rhino.String;

                /**
                 * Returns a name for the locale's country that is appropriate for display to the user.
                 * @returns {$$rhino.String}
                 */
                getDisplayCountry(): $$rhino.String;

                /**
                 * Returns a name for the locale's country that is appropriate for display to the user.
                 * @param inLocale {Locale}
                 * @returns {$$rhino.String}
                 */
                getDisplayCountry(inLocale: Locale): $$rhino.String;

                /**
                 * Returns a name for the locale's language that is appropriate for display to the user.
                 * @returns {$$rhino.String}
                 */
                getDisplayLanguage(): $$rhino.String;

                /**
                 * Returns a name for the locale's language that is appropriate for display to the user.
                 * @param inLocale {Locale}
                 * @returns {$$rhino.String}
                 */
                getDisplayLanguage(inLocale: Locale): $$rhino.String;

                /**
                 * Returns a name for the locale that is appropriate for display to the user.
                 * @returns {$$rhino.String}
                 */
                getDisplayName(): $$rhino.String;

                /**
                 * Returns a name for the locale that is appropriate for display to the user.
                 * @param inLocale {Locale}
                 * @returns {$$rhino.String}
                 */
                getDisplayName(inLocale: Locale): $$rhino.String;

                /**
                 * Returns a name for the the locale's script that is appropriate for display to the user.
                 * @returns {$$rhino.String}
                 */
                getDisplayScript(): $$rhino.String;

                /**
                 * Returns a name for the locale's script that is appropriate for display to the user.
                 * @param inLocale {Locale}
                 * @returns {$$rhino.String}
                 */
                getDisplayScript(inLocale: Locale): $$rhino.String;

                /**
                 * Returns a name for the locale's variant code that is appropriate for display to the user.
                 * @returns {$$rhino.String}
                 */
                getDisplayVariant(): $$rhino.String;

                /**
                 * Returns a name for the locale's variant code that is appropriate for display to the user.
                 * @param inLocale {Locale}
                 * @returns {$$rhino.String}
                 */
                getDisplayVariant(inLocale: Locale): $$rhino.String;

                /**
                 * Returns the extension (or private use) value associated with the specified key, or null if there is no extension associated with the key.
                 * @param key {$$rhino.String}
                 * @returns {$$rhino.String}
                 */
                getExtension(key: $$rhino.String): $$rhino.String;

                /**
                 * Returns the set of extension keys associated with this locale, or the empty set if it has no extensions.
                 * @returns {Set<Character>}
                 */
                getExtensionKeys(): Set<java.lang.Character>;

                /**
                 * Returns a three-letter abbreviation for this locale's country.
                 * @returns {$$rhino.String}
                 */
                getISO3Country(): $$rhino.String;

                /**
                 * Returns a three-letter abbreviation of this locale's language.
                 * @returns {$$rhino.String}
                 */
                getISO3Language(): $$rhino.String;

                /**
                 * Returns the language code of this Locale.
                 * @returns {$$rhino.String}
                 */
                getLanguage(): $$rhino.String;

                /**
                 * Returns the script for this locale, which should either be the empty string or an ISO 15924 4-letter script code.
                 * @returns {$$rhino.String}
                 */
                getScript(): $$rhino.String;

                /**
                 * Returns the set of unicode locale attributes associated with this locale, or the empty set if it has no attributes.
                 * @returns {Set<$$rhino.String>}
                 */
                getUnicodeLocaleAttributes(): Set<$$rhino.String>;

                /**
                 * Returns the set of Unicode locale keys defined by this locale, or the empty set if this locale has none.
                 * @returns {Set<$$rhino.String>}
                 */
                getUnicodeLocaleKeys(): Set<$$rhino.String>;

                /**
                 * Returns the Unicode locale type associated with the specified Unicode locale key for this locale.
                 * @param key {$$rhino.String}
                 * @returns {$$rhino.String}
                 */
                getUnicodeLocaleType(key: $$rhino.String): $$rhino.String;

                /**
                 * Returns the variant code for this locale.
                 * @returns {$$rhino.String}
                 */
                getVariant(): $$rhino.String;

                /**
                 * Returns a well-formed IETF BCP 47 language tag representing this locale.
                 * @returns {$$rhino.String}
                 */
                toLanguageTag(): $$rhino.String;
            }

            /**
             * Represents a group of objects, known as its elements
             * @export
             * @interface Collection
             * @extends {lang.Iterable<E>}
             * @template E - The type of elements in this collection.
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/util/Collection.html}
             */
            export interface Collection<E> extends lang.Iterable<E> {
                /**
                 * Ensures that this collection contains the specified element (optional operation).
                 * @param e {E}
                 * @returns {$$rhino.Boolean}
                 */
                add(e: E): $$rhino.Boolean;

                /**
                 * Adds all of the elements in the specified collection to this collection (optional operation).
                 * @param c {util.Collection<E>}
                 * @returns {$$rhino.Boolean}
                 */
                addAll(c: util.Collection<E>): $$rhino.Boolean;

                /**
                 * Removes all of the elements from this collection (optional operation).
                 */
                clear(): void;

                /**
                 * Returns true if this collection contains the specified element.
                 * @param o {*}
                 * @returns {$$rhino.Boolean}
                 */
                contains(o: any): $$rhino.Boolean;

                /**
                 * Returns true if this collection contains all of the elements in the specified collection.
                 * @param c {util.Collection<any>}
                 * @returns {$$rhino.Boolean}
                 */
                containsAll(c: util.Collection<any>): $$rhino.Boolean;

                /**
                 * Returns true if this collection contains no elements.
                 * @returns {$$rhino.Boolean}
                 */
                isEmpty(): $$rhino.Boolean;

                /**
                 * Returns an iterator over the elements in this collection.
                 * @returns {Iterator<E>}
                 */
                iterator(): Iterator<E>;

                /**
                 * Removes a single instance of the specified element from this collection, if it is present (optional operation).
                 * @param o {*}
                 * @returns {$$rhino.Boolean}
                 */
                remove(o: any): $$rhino.Boolean;

                /**
                 * Removes all of this collection's elements that are also contained in the specified collection (optional operation).
                 * @param c {util.Collection<any>}
                 * @returns {$$rhino.Boolean}
                 */
                removeAll(c: util.Collection<any>): $$rhino.Boolean;

                /**
                 * Retains only the elements in this collection that are contained in the specified collection (optional operation).
                 * @param c {util.Collection<any>}
                 * @returns {$$rhino.Boolean}
                 */
                retainAll(c: util.Collection<any>): $$rhino.Boolean;

                /**
                 * Returns the number of elements in this collection.
                 * @returns {$$rhino.Number}
                 */
                size(): $$rhino.Number;

                /**
                 * Returns an array containing all of the elements in this collection.
                 * @returns {*}
                 */
                toArray(): any;

                /**
                 * Returns a string representation of this collection.
                 * @returns {$$rhino.String}
                 */
                toString(): $$rhino.String;
            }

            /**
             * An iterator over a collection.
             * @export
             * @interface Iterator
             * @template E - The type of elements returned by this iterator.
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/util/Iterator.html}
             */
            export interface Iterator<E> {
                /**
                 * Returns true if the iteration has more elements.
                 * @returns {$$rhino.Boolean}
                 */
                hasNext(): $$rhino.Boolean;

                /**
                 * Returns the next element in the iteration.
                 * @returns {E}
                 */
                next(): E;

                /**
                 * Removes from the underlying collection the last element returned by this iterator.
                 */
                remove(): void;
            }

            /**
             * An iterator for lists that allows the programmer to traverse the list in either direction, modify the list during iteration, and obtain the iterator's current position in the list.
             * @export
             * @interface ListIterator
             * @extends {Iterator<E>}
             * @template E - The type of elements returned by this iterator.
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/util/ListIterator.html}
             */
            export interface ListIterator<E> extends Iterator<E> {
                /**
                 * Inserts the specified element into the list (optional operation).
                 * @param e {E}
                 */
                add(e: E): void;

                /**
                 * Returns true if this list iterator has more elements when traversing the list in the forward direction.
                 * @returns {$$rhino.Boolean}
                 */
                hasNext(): $$rhino.Boolean;

                /**
                 * Returns true if this list iterator has more elements when traversing the list in the reverse direction.
                 * @returns {$$rhino.Boolean}
                 */
                hasPrevious(): $$rhino.Boolean;

                /**
                 * Returns the next element in the list and advances the cursor position.
                 * @returns {E}
                 */
                next(): E;

                /**
                 * Returns the index of the element that would be returned by a subsequent call to next().
                 * @returns {$$rhino.Number}
                 */
                nextIndex(): $$rhino.Number;

                /**
                 * Returns the previous element in the list and moves the cursor position backwards.
                 * @returns {E}
                 */
                previous(): E;

                /**
                 * Returns the index of the element that would be returned by a subsequent call to previous().
                 * @returns {$$rhino.Number}
                 */
                previousIndex(): $$rhino.Number;

                /**
                 * Removes from the list the last element that was returned by next() or previous() (optional operation).
                 */
                remove(): void;

                /**
                 * Replaces the last element returned by next() or previous() with the specified element (optional operation).
                 * @param e {E}
                 */
                set(e: E): void;
            }

            /**
             * An ordered collection (also known as a sequence).
             * @export
             * @interface List
             * @extends {util.Collection<E>}
             * @template E
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/util/List.html}
             */
            export interface List<E> extends util.Collection<E> {
                /**
                 * Appends the specified element to the end of this list (optional operation).
                 * @param e {E}
                 * @returns {$$rhino.Boolean}
                 */
                add(e: E): $$rhino.Boolean;

                /**
                 * Inserts the specified element at the specified position in this list (optional operation).
                 * @param index {$$rhino.Number}
                 * @param element {E}
                 */
                add(index: $$rhino.Number, element: E): void;

                /**
                 * Appends all of the elements in the specified collection to the end of this list, in the order that they are returned by the specified collection's iterator (optional operation).
                 * @param c {util.Collection< E>}
                 * @returns {$$rhino.Boolean}
                 */
                addAll(c: util.Collection<E>): $$rhino.Boolean;

                /**
                 * Inserts all of the elements in the specified collection into this list at the specified position (optional operation).
                 * @param index {$$rhino.Number}
                 * @param c {util.Collection< E>}
                 * @returns {$$rhino.Boolean}
                 */
                addAll(index: $$rhino.Number, c: util.Collection<E>): $$rhino.Boolean;

                /**
                 * Removes all of the elements from this list (optional operation).
                 */
                clear(): void;

                /**
                 * Returns true if this list contains the specified element.
                 * @param o {*}
                 * @returns {$$rhino.Boolean}
                 */
                contains(o: any): $$rhino.Boolean;

                /**
                 * Returns true if this list contains all of the elements of the specified collection.
                 * @param c {util.Collection<any>}
                 * @returns {$$rhino.Boolean}
                 */
                containsAll(c: util.Collection<any>): $$rhino.Boolean;

                /**
                 * Returns the element at the specified position in this list.
                 * @param index {$$rhino.Number}
                 * @returns {E}
                 */
                get(index: $$rhino.Number): E;

                /**
                 * Returns the index of the first occurrence of the specified element in this list, or -1 if this list does not contain the element.
                 * @param o {*}
                 * @returns {$$rhino.Number}
                 */
                indexOf(o: any): $$rhino.Number;

                /**
                 * Returns true if this list contains no elements.
                 * @returns {$$rhino.Boolean}
                 */
                isEmpty(): $$rhino.Boolean;

                /**
                 * Returns an iterator over the elements in this list in proper sequence.
                 * @returns {Iterator<E>}
                 */
                iterator(): Iterator<E>;

                /**
                 * Returns the index of the last occurrence of the specified element in this list, or -1 if this list does not contain the element.
                 * @param o {*}
                 * @returns {$$rhino.Number}
                 */
                lastIndexOf(o: any): $$rhino.Number;

                /**
                 * Returns a list iterator over the elements in this list (in proper sequence).
                 * @returns {ListIterator<E>}
                 */
                listIterator(): ListIterator<E>;

                /**
                 * Returns a list iterator over the elements in this list (in proper sequence), starting at the specified position in the list.
                 * @param index {$$rhino.Number}
                 * @returns {ListIterator<E>}
                 */
                listIterator(index: $$rhino.Number): ListIterator<E>;

                /**
                 * Removes the element at the specified position in this list (optional operation).
                 * @param index {$$rhino.Number}
                 * @returns {E}
                 */
                remove(index: $$rhino.Number): E;

                /**
                 * Removes the first occurrence of the specified element from this list, if it is present (optional operation).
                 * @param o {*}
                 * @returns {$$rhino.Boolean}
                 */
                remove(o: any): $$rhino.Boolean;

                /**
                 * Removes from this list all of its elements that are contained in the specified collection (optional operation).
                 * @param c {util.Collection<any>}
                 * @returns {$$rhino.Boolean}
                 */
                removeAll(c: util.Collection<any>): $$rhino.Boolean;

                /**
                 * Retains only the elements in this list that are contained in the specified collection (optional operation).
                 * @param c {util.Collection<any>}
                 * @returns {$$rhino.Boolean}
                 */
                retainAll(c: util.Collection<any>): $$rhino.Boolean;

                /**
                 * Replaces the element at the specified position in this list with the specified element (optional operation).
                 * @param index {$$rhino.Number}
                 * @param element {E}
                 * @returns {E}
                 */
                set(index: $$rhino.Number, element: E): E;

                /**
                 * Returns the number of elements in this list.
                 * @returns {$$rhino.Number}
                 */
                size(): $$rhino.Number;

                /**
                 * Returns a view of the portion of this list between the specified fromIndex, inclusive, and toIndex, exclusive.
                 * @param fromIndex {$$rhino.Number}
                 * @param toIndex {$$rhino.Number}
                 * @returns {List<E>}
                 */
                subList(fromIndex: $$rhino.Number, toIndex: $$rhino.Number): List<E>;

                /**
                 * Returns an array containing all of the elements in this list in proper sequence (from first to last element).
                 * @returns {*}
                 */
                toArray(): any;
            }

            /**
             * Represents the java.util.AbstractCollection class.
             * @export
             * @interface AbstractCollection
             * @extends {lang.Object}
             * @extends {util.Collection<E>}
             * @template E
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/util/AbstractCollection.html}
             */
            export interface AbstractCollection<E> extends lang.Object, util.Collection<E>  {
                /**
                 * Ensures that this collection contains the specified element (optional operation).
                 * @param e {E}
                 * @returns {$$rhino.Boolean}
                 */
                add(e: E): $$rhino.Boolean;

                /**
                 * Adds all of the elements in the specified collection to this collection (optional operation).
                 * @param c {util.Collection< E>}
                 * @returns {$$rhino.Boolean}
                 */
                addAll(c: util.Collection<E>): $$rhino.Boolean;

                /**
                 * Removes all of the elements from this collection (optional operation).
                 */
                clear(): void;

                /**
                 * Returns true if this collection contains the specified element.
                 * @param o {*}
                 * @returns {$$rhino.Boolean}
                 */
                contains(o: any): $$rhino.Boolean;

                /**
                 * Returns true if this collection contains all of the elements in the specified collection.
                 * @param c {util.Collection<any>}
                 * @returns {$$rhino.Boolean}
                 */
                containsAll(c: util.Collection<any>): $$rhino.Boolean;

                /**
                 * Returns true if this collection contains no elements.
                 * @returns {$$rhino.Boolean}
                 */
                isEmpty(): $$rhino.Boolean;

                /**
                 * Returns an iterator over the elements contained in this collection.
                 * @returns {Iterator<E>}
                 */
                iterator(): Iterator<E>;

                /**
                 * Removes a single instance of the specified element from this collection, if it is present (optional operation).
                 * @param o {*}
                 * @returns {$$rhino.Boolean}
                 */
                remove(o: any): $$rhino.Boolean;

                /**
                 * Removes all of this collection's elements that are also contained in the specified collection (optional operation).
                 * @param c {util.Collection<any>}
                 * @returns {$$rhino.Boolean}
                 */
                removeAll(c: util.Collection<any>): $$rhino.Boolean;

                /**
                 * Retains only the elements in this collection that are contained in the specified collection (optional operation).
                 * @param c {util.Collection<any>}
                 * @returns {$$rhino.Boolean}
                 */
                retainAll(c: util.Collection<any>): $$rhino.Boolean;

                /**
                 * Returns the number of elements in this collection.
                 * @returns {$$rhino.Number}
                 */
                size(): $$rhino.Number;

                /**
                 * Returns an array containing all of the elements in this collection.
                 * @returns {*}
                 */
                toArray(): any;

                /**
                 * Returns a string representation of this collection.
                 * @returns {$$rhino.String}
                 */
                toString(): $$rhino.String;
            }

            /**
             * Represents the java.util.AbstractList class.
             * @export
             * @interface AbstractList
             * @extends {AbstractCollection<E>}
             * @extends {List<E>}
             * @template E
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/util/AbstractList.html}
             */
            export interface AbstractList<E> extends AbstractCollection<E>, List<E> {
                /**
                 * Appends the specified element to the end of this list (optional operation).
                 * @param e {E}
                 * @returns {$$rhino.Boolean}
                 */
                add(e: E): $$rhino.Boolean;

                /**
                 * Inserts the specified element at the specified position in this list (optional operation).
                 * @param index {$$rhino.Number}
                 * @param element {E}
                 */
                add(index: $$rhino.Number, element: E): void;

                /**
                 * Inserts all of the elements in the specified collection into this list at the specified position (optional operation).
                 * @param index {$$rhino.Number}
                 * @param c {util.Collection< E>}
                 * @returns {$$rhino.Boolean}
                 */
                addAll(c: util.Collection<E>): $$rhino.Boolean;
                addAll(index: $$rhino.Number, c: util.Collection<E>): $$rhino.Boolean;

                /**
                 * Removes all of the elements from this list (optional operation).
                 */
                clear(): void;

                /**
                 * Returns the element at the specified position in this list.
                 * @param index {$$rhino.Number}
                 * @returns {E}
                 */
                get(index: $$rhino.Number): E;

                /**
                 * Returns the hash code value for this list.
                 * @returns {$$rhino.Number}
                 */
                hashCode(): $$rhino.Number;

                /**
                 * Returns the index of the first occurrence of the specified element in this list, or -1 if this list does not contain the element.
                 * @param o {*}
                 * @returns {$$rhino.Number}
                 */
                indexOf(o: any): $$rhino.Number;

                /**
                 * Returns an iterator over the elements in this list in proper sequence.
                 * @returns {Iterator<E>}
                 */
                iterator(): Iterator<E>;

                /**
                 * Returns the index of the last occurrence of the specified element in this list, or -1 if this list does not contain the element.
                 * @param o {*}
                 * @returns {$$rhino.Number}
                 */
                lastIndexOf(o: any): $$rhino.Number;

                /**
                 * Returns a list iterator over the elements in this list (in proper sequence).
                 * @returns {ListIterator<E>}
                 */
                listIterator(): ListIterator<E>;

                /**
                 * Returns a list iterator over the elements in this list (in proper sequence), starting at the specified position in the list.
                 * @param index {$$rhino.Number}
                 * @returns {ListIterator<E>}
                 */
                listIterator(index: $$rhino.Number): ListIterator<E>;

                /**
                 * Removes the element at the specified position in this list (optional operation).
                 * @param index {$$rhino.Number}
                 * @returns {E}
                 */
                remove(index: $$rhino.Number): E;
                remove(o: any): $$rhino.Boolean;

                /**
                 * Replaces the element at the specified position in this list with the specified element (optional operation).
                 * @param index {$$rhino.Number}
                 * @param element {E}
                 * @returns {E}
                 */
                set(index: $$rhino.Number, element: E): E;

                /**
                 * Returns a view of the portion of this list between the specified fromIndex, inclusive, and toIndex, exclusive.
                 * @param fromIndex {$$rhino.Number}
                 * @param toIndex {$$rhino.Number}
                 * @returns {List<E>}
                 */
                subList(fromIndex: $$rhino.Number, toIndex: $$rhino.Number): List<E>;
            }

            /**
             * Represents the java.util.ArrayList class.
             * @export
             * @interface ArrayList
             * @extends {AbstractList<E>}
             * @extends {List<E>}
             * @template E
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/util/ArrayList.html}
             */
            export interface ArrayList<E> extends AbstractList<E>, List<E> {
                /**
                 * Appends the specified element to the end of this list.
                 * @param e {E}
                 * @returns {$$rhino.Boolean}
                 */
                add(e: E): $$rhino.Boolean;

                /**
                 * Inserts the specified element at the specified position in this list.
                 * @param index {$$rhino.Number}
                 * @param element {E}
                 */
                add(index: $$rhino.Number, element: E): void;

                /**
                 * Appends all of the elements in the specified collection to the end of this list, in the order that they are returned by the specified collection's Iterator.
                 * @param c {util.Collection< E>}
                 * @returns {$$rhino.Boolean}
                 */
                addAll(c: util.Collection<E>): $$rhino.Boolean;

                /**
                 * Inserts all of the elements in the specified collection into this list, starting at the specified position.
                 * @param index {$$rhino.Number}
                 * @param c {util.Collection< E>}
                 * @returns {$$rhino.Boolean}
                 */
                addAll(index: $$rhino.Number, c: util.Collection<E>): $$rhino.Boolean;

                /**
                 * Removes all of the elements from this list.
                 */
                clear(): void;

                /**
                 * Returns a shallow copy of this ArrayList instance.
                 * @returns {lang.Object}
                 */
                clone(): lang.Object;

                /**
                 * Returns true if this list contains the specified element.
                 * @param o {*}
                 * @returns {$$rhino.Boolean}
                 */
                contains(o: any): $$rhino.Boolean;

                /**
                 * Increases the capacity of this ArrayList instance, if necessary, to ensure that it can hold at least the number of elements specified by the minimum capacity argument.
                 * @param minCapacity {$$rhino.Number}
                 */
                ensureCapacity(minCapacity: $$rhino.Number): void;

                /**
                 * Returns the element at the specified position in this list.
                 * @param index {$$rhino.Number}
                 * @returns {E}
                 */
                get(index: $$rhino.Number): E;

                /**
                 * Returns the index of the first occurrence of the specified element in this list, or -1 if this list does not contain the element.
                 * @param o {*}
                 * @returns {$$rhino.Number}
                 */
                indexOf(o: any): $$rhino.Number;

                /**
                 * Returns true if this list contains no elements.
                 * @returns {$$rhino.Boolean}
                 */
                isEmpty(): $$rhino.Boolean;

                /**
                 * Returns an iterator over the elements in this list in proper sequence.
                 * @returns {Iterator<E>}
                 */
                iterator(): Iterator<E>;

                /**
                 * Returns the index of the last occurrence of the specified element in this list, or -1 if this list does not contain the element.
                 * @param o {*}
                 * @returns {$$rhino.Number}
                 */
                lastIndexOf(o: any): $$rhino.Number;

                /**
                 * Returns a list iterator over the elements in this list (in proper sequence).
                 * @returns {ListIterator<E>}
                 */
                listIterator(): ListIterator<E>;

                /**
                 * Returns a list iterator over the elements in this list (in proper sequence), starting at the specified position in the list.
                 * @param index {$$rhino.Number}
                 * @returns {ListIterator<E>}
                 */
                listIterator(index: $$rhino.Number): ListIterator<E>;

                /**
                 * Removes the element at the specified position in this list.
                 * @param index {$$rhino.Number}
                 * @returns {E}
                 */
                remove(index: $$rhino.Number): E;

                /**
                 * Removes the first occurrence of the specified element from this list, if it is present.
                 * @param o {*}
                 * @returns {$$rhino.Boolean}
                 */
                remove(o: any): $$rhino.Boolean;

                /**
                 * Removes from this list all of its elements that are contained in the specified collection.
                 * @param c {util.Collection<any>}
                 * @returns {$$rhino.Boolean}
                 */
                removeAll(c: util.Collection<any>): $$rhino.Boolean;

                /**
                 * Retains only the elements in this list that are contained in the specified collection.
                 * @param c {util.Collection<any>}
                 * @returns {$$rhino.Boolean}
                 */
                retainAll(c: util.Collection<any>): $$rhino.Boolean;

                /**
                 * Replaces the element at the specified position in this list with the specified element.
                 * @param index {$$rhino.Number}
                 * @param element {E}
                 * @returns {E}
                 */
                set(index: $$rhino.Number, element: E): E;

                /**
                 * Returns the number of elements in this list.
                 * @returns {$$rhino.Number}
                 */
                size(): $$rhino.Number;

                /**
                 * Returns a view of the portion of this list between the specified fromIndex, inclusive, and toIndex, exclusive.
                 * @param fromIndex {$$rhino.Number}
                 * @param toIndex {$$rhino.Number}
                 * @returns {List<E>}
                 */
                subList(fromIndex: $$rhino.Number, toIndex: $$rhino.Number): List<E>;

                /**
                 * Returns an array containing all of the elements in this list in proper sequence (from first to last element).
                 * @returns {*}
                 */
                toArray(): any;

                /**
                 * Trims the capacity of this ArrayList instance to be the list's current size.
                 */
                trimToSize(): void;
            }

            /**
             * A map entry (key-value pair).
             * @export
             * @interface MapEntry
             * @template K - The type of key.
             * @template V - The type of mapped value.
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/util/Map.Entry.html}
             */
            export interface MapEntry<K, V> {
                /**
                 * Returns the key corresponding to this entry.
                 * @returns {K}
                 */
                getKey(): K;

                /**
                 * Returns the value corresponding to this entry.
                 * @returns {V}
                 */
                getValue(): V;

                /**
                 * Replaces the value corresponding to this entry with the specified value (optional operation).
                 * @param value {V}
                 * @returns {V}
                 */
                setValue(value: V): V;
            }

            /**
             * A collection that contains no duplicate elements.
             * @export
             * @interface Set
             * @extends {util.Collection<E>}
             * @template E - The type of elements maintained by this set.
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/util/Set.html}
             */
            export interface Set<E> extends util.Collection<E> {
                /**
                 * Adds the specified element to this set if it is not already present (optional operation).
                 * @param e {E}
                 * @returns {$$rhino.Boolean}
                 */
                add(e: E): $$rhino.Boolean;

                /**
                 * Adds all of the elements in the specified collection to this set if they're not already present (optional operation).
                 * @param c {util.Collection< E>}
                 * @returns {$$rhino.Boolean}
                 */
                addAll(c: util.Collection<E>): $$rhino.Boolean;

                /**
                 * Removes all of the elements from this set (optional operation).
                 */
                clear(): void;

                /**
                 * Returns true if this set contains the specified element.
                 * @param o {*}
                 * @returns {$$rhino.Boolean}
                 */
                contains(o: any): $$rhino.Boolean;

                /**
                 * Returns true if this set contains all of the elements of the specified collection.
                 * @param c {util.Collection<any>}
                 * @returns {$$rhino.Boolean}
                 */
                containsAll(c: util.Collection<any>): $$rhino.Boolean;

                /**
                 * Returns the hash code value for this set.
                 * @returns {$$rhino.Number}
                 */
                hashCode(): $$rhino.Number;

                /**
                 * Returns true if this set contains no elements.
                 * @returns {$$rhino.Boolean}
                 */
                isEmpty(): $$rhino.Boolean;

                /**
                 * Returns an iterator over the elements in this set.
                 * @returns {Iterator<E>}
                 */
                iterator(): Iterator<E>;

                /**
                 * Removes the specified element from this set if it is present (optional operation).
                 * @param o {*}
                 * @returns {$$rhino.Boolean}
                 */
                remove(o: any): $$rhino.Boolean;

                /**
                 * Removes from this set all of its elements that are contained in the specified collection (optional operation).
                 * @param c {util.Collection<any>}
                 * @returns {$$rhino.Boolean}
                 */
                removeAll(c: util.Collection<any>): $$rhino.Boolean;

                /**
                 * Retains only the elements in this set that are contained in the specified collection (optional operation).
                 * @param c {util.Collection<any>}
                 * @returns {$$rhino.Boolean}
                 */
                retainAll(c: util.Collection<any>): $$rhino.Boolean;

                /**
                 * Returns the number of elements in this set (its cardinality).
                 * @returns {$$rhino.Number}
                 */
                size(): $$rhino.Number;

                /**
                 * Returns an array containing all of the elements in this set.
                 * @returns {*}
                 */
                toArray(): any;
            }

            /**
             * An object that maps keys to values.
             * @export
             * @interface Map
             * @template K - The type of keys maintained by this map.
             * @template V - The type of mapped values.
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/util/Map.html}
             */
            export interface Map<K, V> {
                /**
                 * Removes all of the mappings from this map (optional operation).
                 */
                clear(): void;

                /**
                 * Returns true if this map contains a mapping for the specified key.
                 * @param key {*}
                 * @returns {$$rhino.Boolean}
                 */
                containsKey(key: any): $$rhino.Boolean;

                /**
                 * Returns true if this map maps one or more keys to the specified value.
                 * @param value {*}
                 * @returns {$$rhino.Boolean}
                 */
                containsValue(value: any): $$rhino.Boolean;

                /**
                 * Returns a Set view of the mappings contained in this map.
                 * @returns {util.Set<MapEntry<K,V>>}
                 */
                entrySet(): util.Set<MapEntry<K, V>>;

                /**
                 * Returns the value to which the specified key is mapped, or null if this map contains no mapping for the key.
                 * @param key {*}
                 * @returns {V}
                 */
                get(key: any): V;

                /**
                 * Returns the hash code value for this map.
                 * @returns {$$rhino.Number}
                 */
                hashCode(): $$rhino.Number;

                /**
                 * Returns true if this map contains no key-value mappings.
                 * @returns {$$rhino.Boolean}
                 */
                isEmpty(): $$rhino.Boolean;

                /**
                 * Returns a Set view of the keys contained in this map.
                 * @returns {util.Set<K>}
                 */
                keySet(): util.Set<K>;

                /**
                 * Associates the specified value with the specified key in this map (optional operation).
                 * @param key {K}
                 * @param value {V}
                 * @returns {V}
                 */
                put(key: K, value: V): V;

                /**
                 * Removes the mapping for a key from this map if it is present (optional operation).
                 * @param key {*}
                 * @returns {V}
                 */
                remove(key: any): V;

                /**
                 * Returns the number of key-value mappings in this map.
                 * @returns {$$rhino.Number}
                 */
                size(): $$rhino.Number;

                /**
                 * Returns a util.Collection view of the values contained in this map.
                 * @returns {util.Collection<V>}
                 */
                values(): util.Collection<V>;
            }

            /**
             * Represents the java.util.AbstractMap class.
             * @export
             * @class AbstractMap
             * @extends {lang.Object}
             * @implements {Map<K, V>}
             * @template K
             * @template V
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/util/AbstractMap.html}
             */
            export interface AbstractMap<K, V> extends lang.Object, Map<K, V> {
                /**
                 * Removes all of the mappings from this map (optional operation).
                 */
                clear(): void;

                /**
                 * Returns true if this map contains a mapping for the specified key.
                 * @param key {*}
                 * @returns {$$rhino.Boolean}
                 */
                containsKey(key: any): $$rhino.Boolean;

                /**
                 * Returns true if this map maps one or more keys to the specified value.
                 * @param value {*}
                 * @returns {$$rhino.Boolean}
                 */
                containsValue(value: any): $$rhino.Boolean;

                /**
                 * Returns a Set view of the mappings contained in this map.
                 * @returns {util.Set<Map.Entry<K,V>>}
                 */
                entrySet(): util.Set<MapEntry<K, V>>;

                /**
                 * Returns the value to which the specified key is mapped, or null if this map contains no mapping for the key.
                 * @param key {*}
                 * @returns {V}
                 */
                get(key: any): V;

                /**
                 * Returns the hash code value for this map.
                 * @returns {$$rhino.Number}
                 */
                hashCode(): $$rhino.Number;

                /**
                 * Returns true if this map contains no key-value mappings.
                 * @returns {$$rhino.Boolean}
                 */
                isEmpty(): $$rhino.Boolean;

                /**
                 * Returns a Set view of the keys contained in this map.
                 * @returns {util.Set<K>}
                 */
                keySet(): util.Set<K>;

                /**
                 * Associates the specified value with the specified key in this map (optional operation).
                 * @param key {K}
                 * @param value {V}
                 * @returns {V}
                 */
                put(key: K, value: V): V;

                /**
                 * Removes the mapping for a key from this map if it is present (optional operation).
                 * @param key {*}
                 * @returns {V}
                 */
                remove(key: any): V;

                /**
                 * Returns the number of key-value mappings in this map.
                 * @returns {$$rhino.Number}
                 */
                size(): $$rhino.Number;

                /**
                 * Returns a util.Collection view of the values contained in this map.
                 * @returns {util.Collection<V>}
                 */
                values(): util.Collection<V>;
            }

            /**
             * Represents the java.util.HashMap class.
             * @export
             * @interface HashMap
             * @extends {AbstractMap<K, V>}
             * @extends {Map<K, V>}
             * @template K
             * @template V
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/util/HashMap.html}
             */
            export interface HashMap<K, V> extends AbstractMap<K, V>, Map<K, V> {
                /**
                 * Removes all of the mappings from this map.
                 */
                clear(): void;

                /**
                 * Returns a shallow copy of this HashMap instance: the keys and values themselves are not cloned.
                 * @returns {lang.Object}
                 */
                clone(): lang.Object;

                /**
                 * Returns true if this map contains a mapping for the specified key.
                 * @param key {*}
                 * @returns {$$rhino.Boolean}
                 */
                containsKey(key: any): $$rhino.Boolean;

                /**
                 * Returns true if this map maps one or more keys to the specified value.
                 * @param value {*}
                 * @returns {$$rhino.Boolean}
                 */
                containsValue(value: any): $$rhino.Boolean;

                /**
                 * Returns a Set view of the mappings contained in this map.
                 * @returns {util.Set<Map.Entry<K,V>>}
                 */
                entrySet(): util.Set<MapEntry<K, V>>;

                /**
                 * Returns the value to which the specified key is mapped, or null if this map contains no mapping for the key.
                 * @param key {*}
                 * @returns {V}
                 */
                get(key: any): V;

                /**
                 * Returns true if this map contains no key-value mappings.
                 * @returns {$$rhino.Boolean}
                 */
                isEmpty(): $$rhino.Boolean;

                /**
                 * Returns a Set view of the keys contained in this map.
                 * @returns {util.Set<K>}
                 */
                keySet(): util.Set<K>;

                /**
                 * Associates the specified value with the specified key in this map.
                 * @param key {K}
                 * @param value {V}
                 * @returns {V}
                 */
                put(key: K, value: V): V;

                /**
                 * Removes the mapping for the specified key from this map if present.
                 * @param key {*}
                 * @returns {V}
                 */
                remove(key: any): V;

                /**
                 * Returns the number of key-value mappings in this map.
                 * @returns {$$rhino.Number}
                 */
                size(): $$rhino.Number;

                /**
                 * Returns a util.Collection view of the values contained in this map.
                 * @returns {util.Collection<V>}
                 */
                values(): util.Collection<V>;
            }

            /**
             * Represents the java.util.AbstractSet class.
             * @export
             * @interface AbstractSet
             * @extends {AbstractCollection<E>}
             * @extends {util.Set<E>}
             * @template E
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/util/AbstractSet.html}
             */
            export interface AbstractSet<E> extends AbstractCollection<E>, util.Set<E> {
                /**
                 * Removes from this set all of its elements that are contained in the specified collection (optional operation).
                 * @param c {util.Collection<any>}
                 * @returns {$$rhino.Boolean}
                 */
                removeAll(c: util.Collection<any>): $$rhino.Boolean;
            }

            /**
             * Represents the java.util.HashSet class.
             * @export
             * @interface HashSet
             * @extends {AbstractSet<E>}
             * @extends {util.Set<E>}
             * @template E
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/util/HashSet.html}
             */
            export interface HashSet<E> extends AbstractSet<E>, util.Set<E> {
                /**
                 * Adds the specified element to this set if it is not already present.
                 * @param e {E}
                 * @returns {$$rhino.Boolean}
                 */
                add(e: E): $$rhino.Boolean;

                /**
                 * Removes all of the elements from this set.
                 */
                clear(): void;

                /**
                 * Returns a shallow copy of this HashSet instance: the elements themselves are not cloned.
                 * @returns {lang.Object}
                 */
                clone(): lang.Object;

                /**
                 * Returns true if this set contains the specified element.
                 * @param o {*}
                 * @returns {$$rhino.Boolean}
                 */
                contains(o: any): $$rhino.Boolean;

                /**
                 * Returns true if this set contains no elements.
                 * @returns {$$rhino.Boolean}
                 */
                isEmpty(): $$rhino.Boolean;

                /**
                 * Returns an iterator over the elements in this set.
                 * @returns {Iterator<E>}
                 */
                iterator(): Iterator<E>;

                /**
                 * Removes the specified element from this set if it is present.
                 * @param o {*}
                 * @returns {$$rhino.Boolean}
                 */
                remove(o: any): $$rhino.Boolean;

                /**
                 * Returns the number of elements in this set (its cardinality).
                 * @returns {$$rhino.Number}
                 */
                size(): $$rhino.Number;
            }

            /**
             * Represents the java.util.Date class.
             * @export
             * @interface Date
             * @extends {lang.Object}
             * @extends {lang.Comparable<Date>}
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/util/Date.html}
             */
            export interface Date extends lang.Object, lang.Comparable<Date> {
                /**
                 * Tests if this date is after the specified date.
                 * @param when {Date}
                 * @returns {$$rhino.Boolean}
                 */
                after(when: Date): $$rhino.Boolean;

                /**
                 * Tests if this date is before the specified date.
                 * @param when {Date}
                 * @returns {$$rhino.Boolean}
                 */
                before(when: Date): $$rhino.Boolean;

                /**
                 * Return a copy of this object.
                 * @returns {lang.Object}
                 */
                clone(): lang.Object;

                /**
                 * Compares two Dates for ordering.
                 * @param anotherDate {Date}
                 * @returns {$$rhino.Number}
                 */
                compareTo(anotherDate: Date): $$rhino.Number;

                /**
                 * Returns the number of milliseconds since January 1, 1970, 00:00:00 GMT represented by this Date object.
                 * @returns {$$rhino.Number}
                 */
                getTime(): $$rhino.Number;

                /**
                 * Sets this Date object to represent a point in time that is time milliseconds after January 1, 1970 00:00:00 GMT.
                 * @param time {$$rhino.Number}
                 */
                setTime(time: $$rhino.Number): void;

            }

            /**
             * Represents the java.util.TimeZone class.
             * @export
             * @interface TimeZone
             * @see {@link https://docs.oracle.com/javase/10/docs/api/java/util/TimeZone.html}
             */
            export interface TimeZone {
                /**
                 * Creates a copy of this TimeZone.
                 * @returns {lang.Object}
                 */
                clone(): lang.Object;

                /**
                 * Returns a long standard time name of this TimeZone suitable for presentation to the user in the default locale.
                 * @returns {$$rhino.String}
                 */
                getDisplayName(): $$rhino.String;

                /**
                 * Returns a name in the specified style of this TimeZone suitable for presentation to the user in the default locale.
                 * @param daylight {$$rhino.Boolean}
                 * @param style {$$rhino.Number}
                 * @returns {$$rhino.String}
                 */
                getDisplayName(daylight: $$rhino.Boolean, style: $$rhino.Number): $$rhino.String;

                /**
                 * Returns a name in the specified style of this TimeZone suitable for presentation to the user in the specified locale.
                 * @param daylight {$$rhino.Boolean}
                 * @param style {$$rhino.Number}
                 * @param locale {Locale}
                 * @returns {$$rhino.String}
                 */
                getDisplayName(daylight: $$rhino.Boolean, style: $$rhino.Number, locale: Locale): $$rhino.String;

                /**
                 * Returns a long standard time name of this TimeZone suitable for presentation to the user in the specified locale.
                 * @param locale {Locale}
                 * @returns {$$rhino.String}
                 */
                getDisplayName(locale: Locale): $$rhino.String;

                /**
                 * Returns the amount of time to be added to local standard time to get local wall clock time.
                 * @returns {$$rhino.Number}
                 */
                getDSTSavings(): $$rhino.Number;

                /**
                 * Gets the ID of this time zone.
                 * @returns {$$rhino.String}
                 */
                getID(): $$rhino.String;

                /**
                 * Gets the time zone offset, for current date, modified in case of daylight savings.
                 * @param era {$$rhino.Number}
                 * @param year {$$rhino.Number}
                 * @param month {$$rhino.Number}
                 * @param day {$$rhino.Number}
                 * @param dayOfWeek {$$rhino.Number}
                 * @param milliseconds {$$rhino.Number}
                 * @returns {$$rhino.Number}
                 */
                getOffset(era: $$rhino.Number, year: $$rhino.Number, month: $$rhino.Number, day: $$rhino.Number, dayOfWeek: $$rhino.Number, milliseconds: $$rhino.Number): $$rhino.Number;

                /**
                 * Returns the offset of this time zone from UTC at the specified date.
                 * @param date {$$rhino.Number}
                 * @returns {$$rhino.Number}
                 */
                getOffset(date: $$rhino.Number): $$rhino.Number;

                /**
                 * Returns the amount of time in milliseconds to add to UTC to get standard time in this time zone.
                 * @returns {$$rhino.Number}
                 */
                getRawOffset(): $$rhino.Number;

                /**
                 * Returns true if this zone has the same rule and offset as another zone.
                 * @param other {TimeZone}
                 * @returns {$$rhino.Boolean}
                 */
                hasSameRules(other: TimeZone): $$rhino.Boolean;

                /**
                 * Queries if the given date is in Daylight Saving Time in this time zone.
                 * @param date {Date}
                 * @returns {$$rhino.Boolean}
                 */
                inDaylightTime(date: Date): $$rhino.Boolean;

                /**
                 * Returns true if this TimeZone is currently in Daylight Saving Time, or if a transition from Standard Time to Daylight Saving Time occurs at any future time.
                 * @returns {$$rhino.Boolean}
                 */
                observesDaylightTime(): $$rhino.Boolean;

                /**
                 * Sets the time zone ID.
                 * @param ID {$$rhino.String}
                 */
                setID(ID: $$rhino.String): void;

                /**
                 * Sets the base time zone offset to GMT.
                 * @param offsetMillis {$$rhino.Number}
                 */
                setRawOffset(offsetMillis: $$rhino.Number): void;

                /**
                 * Queries if this TimeZone uses Daylight Saving Time.
                 * @returns {$$rhino.Boolean}
                 */
                useDaylightTime(): $$rhino.Boolean;
            }
        }
    }

    export namespace org {
        export namespace w3c {
            export namespace dom {
                /**
                 * The primary datatype for the entire Document Object Model.
                 * @export
                 * @interface Node
                 * @see {@link https://docs.oracle.com/javase/10/docs/api/org/w3c/dom/Node.html}
                 */
                export interface Node {
                    /**
                     * Adds the node newChild to the end of the list of children of this node.
                     * @param newChild {Node}
                     * @returns {Node}
                     */
                    appendChild(newChild: Node): Node;

                    /**
                     * Returns a duplicate of this node, i.e., serves as a generic copy constructor for nodes.
                     * @param deep {$$rhino.Boolean}
                     * @returns {Node}
                     */
                    cloneNode(deep: $$rhino.Boolean): Node;

                    /**
                     * Compares the reference node, i.e.
                     * @param other {Node}
                     * @returns {$$rhino.Number}
                     */
                    compareDocumentPosition(other: Node): $$rhino.Number;

                    /**
                     * A NamedNodeMap containing the attributes of this node (if it is an Element) or null otherwise.
                     * @returns {NamedNodeMap}
                     */
                    getAttributes(): NamedNodeMap;

                    /**
                     * The absolute base URI of this node or null if the implementation wasn't able to obtain an absolute URI.
                     * @returns {$$rhino.String}
                     */
                    getBaseURI(): $$rhino.String;

                    /**
                     * A NodeList that contains all children of this node.
                     * @returns {NodeList}
                     */
                    getChildNodes(): NodeList;

                    /**
                     * This method returns a specialized object which implements the specialized APIs of the specified feature and version, as specified in.
                     * @param feature {$$rhino.String}
                     * @param version {$$rhino.String}
                     * @returns {*}
                     */
                    getFeature(feature: $$rhino.String, version: $$rhino.String): any;

                    /**
                     * The first child of this node.
                     * @returns {Node}
                     */
                    getFirstChild(): Node;

                    /**
                     * The last child of this node.
                     * @returns {Node}
                     */
                    getLastChild(): Node;

                    /**
                     * Returns the local part of the qualified name of this node.
                     * @returns {$$rhino.String}
                     */
                    getLocalName(): $$rhino.String;

                    /**
                     * The namespace URI of this node, or null if it is unspecified (see ).
                     * @returns {$$rhino.String}
                     */
                    getNamespaceURI(): $$rhino.String;

                    /**
                     * The node immediately following this node.
                     * @returns {Node}
                     */
                    getNextSibling(): Node;

                    /**
                     * The name of this node, depending on its type; see the table above.
                     * @returns {$$rhino.String}
                     */
                    getNodeName(): $$rhino.String;

                    /**
                     * A code representing the type of the underlying object, as defined above.
                     * @returns {$$rhino.Number}
                     */
                    getNodeType(): $$rhino.Number;

                    /**
                     * The value of this node, depending on its type; see the table above.
                     * @returns {$$rhino.String}
                     */
                    getNodeValue(): $$rhino.String;

                    /**
                     * The Document object associated with this node.
                     * @returns {Document}
                     */
                    getOwnerDocument(): Document;

                    /**
                     * The parent of this node.
                     * @returns {Node}
                     */
                    getParentNode(): Node;

                    /**
                     * The namespace prefix of this node, or null if it is unspecified.
                     * @returns {$$rhino.String}
                     */
                    getPrefix(): $$rhino.String;

                    /**
                     * The node immediately preceding this node.
                     * @returns {Node}
                     */
                    getPreviousSibling(): Node;

                    /**
                     * This attribute returns the text content of this node and its descendants.
                     * @returns {$$rhino.String}
                     */
                    getTextContent(): $$rhino.String;

                    /**
                     * Retrieves the object associated to a key on a this node.
                     * @param key {$$rhino.String}
                     * @returns {*}
                     */
                    getUserData(key: $$rhino.String): any;

                    /**
                     * Returns whether this node (if it is an element) has any attributes.
                     * @returns {$$rhino.Boolean}
                     */
                    hasAttributes(): $$rhino.Boolean;

                    /**
                     * Returns whether this node has any children.
                     * @returns {$$rhino.Boolean}
                     */
                    hasChildNodes(): $$rhino.Boolean;

                    /**
                     * Inserts the node newChild before the existing child node refChild.
                     * @param newChild {Node}
                     * @param refChild {Node}
                     * @returns {Node}
                     */
                    insertBefore(newChild: Node, refChild: Node): Node;

                    /**
                     * This method checks if the specified namespaceURI is the default namespace or not.
                     * @param namespaceURI {$$rhino.String}
                     * @returns {$$rhino.Boolean}
                     */
                    isDefaultNamespace(namespaceURI: $$rhino.String): $$rhino.Boolean;

                    /**
                     * Tests whether two nodes are equal.
                     * @param arg {Node}
                     * @returns {$$rhino.Boolean}
                     */
                    isEqualNode(arg: Node): $$rhino.Boolean;

                    /**
                     * Returns whether this node is the same node as the given one.
                     * @param other {Node}
                     * @returns {$$rhino.Boolean}
                     */
                    isSameNode(other: Node): $$rhino.Boolean;

                    /**
                     * Tests whether the DOM implementation implements a specific feature and that feature is supported by this node, as specified in.
                     * @param feature {$$rhino.String}
                     * @param version {$$rhino.String}
                     * @returns {$$rhino.Boolean}
                     */
                    isSupported(feature: $$rhino.String, version: $$rhino.String): $$rhino.Boolean;

                    /**
                     * Look up the namespace URI associated to the given prefix, starting from this node.
                     * @param prefix {$$rhino.String}
                     * @returns {$$rhino.String}
                     */
                    lookupNamespaceURI(prefix: $$rhino.String): $$rhino.String;

                    /**
                     * Look up the prefix associated to the given namespace URI, starting from this node.
                     * @param namespaceURI {$$rhino.String}
                     * @returns {$$rhino.String}
                     */
                    lookupPrefix(namespaceURI: $$rhino.String): $$rhino.String;

                    /**
                     * Puts all Text nodes in the full depth of the sub-tree underneath this Node.
                     */
                    normalize(): void;

                    /**
                     * Removes the child node indicated by oldChild from the list of children, and returns it.
                     * @param oldChild {Node}
                     * @returns {Node}
                     */
                    removeChild(oldChild: Node): Node;

                    /**
                     * Replaces the child node oldChild with newChild in the list of children, and returns the oldChild node.
                     * @param newChild {Node}
                     * @param oldChild {Node}
                     * @returns {Node}
                     */
                    replaceChild(newChild: Node, oldChild: Node): Node;

                    /**
                     * The value of this node, depending on its type; see the table above.
                     * @param nodeValue {$$rhino.String}
                     */
                    setNodeValue(nodeValue: $$rhino.String): void;

                    /**
                     * The namespace prefix of this node, or null if it is unspecified.
                     * @param prefix {$$rhino.String}
                     */
                    setPrefix(prefix: $$rhino.String): void;

                    /**
                     * This attribute returns the text content of this node and its descendants.
                     * @param textContent {$$rhino.String}
                     */
                    setTextContent(textContent: $$rhino.String): void;
                }

                /**
                 * Represents an attribute in an {@link Element} object.
                 * @export
                 * @interface Attr
                 * @see {@link https://docs.oracle.com/javase/10/docs/api/org/w3c/dom/Attr.html}
                 */
                export interface Attr {
                    /**
                     * Returns the name of this attribute.
                     * @returns {$$rhino.String}
                     */
                    getName(): $$rhino.String;

                    /**
                     * The Element node this attribute is attached to or null if this attribute is not in use.
                     * @returns {Element}
                     */
                    getOwnerElement(): Element;

                    /**
                     * The type information associated with this attribute.
                     * @returns {TypeInfo}
                     */
                    getSchemaTypeInfo(): TypeInfo;

                    /**
                     * True if this attribute was explicitly given a value in the instance document, false otherwise.
                     * @returns {$$rhino.Boolean}
                     */
                    getSpecified(): $$rhino.Boolean;

                    /**
                     * On retrieval, the value of the attribute is returned as a string.
                     * @returns {$$rhino.String}
                     */
                    getValue(): $$rhino.String;

                    /**
                     * Returns whether this attribute is known to be of type ID (i.e.
                     * @returns {$$rhino.Boolean}
                     */
                    isId(): $$rhino.Boolean;

                    /**
                     * On retrieval, the value of the attribute is returned as a string.
                     * @param value {$$rhino.String}
                     */
                    setValue(value: $$rhino.String): void;
                }

                /**
                 * Provides the abstraction of an ordered collection of nodes, without defining or constraining how this collection is implemented.
                 * @export
                 * @interface NodeList
                 * @see {@link https://docs.oracle.com/javase/10/docs/api/org/w3c/dom/NodeList.html}
                 */
                export interface NodeList {
                    /**
                     * Test if a name is part of this NameList.
                     * @param str {$$rhino.String}
                     * @returns {$$rhino.Boolean}
                     */
                    contains(str: $$rhino.String): $$rhino.Boolean;

                    /**
                     * Test if the pair namespaceURI/name is part of this NameList.
                     * @param namespaceURI {$$rhino.String}
                     * @param name {$$rhino.String}
                     * @returns {$$rhino.Boolean}
                     */
                    containsNS(namespaceURI: $$rhino.String, name: $$rhino.String): $$rhino.Boolean;

                    /**
                     * The number of pairs (name and namespaceURI) in the list.
                     * @returns {$$rhino.Number}
                     */
                    getLength(): $$rhino.Number;

                    /**
                     * Returns the indexth name item in the collection.
                     * @param index {$$rhino.Number}
                     * @returns {$$rhino.String}
                     */
                    getName(index: $$rhino.Number): $$rhino.String;

                    /**
                     * Returns the indexth namespaceURI item in the collection.
                     * @param index {$$rhino.Number}
                     * @returns {$$rhino.String}
                     */
                    getNamespaceURI(index: $$rhino.Number): $$rhino.String;
                }

                /**
                 * Represents a type referenced from {@link Element} or {@link Attr} nodes, specified in the schemas associated with the document
                 * @export
                 * @interface TypeInfo
                 * @see {@link https://docs.oracle.com/javase/10/docs/api/org/w3c/dom/TypeInfo.html}
                 */
                export interface TypeInfo {
                    /**
                     * The name of a type declared for the associated element or attribute, or null if unknown.
                     * @returns {$$rhino.String}
                     */
                    getTypeName(): $$rhino.String;

                    /**
                     * The namespace of the type declared for the associated element or attribute or null if the element does not have declaration or if no namespace information is available.
                     * @returns {$$rhino.String}
                     */
                    getTypeNamespace(): $$rhino.String;

                    /**
                     * This method returns if there is a derivation between the reference type definition, i.e.
                     * @param typeNamespaceArg {$$rhino.String}
                     * @param typeNameArg {$$rhino.String}
                     * @param derivationMethod {$$rhino.Number}
                     * @returns {$$rhino.Boolean}
                     */
                    isDerivedFrom(typeNamespaceArg: $$rhino.String, typeNameArg: $$rhino.String, derivationMethod: $$rhino.Number): $$rhino.Boolean;
                }

                /**
                 * Provides a set of attributes and methods for accessing character data in the DOM.
                 * @export
                 * @interface CharacterData
                 * @extends {Node}
                 * @see {@link https://docs.oracle.com/javase/10/docs/api/org/w3c/dom/CharacterData.html}
                 */
                export interface CharacterData extends Node {
                    /**
                     * Append the string to the end of the character data of the node.
                     * @param arg {$$rhino.String}
                     */
                    appendData(arg: $$rhino.String): void;

                    /**
                     * Remove a range of 16-bit units from the node.
                     * @param offset {$$rhino.Number}
                     * @param count {$$rhino.Number}
                     */
                    deleteData(offset: $$rhino.Number, count: $$rhino.Number): void;

                    /**
                     * The character data of the node that implements this interface.
                     * @returns {$$rhino.String}
                     */
                    getData(): $$rhino.String;

                    /**
                     * The number of 16-bit units that are available through data and the substringData method below.
                     * @returns {$$rhino.Number}
                     */
                    getLength(): $$rhino.Number;

                    /**
                     * Insert a string at the specified 16-bit unit offset.
                     * @param offset {$$rhino.Number}
                     * @param arg {$$rhino.String}
                     */
                    insertData(offset: $$rhino.Number, arg: $$rhino.String): void;

                    /**
                     * Replace the characters starting at the specified 16-bit unit offset with the specified string.
                     * @param offset {$$rhino.Number}
                     * @param count {$$rhino.Number}
                     * @param arg {$$rhino.String}
                     */
                    replaceData(offset: $$rhino.Number, count: $$rhino.Number, arg: $$rhino.String): void;

                    /**
                     * The character data of the node that implements this interface.
                     * @param data {$$rhino.String}
                     */
                    setData(data: $$rhino.String): void;

                    /**
                     * Extracts a range of data from the node.
                     * @param offset {$$rhino.Number}
                     * @param count {$$rhino.Number}
                     * @returns {$$rhino.String}
                     */
                    substringData(offset: $$rhino.Number, count: $$rhino.Number): $$rhino.String;
                }

                /**
                 * Represents the textual content (termed character data in XML) of an {@link Element} or {@link Attr}.
                 * @export
                 * @interface Text
                 * @extends {CharacterData}
                 * @see {@link https://docs.oracle.com/javase/10/docs/api/org/w3c/dom/Text.html}
                 */
                export interface Text extends CharacterData {
                    /**
                     * Returns all text of Text nodes logically-adjacent text nodes to this node, concatenated in document order.
                     * @returns {$$rhino.String}
                     */
                    getWholeText(): $$rhino.String;

                    /**
                     * Returns whether this text node contains element content whitespace, often abusively called "ignorable whitespace".
                     * @returns {$$rhino.Boolean}
                     */
                    isElementContentWhitespace(): $$rhino.Boolean;

                    /**
                     * Replaces the text of the current node and all logically-adjacent text nodes with the specified text.
                     * @param content {$$rhino.String}
                     * @returns {Text}
                     */
                    replaceWholeText(content: $$rhino.String): Text;

                    /**
                     * Breaks this node into two nodes at the specified offset, keeping both in the tree as siblings.
                     * @param offset {$$rhino.Number}
                     * @returns {Text}
                     */
                    splitText(offset: $$rhino.Number): Text;
                }

                /**
                 * Escaped block of text containing characters that would otherwise be regarded as markup.
                 * @export
                 * @interface CDATASection
                 * @extends {Text}
                 * @see {@link https://docs.oracle.com/javase/10/docs/api/org/w3c/dom/CDATASection.html}
                 */
                export interface CDATASection extends Text { }

                /**
                 * Represents the content of a comment.
                 * @export
                 * @interface Comment
                 * @extends {CharacterData}
                 * @see {@link https://docs.oracle.com/javase/10/docs/api/org/w3c/dom/Comment.html}
                 */
                export interface Comment extends CharacterData { }

                /**
                 * A "lightweight" or "minimal" Document object.
                 * @export
                 * @interface DocumentFragment
                 * @extends {Node}
                 * @see {@link https://docs.oracle.com/javase/10/docs/api/org/w3c/dom/DocumentFragment.html}
                 */
                export interface DocumentFragment extends Node { }

                /**
                 * Represents a character entity reference.
                 * @export
                 * @interface EntityReference
                 * @extends {Node}
                 * @see {@link https://docs.oracle.com/javase/10/docs/api/org/w3c/dom/EntityReference.html}
                 */
                export interface EntityReference extends Node { }

                /**
                 * Represents a "processing instruction", used in XML as a way to keep processor-specific information in the text of the document.
                 * @export
                 * @interface ProcessingInstruction
                 * @extends {Node}
                 * @see {@link https://docs.oracle.com/javase/10/docs/api/org/w3c/dom/ProcessingInstruction.html}
                 */
                export interface ProcessingInstruction extends Node {
                    /**
                     * The content of this processing instruction.
                     * @returns {$$rhino.String}
                     */
                    getData(): $$rhino.String;

                    /**
                     * The target of this processing instruction.
                     * @returns {$$rhino.String}
                     */
                    getTarget(): $$rhino.String;

                    /**
                     * The content of this processing instruction.
                     * @param data {$$rhino.String}
                     */
                    setData(data: $$rhino.String): void;
                }

                /**
                 * Provides an interface to the list of entities that are defined for the document.
                 * @export
                 * @interface DocumentType
                 * @extends {Node}
                 * @see {@link https://docs.oracle.com/javase/10/docs/api/org/w3c/dom/DocumentType.html}
                 */
                export interface DocumentType extends Node {
                    /**
                     * A NamedNodeMap containing the general entities, both external and internal, declared in the DTD.
                     * @returns {NamedNodeMap}
                     */
                    getEntities(): NamedNodeMap;

                    /**
                     * The internal subset as a string, or null if there is none.
                     * @returns {$$rhino.String}
                     */
                    getInternalSubset(): $$rhino.String;

                    /**
                     * The name of DTD; i.e., the name immediately following the DOCTYPE keyword.
                     * @returns {$$rhino.String}
                     */
                    getName(): $$rhino.String;

                    /**
                     * A NamedNodeMap containing the notations declared in the DTD.
                     * @returns {NamedNodeMap}
                     */
                    getNotations(): NamedNodeMap;

                    /**
                     * The public identifier of the external subset.
                     * @returns {$$rhino.String}
                     */
                    getPublicId(): $$rhino.String;

                    /**
                     * The system identifier of the external subset.
                     * @returns {$$rhino.String}
                     */
                    getSystemId(): $$rhino.String;
                }

                /**
                 * Provides the abstraction of an ordered collection of DOMString values, without defining or constraining how this collection is implemented
                 * @export
                 * @interface DOMStringList
                 * @see {@link https://docs.oracle.com/javase/10/docs/api/org/w3c/dom/DOMStringList.html}
                 */
                export interface DOMStringList {
                    /**
                     * Test if a string is part of this DOMStringList.
                     * @param str {$$rhino.String}
                     * @returns {$$rhino.Boolean}
                     */
                    contains(str: $$rhino.String): $$rhino.Boolean;

                    /**
                     * The number of DOMStrings in the list.
                     * @returns {$$rhino.Number}
                     */
                    getLength(): $$rhino.Number;

                    /**
                     * Returns the indexth item in the collection.
                     * @param index {$$rhino.Number}
                     * @returns {$$rhino.String}
                     */
                    item(index: $$rhino.Number): $$rhino.String;
                }

                /**
                 * Represents the configuration of a document and maintains a table of recognized parameters.
                 * @export
                 * @interface DOMConfiguration
                 * @see {@link https://docs.oracle.com/javase/10/docs/api/org/w3c/dom/DOMConfiguration.html}
                 */
                export interface DOMConfiguration {
                    /**
                     * Check if setting a parameter to a specific value is supported.
                     * @param name {$$rhino.String}
                     * @param value {object}
                     * @returns {$$rhino.Boolean}
                     */
                    canSetParameter(name: $$rhino.String, value: object): $$rhino.Boolean;

                    /**
                     * Return the value of a parameter if known.
                     * @param name {$$rhino.String}
                     * @returns {*}
                     */
                    getParameter(name: $$rhino.String): any;

                    /**
                     * The list of the parameters supported by this DOMConfiguration object and for which at least one value can be set by the application.
                     * @returns {DOMStringList}
                     */
                    getParameterNames(): DOMStringList;

                    /**
                     * Set the value of a parameter.
                     * @param name {$$rhino.String}
                     * @param value {object}
                     */
                    setParameter(name: $$rhino.String, value: object): void;
                }

                /**
                 * Provides a number of methods for performing operations that are independent of any particular instance of the document object model.
                 * @export
                 * @interface DOMImplementation
                 * @see {@link https://docs.oracle.com/javase/10/docs/api/org/w3c/dom/DOMImplementation.html}
                 */
                export interface DOMImplementation {
                    /**
                     * Creates a DOM Document object of the specified type with its document element.
                     * @param namespaceURI {$$rhino.String}
                     * @param qualifiedName {$$rhino.String}
                     * @param doctype {DocumentType}
                     * @returns {Document}
                     */
                    createDocument(namespaceURI: $$rhino.String, qualifiedName: $$rhino.String, doctype: DocumentType): Document;

                    /**
                     * Creates an empty DocumentType node.
                     * @param qualifiedName {$$rhino.String}
                     * @param publicId {$$rhino.String}
                     * @param systemId {$$rhino.String}
                     * @returns {DocumentType}
                     */
                    createDocumentType(qualifiedName: $$rhino.String, publicId: $$rhino.String, systemId: $$rhino.String): DocumentType;

                    /**
                     * This method returns a specialized object which implements the specialized APIs of the specified feature and version, as specified in DOM Features.
                     * @param feature {$$rhino.String}
                     * @param version {$$rhino.String}
                     * @returns {*}
                     */
                    getFeature(feature: $$rhino.String, version: $$rhino.String): any;

                    /**
                     * Test if the DOM implementation implements a specific feature and version, as specified in DOM Features.
                     * @param feature {$$rhino.String}
                     * @param version {$$rhino.String}
                     * @returns {$$rhino.Boolean}
                     */
                    hasFeature(feature: $$rhino.String, version: $$rhino.String): $$rhino.Boolean;
                }

                /**
                 * Represents the entire HTML or XML document.
                 * @export
                 * @interface Document
                 * @extends {Node}
                 * @see {@link https://docs.oracle.com/javase/10/docs/api/org/w3c/dom/Document.html}
                 */
                export interface Document extends Node {
                    /**
                     * Attempts to adopt a node from another document to this document.
                     * @param source {Node}
                     * @returns {Node}
                     */
                    adoptNode(source: Node): Node;

                    /**
                     * Creates an Attr of the given name.
                     * @param name {$$rhino.String}
                     * @returns {Attr}
                     */
                    createAttribute(name: $$rhino.String): Attr;

                    /**
                     * Creates an attribute of the given qualified name and namespace URI.
                     * @param namespaceURI {$$rhino.String}
                     * @param qualifiedName {$$rhino.String}
                     * @returns {Attr}
                     */
                    createAttributeNS(namespaceURI: $$rhino.String, qualifiedName: $$rhino.String): Attr;

                    /**
                     * Creates a CDATASection node whose value is the specified string.
                     * @param data {$$rhino.String}
                     * @returns {CDATASection}
                     */
                    createCDATASection(data: $$rhino.String): CDATASection;

                    /**
                     * Creates a Comment node given the specified string.
                     * @param data {$$rhino.String}
                     * @returns {Comment}
                     */
                    createComment(data: $$rhino.String): Comment;

                    /**
                     * Creates an empty DocumentFragment object.
                     * @returns {DocumentFragment}
                     */
                    createDocumentFragment(): DocumentFragment;

                    /**
                     * Creates an element of the type specified.
                     * @param tagName {$$rhino.String}
                     * @returns {Element}
                     */
                    createElement(tagName: $$rhino.String): Element;

                    /**
                     * Creates an element of the given qualified name and namespace URI.
                     * @param namespaceURI {$$rhino.String}
                     * @param qualifiedName {$$rhino.String}
                     * @returns {Element}
                     */
                    createElementNS(namespaceURI: $$rhino.String, qualifiedName: $$rhino.String): Element;

                    /**
                     * Creates an EntityReference object.
                     * @param name {$$rhino.String}
                     * @returns {EntityReference}
                     */
                    createEntityReference(name: $$rhino.String): EntityReference;

                    /**
                     * Creates a ProcessingInstruction node given the specified name and data strings.
                     * @param target {$$rhino.String}
                     * @param data {$$rhino.String}
                     * @returns {ProcessingInstruction}
                     */
                    createProcessingInstruction(target: $$rhino.String, data: $$rhino.String): ProcessingInstruction;

                    /**
                     * Creates a Text node given the specified string.
                     * @param data {$$rhino.String}
                     * @returns {Text}
                     */
                    createTextNode(data: $$rhino.String): Text;

                    /**
                     * The Document Type Declaration (see DocumentType) associated with this document.
                     * @returns {DocumentType}
                     */
                    getDoctype(): DocumentType;

                    /**
                     * This is a convenience attribute that allows direct access to the child node that is the document element of the document.
                     * @returns {Element}
                     */
                    getDocumentElement(): Element;

                    /**
                     * The location of the document or null if undefined or if the Document was created using DOMImplementation.createDocument.
                     * @returns {$$rhino.String}
                     */
                    getDocumentURI(): $$rhino.String;

                    /**
                     * The configuration used when Document.normalizeDocument() is invoked.
                     * @returns {DOMConfiguration}
                     */
                    getDomConfig(): DOMConfiguration;

                    /**
                     * Returns the Element that has an ID attribute with the given value.
                     * @param elementId {$$rhino.String}
                     * @returns {Element}
                     */
                    getElementById(elementId: $$rhino.String): Element;

                    /**
                     * Returns a NodeList of all the Elements in document order with a given tag name and are contained in the document.
                     * @param tagname {$$rhino.String}
                     * @returns {NodeList}
                     */
                    getElementsByTagName(tagname: $$rhino.String): NodeList;

                    /**
                     * Returns a NodeList of all the Elements with a given local name and namespace URI in document order.
                     * @param namespaceURI {$$rhino.String}
                     * @param localName {$$rhino.String}
                     * @returns {NodeList}
                     */
                    getElementsByTagNameNS(namespaceURI: $$rhino.String, localName: $$rhino.String): NodeList;

                    /**
                     * The DOMImplementation object that handles this document.
                     * @returns {DOMImplementation}
                     */
                    getImplementation(): DOMImplementation;

                    /**
                     * An attribute specifying the encoding used for this document at the time of the parsing.
                     * @returns {$$rhino.String}
                     */
                    getInputEncoding(): $$rhino.String;

                    /**
                     * An attribute specifying whether error checking is enforced or not.
                     * @returns {$$rhino.Boolean}
                     */
                    getStrictErrorChecking(): $$rhino.Boolean;

                    /**
                     * An attribute specifying, as part of the XML declaration, the encoding of this document.
                     * @returns {$$rhino.String}
                     */
                    getXmlEncoding(): $$rhino.String;

                    /**
                     * An attribute specifying, as part of the XML declaration, whether this document is standalone.
                     * @returns {$$rhino.Boolean}
                     */
                    getXmlStandalone(): $$rhino.Boolean;

                    /**
                     * An attribute specifying, as part of the XML declaration, the version number of this document.
                     * @returns {$$rhino.String}
                     */
                    getXmlVersion(): $$rhino.String;

                    /**
                     * Imports a node from another document to this document, without altering or removing the source node from the original document; this method creates a new copy of the source node.
                     * @param importedNode {Node}
                     * @param deep {$$rhino.Boolean}
                     * @returns {Node}
                     */
                    importNode(importedNode: Node, deep: $$rhino.Boolean): Node;

                    /**
                     * This method acts as if the document was going through a save and load cycle, putting the document in a "normal" form.
                     */
                    normalizeDocument(): void;

                    /**
                     * Rename an existing node of type ELEMENT_NODE or ATTRIBUTE_NODE.
                     * @param n {Node}
                     * @param namespaceURI {$$rhino.String}
                     * @param qualifiedName {$$rhino.String}
                     * @returns {Node}
                     */
                    renameNode(n: Node, namespaceURI: $$rhino.String, qualifiedName: $$rhino.String): Node;

                    /**
                     * The location of the document or null if undefined or if the Document was created using DOMImplementation.createDocument.
                     * @param documentURI {$$rhino.String}
                     */
                    setDocumentURI(documentURI: $$rhino.String): void;

                    /**
                     * An attribute specifying whether error checking is enforced or not.
                     * @param strictErrorChecking {$$rhino.Boolean}
                     */
                    setStrictErrorChecking(strictErrorChecking: $$rhino.Boolean): void;

                    /**
                     * An attribute specifying, as part of the XML declaration, whether this document is standalone.
                     * @param xmlStandalone {$$rhino.Boolean}
                     */
                    setXmlStandalone(xmlStandalone: $$rhino.Boolean): void;

                    /**
                     * An attribute specifying, as part of the XML declaration, the version number of this document.
                     * @param xmlVersion {$$rhino.String}
                     */
                    setXmlVersion(xmlVersion: $$rhino.String): void;
                }

                /**
                 * Represents collections of nodes that can be accessed by name.
                 * @export
                 * @interface NamedNodeMap
                 * @see {@link https://docs.oracle.com/javase/10/docs/api/org/w3c/dom/NamedNodeMap.html}
                 */
                export interface NamedNodeMap {
                    /**
                     * The number of nodes in this map.
                     * @returns {$$rhino.Number}
                     */
                    getLength(): $$rhino.Number;

                    /**
                     * Retrieves a node specified by name.
                     * @param name {$$rhino.String}
                     * @returns {Node}
                     */
                    getNamedItem(name: $$rhino.String): Node;

                    /**
                     * Retrieves a node specified by local name and namespace URI.
                     * @param namespaceURI {$$rhino.String}
                     * @param localName {$$rhino.String}
                     * @returns {Node}
                     */
                    getNamedItemNS(namespaceURI: $$rhino.String, localName: $$rhino.String): Node;

                    /**
                     * Returns the indexth item in the map.
                     * @param index {$$rhino.Number}
                     * @returns {Node}
                     */
                    item(index: $$rhino.Number): Node;

                    /**
                     * Removes a node specified by name.
                     * @param name {$$rhino.String}
                     * @returns {Node}
                     */
                    removeNamedItem(name: $$rhino.String): Node;

                    /**
                     * Removes a node specified by local name and namespace URI.
                     * @param namespaceURI {$$rhino.String}
                     * @param localName {$$rhino.String}
                     * @returns {Node}
                     */
                    removeNamedItemNS(namespaceURI: $$rhino.String, localName: $$rhino.String): Node;

                    /**
                     * Adds a node using its nodeName attribute.
                     * @param arg {Node}
                     * @returns {Node}
                     */
                    setNamedItem(arg: Node): Node;

                    /**
                     * Adds a node using its namespaceURI and localName.
                     * @param arg {Node}
                     * @returns {Node}
                     */
                    setNamedItemNS(arg: Node): Node;
                }

                /**
                 * Represents an element in an HTML or XML document.
                 * @export
                 * @interface Element
                 * @extends {Node}
                 * @see {@link https://docs.oracle.com/javase/10/docs/api/org/w3c/dom/Element.html}
                 */
                export interface Element extends Node {
                    /**
                     * Retrieves an attribute value by name.
                     * @param name {$$rhino.String}
                     * @returns {$$rhino.String}
                     */
                    getAttribute(name: $$rhino.String): $$rhino.String;

                    /**
                     * Retrieves an attribute node by name.
                     * @param name {$$rhino.String}
                     * @returns {Attr}
                     */
                    getAttributeNode(name: $$rhino.String): Attr;

                    /**
                     * Retrieves an Attr node by local name and namespace URI.
                     * @param namespaceURI {$$rhino.String}
                     * @param localName {$$rhino.String}
                     * @returns {Attr}
                     */
                    getAttributeNodeNS(namespaceURI: $$rhino.String, localName: $$rhino.String): Attr;

                    /**
                     * Retrieves an attribute value by local name and namespace URI.
                     * @param namespaceURI {$$rhino.String}
                     * @param localName {$$rhino.String}
                     * @returns {$$rhino.String}
                     */
                    getAttributeNS(namespaceURI: $$rhino.String, localName: $$rhino.String): $$rhino.String;

                    /**
                     * Returns a NodeList of all descendant Elements with a given tag name, in document order.
                     * @param name {$$rhino.String}
                     * @returns {NodeList}
                     */
                    getElementsByTagName(name: $$rhino.String): NodeList;

                    /**
                     * Returns a NodeList of all the descendant Elements with a given local name and namespace URI in document order.
                     * @param namespaceURI {$$rhino.String}
                     * @param localName {$$rhino.String}
                     * @returns {NodeList}
                     */
                    getElementsByTagNameNS(namespaceURI: $$rhino.String, localName: $$rhino.String): NodeList;

                    /**
                     * The type information associated with this element.
                     * @returns {TypeInfo}
                     */
                    getSchemaTypeInfo(): TypeInfo;

                    /**
                     * The name of the element.
                     * @returns {$$rhino.String}
                     */
                    getTagName(): $$rhino.String;

                    /**
                     * Returns true when an attribute with a given name is specified on this element or has a default value, false otherwise.
                     * @param name {$$rhino.String}
                     * @returns {$$rhino.Boolean}
                     */
                    hasAttribute(name: $$rhino.String): $$rhino.Boolean;

                    /**
                     * Returns true when an attribute with a given local name and namespace URI is specified on this element or has a default value, false otherwise.
                     * @param namespaceURI {$$rhino.String}
                     * @param localName {$$rhino.String}
                     * @returns {$$rhino.Boolean}
                     */
                    hasAttributeNS(namespaceURI: $$rhino.String, localName: $$rhino.String): $$rhino.Boolean;

                    /**
                     * Removes an attribute by name.
                     * @param name {$$rhino.String}
                     */
                    removeAttribute(name: $$rhino.String): void;

                    /**
                     * Removes the specified attribute node.
                     * @param oldAttr {Attr}
                     * @returns {Attr}
                     */
                    removeAttributeNode(oldAttr: Attr): Attr;

                    /**
                     * Removes an attribute by local name and namespace URI.
                     * @param namespaceURI {$$rhino.String}
                     * @param localName {$$rhino.String}
                     */
                    removeAttributeNS(namespaceURI: $$rhino.String, localName: $$rhino.String): void;

                    /**
                     * Adds a new attribute.
                     * @param name {$$rhino.String}
                     * @param value {$$rhino.String}
                     */
                    setAttribute(name: $$rhino.String, value: $$rhino.String): void;

                    /**
                     * Adds a new attribute node.
                     * @param newAttr {Attr}
                     * @returns {Attr}
                     */
                    setAttributeNode(newAttr: Attr): Attr;

                    /**
                     * Adds a new attribute.
                     * @param newAttr {Attr}
                     * @returns {Attr}
                     */
                    setAttributeNodeNS(newAttr: Attr): Attr;

                    /**
                     * Adds a new attribute.
                     * @param namespaceURI {$$rhino.String}
                     * @param qualifiedName {$$rhino.String}
                     * @param value {$$rhino.String}
                     */
                    setAttributeNS(namespaceURI: $$rhino.String, qualifiedName: $$rhino.String, value: $$rhino.String): void;

                    /**
                     * If the parameter isId is true, this method declares the specified attribute to be a user-determined ID attribute.
                     * @param name {$$rhino.String}
                     * @param isId {$$rhino.Boolean}
                     */
                    setIdAttribute(name: $$rhino.String, isId: $$rhino.Boolean): void;

                    /**
                     * If the parameter isId is true, this method declares the specified attribute to be a user-determined ID attribute.
                     * @param idAttr {Attr}
                     * @param isId {$$rhino.Boolean}
                     */
                    setIdAttributeNode(idAttr: Attr, isId: $$rhino.Boolean): void;

                    /**
                     * If the parameter isId is true, this method declares the specified attribute to be a user-determined ID attribute.
                     * @param namespaceURI {$$rhino.String}
                     * @param localName {$$rhino.String}
                     * @param isId {$$rhino.Boolean}
                     */
                    setIdAttributeNS(namespaceURI: $$rhino.String, localName: $$rhino.String, isId: $$rhino.Boolean): void;
                }
            }
        }

        export namespace mozilla {
            export namespace javascript {
                /**
                 * This is interface that all objects in JavaScript must implement. The interface provides for the management of properties and for performing conversions.
                 * @export
                 * @interface Scriptable
                 * @see {@link https://www-archive.mozilla.org/rhino/apidocs/org/mozilla/javascript/scriptable}
                 * @description Library: rhino-ng-0.0.80.jar;
                 * Java declaration: ' '.
                 */
                export interface Scriptable { }

                /**
                 * This is the default implementation of the Scriptable interface. This class provides convenient default behavior that makes it easier to define host objects.
                 * @export
                 * @interface Scriptable
                 * @see {@link https://www-archive.mozilla.org/rhino/apidocs/org/mozilla/javascript/scriptableobject}
                 * @description Library: rhino-ng-0.0.80.jar;
                 */
                export interface ScriptableObject extends java.lang.Object, Scriptable { }
            }
        }
    }
}

declare interface IJavaArray<E> {
    /**
     * Gets the length of the array.
     */
    readonly length: Packages.java.lang.Integer;
    [n: number]: E;
}