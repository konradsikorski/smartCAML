using KoS.Apps.SharePoint.SmartCAML.Editor.Builder;
using KoS.Apps.SharePoint.SmartCAML.Editor.Builder.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace KoS.Apps.SharePoint.SmartCAML.UnitTests
{
    [TestClass]
    public class XMLParserTest
    {
        string xml =
            @"<Query>
  <Where>
    <And>
      <Eq>
        <FieldRef Name=""BooleanInternal"" />
        <Value Type=""Boolean"">0</Value>
      </Eq>
      <Or>
        <Gt>
          <FieldRef Name=""DateInternal"" />
          <Value Type=""DateTime"" IncludeTimeValue=""False"">2018-01-04T01:00:00Z</Value>
        </Gt>
        <And>
          <Eq>
            <FieldRef Name=""LookupInternal"" LookupId=""TRUE"" />
            <Value Type=""Lookup"">qwe</Value>
          </Eq>
          <In>
            <FieldRef Name=""UserInternal"" />
            <Values>
              <Value Type=""User"">1</Value>
              <Value Type=""User"">2</Value>
            </Values>
          </In>
        </And>
      </Or>
    </And>
  </Where>
</Query>";

        [TestMethod]
        public void Parse()
        {
            var filters = CamlParser.Parse(xml);

            Assert.IsNotNull(filters);
            Assert.IsTrue(filters.Count == 4);
        }

        [TestMethod]
        public void Parse_null_string()
        {
            var filters = CamlParser.Parse((string)null);

            Assert.IsNotNull(filters);
            Assert.IsTrue(filters.Count == 0);
        }

        [TestMethod]
        public void Parse_null_node()
        {
            var filters = CamlParser.Parse((XElement)null);

            Assert.IsNotNull(filters);
            Assert.IsTrue(filters.Count == 0);
        }

        [TestMethod]
        public void Parse_empty()
        {
            var filters = CamlParser.Parse(string.Empty);

            Assert.IsNotNull(filters);
            Assert.IsTrue(filters.Count == 0);
        }

        [TestMethod]
        [ExpectedException(typeof(System.Xml.XmlException))]
        public void Parse_wrong_xml()
        {
            var filters = CamlParser.Parse("<x");
        }

        [TestMethod]
        public void Parse_Eq()
        {
            var xml = @"
                <Where a1=""a1v"" a2=""a2v"">
                <Eq b1=""b1v"" b2=""b2v"">
                    <FieldRef Name=""BooleanInternal"" c1=""c1v"" c2=""c2v""/>
                    <Value Type=""Boolean"" d1=""d1v"" d2=""d2v"">QwE</Value>
                </Eq>
                </Where>";

            var filters = CamlParser.Parse(xml);

            Assert.IsNotNull(filters);
            Assert.IsTrue(filters.Count == 1);
            var filter = (Filter)filters[0];

            Assert.AreEqual(filter.FieldInternalName, "BooleanInternal");
            Assert.AreEqual(filter.QueryFilter, Editor.Enums.FilterOperator.Eq);
            Assert.AreEqual(filter.QueryOperator, Editor.Enums.QueryOperator.And);
            Assert.AreEqual(filter.FieldType, "Boolean");
            Assert.AreEqual(filter.FieldValue, "QwE");
            Assert.AreEqual(filter.ValueAttributes["d1"], "d1v");
            Assert.AreEqual(filter.ValueAttributes["d2"], "d2v");
            Assert.AreEqual(filter.FieldRefAttributes["c1"], "c1v");
            Assert.AreEqual(filter.FieldRefAttributes["c2"], "c2v");
            Assert.AreEqual(filter.FilterAttributes["b1"], "b1v");
            Assert.AreEqual(filter.FilterAttributes["b2"], "b2v");
        }
    }
}
