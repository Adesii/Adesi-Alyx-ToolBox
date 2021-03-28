using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.Serialization.KeyValues;

namespace AAT.Soundevents
{
    public enum EventDisplays
    {
        FloatValue,
        SoundeventPicker,
        ArrayValue,
        StringValue,
        TypePicker,
        FilePicker
    }
    public class SoundeventProperty : INotifyPropertyChanged
    {
        private string typeName;
        private Type type;
        private object value;
        private EventDisplays disAs;


        public event PropertyChangedEventHandler PropertyChanged;

        public SoundeventProperty(PropertyNames PropertyName,Type property)
        {
            this.typeName = PropertyName.ToString();
            this.type = property;
            this.disAs = getDisplayType(property);

        }
        public SoundeventProperty(string typeName,object v)
        {
            this.typeName = typeName;
            this.disAs = getDisplayType(v);
            this.value = v;
        }
        public SoundeventProperty(string typeName, Type type)
        {
            this.typeName = typeName;
            this.type = type;
            this.disAs = getDisplayType(type);
        }
        public SoundeventProperty(string typeName,  EventDisplays t = EventDisplays.StringValue,object v = null)
        {
            this.typeName = typeName;
            this.disAs = t;
            this.value = v;
        }
        public SoundeventProperty(SoundeventsPropertyDefinitions.EventTypeStruct ts)
        {
            this.typeName = ts.Name;
            this.disAs = getDisplayType(ts.Type);
        }

        public EventDisplays getDisplayType(object val)
        {
            EventDisplays t = EventDisplays.StringValue;
            if (val == null) return t;
            System.Diagnostics.Debug.WriteLine(val?.ToString()+" Something or else");
            if (IsNumeric(val as Type))
            {
                t = EventDisplays.FloatValue;
            }
            if ((val as Type) == typeof(ValveResourceFormat.Serialization.KeyValues.KVObject))
            {
                t = EventDisplays.ArrayValue;
            }
            return t;
        }
        public string TypeName { get => typeName; set { typeName = value; NotifyPropertyChanged(nameof(TypeName)); } }
        public object Value { get => value; set { this.value = value; NotifyPropertyChanged(nameof(Value)); } }
        public Type Type { get => type; set { type = value; NotifyPropertyChanged(nameof(Type)); } }
        public EventDisplays DisAs { get => disAs; set { disAs = value; NotifyPropertyChanged(nameof(DisAs)); } }

        public static bool IsNumeric(Type type)
        {
            if (type == null) { return false; }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = type.GetGenericArguments()[0];
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public void NotifyPropertyChanged(string propertyName)
        {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public override string ToString()
        {
            return TypeName;
        }
    }
}
