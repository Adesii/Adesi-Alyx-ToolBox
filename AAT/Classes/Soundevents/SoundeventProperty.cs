using AAT.AKV;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        FilePicker,
        EventTypePicker
    }
    public class SoundeventProperty : INotifyPropertyChanged
    {
        private string typeName;
        private Type type;
        private object value;
        private EventDisplays disAs;


        public event PropertyChangedEventHandler PropertyChanged;
        private DependencyObject m_valCon;
        public DependencyObject ValueContainer
        {
            get => m_valCon;
            set
            {
                if (m_valCon != null) PropertyChanged -= SoundeventProperty_PropertyChanged;
                m_valCon = value;
                PropertyChanged += SoundeventProperty_PropertyChanged;
            }
        }

        private void SoundeventProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        public SoundeventProperty(PropertyNames PropertyName, Type property)
        {
            this.typeName = PropertyName.ToString();
            this.type = property;
            this.disAs = getDisplayType(property);
            HardcodedTypes();


        }
        public SoundeventProperty(PropertyNames PropertyName, SoundeventsPropertyDefinitions.EventTypeStruct tStruct)
        {
            unpackEventStruct(tStruct);
            this.typeName = PropertyName.ToString();
            HardcodedTypes();

        }
        public int arrayDepth = 0;
        public SoundeventProperty(string typeName, object v)
        {
            this.typeName = typeName;
            if (v is KVValue vks)
                this.Type = GetTypeByEnum(vks.Type);
            else if (v is AKValue vk)
                this.Type = GetTypeByEnum(vk.Type);
                
            else
                this.disAs = getDisplayType(v);
            this.value = v;
            HardcodedTypes();

        }
        public SoundeventProperty(string typeName, Type type)
        {
            this.typeName = typeName;
            this.type = type;
            this.disAs = getDisplayType(type);
            HardcodedTypes();

        }
        public SoundeventProperty(string typeName, EventDisplays t = EventDisplays.StringValue, object v = null)
        {
            this.typeName = typeName;
            this.disAs = t;
            this.value = v;
            if (v is KVValue vks)
                this.Type = GetTypeByEnum(vks.Type);
            else if (v is AKValue vk)
                this.Type = GetTypeByEnum(vk.Type);
            HardcodedTypes();

        }
        public SoundeventProperty(SoundeventsPropertyDefinitions.EventTypeStruct ts)
        {
            unpackEventStruct(ts);
        }

        public Type GetTypeByEnum(KVType kvt)
        {
            switch (kvt)
            {

                case KVType.BOOLEAN:
                    this.disAs = EventDisplays.StringValue;
                    return typeof(bool);
                case KVType.OBJECT:
                case KVType.NULL:
                case KVType.STRING:
                case KVType.STRING_MULTI:
                    this.disAs = EventDisplays.StringValue;
                    
                    return typeof(string);
                case KVType.BINARY_BLOB:
                case KVType.ARRAY:
                case KVType.ARRAY_TYPED:
                    this.disAs = EventDisplays.ArrayValue;
                    return typeof(KVObject);
                case KVType.BOOLEAN_TRUE:
                case KVType.BOOLEAN_FALSE:
                    return typeof(bool);
                case KVType.INT64:
                case KVType.UINT64:
                case KVType.DOUBLE:
                case KVType.INT32:
                case KVType.UINT32:
                case KVType.INT64_ZERO:
                case KVType.INT64_ONE:
                case KVType.DOUBLE_ZERO:
                case KVType.DOUBLE_ONE:
                    this.disAs = EventDisplays.FloatValue;
                    return typeof(float);
                default:
                    this.disAs = EventDisplays.StringValue;
                    return typeof(string);
            }
        }

        public void unpackEventStruct(SoundeventsPropertyDefinitions.EventTypeStruct t)
        {
            this.Type = GetTypeByEnum(t.Type);
            this.Value = t.KVValue;
            this.typeName = t.Name;

            HardcodedTypes();
        }

        public void HardcodedTypes()
        {
            switch (typeName)
            {
                case "soundevent_01":
                case "soundevent_02":
                case "soundevent_03":
                case "soundevent_04":
                case "base":
                    this.disAs = EventDisplays.SoundeventPicker;
                    break;
                case "vsnd_files":
                    //System.Diagnostics.Debug.WriteLine(value.GetType());
                    this.disAs = EventDisplays.FilePicker;
                    break;
                case "type":
                    this.disAs = EventDisplays.EventTypePicker;
                    break;
            }
        }
        public EventDisplays getDisplayType(object val)
        {
            EventDisplays t = EventDisplays.StringValue;
            if (val == null) return t;
            //System.Diagnostics.Debug.WriteLine(val?.ToString() + " Something or else");
            if (IsNumeric(val.GetType()) && float.TryParse(val.ToString(),out float b))
            {
                t = EventDisplays.FloatValue;
                if(b != null)
                    value = b; 
            }
            if ((val as Type) == typeof(KVObject))
            {
                t = EventDisplays.ArrayValue;
            }
            return t;
        }
        public string TypeName { get => typeName; set { typeName = value; NotifyPropertyChanged(nameof(TypeName)); } }
        public object Value
        {
            get => value;
            set
            {
                if (value is KVValue kvva)
                    if (kvva.Value is KVObject kvo)
                        this.value = createArrayFromKVO(kvo);
                    else
                        this.value = value;
                else
                    this.value = value;
                NotifyPropertyChanged(nameof(Value));
            }
        }
        public Type Type { get => type; set { type = value; NotifyPropertyChanged(nameof(Type)); } }
        public EventDisplays DisAs { get => disAs; set { disAs = value; NotifyPropertyChanged(nameof(DisAs)); } }


        public List<AKV.AKValue> createArrayFromKVO(KVObject kvo)
        {
            List<AKV.AKValue> kl = new();

            foreach (var item in kvo.Properties)
            {
                AKV.AKValue kvv;
                if (item.Value.Value is KVObject kc)
                    kvv = new(KVType.ARRAY, createArrayFromKVO(kc));
                else
                {
                    kvv = new AKV.AKValue(item.Value.Type, item.Value.Value);
                }
                kl.Add(kvv);

            }
            return kl;
        }
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
