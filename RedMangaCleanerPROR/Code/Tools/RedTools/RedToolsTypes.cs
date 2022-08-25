using RedsTools.Strings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace RedsTools
{
    namespace Types
    {
        public static class TypeConverter
        {
            //converts TInput to TOutput(with values) (converts only common properties other ignored) and return it
            public static TOutput ChildParent<TInput, TOutput>(TInput input)
            {
                string[] inputProps = typeof(TInput).GetProperties().Select(x => x.Name).ToArray();
                string[] outputProps = typeof(TOutput).GetProperties().Select(x => x.Name).ToArray();

                string[] commonProps = inputProps.Intersect(outputProps).ToArray();

                TOutput output = (TOutput)Activator.CreateInstance(typeof(TOutput));

                foreach (string prop in commonProps)
                {
                    object inputValue = typeof(TInput).GetProperty(prop).GetValue(input);
                    typeof(TOutput).GetProperty(prop).SetValue(output, inputValue);
                }

                return output;
            }

            //curently same as "ChildParent"
            public static TOutput StrangerStranger<TInput, TOutput>(TInput input)
            {
                string[] inputProps = typeof(TInput).GetProperties().Select(x => x.Name).ToArray();
                string[] outputProps = typeof(TOutput).GetProperties().Select(x => x.Name).ToArray();

                string[] commonProps = inputProps.Intersect(outputProps).ToArray();

                TOutput output = (TOutput)Activator.CreateInstance(typeof(TOutput));

                foreach (string prop in commonProps)
                {
                    object inputValue = typeof(TInput).GetProperty(prop).GetValue(input);
                    typeof(TOutput).GetProperty(prop).SetValue(output, inputValue);
                }

                return output;
            }

            //converts TInput to TOutput(with values) (converts only common properties other ignored) and set result to TOutput (output)
            public static void ChildParentSetTo<TInput, TOutput>(TInput input, TOutput output)
            {
                string[] inputProps = typeof(TInput).GetProperties().Select(x => x.Name).ToArray();
                string[] outputProps = typeof(TOutput).GetProperties().Select(x => x.Name).ToArray();

                string[] commonProps = inputProps.Intersect(outputProps).ToArray();

                foreach (string prop in commonProps)
                {
                    object inputValue = typeof(TInput).GetProperty(prop).GetValue(input);
                    typeof(TOutput).GetProperty(prop).SetValue(output, inputValue);
                }
            }

            //curently same as "ChildParentSetTo"
            public static void StrangerStrangerSetTo<TInput, TOutput>(TInput input, TOutput output)
            {
                string[] inputProps = typeof(TInput).GetProperties().Select(x => x.Name).ToArray();
                string[] outputProps = typeof(TOutput).GetProperties().Select(x => x.Name).ToArray();

                string[] commonProps = inputProps.Intersect(outputProps).ToArray();

                foreach (string prop in commonProps)
                {
                    object inputValue = typeof(TInput).GetProperty(prop).GetValue(input);
                    typeof(TOutput).GetProperty(prop).SetValue(output, inputValue);
                }
            }

            public static void StrangerStranger<TInput, TOutput>(TInput input, TOutput output, string[] propPairs, bool onlyInputPairs = false) //prop pair == inputpropname>>>outputpropname
            {
                string[] inputProps = typeof(TInput).GetProperties().Select(x => x.Name).ToArray();
                string[] outputProps = typeof(TOutput).GetProperties().Select(x => x.Name).ToArray();

                List<string> commonProps = inputProps.Intersect(outputProps).ToList();
                List<string[]> propPairsToApply = new List<string[]>();

                for (int i = 0; i < propPairs.Length; i++)
                    propPairsToApply.Add(RStrings.Split(propPairs[i], ">>>"));

                for (int i = 0; i < commonProps.Count; i++)
                {
                    for (int j = 0; j < propPairsToApply.Count; j++)
                    {
                        if (commonProps[i] == propPairsToApply[j][0])
                        {
                            commonProps.RemoveAt(i);
                        }
                        else if (commonProps[i] == propPairsToApply[j][1])
                        {
                            commonProps.RemoveAt(i);
                        }
                    }
                }

                foreach (string prop in commonProps)
                {
                    object inputValue = typeof(TInput).GetProperty(prop).GetValue(input);
                    typeof(TOutput).GetProperty(prop).SetValue(output, inputValue);
                }

                for (int i = 0; i < propPairsToApply.Count; i++)
                {
                    object inputValue = typeof(TInput).GetProperty(propPairsToApply[i][0]).GetValue(input);
                    typeof(TOutput).GetProperty(propPairsToApply[i][1]).SetValue(output, inputValue);
                }
            }
        }

        public static class TypeGeneralUtility
        {
            public static T DeepClone<T>(this T obj)
            {
                using (var ms = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(ms, obj);
                    ms.Position = 0;

                    return (T)formatter.Deserialize(ms);
                }
            }
        }
    }
}

