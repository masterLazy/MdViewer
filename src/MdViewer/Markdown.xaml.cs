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
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MdViewer {
    /// <summary>
    /// Interaction logic of MdViewer.xaml
    /// </summary>
    public partial class Markdown : UserControl, IDisposable {
        // Temporary files
        private readonly string _tempHtmlFile;

        // "Content" Property
        public new static readonly DependencyProperty ContentProperty =
           DependencyProperty.Register("Content", typeof(string), typeof(Markdown));
        public new string Content {
            get { return (string)GetValue(ContentProperty); }
            set {
                SetValue(ContentProperty, value);
                LoadMarkdown();
            }
        }

        public Markdown() {
            InitializeComponent();
            _tempHtmlFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".html");
            LoadMarkdown();

            WebViewer.Navigating += (s, e) => {
                var uri = e.Uri.ToString();
                // Use default browser when navigating to internet
                if (uri.StartsWith("https://") || uri.StartsWith("http://")) {
                    e.Cancel = true;
                    System.Diagnostics.Process.Start("explorer.exe", uri);
                }
            };
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
            if (File.Exists(_tempHtmlFile)) {
                File.Delete(_tempHtmlFile);
            }
        }

        private void LoadMarkdown() {
            File.WriteAllText(_tempHtmlFile, MdConverter.ToHtml(Content, this));
            WebViewer.Navigate(new Uri(_tempHtmlFile));
        }
    }
}
