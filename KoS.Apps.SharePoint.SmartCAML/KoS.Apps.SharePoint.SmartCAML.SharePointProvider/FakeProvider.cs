using KoS.Apps.SharePoint.SmartCAML.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KoS.Apps.SharePoint.SmartCAML.SharePointProvider
{
    public class FakeProvider : ISharePointProvider
    {
        private class Internals
        {
            public const string Text = "TextInternal";
            public const string User = "UserInternal";
            public const string Date = "DateInternal";
            public const string DateTime = "DateTimeInternal";
            public const string Lookup = "LookupInternal";
            public const string MultiLookup = "MultiLookupInternal";
            public const string Url = "UrlInternal";
            public const string Integer = "IntegerInternal";
            public const string Note = "NoteInternal";
            public const string Choice = "ChoiceInternal";
            public const string Boolean = "BooleanInternal";
            public const string Readonly = "ReadonlyInternal";
            public const string Hidden = "HiddenInternal";
            public const string Number = "NumberInternal";
            public const string Currency = "CurrencyInternal";
            public const string Computed = "ComputedInternal";
            public const string MultiChoice = "MultiChoiceInternal";
            public const string Guid = "GuidInternal";
            public const string Calculated = "CalculatedInternal";
            public const string File = "FileInternal";
            public const string Attachments = "AttachmentsInternal";
            public const string ContentTypeId = "ContentTypeIdInternal";
            public const string Geolocation = "GeolocationInternal";
            public const string SortTest = "ASortTest";
        }

        public Web Web { get; set; }

        public async Task<Web> Connect(string url)
        {
            if (!url.StartsWith("http")) return null;

            return await Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(2000);

                Web = new Web(this)
                {
                    Id = Guid.Empty,
                    Title = "Test",
                    Url = url,
                };

                Web.Lists = new List<SList>
                {
                    new SList {Id = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1), Title = "List1", Web = Web},
                    new SList {Id = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2), Title = "List2", Web = Web},
                    new SList {Id = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3), Title = "List3", Web = Web},
                    new SList {Id = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4), Title = "List4", Web = Web},
                    new SList {Id = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5), Title = "List5", Web = Web},
                    new SList {Id = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6), Title = "List6", Web = Web},
                    new SList {Id = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7), Title = "List7", Web = Web},
                };

                return Web;
            });
        }

        public async Task<Web> Connect(string url, string userName, string password)
        {
            if (String.IsNullOrEmpty(userName) != String.IsNullOrEmpty(password))
                throw new ArgumentException("The user or password is null.");

            return await Connect(url);
        }

        public async Task<List<ListItem>> ExecuteQuery(ListQuery query, int? pageSize)
        {
            return await Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(2000);

                return new List<ListItem>
                {
                    new ListItem(query.List)
                    {
                        Id = 1,
                        Columns = new Dictionary<string, string>
                        {
                            {Internals.Text, "Test1"},
                            {Internals.User, "Owner1"},
                            {Internals.Date, "Date1"},
                            {Internals.DateTime, "DateTime1"},
                            {Internals.Lookup, "Lookup1"},
                            {Internals.MultiLookup, "MultiLookup1"},
                            {Internals.Integer, "Integer1"},
                            {Internals.Note, "Note1" + Environment.NewLine + "Seccond Line"},
                            {Internals.Url, "Url1"},
                            {Internals.Choice, "Choice1"},
                            {Internals.Boolean, "Boolean1"},
                            {Internals.Readonly, "Readonly1"},
                            {Internals.Hidden, "Hidden1"},
                            {Internals.Number, "Number1"},
                            {Internals.Currency, "Currency1"},
                            {Internals.Computed, "Computed1"},
                            {Internals.MultiChoice, "MultiChoice1"},
                            {Internals.Guid, "Guid1"},
                            {Internals.Calculated, "Calculated1"},
                            {Internals.File, "File1"},
                            {Internals.Attachments, "Attachments1"},
                            {Internals.ContentTypeId, "ContentTypeId1"},
                            {Internals.Geolocation, "Geolocation1"},
                            {Internals.SortTest, "SortTest1"},
                        }
                    },
                    new ListItem(query.List)
                    {
                        Id = 2,
                        Columns = new Dictionary<string, string>
                        {
                            {Internals.Text, "Test2"},
                            {Internals.User, "Owner2"},
                            {Internals.Date, ""},
                            {Internals.Url, null},
                        }
                    },
                    new ListItem(query.List)
                    {
                        Id = 3,
                        Columns = new Dictionary<string, string>
                        {
                            {Internals.Text, "Test3"},
                            {Internals.User, "Owner3"}
                        }
                    },
                };
            });
        }

        public async Task FillListFields(SList list)
        {
            await Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(2000);

                var fields = new List<Field>
                {
                    new Field { Group = "1", Title = "Text",            Type=FieldType.Text,            InternalName = Internals.Text       },
                    new Field { Group = "1", Title = "User",            Type=FieldType.User,            InternalName = Internals.User       },
                    new FieldDateTime { Group = "1", Title = "Date",    Type=FieldType.DateTime,        InternalName = Internals.Date       , DateOnly = true},
                    new FieldDateTime { Group = "1", Title = "DateTime",Type=FieldType.DateTime,        InternalName = Internals.DateTime   , DateOnly = false},
                    new FieldLookup { Group = "2", Title = "Lookup",    Type=FieldType.Lookup,          InternalName = Internals.Lookup     },
                    new FieldLookup { Group = "2", Title = "MultiLookup",Type=FieldType.Lookup,         InternalName = Internals.MultiLookup , AllowMultivalue = true},
                    new Field { Group = "2", Title = "Integer",         Type=FieldType.Integer,         InternalName = Internals.Integer    },
                    new Field { Group = "3", Title = "Note",            Type=FieldType.Note,            InternalName = Internals.Note       },
                    new Field { Group = "3", Title = "Url",             Type=FieldType.Url,             InternalName = Internals.Url        },
                    new FieldChoice { Group = "3", Title = "Choice",    Type=FieldType.Choice,          InternalName = Internals.Choice     , Choices = new[] {"Choice1", "Choice2", "Choice3"}},
                    new Field { Group = "3", Title = "Boolean",         Type=FieldType.Boolean,         InternalName = Internals.Boolean    },
                    new Field { Group = "4", Title = "Readonly",        Type=FieldType.Text,            InternalName = Internals.Readonly   , IsReadonly = true},
                    new Field { Group = "4", Title = "Hidden",          Type=FieldType.Text,            InternalName = Internals.Hidden     , IsHidden = true},
                    new Field { Group = "4", Title = "Number",          Type=FieldType.Number,          InternalName = Internals.Number     },
                    new Field { Group = "4", Title = "Computed",        Type=FieldType.Computed,        InternalName = Internals.Computed   },
                    new Field { Group = "4", Title = "Currency",        Type=FieldType.Currency,        InternalName = Internals.Currency   },
                    new Field { Group = "4", Title = "MultiChoice",     Type=FieldType.MultiChoice,     InternalName = Internals.MultiChoice },
                    new Field { Group = "5", Title = "Guid",            Type=FieldType.Guid,            InternalName = Internals.Guid       },
                    new Field { Group = "5", Title = "Calculated",      Type=FieldType.Calculated,      InternalName = Internals.Calculated },
                    new Field { Group = "5", Title = "File",            Type=FieldType.File,            InternalName = Internals.File       },
                    new Field { Group = "5", Title = "Attachments",     Type=FieldType.Attachments,     InternalName = Internals.Attachments },
                    new Field { Group = "5", Title = "ContentTypeId",   Type=FieldType.ContentTypeId,   InternalName = Internals.ContentTypeId },
                    new Field { Group = "5", Title = "Geolocation",     Type=FieldType.Geolocation,     InternalName = Internals.Geolocation },
                    new Field { Group = "5", Title = "SortTest",        Type=FieldType.Geolocation,     InternalName = Internals.SortTest },
                };

                list.Fields = fields;
            });
        }

        public async Task SaveItem(ListItem item)
        {
            await Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(2000);

                if (item.Id == 2) throw new Exception("Item save failed");
            });
        }

        public async Task FillContentTypes(SList list, bool fillAlsoWeb = true)
        {
            await Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(2000);

                list.ContentTypes = new List<ContentType>
                {
                    new ContentType { Id= "0x01231", Name = "Folder"},
                    new ContentType { Id= "0x01232", Name = "Item"},
                };

                list.Web.ContentTypes = new List<ContentType>
                {
                    new ContentType { Id= "0x0123", Name = "Folder"},
                    new ContentType { Id= "0x0124", Name = "Item 1"},
                    new ContentType { Id= "0x0125", Name = "Item 2"},
                    new ContentType { Id= "0x0126", Name = "Item 3"},
                };
            });
        }

        public async Task<List<KeyValuePair<string, string>>> GetLookupItems(FieldLookup lookup)
        {
            await Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(2000);
            });

            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("1", "Test 1"),
                new KeyValuePair<string, string>("4", "Test 4"),
                new KeyValuePair<string, string>("8", "Test 8"),
                new KeyValuePair<string, string>("3", "Test 3"),
                new KeyValuePair<string, string>("6", "Test 6"),
                new KeyValuePair<string, string>("7", "Test 7"),
                new KeyValuePair<string, string>("5", "Test 5"),
                new KeyValuePair<string, string>("2", "Test 2"),
                new KeyValuePair<string, string>("9", "Test 9")
            };
        }
    }
}
