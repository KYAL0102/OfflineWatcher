using Core.Extensions;

namespace UnitTests
{
    public class DataStreamExtenstionsTests
    {

        public record ParseTestRecord(string Input, char[] Separators, List<decimal> ExpectedValues);
        public static IEnumerable<object[]> ParseTestData =>
        [
            [new ParseTestRecord("1;1",[';'], [1, 1])],
            [new ParseTestRecord("5 10", [' '], [5, 10])],
            [new ParseTestRecord("5 10;3", [' ', ';'], [5, 10, 3])],
            [new ParseTestRecord("5 10.3;7", [' ', ';'], [5, 10.3m, 7])],
            [new ParseTestRecord("5.4 10,3;7", [' ', ',', ';'], [5.4m, 10, 3, 7])],

        ];

        [Theory]
        [MemberData(nameof(ParseTestData))]
        public void T01_ParseStringOfPoints_CorrectPoints(ParseTestRecord data)
        {
            var result = data.Input.ParseToDecimalList(data.Separators);
            Assert.NotNull(result);
            Assert.Equal(data.ExpectedValues.Count, result.Count);
            Assert.Equal(data.ExpectedValues, result);
        }

        public record SerializeTestRecord(List<decimal> Values, string ExpectedOutput);
        public static IEnumerable<object[]> SerializeTestData =>
        [
            [new SerializeTestRecord([1.23m, 4.56m, 7.89m], "1.23;4.56;7.89")],
            [new SerializeTestRecord([], "")],
            [new SerializeTestRecord([0.1m], "0.1")],
            [new SerializeTestRecord([1000.01m, -500.5m], "1000.01;-500.5")]
        ];

        [Theory]
        [MemberData(nameof(SerializeTestData))]
        public void T02_SerializeValuesToString_Ok(SerializeTestRecord data)
        {
            var result = data.Values.ToValuesString();
            Assert.NotNull(result);
            Assert.Equal(data.ExpectedOutput, result);
        }

    }
}