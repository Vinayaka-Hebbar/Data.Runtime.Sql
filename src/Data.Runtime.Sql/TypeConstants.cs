﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Data.Runtime.Sql
{
    /// <summary>
    /// Constant Values
    /// </summary>
    public static class Constants
    {
        public const string DataMemberName = "Name";
        public const string IsRequired = "IsRequired";
        public const string Order = "Order";
        public const string Comma = ",";
        public const char CommaChar = ',';
        public const string DateFormat = "yyyy-MM-dd HH:mm:ss";
        public const string EnumValue = "value__";
        public const string OperatorImplicit = "op_Implicit";

        public const string TypeString = "System.String";
        public const string TypeInt16 = "System.Int16";
        public const string TypeUInt16 = "System.UInt16";
        public const string TypeInt32 = "System.Int32";
        public const string TypeUInt32 = "System.UInt32";
        public const string TypeInt64 = "System.Int64";
        public const string TypeUInt64 = "System.UInt64";
        public const string TypeBool = "System.Boolean";
        //Todo Float
        public const string TypeDouble = "System.Double";
        public const string TypeStringArray = "System.String[]";
        public const string TypeDateTime = "System.DateTime";

        public const string EmptyJsonObject = "{}";

        public const BindingFlags InstantPublic = BindingFlags.Instance | BindingFlags.Public;
    }
}