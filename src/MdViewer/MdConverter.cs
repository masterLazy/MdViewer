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
using ColorCode;
using Markdig;
using System.Net;
using System.Text;
using System.Windows.Controls;
namespace MdViewer {
    public class MdConverter {
        private static readonly MarkdownPipeline _pipeline;
        private static readonly string _head;
        private static readonly HtmlClassFormatter _formatter = new();

        static MdConverter() {
            _pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();
            // Make up common components in <head>
            StringBuilder sb = new();
            sb.Append($@"<meta charset=""UTF-8"">");
            // sb.Append($@"<style>{_formatter.GetCSSString()}</style>");
            sb.Append($@"<style>{Helper.GetEmbeddedResource("MdViewer.colorcode.css")}</style>");
            sb.Append($@"<style>{Helper.GetEmbeddedResource("MdViewer.style.css")}</style>");
            _head = sb.ToString();
        }

        public static string ToHtml(in string markdown, in UserControl userControl) {
            if (markdown == null) return MakeHtml("", userControl);
            var rawHtml = Markdig.Markdown.ToHtml(markdown, _pipeline);
            _firstException = null;
            try {
                rawHtml = ProcessHtml(rawHtml);
            }
            catch (Exception e) {
                rawHtml = MakeErrorAlert(true, e) + rawHtml;
            }
            if (_firstException != null) {
                rawHtml = MakeErrorAlert(false, _firstException) + rawHtml;
            }
            return MakeHtml(rawHtml, userControl);
        }

        private static string MakeErrorAlert(bool isFatal, in Exception e) {
            return "<div style=\"color:tomato;background:#fafa88;border:tomato solid 1px;padding:8px\">" +
                    $"<p style=\"margin:0\">{(isFatal ? "Fatal" : "Error")}: " +
                    "Exception(s) occurred during processing html. Some functions have been disabled.</p>" +
                    $"<pre><code>{e.Message}<code></pre></div>";
        }

        private static string MakeHtml(in string body, in UserControl userControl) {
            StringBuilder sbHead = new(_head);
            sbHead.Append($@"<style>body,p,span,pre,li,th,td{{");
            sbHead.Append($@"font-size:{userControl.FontSize}px;");
            sbHead.Append($@"font-family:""{userControl.FontFamily}"";");
            sbHead.Append($@"}}</style>");
            return $@"<html><head>{sbHead}</head><body>{body}</body></html>";
        }

        private static Exception? _firstException = null;
        private static string ProcessHtml(in string rawHtml) {
            StringBuilder html = new(rawHtml);
            Stack<string> stack = [];
            HtmlContext ctx = new();
            for (int i = 0; i < html.Length; i++) {
                if (html[i] == '<') {
                    var end = html.ToString().IndexOf('>', i);
                    if (end == -1) {
                        throw new Exception($"No matching closing tag found for '<' at {i}");
                    }
                    var element = html.ToString()[i..(end + 1)];
                    // End of element
                    if (element.StartsWith("</")) {
                        stack.Pop();
                        if (element.StartsWith("</ul") || element.StartsWith("</ol")) {
                            ctx.ListDepth--;
                        } else if (element.StartsWith("</pre")) {
                            var oldPre = html.ToString()[ctx.PreStart..i];
                            var newPre = GetPre(ctx);
                            html.Remove(ctx.PreStart, oldPre.Length);
                            html.Insert(ctx.PreStart, newPre);
                            end += newPre.Length - oldPre.Length;
                        } else if (element.StartsWith("</code")) {
                            ctx.CodeContent = html.ToString()[ctx.CodeStart..i];
                            ctx.CodeContent = WebUtility.HtmlDecode(ctx.CodeContent);
                        }
                    }
                    // Begin of element
                    else if (!element.EndsWith("/>")) {
                        if (element.StartsWith("<ul") || element.StartsWith("<ol")) {
                            ctx.ListDepth++;
                        } else if (element.StartsWith("<li") && stack.Peek().StartsWith("<ul")) {
                            var header = GetListHeader(ctx);
                            html.Insert(end + 1, header);
                            end = html.ToString().IndexOf('>', i + header.Length);
                        } else if (element.StartsWith("<pre")) {
                            ctx.PreStart = end + 1;
                        } else if (element.StartsWith("<code")) {
                            ctx.CodeStart = end + 1;
                            var cls = element.IndexOf("class");
                            if (cls != -1) {
                                var s = element.IndexOf('\"', cls) + 1;
                                var t = element.IndexOf('\"', s);
                                var className = element[s..t];
                                if (className.StartsWith("language-")) {
                                    ctx.CodeLang = className["language-".Length..];
                                } else {
                                    ctx.CodeLang = "";
                                }
                            } else {
                                ctx.CodeLang = "";
                            }
                        }
                        stack.Push(element);
                    }
                    i = end;
                    if (i + 1 < html.Length && (html[i + 1] == '\r' || html[i + 1] == '\n')) {
                        html.Remove(i + 1, 1); // Compress html
                    }
                }
            }
            return html.ToString();
        }

        private record HtmlContext {
            public int PreStart = 0;
            public int ListDepth = 0;
            public int CodeStart = 0;
            public string CodeContent = "";
            public string CodeLang = "";
        }

        private static string GetListHeader(in HtmlContext ctx) {
            return ctx.ListDepth switch {
                1 => @"<span class=""ul-h"">●</span>",
                2 => @"<span class=""ul-h"">○</span>",
                _ => @"<span class=""ul-h"">■</span>",
            };
        }

        private static string GetPre(in HtmlContext ctx) {
            if (string.IsNullOrEmpty(ctx.CodeLang)) {
                return ctx.CodeContent;
            } else if (ctx.CodeLang == "jsx" || ctx.CodeLang == "tsx") {
                ctx.CodeLang = "html";
            }
            try {
                var raw = _formatter.GetHtmlString(ctx.CodeContent, Languages.FindById(ctx.CodeLang));
                int s = raw.IndexOf('>', raw.IndexOf("<pre")) + 1;
                int t = raw.IndexOf("</pre");
                return "<code>" + raw[s..t].Trim() + "</code>";
            }
            catch (Exception e) {
                _firstException ??= new Exception(e.Message + $" (language: '{ctx.CodeLang}')");
                return ctx.CodeContent;
            }
        }
    }
}