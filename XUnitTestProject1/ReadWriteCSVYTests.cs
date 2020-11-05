using System;
using System.IO;
using Xunit;

namespace Inventory.UnitTests
{
    public class ReadWriteCSVTests
    {
        ReadWriteCSV sut = new ReadWriteCSV("test.txt");

        // CONSTRUCTOR
        
        [Theory]
        [InlineData("test.txt")] //correct
        [InlineData("/test.csv")] //correct
        public void Constructor_ValidFilepathAndExtension_ShouldReturnObject(string filepath)
        {
            Assert.NotNull(new ReadWriteCSV(filepath));
        }

        [Theory]
        [InlineData("/test")] //missing extension
        [InlineData("/test.bla")] //invalid extension
        [InlineData("txt")] //extension only
        [InlineData(".txt")] //missing filepath
        [InlineData("/.txt")] //missing filepath
        [InlineData("")] //empty parameter
        [InlineData(null)] //null reference
        public void Constructor_InvalidFilepathOrExtension_ShouldThrowArgumentException(string filepath)
        {
            Assert.Throws<ArgumentException>(() => new ReadWriteCSV(filepath));
        }

        [Theory]
        [InlineData(';')] //correct
        [InlineData(',')] //correct
        public void Constructor_ValidSeperator_ShouldPass(char seperator)
        {
            Assert.NotNull(new ReadWriteCSV("test.txt", seperator));
        }

        [Theory]
        [InlineData('a')] //invalid seperator
        [InlineData('|')] //invalid seperator
        [InlineData('.')] //invalid seperator
        [InlineData(' ')] //whitespace
        [InlineData('\t')] //tab delemited
        [InlineData(null)] //null reference
        public void Constructor_InvalidSeperator_ShouldThrowArgumenntException(char seperator)
        {
            Assert.Throws<ArgumentException>(() => new ReadWriteCSV("test.txt", seperator));
        }

        // ADDRECORD

        [Theory]
        [InlineData("text")] //string
        [InlineData("se;pe;ra;ted")] //semicolon-delimited
        [InlineData("se,pe,ra,ted")] //comma-delimited
        [InlineData("!@#$%^&*")] //symbols
        [InlineData("\"\"")] //special characters
        [InlineData("\'\'")] //special characters
        [InlineData("")] //empty
        [InlineData(" ")] //whitespace
        public void AddRecord_ValidInput_ShouldReturnCreated(string input)
        {
            var response = sut.AddRecord(input);
            Assert.Equal(System.Net.HttpStatusCode.Created, response.Status);
            Assert.True(response.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData("\t")] //dangerous characters
        [InlineData("\n")] //dangerous characters
        [InlineData(null)] //null
        public void AddRecord_InvalidInput_ShouldReturnBadRequest(string input)
        {
            var response = sut.AddRecord(input);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.Status);
            Assert.False(response.IsSuccessStatusCode);
        }

        // AddRecord

        [Theory]
        [InlineData("a")] //arraysize = 1
        [InlineData("a", "b", "c")] //arraysize = 3
        [InlineData("0", "1", "2")] //numbers
        [InlineData("", "text", "")] //empty fields
        [InlineData(" ", " ", " ")] //whitespaces
        public void AddRecord_ValidInputStrArray_ShouldReturnCreated(params string[] input)
        {
            var response = sut.AddRecord(input);
            Assert.Equal(System.Net.HttpStatusCode.Created, response.Status);
            Assert.True(response.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData("")] //empty
        [InlineData("", "", "")] //array full of nothing
        public void AddRecord_EmptyInputStrArray_ShouldReturnCreated(params string[] input)
        {
            var response = sut.AddRecord(input);
            Assert.Equal(System.Net.HttpStatusCode.Created, response.Status);
            Assert.True(response.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData("\t")] //tab
        [InlineData("", "\t", "")] //tab in middle of array
        [InlineData("\n")] //CR
        [InlineData("", "\n", "")] //CR in middle of array
        public void AddRecord_DangerousCharactersStrArray_ShouldReturnBadRequest(params string[] input)
        {
            var response = sut.AddRecord(input);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.Status);
            Assert.False(response.IsSuccessStatusCode);
        }

        [Fact]
        public void AddRecord_InvalidInputNullStrArray_ShouldReturnBadRequest()
        {
            string[] input = null;
            var response = sut.AddRecord(input);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.Status);
            Assert.False(response.IsSuccessStatusCode);
        }

        [Fact]
        public void AddRecord_InvalidInputEmptyStrArray_ShouldReturnBadRequest()
        {
            string[] input = new string[] { };
            var response = sut.AddRecord(input);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.Status);
            Assert.False(response.IsSuccessStatusCode);
        }
        
        [Fact]
        public void AddRecord_InvalidInputEmptyStrArraySizeTwo_ShouldReturnBadRequest()
        {
            string[] input = new string[2];
            var response = sut.AddRecord(input);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.Status);
            Assert.False(response.IsSuccessStatusCode);
        }

        // RECORDEXISTS

        [Theory]
        [InlineData("1",1)]
        [InlineData("c", 3)]
        [InlineData(" ", 2)]
        [InlineData("   ", 2)]
        [InlineData("\"\"", 2)]
        public void RecordExists_Valid_ShouldReturnTrue(string key, int column)
        {
            File.Delete(@"test.txt");
            var sut = new ReadWriteCSV("test.txt", ';');
            sut.AddRecord("1;2");
            sut.AddRecord("a;b;c");
            sut.AddRecord("a; ;c");
            sut.AddRecord("e;   ;f");
            sut.AddRecord("a;\"\";c;d");
            var response = sut.RecordExists(key, column);
            Assert.True(response.Result);
            Assert.True(response.IsSuccessStatusCode);
        }

        //////add support for case(in)sensitivty

        [Theory]
        [InlineData("1", 3)]
        [InlineData("c", 1)]
        [InlineData("over9000!", 9999)]
        public void RecordExists_Invalid_ShouldReturnFalse(string key, int column)
        {
            File.Delete(@"test.txt");
            var sut = new ReadWriteCSV("test.txt", ';');
            sut.AddRecord("1;2;3");
            sut.AddRecord("a;b;c");
            var response = sut.RecordExists(key, column);
            Assert.False(response.Result);
            Assert.True(response.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void RecordExists_InvalidKey_ShouldReturnBadRequest(string key)
        {
            var response = sut.RecordExists(key, 1);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.Status);
            Assert.False(response.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-2)]
        public void RecordExists_ZeroOrNegativeColumn_ShouldThrowArgumentOutOfRangeException(int column) //@Joep BadRequest of ArgumentOutOfRange exception?
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.RecordExists("key", column));
        }

        [Fact]
        public void RemoveUnnecessarySeperators_ValidInput_ShouldReturnCleanString()
        {
            Assert.Equal("a", sut.RemoveUnnecessarySeperators("a"));
            Assert.Equal("a", sut.RemoveUnnecessarySeperators("a;"));
            Assert.Equal("abc", sut.RemoveUnnecessarySeperators("abc"));
            Assert.Equal("abc", sut.RemoveUnnecessarySeperators("abc;;;"));
            Assert.Equal("a;b;c", sut.RemoveUnnecessarySeperators("a;b;c"));
            Assert.Equal("a;b;c", sut.RemoveUnnecessarySeperators("a;b;c;"));
            Assert.Equal("a;;b;;c", sut.RemoveUnnecessarySeperators("a;;b;;c"));
            Assert.Equal("a;;b;;c", sut.RemoveUnnecessarySeperators("a;;b;;c;;;;;;"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(";")]
        [InlineData(";;;")]
        [InlineData(null)]
        public void RemoveUnnecessarySeperators_MeaninglessInput_ShouldReturnEmptyString (string input)
        {
            Assert.Equal(sut.RemoveUnnecessarySeperators(input), "");
        }
    }
}