using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetyMp.FileHandling
{
    /// <summary>
    /// ExpenseMapping class is a container class to be used
    /// as a staging object between the CSV and the basic model
    /// structure used by the application.
    /// 
    /// It mirrors the structure perfectly of the CSV flat file
    /// and the results of this will be used to populate the model.
    /// 
    /// Properties are not mappable with converters using FileHelper
    /// so this class exposes public fields rather than properties.
    /// 
    /// The choice between some properties mapping to Nullable<T> and
    /// others using FieldNullValue attributes is deliberate and 
    /// meaningful in the context that some fields should NEVER be
    /// null (i.e. amount claimed, even if 0) whereas others may legitimately
    /// map to null
    /// </summary>
    [DelimitedRecord(",")]
    [IgnoreEmptyLines(true)]
    [IgnoreFirst(1)]
    class ExpenseMapping
    {
        [FieldQuoted(QuoteMode.OptionalForRead)]
        public string Year;

        [FieldConverter(ConverterKind.Date, "dd/MM/yyyy")]
        public Nullable<DateTime> Date;

        [FieldQuoted(QuoteMode.OptionalForRead)]
        public string ClaimNumber;

        [FieldQuoted(QuoteMode.OptionalForRead)]
        public string MemberName;

        [FieldQuoted(QuoteMode.OptionalForRead)]
        public string MemberConstituency;

        [FieldQuoted(QuoteMode.OptionalForRead)]
        public string Category;

        [FieldQuoted(QuoteMode.OptionalForRead)]
        public string Type;

        [FieldQuoted(QuoteMode.OptionalForRead)]
        [FieldTrim(TrimMode.Both)]
        public string Description;

        [FieldQuoted(QuoteMode.OptionalForRead)]
        [FieldTrim(TrimMode.Both)]
        public string Details;

        [FieldQuoted(QuoteMode.OptionalForRead)]
        [FieldTrim(TrimMode.Both)]
        public string JourneyType;

        [FieldQuoted(QuoteMode.OptionalForRead)]
        [FieldTrim(TrimMode.Both)]
        public string From;

        [FieldQuoted(QuoteMode.OptionalForRead)]
        [FieldTrim(TrimMode.Both)]
        public string To;

        [FieldQuoted(QuoteMode.OptionalForRead)]
        [FieldTrim(TrimMode.Both)]
        public string Travel;

        [FieldValueDiscarded] //WAAAY too much issues with this field
        public Nullable<int> Nights;

        [FieldValueDiscarded]
        public Nullable<double> Mileage;

        [FieldQuoted(QuoteMode.OptionalForRead)]
        [FieldNullValue((double)0)]
        [FieldConverter(typeof(MySimpleDoubleConverter))]
        public double AmountClaimed;

        [FieldQuoted(QuoteMode.OptionalForRead)]
        [FieldNullValue((double)0)]
        [FieldConverter(typeof(MySimpleDoubleConverter))]
        public double AmountPaid;

        [FieldQuoted(QuoteMode.OptionalForRead)]
        [FieldNullValue((double)0)]
        [FieldConverter(typeof(MySimpleDoubleConverter))]
        public double AmountNotPaid;

        [FieldQuoted(QuoteMode.OptionalForRead)]
        [FieldNullValue((double)0)]
        [FieldConverter(typeof(MySimpleDoubleConverter))]
        public Double AmountRepaid;

        [FieldQuoted(QuoteMode.OptionalForRead)]
        [FieldConverter(typeof(StatusConverter))]
        public Nullable<ExpenseStatus> Status;

        [FieldQuoted(QuoteMode.OptionalForRead)]
        public string ReasonNotPaid;
    }
}
