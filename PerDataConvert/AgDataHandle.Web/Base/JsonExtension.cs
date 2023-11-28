using JsonDiffPatchDotNet;
using JsonDiffPatchDotNet.Formatters.JsonPatch;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace AgDataHandle.Web
{
    public static class JsonExtension
    {
        public static void ApplyTo(this string json, object to)
        {
            string str = "";
            try
            {
                str = json;
                var token1 = JToken.FromObject(to);

                // var token2 = JToken.FromObject(src);
                var token2 = JToken.Parse(str);

                var diff = new JsonDiffPatch(new Options()
                {
                    ArrayDiff = ArrayDiffMode.Efficient,
                    TextDiff = TextDiffMode.Efficient,
                    MinEfficientTextDiffLength = long.MaxValue
                });
                var token3 = diff.Diff(token1, token2);
                if (token3 == null || !token3.HasValues)
                {
                    return;
                }
                var output = diff.Patch(token1, token3);

                var formatter = new JsonDeltaFormatter();
                var operations = formatter.Format(token3);
                operations.RemoveWhere(p => p.Op != "replace");
                List<Microsoft.AspNetCore.JsonPatch.Operations.Operation> listOperation = new List<Microsoft.AspNetCore.JsonPatch.Operations.Operation>();
                foreach (var operation in operations)
                {
                    listOperation.Add(new Microsoft.AspNetCore.JsonPatch.Operations.Operation(operation.Op, operation.Path, operation.From, operation.Value));
                }
                JsonPatchDocument doc = new JsonPatchDocument(listOperation, new DefaultContractResolver());
                doc.ApplyTo(to);
            }
            catch (System.Exception ex)
            {
                throw new ArgumentException("Raw string :" + str + "", ex);
            }
        }

        public static void ApplyTo(this JsonElement jsonObj, object to)
        {
            string str = "";
            try
            {
                str = jsonObj.GetRawText();
                var token1 = JToken.FromObject(to);

                // var token2 = JToken.FromObject(src);
                var token2 = JToken.Parse(str);

                var diff = new JsonDiffPatch(new Options()
                {
                    ArrayDiff = ArrayDiffMode.Efficient,
                    TextDiff = TextDiffMode.Efficient,
                    MinEfficientTextDiffLength = long.MaxValue
                });
                var token3 = diff.Diff(token1, token2);
                if (token3 == null || !token3.HasValues)
                {
                    return;
                }
                var output = diff.Patch(token1, token3);

                var formatter = new JsonDeltaFormatter();
                var operations = formatter.Format(token3);
                operations.RemoveWhere(p => p.Op != "replace");
                List<Microsoft.AspNetCore.JsonPatch.Operations.Operation> listOperation = new List<Microsoft.AspNetCore.JsonPatch.Operations.Operation>();
                foreach (var operation in operations)
                {
                    listOperation.Add(new Microsoft.AspNetCore.JsonPatch.Operations.Operation(operation.Op, operation.Path, operation.From, operation.Value));
                }
                JsonPatchDocument doc = new JsonPatchDocument(listOperation, new DefaultContractResolver());
                doc.ApplyTo(to);
            }
            catch (System.Exception ex)
            {
                throw new ArgumentException("Raw string :" + str + "", ex);
            }
        }

        public static void ApplyTo<T>(this JsonElement jsonObj, T to)
        {
            string str = "";
            try
            {
                str = jsonObj.GetRawText();
                var token1 = JToken.FromObject(to);

                // var token2 = JToken.FromObject(src);
                var token2 = JToken.Parse(str);

                var diff = new JsonDiffPatch(new Options()
                {
                    ArrayDiff = ArrayDiffMode.Efficient,
                    TextDiff = TextDiffMode.Efficient,
                    MinEfficientTextDiffLength = long.MaxValue
                });
                var token3 = diff.Diff(token1, token2);
                if (token3 == null || !token3.HasValues)
                {
                    return;
                }
                var output = diff.Patch(token1, token3);

                var formatter = new JsonDeltaFormatter();
                var operations = formatter.Format(token3);
                operations.RemoveWhere(p => p.Op != "replace");
                List<Microsoft.AspNetCore.JsonPatch.Operations.Operation> listOperation = new List<Microsoft.AspNetCore.JsonPatch.Operations.Operation>();
                foreach (var operation in operations)
                {
                    listOperation.Add(new Microsoft.AspNetCore.JsonPatch.Operations.Operation(operation.Op, operation.Path, operation.From, operation.Value));
                }
                JsonPatchDocument doc = new JsonPatchDocument(listOperation, new DefaultContractResolver());
                doc.ApplyTo(to);
            }
            catch (System.Exception ex)
            {
                throw new ArgumentException("Raw string :" + str + "", ex);
            }
        }
    }
}
