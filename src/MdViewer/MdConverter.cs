/**
   Copyright 2025 masterLazy

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */
using Markdig;
using System.Text;
using System.Windows.Controls;
namespace MdViewer {
    public class MdConverter {
        private static readonly MarkdownPipeline _pipeline;
        private static readonly string _head;

        static MdConverter() {
            _pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();
            // Make up common components in <head>
            StringBuilder sb = new();
            sb.Append($@"<meta charset=""UTF-8"">");
            sb.Append($@"<style>{Helper.GetEmbeddedResource("MdViewer.style.css")}</style>");
            _head = sb.ToString();
        }

        public static string ToHtml(in string markdown, in UserControl userControl) {
            if (markdown == null) return MakeHtml("", userControl);
            var rawHtml = Markdig.Markdown.ToHtml(markdown, _pipeline);
            rawHtml = ProcessUnorderedListItems(rawHtml);
            return MakeHtml(rawHtml, userControl);
        }

        private static string MakeHtml(in string body, in UserControl userControl) {
            StringBuilder sbHead = new(_head);
            sbHead.Append($@"<style>body,p,span,pre,li,th,td{{");
            sbHead.Append($@"font-size:{userControl.FontSize}px;");
            sbHead.Append($@"font-family:""{userControl.FontFamily}"";");
            sbHead.Append($@"}}</style>");
            return $@"<html><head>{sbHead}</head><body>{body}</body></html>";
        }


        private static string ProcessUnorderedListItems(in string html) {
            StringBuilder result = new StringBuilder();
            int currentPos = 0;
            int listLevel = 0;
            for (int i = 0; i < html.Length; i++) {
                if (i + 3 < html.Length && html.Substring(i, 4) == "<ul>") {
                    listLevel++;
                    i += 3;
                    result.Append(html.AsSpan(currentPos, i - currentPos + 1));
                    currentPos = i + 1;
                    continue;
                }
                if (i + 4 < html.Length && html.Substring(i, 5) == "</ul>") {
                    listLevel = Math.Max(0, listLevel - 1);
                    i += 4;
                    result.Append(html.AsSpan(currentPos, i - currentPos + 1));
                    currentPos = i + 1;
                    continue;
                }
                if (i + 3 < html.Length && html.Substring(i, 4) == "<li>") {
                    result.Append(html.Substring(currentPos, i - currentPos + 4));
                    currentPos = i + 4;
                    while (currentPos < html.Length && char.IsWhiteSpace(html[currentPos])) {
                        currentPos++;
                    }
                    if (listLevel == 1) {
                        result.Append(@"<span class=""ul-h"">●</span>");
                    } else if (listLevel == 2) {
                        result.Append(@"<span class=""ul-h"">○</span>");
                    } else if (listLevel >= 3) {
                        result.Append(@"<span class=""ul-h"">■</span>");
                    }
                    i = currentPos - 1;
                    continue;
                }
            }
            if (currentPos < html.Length) {
                result.Append(html.Substring(currentPos));
            }
            return result.ToString();
        }
    }
}