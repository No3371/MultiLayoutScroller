// MIT License
// Original author: Mohammed Iqubal Hussain (Polyandcode.com)

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BAStudio.MultiLayoutScroller
{
    public class MultiLayoutScrollerSchemaConverter : JsonConverter
    {
        const string KEY_VIEW_ID = "ViewID",
                     KEY_LAYOUT_TYPE = "LayoutType",
                     KEY_VIEW_LAYOUTS = "Layouts",
                     KEY_LAYOUT_ITEMS = "Items",
                     KEY_ITEM_TYPE = "Type",
                     KEY_ITEM_ID = "ID";
        public MultiLayoutScrollerSchemaConverter(System.Func<string, int> viewNameToID, System.Func<string, int> layoutTypeNameToID)
        {
            if (viewNameToID == null || layoutTypeNameToID == null) throw new NullReferenceException();
            ViewNameToID = viewNameToID;
            LayoutTypeNameToID = layoutTypeNameToID;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            MultiLayoutScrollerSchema scroller;
            if (existingValue == null || !(existingValue is MultiLayoutScrollerSchema)) existingValue = scroller = new MultiLayoutScrollerSchema();
            else scroller = existingValue as MultiLayoutScrollerSchema;
            // Some boxing happening here but should be ok for it's executed bery infrequente.
            if (!ReadAndAssureTokenType(reader, JsonToken.StartArray)) ThrowUnexpectedJson("(START) View json array");
            while (ReadAndAssureTokenType(reader, JsonToken.StartObject))
            {
                if (scroller.views == null) scroller.views = new List<MutliScrollerViewSchema>();
                scroller.views.Add(ReadView(reader));
            }
            if (!ReadAndAssureTokenType(reader, JsonToken.EndArray)) ThrowUnexpectedJson("(END) View json array");
            return scroller;
        }

        MutliScrollerViewSchema ReadView (JsonReader reader)
        {
            MutliScrollerViewSchema view = new MutliScrollerViewSchema();
            // Some boxing happening here but should be ok for it's executed bery infrequente.
            AssurePropName(reader, KEY_VIEW_ID);
            if (!ReadAndAssureTokenType(reader, JsonToken.Integer)) ThrowUnexpectedJson("(Int) View ID");
            view.viewID = ViewNameToID((string) reader.Value);
            AssurePropName(reader, KEY_VIEW_LAYOUTS);
            if (!ReadAndAssureTokenType(reader, JsonToken.StartArray)) ThrowUnexpectedJson("(START) Layout array)");
            while (ReadAndAssureTokenType(reader, JsonToken.StartObject))
            {
                view.layouts.Add(ReadLayout(reader));
            }
            ReadAndAssureTokenType(reader, JsonToken.EndArray);
            return view;
        }

        MutliLayoutScrollerLayoutSchema ReadLayout (JsonReader reader)
        {
            MutliLayoutScrollerLayoutSchema layout = new MutliLayoutScrollerLayoutSchema();
            // Some boxing happening here but should be ok for it's executed bery infrequente.
            AssurePropName(reader, KEY_LAYOUT_TYPE);
            if (ReadAndAssureTokenType(reader, JsonToken.Integer)) ThrowUnexpectedJson("(type) layout type ID");
            layout.typeID = LayoutTypeNameToID((string) reader.Value);
            AssurePropName(reader, KEY_LAYOUT_ITEMS);
            if (!ReadAndAssureTokenType(reader, JsonToken.StartArray)) ThrowUnexpectedJson("(START) Item array");
                while (ReadAndAssureTokenType(reader, JsonToken.StartObject))
                {
                    layout.items.Add(ReadItemTypeIDPair(reader));
                }
            if (!ReadAndAssureTokenType(reader, JsonToken.EndArray)) ThrowUnexpectedJson("(END) Item array");
            if (!ReadAndAssureTokenType(reader, JsonToken.EndObject)) ThrowUnexpectedJson("(END) Layout Object");
            return layout;
        }

        ItemTypeIDPair ReadItemTypeIDPair (JsonReader reader)
        {
            if (!reader.Read()) throw new JsonSerializationException("Unexpected end of json object when reading items");
            ItemTypeIDPair pair;
            AssurePropName(reader, KEY_ITEM_TYPE);
            if (!ReadAndAssureTokenType(reader, JsonToken.Integer)) ThrowUnexpectedJson("(Int) Item type");
            pair.type = (int) reader.Value;
            AssurePropName(reader, KEY_ITEM_ID);
            if (!ReadAndAssureTokenType(reader, JsonToken.Integer)) ThrowUnexpectedJson("(Int) Item data ID");
            pair.id = (int) reader.Value;
            if (!ReadAndAssureTokenType(reader, JsonToken.EndObject)) ThrowUnexpectedJson("(END) Item object");
            return pair;            
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite => false;

        public Func<string, int> ViewNameToID { get; }
        public Func<string, int> LayoutTypeNameToID { get; }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool ReadAndAssureTokenType (JsonReader reader, JsonToken type)
        {
            return reader.Read() && reader.TokenType != type;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void AssurePropName (JsonReader reader, string name)
        {
            if (!reader.Read() || reader.TokenType != JsonToken.PropertyName || (string) reader.Value != name) ThrowUnexpectedJson("(name) " + name);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void ThrowUnexpectedJson (string expected)
        {
            throw new JsonSerializationException("Unexpected json, expecting: " + expected);
        }
    }


    // LayoutName: { 0, 1, 4, 2, 11 ,155}  => The layout pulls 

    // MultiLayoutScroller
    // - View (Where scrolling happening)
    //   - Layout (A set of predefined item positon&dimension)
    //     - Item ()
    
    // View
    // View controls how layouts are placed, moved, culled

    // Layouts
    // A layout is just a set of predefined slots, items get place under nnthe slot transforms
    // It does not control it's own placement

    // Item
    // Prefab driving data

}