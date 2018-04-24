﻿using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace ZTn.Json.Helpers
{
    public static class JsonHelpers
    {
        /// <summary>
        /// Reads (deserializes) an instance of type <typeparamref Name="T"/> from a JSON stream.
        /// The stream is safely disposed by the method.
        /// </summary>
        /// <typeparam name="T">Target instance type.</typeparam>
        /// <param name="stream">JSON source stream.</param>
        /// <returns>A new instance of <typeparamref Name="T"/> read from <paramref Name="stream"/>.</returns>
        public static T CreateFromJsonStream<T>(this Stream stream)
        {
            var serializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.Auto };
            T data;

            using (var streamReader = new StreamReader(stream))
            {
                data = (T)serializer.Deserialize(streamReader, typeof(T));
            }

            return data;
        }

        /// <summary>
        /// Reads (deserializes) an instance of type <typeparamref Name="T"/> from a JSON stream.
        /// Warning: the stream is not closed and must be disposed by the caller.
        /// </summary>
        /// <typeparam name="T">Target instance type.</typeparam>
        /// <param name="stream">JSON source stream.</param>
        /// <returns>A new instance of <typeparamref Name="T"/> read from <paramref Name="stream"/>.</returns>
        public static T CreateFromJsonPersistentStream<T>(this Stream stream)
        {
            var serializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.Auto };

            var streamReader = new StreamReader(stream);
            var data = (T)serializer.Deserialize(streamReader, typeof(T));

            return data;
        }

        /// <summary>
        /// Reads (deserializes) an instance of type <typeparamref Name="T"/> from a JSON string.
        /// </summary>
        /// <typeparam name="T">Target instance type.</typeparam>
        /// <param name="json">JSON source string.</param>
        /// <returns>A new instance of <typeparamref Name="T"/> read from <paramref Name="json"/>.</returns>
        public static T CreateFromJsonString<T>(this string json)
        {
            return json.CreateFromJsonString<T>(Encoding.Default);
        }

        /// <summary>
        /// Reads (deserializes) an instance of type <typeparamref Name="T"/> from a JSON string.
        /// </summary>
        /// <typeparam name="T">Target instance type.</typeparam>
        /// <param name="json">JSON source string.</param>
        /// <param name="encoding">Encoding of the <paramref name="json"/> string.</param>
        /// <returns>A new instance of <typeparamref Name="T"/> read from <paramref Name="json"/>.</returns>
        public static T CreateFromJsonString<T>(this string json, Encoding encoding)
        {
            T data;

            using (var stream = new MemoryStream(encoding.GetBytes(json)))
            {
                data = CreateFromJsonStream<T>(stream);
            }

            return data;
        }

        /// <summary>
        /// Reads (deserializes) an instance of type <typeparamref Name="T"/> from a JSON file.
        /// </summary>
        /// <typeparam name="T">Target instance type.</typeparam>
        /// <param name="fileName">JSON source path and filename.</param>
        /// <returns>A new instance of <typeparamref Name="T"/> read from <paramref Name="fileName"/>.</returns>
        public static T CreateFromJsonFile<T>(this string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            T data;

            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                data = CreateFromJsonStream<T>(fileStream);
            }

            return data;
        }

        /// <summary>
        /// Writes (serializes) an instance to a JSON file.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="fileName"></param>
        public static void WriteToJsonFile(this object instance, string fileName)
        {
            instance.WriteToJsonFile(fileName, false);
        }

        /// <summary>
        /// Writes (serializes) an instance to a JSON file.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="fileName"></param>
        /// <param name="indented"></param>
        public static void WriteToJsonFile(this object instance, string fileName, bool indented)
        {
            var formatting = (indented ? Formatting.Indented : Formatting.None);

            var serializer = new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = formatting
            };

            using (var streamWriter = new StreamWriter(fileName))
            {
                serializer.Serialize(streamWriter, instance);
            }
        }

        /// <summary>
        /// Writes (serializes) an instance to a json string.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="jsonConverters"></param>
        public static string WriteToJsonString(this object instance, params JsonConverter[] jsonConverters)
        {
            return instance.WriteToJsonString(false, jsonConverters);
        }

        /// <summary>
        /// Writes (serializes) an instance to a json string.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="indented"></param>
        /// <param name="jsonConverters"></param>
        public static string WriteToJsonString(this object instance, bool indented, params JsonConverter[] jsonConverters)
        {
            var formatting = indented ? Formatting.Indented : Formatting.None;

            var serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = formatting,
                Converters = jsonConverters
            };

            return JsonConvert.SerializeObject(instance, serializerSettings);
        }
    }
}
