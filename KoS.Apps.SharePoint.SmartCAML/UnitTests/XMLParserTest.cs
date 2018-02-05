using KoS.Apps.SharePoint.SmartCAML.Editor.Builder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            var filters = new CamlParser().Parse(xml);
        }
    }
}
