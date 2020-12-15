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
            if (!reader.Read() || reader.TokenType != JsonToken.StartArray) throw new JsonSerializationException("Unexpected json, expecting: start of view list");
            while (reader.Read() && reader.TokenType == JsonToken.StartObject)
            {
                if (scroller.views == null) scroller.views = new List<MutliScrollerViewSchema>();
                while (reader.Read() && reader.TokenType == JsonToken.StartObject)
                {
                    scroller.views.Add(ReadView(reader));
                }
            }
            if (!reader.Read() || reader.TokenType != JsonToken.EndArray) throw new JsonSerializationException("Unexpected json, expecting: end of view list");
            return scroller;
        }

        MutliScrollerViewSchema ReadView (JsonReader reader)
        {
            MutliScrollerViewSchema view = new MutliScrollerViewSchema();
            // Some boxing happening here but should be ok for it's executed bery infrequente.
            AssurePropName(reader, KEY_VIEW_ID);
            if (!reader.Read() || reader.TokenType != JsonToken.Integer) ThrowUnexpectedJson("(int) view ID");
            view.viewID = ViewNameToID((string) reader.Value);
            AssurePropName(reader, KEY_VIEW_LAYOUTS);
            if (!reader.Read() || reader.TokenType != JsonToken.StartArray) throw new JsonSerializationException("Unexpected json, expecting: list of layouts (integer seperated by ,)");
            while (reader.Read() && reader.TokenType == JsonToken.StartObject)
            {
                view.layouts.Add(ReadLayout(reader));
            }
            reader.Read(); // End of object
            return view;
        }

        MutliLayoutScrollerLayoutSchema ReadLayout (JsonReader reader)
        {
            MutliLayoutScrollerLayoutSchema layout = new MutliLayoutScrollerLayoutSchema();
            // Some boxing happening here but should be ok for it's executed bery infrequente.
            AssurePropName(reader, KEY_LAYOUT_TYPE);
            if (AssureValue(reader, JsonToken.Integer)) ThrowUnexpectedJson("(type) layout type ID");
            layout.typeID = LayoutTypeNameToID((string) reader.Value);
            AssurePropName(reader, KEY_LAYOUT_ITEMS);
            while (reader.Read() && reader.TokenType == JsonToken.StartArray)
            {
                reader.Read();
                if (reader.TokenType == JsonToken.EndArray) break;
                else if (reader.TokenType == JsonToken.StartObject) layout.items.Add(ReadItemTypeIDPair(reader));
                else throw new JsonSerializationException("Unexpected json, expecting: item json object");
            }
            if (!reader.Read() || reader.TokenType != JsonToken.EndObject) throw new JsonSerializationException("Unexpected json, expecting: end of layout object");
            return layout;
        }

        TypeIDPair ReadItemTypeIDPair (JsonReader reader)
        {
            if (!reader.Read()) throw new JsonSerializationException("Unexpected end of json object when reading items");
            TypeIDPair pair;
            if (!reader.Read() || reader.TokenType != JsonToken.PropertyName || (string) reader.Value != KEY_ITEM_TYPE) 
            if (!reader.Read() || reader.TokenType != JsonToken.Integer) throw new JsonSerializationException("Unexpected json, expecting: item type");
            pair.type = (int) reader.Value;
            if (!reader.Read() || reader.TokenType != JsonToken.PropertyName || (string) reader.Value != KEY_ITEM_TYPE) throw new JsonSerializationException("Unexpected json, expecting: property name (ID)");
            if (!reader.Read() || reader.TokenType != JsonToken.Integer) throw new JsonSerializationException("Unexpected json, expecting: item id");
            pair.id = (int) reader.Value;
            if (!reader.Read() || reader.TokenType != JsonToken.EndObject) throw new JsonSerializationException("Unexpected json, expecting: end of item json object");
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
        public bool AssureValue (JsonReader reader, JsonToken type)
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