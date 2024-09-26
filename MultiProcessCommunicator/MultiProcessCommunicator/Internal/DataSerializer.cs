using System;
using System.Collections;
using System.IO;

namespace MultiProcessCommunicator.Internal
{
    internal class DataSerializer
    {
        public static void Serialize(BinaryWriter writer, object value, Type type)
        {
            if (type == typeof(Int16))
            {
                writer.Write((Int16)value);
            }
            else if (type == typeof(Int32))
            {
                writer.Write((Int32)value);
            }
            else if (type == typeof(Double))
            {
                writer.Write((double)value);
            }
            else if (type == typeof(Int64))
            {
                writer.Write((Int64)value);
            }
            else if (type == typeof(byte))
            {
                writer.Write((byte)value);
            }
            else if (type == typeof(byte[]))
            {
                var byteArray = value as byte[];
                var byteArrayLen = byteArray == null ? 0 : byteArray.Length;
                writer.Write(byteArrayLen);

                if (byteArrayLen > 0)
                    writer.Write(byteArray);
            }
            else if (type == typeof(string))
            {
                var strVal = value as string;
                var strLen = string.IsNullOrEmpty(strVal) ? 0 : strVal.Length;
                writer.Write(strLen);
                writer.Write(strVal);
            }
            else if (type.IsEnum)
            {
                writer.Write((Int32)value);
            }
            else if (type == typeof(bool))
            {
                writer.Write((bool)value);
            }
            else if (type.IsArray)
            {
                var array = value as IList;
                var arrayLen = array == null ? 0 : array.Count;
                writer.Write(arrayLen);
                for (int i = 0; i < arrayLen; i++)
                {
                    var arrayItem = array[i];
                    Serialize(writer, arrayItem, type.GetElementType());
                }
            }
            else if (type == typeof(DateTime))
            {
                var dateData = (DateTime)value;
                writer.Write(dateData.Ticks);
            }
            else if (type == typeof(TimeSpan))
            {
                var timespanData = (TimeSpan)value;
                writer.Write(timespanData.Ticks);
            }
            else if (type == typeof(Guid))
            {
                var guidObject = (Guid)value;
                writer.Write(guidObject.ToByteArray());
            }
            else if (type.IsValueType)// do not change position!
            {
                var typeProperties = type.GetProperties();

                foreach (var proertyInfo in typeProperties)
                {
                    var propertyValue = proertyInfo.GetValue(value);
                    Serialize(writer, propertyValue, proertyInfo.PropertyType);
                }
            }
            else if (type.IsClass)
            {
                var typeProperties = type.GetProperties();

                foreach (var propertyInfo in typeProperties)
                {
                    var propertyValue = propertyInfo.GetValue(value);
                    Serialize(writer, propertyValue, propertyInfo.PropertyType);
                }
            }
            else
            {
                throw new NotSupportedException($"type {type} not supported");
            }
        }


        public static object Deserialize(BinaryReader reader, Type type)
        {
            if (type == typeof(Int16))
            {
                return reader.ReadInt16();
            }
            else if (type == typeof(Int32))
            {
                return reader.ReadInt32();
            }
            else if (type == typeof(Double))
            {
                return reader.ReadDouble();
            }
            else if (type == typeof(Int64))
            {
                return reader.ReadInt64();
            }
            else if (type == typeof(byte))
            {
                return reader.ReadByte();
            }
            else if (type == typeof(byte[]))
            {
                var byteArrayLen = reader.ReadInt32();
                return reader.ReadBytes(byteArrayLen);
            }
            else if (type == typeof(string))
            {
                var strLen = reader.ReadInt32();
                return reader.ReadString();
            }
            else if (type.IsEnum)
            {
                return Enum.ToObject(type, reader.ReadInt32());
            }
            else if (type == typeof(bool))
            {
                return reader.ReadBoolean();
            }
            else if (type.IsArray)
            {
                var arrayLen = reader.ReadInt32();
                var array = Array.CreateInstance(type.GetElementType(), arrayLen);
                for (int i = 0; i < arrayLen; i++)
                {
                    var arrayItem = Deserialize(reader, type.GetElementType());
                    array.SetValue(arrayItem, i);
                }
                return array;
            }
            else if (type == typeof(DateTime))
            {
                var datetimeTicks = reader.ReadInt64();
                return new DateTime(datetimeTicks);
            }
            else if (type == typeof(TimeSpan))
            {
                var timespanTicks = reader.ReadInt64();
                return new TimeSpan(timespanTicks);
            }
            else if (type == typeof(Guid))
            {
                var guidBytes = reader.ReadBytes(16);
                return new Guid(guidBytes);
            }
            else if (type.IsValueType)// do not change position!
            {
                var typeProperties = type.GetProperties();
                var classObject = Activator.CreateInstance(type);
                foreach (var proertyInfo in typeProperties)
                {
                    var propertyValue = Deserialize(reader, proertyInfo.PropertyType);

                    if (proertyInfo.CanWrite)
                        proertyInfo.SetValue(classObject, propertyValue);
                }
                return classObject;
            }
            else if (type.IsClass)
            {
                var typeProperties = type.GetProperties();
                var classObject = Activator.CreateInstance(type);
                foreach (var proertyInfo in typeProperties)
                {
                    var propertyValue = Deserialize(reader, proertyInfo.PropertyType);

                    if (proertyInfo.CanWrite)
                        proertyInfo.SetValue(classObject, propertyValue);
                }
                return classObject;
            }
            else
            {
                throw new NotSupportedException($"type {type} not supported");
            }

        }
    }
}
