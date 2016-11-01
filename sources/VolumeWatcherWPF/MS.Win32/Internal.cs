// ==++== 
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//
// ==--== 
/*============================================================
** 
** This file exists to contain miscellaneous module-level attributes 
** and other miscellaneous stuff.
** 
**
**
===========================================================*/
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Reflection;

#if FEATURE_COMINTEROP
 
[assembly:Guid("BED7F4EA-1A96-11d2-8F08-00A0C9A6186D")]
 
// The following attribute are required to ensure COM compatibility. 
[assembly:System.Runtime.InteropServices.ComCompatibleVersion(1, 0, 3300, 0)]
[assembly:System.Runtime.InteropServices.TypeLibVersion(2, 4)] 
  
#endif // FEATURE_COMINTEROP

[assembly: DefaultDependencyAttribute(LoadHint.Always)]
// mscorlib would like to have its literal strings frozen if possible
[assembly: System.Runtime.CompilerServices.StringFreezingAttribute()]

namespace System
{
    static class Internal
    {
        // This method is purely an aid for NGen to statically deduce which 
        // instantiations to save in the ngen image.
        // Otherwise, the JIT-compiler gets used, which is bad for working-set.
        // Note that IBC can provide this information too.
        // However, this helps in keeping the JIT-compiler out even for 
        // test scenarios which do not use IBC.
        // This can be removed after V2, when we implement other schemes 
        // of keeping the JIT-compiler out for generic instantiations. 

        static void CommonlyUsedGenericInstantiations_HACK()
        {
            // Make absolutely sure we include some of the most common
            // instantiations here in mscorlib's ngen image.
            // Note that reference type instantiations are already included 
            // automatically for us.

            System.Array.Sort<double>(null);
            System.Array.Sort<int>(null);
            System.Array.Sort<intptr>(null);

            new ArraySegment<byte>(new byte[1], 0, 0);

            new Dictionary<char, object= "" > ();
            new Dictionary<guid, byte= "" > ();
            new Dictionary<guid, object= "" > ();
            new Dictionary<guid, guid= "" > (); // Added for Visual Studio 2010 
            new Dictionary<int16, intptr= "" > ();
            new Dictionary<int32, byte= "" > ();
            new Dictionary<int32, int32= "" > ();
            new Dictionary<int32, object= "" > ();
            new Dictionary<intptr, boolean= "" > ();
            new Dictionary<intptr, int16= "" > ();
            new Dictionary<object, boolean= "" > ();
            new Dictionary<object, char= "" > ();
            new Dictionary<object, guid= "" > ();
            new Dictionary<object, int32= "" > ();
            new Dictionary<object, int64= "" > (); // Added for Visual Studio 2010 
            new Dictionary<uint, weakreference= "" > ();  // NCL team needs this
            new Dictionary<object, uint32= "" > ();
            new Dictionary<uint32, object= "" > ();
            new Dictionary<int64, object= "" > ();

            // Microsoft.Windows.Design 
            new Dictionary<system.reflection.membertypes, object= "" > ();
            new EnumEqualityComparer<system.reflection.membertypes>();

            // Microsoft.Expression.DesignModel
            new Dictionary<object, keyvaluepair<object, object= "" >> ();
            new Dictionary<keyvaluepair<object, object>, Object>();

            NullableHelper_HACK<boolean>();
            NullableHelper_HACK<byte>();
            NullableHelper_HACK<char>();
            NullableHelper_HACK<datetime>();
            NullableHelper_HACK<decimal>();
            NullableHelper_HACK<double>();
            NullableHelper_HACK<guid>();
            NullableHelper_HACK<int16>();
            NullableHelper_HACK<int32>();
            NullableHelper_HACK<int64>();
            NullableHelper_HACK<single>();
            NullableHelper_HACK<timespan>();
            NullableHelper_HACK<datetimeoffset>();  // For SQL

            new List<boolean>();
            new List<byte>();
            new List<char>();
            new List<datetime>();
            new List<decimal>();
            new List<double>();
            new List<guid>();
            new List<int16>();
            new List<int32>();
            new List<int64>();
            new List<timespan>();
            new List<sbyte>();
            new List<single>();
            new List<uint16>();
            new List<uint32>();
            new List<uint64>();
            new List<intptr>();
            new List<keyvaluepair<object, object= "" >> ();
            new List<gchandle>();  // NCL team needs this
            new List<datetimeoffset>();

            RuntimeType.RuntimeTypeCache.Prejitinit_HACK();

            new CerArrayList<runtimemethodinfo>(0);
            new CerArrayList<runtimeconstructorinfo>(0);
            new CerArrayList<runtimepropertyinfo>(0);
            new CerArrayList<runtimeeventinfo>(0);
            new CerArrayList<runtimefieldinfo>(0);
            new CerArrayList<runtimetype>(0);

            new KeyValuePair<char, uint16= "" > ('\0', UInt16.MinValue);
            new KeyValuePair<uint16, double= "" > (UInt16.MinValue, Double.MinValue);
            new KeyValuePair<object, int32= "" > (String.Empty, Int32.MinValue);
            new KeyValuePair<int32, int32= "" > (Int32.MinValue, Int32.MinValue);
            SZArrayHelper_HACK<boolean>(null);
            SZArrayHelper_HACK<byte>(null);
            SZArrayHelper_HACK<datetime>(null);
            SZArrayHelper_HACK<decimal>(null);
            SZArrayHelper_HACK<double>(null);
            SZArrayHelper_HACK<guid>(null);
            SZArrayHelper_HACK<int16>(null);
            SZArrayHelper_HACK<int32>(null);
            SZArrayHelper_HACK<int64>(null);
            SZArrayHelper_HACK<timespan>(null);
            SZArrayHelper_HACK<sbyte>(null);
            SZArrayHelper_HACK<single>(null);
            SZArrayHelper_HACK<uint16>(null);
            SZArrayHelper_HACK<uint32>(null);
            SZArrayHelper_HACK<uint64>(null);
            SZArrayHelper_HACK<datetimeoffset>(null);

            SZArrayHelper_HACK<customattributetypedargument>(null);
            SZArrayHelper_HACK<customattributenamedargument>(null);
        }

        static T NullableHelper_HACK<t>() where T : struct
        {
            Nullable.Compare<t>(null, null);
            Nullable.Equals<t>(null, null);
            Nullable<t> nullable = new Nullable<t>();
            return nullable.GetValueOrDefault();
        }

        static void SZArrayHelper_HACK<t>(SZArrayHelper oSZArrayHelper)
        {
            // Instantiate common methods for IList implementation on Array
            oSZArrayHelper.get_Count<t>();
            oSZArrayHelper.get_Item<t>(0);
            oSZArrayHelper.GetEnumerator();
        }
    }
}

// File provided for Reference Use Only by Microsoft Corporation (c) 2007.