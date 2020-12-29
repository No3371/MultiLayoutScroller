using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BAStudio.MultiLayoutScroller
{   
    public static class SchemaScriptableObjectOCombiner
    {
        public static ScrollerSchema CombineScrollerSchema (ScrollerSchemaScriptableObject so)
        {
            for (var i = 0; i < so.viewAssets.Length; i++)
            {
                so.schema.Views.Add(CombineViewSchema(so.viewAssets[i]));
            }
            return so.schema;
        }

        public static ViewSchema CombineViewSchema (ViewSchemaScriptableObject so)
        {
            for (var i = 0; i < so.layoutAssets.Length; i++)
            {
                so.viewSchema.Layouts.Add(so.layoutAssets[i].viewSchema);
            }
            return so.viewSchema;
        }
    }
}
