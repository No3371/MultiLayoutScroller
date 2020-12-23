using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BAStudio.MultiLayoutScroller
{
    public static class SchemaSOCombiner
    {
        public static MultiLayoutScrollerSchema CombineScrollerSchema (MultiLayoutScrollerSchemaSO so)
        {
            for (var i = 0; i < so.viewAssets.Length; i++)
            {
                so.schema.views.Add(CombineViewSchema(so.viewAssets[i]));
            }
            return so.schema;
        }

        public static MutliScrollerViewSchema CombineViewSchema (MultiLayoutScrollerViewSchemaSO so)
        {
            for (var i = 0; i < so.layoutAssets.Length; i++)
            {
                so.viewSchema.layouts.Add(so.layoutAssets[i].viewSchema);
            }
            return so.viewSchema;
        }
    }
}
