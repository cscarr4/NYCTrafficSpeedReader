using System;
using System.Collections.Generic;
using System.Text;
using FileHelpers;

namespace NYCTrafficSpeedReader
{
    [DelimitedRecord("\t")]
    [IgnoreFirst(1)]
    class TrafficDataRecord
    {
        [FieldQuoted]
        public string Id { get; set; }
        [FieldQuoted]
        public double Speed { get; set; }
        [FieldQuoted]
        public double TravelTime { get; set; }
        [FieldQuoted]
        public string Status { get; set; }
        [FieldQuoted]
        [FieldConverter(ConverterKind.Date, "M/d/yyyy HH:mm:ss")]
        public DateTime DataAsOf { get; set; }
        [FieldQuoted]
        public string LinkId { get; set; }
        [FieldQuoted]
        public string LinkPoints { get; set; }
        [FieldQuoted]
        public string EncodedPolyLine { get; set; }
        [FieldQuoted]
        public string EncodedPolyLineLvls { get; set; }
        [FieldQuoted]
        public string Owner { get; set; }
        [FieldQuoted]
        public string TranscomID { get; set; }
        [FieldQuoted]
        public string Borough { get; set; }
        [FieldQuoted]
        public string LinkName { get; set; }
    }
}
